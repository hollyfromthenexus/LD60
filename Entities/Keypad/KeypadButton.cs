using System;
using Godot;

[Tool]
public partial class KeypadButton : StaticBody3D
{
    public event Action<string> OnPressed;

    [Export]
    private string Digit { get; set; } = "?";

    [Export]
    private Label3D Label { get; set; }

    [Export]
    private KeypadInteractable Interactable { get; set; }

    public override void _Ready()
    {
        if (Engine.IsEditorHint())
        {
            return;
        }

        Label.Text = Digit;
        Interactable.OnClick += OnClick;
    }

    public void SetEnabled(bool isEnabled) => Interactable.Enabled = isEnabled;

    private void OnClick()
    {
        OnPressed?.Invoke(Digit);
    }
}
