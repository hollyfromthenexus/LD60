using System;
using System.Threading;
using DoveDraft;
using Godot;
using GodotTask;

public partial class OutOfMemoryAppState : AppState
{
    [Export]
    private PackedScene OutOfMemoryScene { get; set; }

    [Export]
    private string NextState { get; set; } = "InGame";

    private CancellationTokenSource cancellationSource;
    private OutOfMemory outOfMemory;

    //
    //  State Methods
    //

    public override void EnterState(AppState prev)
    {
        // Load assets
        outOfMemory = OutOfMemoryScene.Instantiate<OutOfMemory>();
        AddChild(outOfMemory);

        // Start the routine
        cancellationSource = new();
        StateRoutine(cancellationSource.Token).Forget();
    }

    public override void ExitState(AppState next)
    {
        // Stop the routine
        cancellationSource.Cancel();
        cancellationSource = null;

        // Unload assets
        outOfMemory.QueueFree();
        outOfMemory = null;
    }

    //
    //	Private Methods
    //

    private async GDTaskVoid StateRoutine(CancellationToken token)
    {
        await outOfMemory.CrashRoutine(token);

        Services.App.TransitionToState(NextState);
    }
}
