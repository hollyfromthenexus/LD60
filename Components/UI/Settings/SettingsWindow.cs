using DoveDraft;
using Godot;

public partial class SettingsWindow : AcceptDialog
{
    private Button applyButton;

    public override void _EnterTree()
    {
        applyButton = AddButton("Apply", true);
        applyButton.Pressed += OnApplyPressed;

        GetOkButton().Pressed += OnOkPressed;
    }

    public override void _ExitTree()
    {
        GetOkButton().Pressed -= OnOkPressed;

        applyButton.Pressed -= OnApplyPressed;
        RemoveButton(applyButton);
        applyButton = null;
    }

    private void OnApplyPressed()
    {
        Services.Settings.Apply();
    }

    private void OnOkPressed()
    {
        Services.Settings.Apply();
    }
}
