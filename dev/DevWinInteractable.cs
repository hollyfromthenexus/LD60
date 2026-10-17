namespace DoveDraft;

public partial class DevWinInteractable : Interactable
{
    public override bool IsInteractable => true;

    //
    //  Exports
    //

    //
    //  Interactable Methods
    //

    protected override void UseStart()
    {
        Services.App.TransitionToState("GameWin");
    }
}
