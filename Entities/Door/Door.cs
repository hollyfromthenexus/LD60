using DoveDraft;
using Godot;

[Tool]
public partial class Door : StaticBody3D
{
    [Export]
    private float MemoryUsage { get; set; } = 256;

    [Export]
    private AnimationTree Anim { get; set; }

    [Export]
    private DoorInteractable Interactable { get; set; }

    [Export]
    private CollisionShape3D Collider { get; set; }

    public bool IsOpen
    {
        get => isOpenInternal;
        set
        {
            if (value == isOpenInternal)
            {
                return;
            }

            isOpenInternal = value;

            // Play animation
            AnimationNodeStateMachinePlayback stateMachine = (AnimationNodeStateMachinePlayback)
                Anim.Get("parameters/playback");
            stateMachine.Travel(value ? "opened" : "closed");

            Collider.Disabled = value;
        }
    }

    private bool isOpenInternal;

    public override void _Ready()
    {
        if (Engine.IsEditorHint())
        {
            return;
        }

        Interactable.OnInteractStart += OnInteractStart;
    }

    public void _func_godot_apply_properties(Godot.Collections.Dictionary entity_properties)
    {
        if (entity_properties.TryGetValue("memory_usage", out Variant value))
        {
            MemoryUsage = value.AsInt32();
        }
    }

    private void OnInteractStart()
    {
        IsOpen = !IsOpen;

        Services.Memory.EatMemory(MemoryUsage);
    }
}
