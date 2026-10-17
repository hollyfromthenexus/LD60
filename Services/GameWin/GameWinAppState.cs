using System;
using System.Threading;
using DoveDraft;
using Godot;
using GodotTask;

public partial class GameWinAppState : AppState
{
    [Export]
    private PackedScene GameWinScene { get; set; }

    private CancellationTokenSource cancellationSource;
    private GameWin gameWin;

    //
    //  State Methods
    //

    public override void EnterState(AppState prev)
    {
        // Load assets
        gameWin = GameWinScene.Instantiate<GameWin>();
        AddChild(gameWin);

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
        gameWin.QueueFree();
        gameWin = null;
    }

    //
    //	Private Methods
    //

    private async GDTaskVoid StateRoutine(CancellationToken token)
    {
        await gameWin.WinRoutine(token);
    }
}
