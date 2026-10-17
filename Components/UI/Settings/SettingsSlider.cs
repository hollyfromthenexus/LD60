using DoveDraft;
using Godot;

public partial class SettingsSlider : HSlider
{
    [Export]
    private string SettingName { get; set; }

    [Export]
    private float DefaultValue { get; set; } = 0;

    [Export]
    private Label Label { get; set; }

    [Export]
    private string FormatString { get; set; } = "{0}";

    public override void _EnterTree()
    {
        SnapToSettingsValue();

        DragEnded += OnDragEnded;
        ValueChanged += OnValueChanged;
        Services.Settings.OnSettingsApplied += OnSettingsApplied;
    }

    public override void _ExitTree()
    {
        Services.Settings.OnSettingsApplied -= OnSettingsApplied;
        ValueChanged -= OnValueChanged;
        DragEnded -= OnDragEnded;
    }

    private void OnValueChanged(double _)
    {
        UpdateVisualValue();
    }

    private void OnDragEnded(bool didValueChange)
    {
        if (didValueChange == false)
        {
            return;
        }

        Services.Settings.SetFloat(SettingName, (float)Value);
    }

    private void OnSettingsApplied()
    {
        SnapToSettingsValue();
    }

    private void SnapToSettingsValue()
    {
        if (Services.Settings.TryGetFloat(SettingName, out float preExistingValue))
        {
            Value = preExistingValue;
            UpdateVisualValue();
        }
        else
        {
            Value = DefaultValue;
            UpdateVisualValue();
            Services.Settings.SetFloat(SettingName, DefaultValue);
        }
    }

    private void UpdateVisualValue()
    {
        Label.Text = string.Format(FormatString, Value);
    }
}
