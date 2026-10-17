using System;
using DoveDraft;
using Godot;

public interface IPauseService : IService
{
    public bool IsPaused { get; }

    public bool CanPause { get; }

    public void Pause();

    public void UnPause();

    public void SetCanPause(string key, bool canPause, int priority);

    public void ClearCanPause(string key);
}

public partial class PauseService : CanvasLayer, IPauseService
{
    [Export]
    private Button ResumeButton { get; set; }

    public bool IsPaused { get; private set; }

    public bool CanPause => canPausePreference.Value;

    private readonly PrioritizedPref<string, bool> canPausePreference = new(false);

    //
    //  Godot Methods
    //

    public override void _EnterTree()
    {
        Services.Register<IPauseService>(this);
    }

    public override void _Ready()
    {
        ResumeButton.Pressed += OnResumePressed;
        Visible = false;
    }

    public override void _ExitTree()
    {
        Services.Unregister<IPauseService>();
    }

    public override void _Input(InputEvent @event)
    {
        switch (@event)
        {
            case InputEventKey keyEvent:
                if (keyEvent.Pressed && keyEvent.Keycode == Key.Escape)
                {
                    if (IsPaused)
                    {
                        UnPause();
                    }
                    else
                    {
                        Pause();
                    }
                }
                break;
        }
    }

    //
    //	Public Methods
    //

    public void SetCanPause(string key, bool canPause, int priority) =>
        canPausePreference.SetPreference(key, canPause, priority);

    public void ClearCanPause(string key) => canPausePreference.ClearPreference(key);

    public void Pause()
    {
        if (IsPaused || !CanPause)
        {
            return;
        }

        Visible = true;
        Services.MouseLock.SetLock(nameof(PauseService), false, 500);
        IsPaused = true;
    }

    public void UnPause()
    {
        if (!IsPaused)
        {
            return;
        }

        Visible = false;
        Services.MouseLock.ClearLock(nameof(PauseService));
        IsPaused = false;
    }

    //
    //	Private Methods
    //

    private void OnResumePressed()
    {
        UnPause();
    }
}

namespace DoveDraft
{
    public partial class Services
    {
        public static IPauseService Pause => Get<IPauseService>();
    }
}
