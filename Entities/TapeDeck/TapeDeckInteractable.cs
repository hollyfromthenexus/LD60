using System;
using System.Linq;
using DoveDraft;
using Godot;

[Tool]
public partial class TapeDeckInteractable : Interactable
{
    [Export]
    public bool Enabled { get; set; } = true;

    [Export]
    public bool RequiresTape { get; set; } = true;

    public override bool IsInteractable =>
        Enabled
        && (RequiresTape == false || Services.Inventory.AllItems.Contains(nameof(TapePickup)));

    public event Action OnClick;

    protected override void UseStart()
    {
        OnClick?.Invoke();
    }
}
