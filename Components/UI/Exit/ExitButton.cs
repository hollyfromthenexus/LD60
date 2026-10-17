using System;
using Godot;

public partial class ExitButton : Button
{
    [Export]
    private PackedScene ExitConfirmationDialogScene { get; set; }

    private ConfirmationDialog exitConfirmationDialog;

    public override void _EnterTree()
    {
        Pressed += OnPressed;
    }

    public override void _ExitTree()
    {
        CloseWindow();
        Pressed -= OnPressed;
    }

    private void OpenWindow()
    {
        if (exitConfirmationDialog != null)
        {
            return;
        }

        exitConfirmationDialog = ExitConfirmationDialogScene.Instantiate<ConfirmationDialog>();
        GetTree().Root.AddChild(exitConfirmationDialog);
        exitConfirmationDialog.CloseRequested += OnCloseRequested;
        exitConfirmationDialog.Canceled += OnCloseRequested;
        exitConfirmationDialog.Confirmed += OnConfirm;
    }

    private void CloseWindow()
    {
        if (exitConfirmationDialog == null)
        {
            return;
        }

        exitConfirmationDialog.Visible = false;
        exitConfirmationDialog.QueueFree();
        exitConfirmationDialog = null;
    }

    private void OnPressed() => OpenWindow();

    private void OnCloseRequested() => CloseWindow();

    private void OnConfirm()
    {
        GetTree().Quit();
    }
}
