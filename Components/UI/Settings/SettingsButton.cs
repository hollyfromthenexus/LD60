using Godot;

public partial class SettingsButton : Button
{
    [Export]
    private PackedScene SettingsWindowScene { get; set; }

    private SettingsWindow settingsWindow;

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
        if (settingsWindow != null)
        {
            return;
        }

        settingsWindow = SettingsWindowScene.Instantiate<SettingsWindow>();
        GetTree().Root.AddChild(settingsWindow);
        settingsWindow.CloseRequested += OnCloseRequested;
    }

    private void CloseWindow()
    {
        if (settingsWindow == null)
        {
            return;
        }

        settingsWindow.Visible = false;
        settingsWindow.QueueFree();
        settingsWindow = null;
    }

    private void OnPressed() => OpenWindow();

    private void OnCloseRequested() => CloseWindow();
}
