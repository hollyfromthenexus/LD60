using System.Threading;
using DoveDraft;
using GodotTask;

public partial class InGameAppState : AppState
{
    public override void EnterState(AppState prev)
    {
        Services.HUD.Visible = true;
        Services.Dialog.Visible = true;
        Services.Memory.Visible = true;
        Services.Pause.SetCanPause(nameof(InGameAppState), true, 0);

        Services.Memory.Reset();
        Services.Memory.SetIsTracking(nameof(InGameAppState), true, 0);

        Services.Inventory.Clear();

        // Load and start the first map
        LaunchWorld();
    }

    public override void ExitState(AppState next)
    {
        KillWorld();

        Services.Inventory.Clear();

        Services.Memory.ClearIsTracking(nameof(InGameAppState));
        Services.Player.ClearPlayer();

        Services.Pause.ClearCanPause(nameof(InGameAppState));
        Services.Memory.Visible = false;
        Services.Dialog.Visible = false;
        Services.HUD.Visible = false;
    }

    private void LaunchWorld()
    {
        LD60Config config = LD60Config.Get();
        Services.Map.TransitionTo(config.InitialMapPath, CancellationToken.None).Forget();
    }

    private void KillWorld()
    {
        Services.Map.Clear();
    }
}
