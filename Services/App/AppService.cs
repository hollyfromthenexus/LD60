using System;
using DoveDraft;
using Godot;
using Godot.Collections;

public interface IAppService : IService
{
    public AppState CurrentState { get; }

    public void TransitionToState(string nextStateName);
}

public partial class AppService : Node, IAppService
{
    public AppState CurrentState { get; private set; }

    //
    //  Private Data
    //

    /// <summary>
    /// A collection of all states, stored by the name of the state node.
    /// </summary>
    private Dictionary<string, AppState> states = new();

    //
    //  Godot Methods
    //

    public override void _EnterTree()
    {
        Services.Register<IAppService>(this);
    }

    public override void _Ready()
    {
        // Set initial UI states
        Services.HUD.Visible = false;
        Services.Dialog.Visible = false;

        LD60Config config = LD60Config.Get();

        // Populate the states by their name.
        foreach (PackedScene appStateScene in config.AppStates)
        {
            AppState appState = appStateScene.Instantiate<AppState>();
            AddChild(appState);
            states.Add(appState.Name, appState);
        }

        TransitionToState(config.InitialAppState);
    }

    public override void _ExitTree()
    {
        Services.Unregister<IAppService>();
    }

    public override void _Process(double delta)
    {
        CurrentState?.ProcessState(delta);
    }

    //
    //  State Machine
    //

    public void TransitionToState(string nextStateName)
    {
        Log.For<AppService>($"Transitioning {CurrentState?.Name ?? "NONE"} -> {nextStateName}...");
        AppState nextState = null;

        // If a state by this name doesn't exist, THROW.
        if (states.TryGetValue(nextStateName, out nextState) == false)
        {
            throw new Exception($"No state by {nextStateName} known...!");
        }

        // If we were already in a state, make sure to exit.
        AppState prevState = CurrentState;
        Log.For<AppService>($"...exiting state {prevState?.Name ?? "NONE"}...");
        prevState?.ExitState(nextState);

        // Enter the next state
        Log.For<AppService>($"..entering state {nextState.Name}...");
        nextState.EnterState(prevState);
        CurrentState = nextState;

        Log.For<AppService>("...transition complete.");
    }
}

namespace DoveDraft
{
    public partial class Services
    {
        public static IAppService App => Get<IAppService>();
    }
}
