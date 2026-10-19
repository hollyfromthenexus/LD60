using System;
using System.Collections;
using System.Collections.Generic;
using DoveDraft;
using Godot;

[Tool]
public partial class Lock : StaticBody3D
{
    [Export]
    public string Target { get; private set; }

    [Export]
    private float MemoryUsage { get; set; } = 256;

    [Export]
    private LockInteractable Interactable { get; set; }

    [Export]
    private Node3D KeyModel { get; set; }

    public override void _Ready()
    {
        if (Engine.IsEditorHint())
        {
            return;
        }

        foreach (Door targetDoor in FindTargetDoors())
        {
            targetDoor.Lock(GetInstanceId());
        }

        Interactable.OnInteractStart += OnInteractStart;
        Interactable.IsEnabled = true;
        KeyModel.Visible = false;
    }

    public void _func_godot_apply_properties(Godot.Collections.Dictionary entity_properties)
    {
        if (entity_properties.TryGetValue("memory_usage", out Variant memoryUsage))
        {
            MemoryUsage = memoryUsage.AsInt32();
        }

        if (entity_properties.TryGetValue("target", out Variant target))
        {
            Target = target.AsString();
        }
    }

    private void OnInteractStart()
    {
        Interactable.IsEnabled = false;
        KeyModel.Visible = true;

        Services.Memory.EatMemory(MemoryUsage);
        Services.Inventory.RemoveItem(nameof(KeyPickup));

        foreach (Door targetDoor in FindTargetDoors())
        {
            targetDoor.Unlock(GetInstanceId());
        }
    }

    private IEnumerable<Door> FindTargetDoors()
    {
        if (string.IsNullOrEmpty(Target))
        {
            yield break;
        }

        foreach (Node node in GetTree().GetNodesInGroup(nameof(Door)))
        {
            if (node is Door door && door.TargetName == Target)
            {
                yield return door;
            }
        }
    }
}
