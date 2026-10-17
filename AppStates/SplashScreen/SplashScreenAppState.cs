using System.Threading;
using DoveDraft;
using Godot;
using GodotTask;

public partial class SplashScreenAppState : AppState
{
    [Export]
    private string NextState { get; set; } = "MainMenu";

    [Export]
    private PackedScene SplashScreenScene { get; set; }

    private CancellationTokenSource cancellationSource;
    private SplashScreen splashScreen;

    //
    //  State Methods
    //

    public override void EnterState(AppState prev)
    {
        // Load assets
        splashScreen = SplashScreenScene.Instantiate<SplashScreen>();
        AddChild(splashScreen);

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
        splashScreen.QueueFree();
        splashScreen = null;
    }

    private async GDTaskVoid StateRoutine(CancellationToken token)
    {
        await splashScreen.SplashRoutine(token);

        Services.App.TransitionToState(NextState);
    }
}
