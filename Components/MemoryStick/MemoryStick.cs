using DoveDraft;
using Godot;

public partial class MemoryStick : Area3D
{
    [Export]
    public int UpgradeCapacity { get; private set; } = 1024;

    public override void _Ready()
    {
        BodyEntered += OnBodyEntered;
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
