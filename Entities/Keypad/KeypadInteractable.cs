using System;
using DoveDraft;
using Godot;

[Tool]
public partial class KeypadInteractable : Interactable
{
    [Export]
    public bool Enabled { get; set; } = true;

    public override bool IsInteractable => Enabled;

    public event Action OnClick;

    protected override void UseStart()
    {
        OnClick?.Invoke();
    }
}
