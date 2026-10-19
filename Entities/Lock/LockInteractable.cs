using System;
using System.Linq;
using DoveDraft;
using Godot;

[Tool]
public partial class LockInteractable : Interactable
{
    public bool IsEnabled { get; set; } = true;

    public override bool IsInteractable =>
        IsEnabled && Services.Inventory.AllItems.Contains(nameof(KeyPickup));

    public event Action OnInteractStart;

    protected override void UseStart()
    {
        OnInteractStart?.Invoke();
    }
}
