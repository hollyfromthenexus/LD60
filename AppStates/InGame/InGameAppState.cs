using DoveDraft;

public partial class InGameAppState : AppState
{
    public override void EnterState(AppState prev)
    {
        Services.HUD.Visible = true;
        Services.Dialog.Visible = true;
    }

    public override void ExitState(AppState next)
    {
        Services.Dialog.Visible = false;
        Services.HUD.Visible = false;
    }
}
