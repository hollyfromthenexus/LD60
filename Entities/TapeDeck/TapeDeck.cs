using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using DoveDraft;
using Godot;
using GodotTask;

[Tool]
public partial class TapeDeck : StaticBody3D
{
    [Export]
    private int MemoryUsage { get; set; } = 1024;

    [Export]
    private string Target { get; set; }

    [Export]
    private int SequenceLength { get; set; } = 6;

    [Export]
    private TapeDeckInteractable Interactable { get; set; }

    [Export]
    private Node3D Bay { get; set; }

    [Export]
    private Vector3 CloseRotationDegrees { get; set; }

    private List<int> sequence = new();
    private bool hasTape;

    public override void _Ready()
    {
        if (Engine.IsEditorHint())
        {
            return;
        }

        Interactable.OnClick += OnInteract;
        hasTape = false;
        GenerateNewSequence();
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
    }

    private IEnumerable<Keypad> FindTargetKeypads()
    {
        if (string.IsNullOrEmpty(Target))
        {
            yield break;
        }

        foreach (Node node in GetTree().GetNodesInGroup(nameof(Keypad)))
        {
            if (node is Keypad keypad && keypad.TargetName == Target)
            {
                yield return keypad;
            }
        }
    }

    private void OnInteract()
    {
        if (!hasTape)
        {
            InsertTape().Forget();
        }
        else
        {
            PlayTapeSequence().Forget();
        }
    }

    private async GDTaskVoid InsertTape()
    {
        Interactable.RequiresTape = false;
        Bay.RotationDegrees = CloseRotationDegrees;

        await PlayTapeSequence();
    }

    private async GDTask PlayTapeSequence()
    {
        Interactable.Enabled = false;

        SfxTapeStart();
        await GDTask.Delay(1000);

        foreach (int digit in sequence)
        {
            SfxDigit(digit);
            await GDTask.Delay(800);
        }

        await GDTask.Delay(1000);
        SfxTapeEnd();

        Interactable.Enabled = true;
    }

    private void GenerateNewSequence()
    {
        sequence.Clear();
        StringBuilder sb = new StringBuilder();

        for (int i = 0; i < SequenceLength; i++)
        {
            int digit = GD.RandRange(0, 9);
            sequence.Add(digit);
            sb.Append(digit);
        }

        string combination = sb.ToString();
        foreach (Keypad keypad in FindTargetKeypads())
        {
            keypad.Combination = combination;
        }
    }

    private void SfxTapeStart() { }

    private void SfxTapeEnd() { }

    private void SfxDigit(int digit)
    {
        Log.For<TapeDeck>($"{digit}");
    }
}
