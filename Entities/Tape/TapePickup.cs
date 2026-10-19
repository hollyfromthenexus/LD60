using DoveDraft;
using Godot;

public partial class TapePickup : Area3D
{
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

        Services.Inventory.AddItem(nameof(TapePickup));
        QueueFree();
    }
}
