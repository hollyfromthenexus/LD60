using System.Collections.Generic;
using DoveDraft;
using Godot;

[Tool]
public partial class Door : StaticBody3D
{
    [Export]
    public string TargetName { get; private set; }

    [Export]
    private float MemoryUsage { get; set; } = 256;

    [Export]
    private AnimationTree Anim { get; set; }

    [Export]
    private DoorInteractable Interactable { get; set; }

    [Export]
    private CollisionShape3D Collider { get; set; }

    public bool IsOpen
    {
        get => isOpenInternal;
        set
        {
            if (value == isOpenInternal)
            {
                return;
            }

            isOpenInternal = value;

            // Play animation
            AnimationNodeStateMachinePlayback stateMachine = (AnimationNodeStateMachinePlayback)
                Anim.Get("parameters/playback");
            stateMachine.Travel(value ? "opened" : "closed");

            Collider.Disabled = value;
        }
    }

    private bool isOpenInternal;
    private HashSet<ulong> lockIds = new();

    public override void _EnterTree()
    {
        if (Engine.IsEditorHint())
        {
            return;
        }

        AddToGroup(nameof(Door));
    }

    public override void _ExitTree()
    {
        if (Engine.IsEditorHint())
        {
            return;
        }

        RemoveFromGroup(nameof(Door));
    }

    public override void _Ready()
    {
        if (Engine.IsEditorHint())
        {
            return;
        }

        Interactable.OnInteractStart += OnInteractStart;
    }

    public void _func_godot_apply_properties(Godot.Collections.Dictionary entity_properties)
    {
        if (entity_properties.TryGetValue("memory_usage", out Variant memoryUsage))
        {
            MemoryUsage = memoryUsage.AsInt32();
        }

        if (entity_properties.TryGetValue("targetname", out Variant targetName))
        {
            TargetName = targetName.AsString();
        }
    }

    public void Lock(ulong id)
    {
        lockIds.Add(id);
        RefreshLockStatus();
    }

    public void Unlock(ulong id)
    {
        lockIds.Remove(id);
        RefreshLockStatus();
    }

    private void RefreshLockStatus()
    {
        Interactable.DoorIsInteractable = lockIds.Count == 0;

        ADD OPEN ON UNLOCK SETTING
    }

    private void OnInteractStart()
    {
        IsOpen = !IsOpen;

        Services.Memory.EatMemory(MemoryUsage);
    }
}
