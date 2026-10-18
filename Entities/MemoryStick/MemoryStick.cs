using DoveDraft;
using Godot;

[Tool]
public partial class MemoryStick : Area3D
{
    [Export]
    public int UpgradeCapacity { get; private set; } = 1024;

    public override void _Ready()
    {
        if (Engine.IsEditorHint())
        {
            return;
        }

        BodyEntered += OnBodyEntered;
    }

    public void _func_godot_apply_properties(Godot.Collections.Dictionary entity_properties)
    {
        if (entity_properties.TryGetValue("upgrade_capacity", out Variant value))
        {
            UpgradeCapacity = value.AsInt32();
        }
    }

    private void OnBodyEntered(Node3D body)
    {
        if (body is not Player)
        {
            return;
        }

        Services.Memory.AddMemory(UpgradeCapacity);
        QueueFree();
    }
}
