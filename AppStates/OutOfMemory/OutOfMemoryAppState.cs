using System;
using Godot;

public partial class OutOfMemoryAppState : AppState
{
    [Export]
    private PackedScene OutOfMemoryScene { get; set; }

    private OutOfMemory outOfMemory;

    //
    //  State Methods
    //

    public override void EnterState(AppState prev)
    {
        // Load assets
        outOfMemory = OutOfMemoryScene.Instantiate<OutOfMemory>();
        AddChild(outOfMemory);
    }

    public override void ExitState(AppState next)
    {
        // Unload assets
        outOfMemory.QueueFree();
        outOfMemory = null;
    }
}
