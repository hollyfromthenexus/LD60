using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using DoveDraft;
using Godot;
using GodotTask;

[Tool]
public partial class Keypad : StaticBody3D
{
    [Export]
    private string Target { get; set; }

    [Export]
    public string TargetName { get; set; }

    [Export]
    private int MemoryUsage { get; set; }

    [Export]
    public string Combination { get; set; } = "1337";

    [Export]
    private KeypadButton[] Buttons { get; set; }

    [Export]
    private OmniLight3D Light { get; set; }

    [Export]
    private StandardMaterial3D CookieMaterial { get; set; }

    private CancellationTokenSource cancelSource;
    private StringBuilder currentSequence = new();

    public override void _EnterTree()
    {
        if (Engine.IsEditorHint())
        {
            return;
        }

        AddToGroup(nameof(Keypad));
        cancelSource = new();
    }

    public override void _ExitTree()
    {
        if (Engine.IsEditorHint())
        {
            return;
        }

        RemoveFromGroup(nameof(Keypad));

        cancelSource.Cancel();
        cancelSource = null;
    }

    public override void _Ready()
    {
        if (Engine.IsEditorHint())
        {
            return;
        }

        foreach (KeypadButton button in Buttons)
        {
            button.OnPressed += OnDigitPressed;
        }

        SetTargetDoorsLocked(true);
        SetButtonsEnabled(true);
        SetLightColor(Colors.Blue);
    }

    public void _func_godot_apply_properties(Godot.Collections.Dictionary entity_properties)
    {
        if (entity_properties.TryGetValue("memory_usage", out Variant memoryUsage))
        {
            MemoryUsage = memoryUsage.AsInt32();
        }

        if (entity_properties.TryGetValue("target", out Variant target))
        {
            Target = target.AsString();
        }

        if (entity_properties.TryGetValue("targetname", out Variant targetname))
        {
            TargetName = targetname.AsString();
        }

        if (entity_properties.TryGetValue("combination", out Variant combination))
        {
            Combination = combination.AsString();
        }
    }

    private void OnDigitPressed(string digit)
    {
        currentSequence.Append(digit);
        Services.Memory.EatMemory(MemoryUsage);
        PressDigit(cancelSource.Token).Forget();
    }

    private async GDTaskVoid PressDigit(CancellationToken token)
    {
        string result = currentSequence.ToString();

        if (result.Length >= Combination.Length)
        {
            if (result == Combination)
            {
                await CorrectCombination(token);
            }
            else
            {
                await IncorrectCombination(token);
            }

            currentSequence.Clear();
        }
        else
        {
            SetButtonsEnabled(false);

            SetLightColor(Colors.Black);
            await GDTask.Delay(100, token);
            SetLightColor(Colors.Blue);

            SetButtonsEnabled(true);
        }
    }

    private async GDTask CorrectCombination(CancellationToken token)
    {
        SetButtonsEnabled(false);

        for (int i = 0; i < 4; i++)
        {
            SetLightColor(Colors.Green * 0.5f);
            await GDTask.Delay(100, token);

            SetLightColor(Colors.Green);
            await GDTask.Delay(200, token);
        }

        SetLightColor(Colors.Green);
        SetTargetDoorsLocked(false);
    }

    private async GDTask IncorrectCombination(CancellationToken token)
    {
        SetButtonsEnabled(false);

        for (int i = 0; i < 1; i++)
        {
            SetLightColor(Colors.Black);
            await GDTask.Delay(100, token);

            SetLightColor(Colors.Red);
            await GDTask.Delay(200, token);
        }

        await GDTask.Delay(1000, token);

        SetLightColor(Colors.Black);
        await GDTask.Delay(100, token);

        SetLightColor(Colors.Blue);
        SetButtonsEnabled(true);
    }

    private void SetButtonsEnabled(bool isEnabled)
    {
        foreach (KeypadButton button in Buttons)
        {
            button.SetEnabled(isEnabled);
        }
    }

    private void SetTargetDoorsLocked(bool isLocked)
    {
        ulong instanceId = GetInstanceId();

        foreach (Door targetDoor in FindTargetDoors())
        {
            if (isLocked)
            {
                targetDoor.Lock(instanceId);
            }
            else
            {
                targetDoor.Unlock(instanceId);
            }
        }
    }

    private IEnumerable<Door> FindTargetDoors()
    {
        if (string.IsNullOrEmpty(Target))
        {
            yield break;
        }

        foreach (Node node in GetTree().GetNodesInGroup(nameof(Door)))
        {
            if (node is Door door && door.TargetName == Target)
            {
                yield return door;
            }
        }
    }

    private void SetLightColor(Color color)
    {
        Light.LightColor = color;
        CookieMaterial.AlbedoColor = color;
    }
}
