using Godot;

public partial class MainMenuAppState : AppState
{
    [Export]
    private PackedScene MainMenuScene { get; set; }

    private MainMenu mainMenu;

    //
    //  State Methods
    //

    public override void EnterState(AppState prev)
    {
        // Load assets
        mainMenu = MainMenuScene.Instantiate<MainMenu>();
        AddChild(mainMenu);
    }

    public override void ExitState(AppState next)
    {
        // Unload assets
        mainMenu.QueueFree();
        mainMenu = null;
    }
}
