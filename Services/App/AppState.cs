using Godot;

public partial class AppState : Node
{
    public virtual void EnterState(AppState prev) { }

    public virtual void ProcessState(double delta) { }

    public virtual void ExitState(AppState next) { }
}
