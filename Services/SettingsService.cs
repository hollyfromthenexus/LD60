using System.Collections.Generic;
using DoveDraft;
using Godot;

public interface ISettingsService : IService
{
    public delegate void SettingsAppliedDelegate();

    public event SettingsAppliedDelegate OnSettingsApplied;

    public bool TryGetFloat(string settingName, out float value);
    public void SetFloat(string settingName, float value);

    public bool TryGetBool(string settingName, out bool value);
    public void SetBool(string settingName, bool value);

    public void Apply();
}

public partial class SettingsService : Node, ISettingsService
{
    public event ISettingsService.SettingsAppliedDelegate OnSettingsApplied;

    private Dictionary<string, float> settingFloats = new();
    private Dictionary<string, bool> settingBools = new();

    //
    //  Godot Methods
    //

    public override void _EnterTree()
    {
        Services.Register<ISettingsService>(this);
    }

    public override void _ExitTree()
    {
        Services.Unregister<ISettingsService>();
    }

    //
    //	Public Methods
    //

    public bool TryGetFloat(string settingName, out float value) =>
        settingFloats.TryGetValue(settingName, out value);

    public void SetFloat(string settingName, float value) => settingFloats[settingName] = value;

    public bool TryGetBool(string settingName, out bool value) =>
        settingBools.TryGetValue(settingName, out value);

    public void SetBool(string settingName, bool value) => settingBools[settingName] = value;

    public void Apply()
    {
        OnSettingsApplied?.Invoke();
    }
}

namespace DoveDraft
{
    public partial class Services
    {
        public static ISettingsService Settings => Get<ISettingsService>();
    }
}
