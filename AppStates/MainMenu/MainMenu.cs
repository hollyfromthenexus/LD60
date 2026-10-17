using DoveDraft;
using Godot;

public partial class MainMenu : CanvasLayer
{
    [Export]
    private string NextState { get; set; } = "InGame";

    [Export]
    private Button StartGameButton { get; set; }

    public override void _Ready()
    {
        StartGameButton.Pressed += OnStartGamePressed;
    }

    private void OnStartGamePressed()
    {
        Services.App.TransitionToState(NextState);
    }
}
