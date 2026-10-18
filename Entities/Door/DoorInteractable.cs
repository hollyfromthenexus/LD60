using System;
using DoveDraft;
using Godot;

[Tool]
public partial class DoorInteractable : Interactable
{
    public bool DoorIsInteractable { get; set; } = true;

    public override bool IsInteractable => DoorIsInteractable;

    public event Action OnInteractStart;

    protected override void UseStart()
    {
        OnInteractStart?.Invoke();
    }
}
