using DoveDraft;
using Godot;
using Godot.Collections;

[GlobalClass]
public partial class LD60Config : DoveDraftConfig
{
    public static LD60Config Get() => Services.Config.Config as LD60Config;

    [Export]
    public Array<PackedScene> AppStates { get; private set; } = new();

    [Export]
    public string InitialAppState { get; private set; } = "SplashScreen";

    [Export]
    public string InitialMapPath { get; private set; } = "Maps/dev_scene.tscn";
}
