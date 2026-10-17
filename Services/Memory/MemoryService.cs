using DoveDraft;
using Godot;

public interface IMemoryService : IService
{
    public bool Visible { get; set; }

    public bool IsTracking { get; set; }

    public float Capacity { get; }

    public float Current { get; }
}

public partial class MemoryService : CanvasLayer, IMemoryService
{
    [Export]
    private float MovementCost { get; set; } = 1;

    [Export]
    private float LookCost { get; set; } = 0.4f;

    [Export]
    private float TimeCost { get; set; } = 0.02f;

    [Export]
    private ProgressBar ProgressBar { get; set; }

    [Export]
    private Label Label { get; set; }

    public bool IsTracking
    {
        get => isTrackingInternal;
        set
        {
            previousRotation = Services.Player?.Current?.HeadNode.Rotation ?? Vector3.Zero;
            isTrackingInternal = value;
        }
    }
    public float Capacity { get; private set; } = 1024;
    public float Current { get; private set; } = 128;

    //
    //	Private Data
    //

    private bool isTrackingInternal = false;

    private Vector3 previousRotation;

    //
    //	Lifecycle Methods
    //

    public override void _EnterTree()
    {
        Services.Map.OnMapLoadStatusChanged += OnMapLoadStatusChanged;
        Services.Register<IMemoryService>(this);
    }

    public override void _ExitTree()
    {
        Services.Unregister<IMemoryService>();
        Services.Map.OnMapLoadStatusChanged -= OnMapLoadStatusChanged;
    }

    //
    //	Godot Methods
    //

    public override void _Process(double delta)
    {
        if (IsTracking)
        {
            ConsumeMemory(delta);
        }

        ProgressBar.MaxValue = Capacity;
        ProgressBar.Value = Current;
        Label.Text = $"MEMORY: {Current:0.00} B / {Capacity:0} B";
    }

    //
    //	Private Methods
    //

    private void ConsumeMemory(double delta)
    {
        float memJustUsed = GetMemoryUsedThisFrame(delta);

        Current += memJustUsed;
        if (Current >= Capacity)
        {
            IsTracking = false;
            Log.WarnFor<MemoryService>("OUT OF MEM...!!!");
        }
    }

    private float GetMemoryUsedThisFrame(double delta)
    {
        Player player = Services.Player.Current;
        if (player == null)
        {
            return 0;
        }

        float memoryUsed = (float)delta * TimeCost;

        memoryUsed += player.Velocity.Length() * (float)delta * MovementCost;

        Vector3 lookRotationDelta = player.HeadNode.Rotation - previousRotation;
        previousRotation = player.HeadNode.Rotation;
        memoryUsed += lookRotationDelta.Length() * LookCost;

        return memoryUsed;
    }

    private void OnMapLoadStatusChanged(MapLoadStatus status)
    {
        switch (status)
        {
            case MapLoadStatus.Starting:
                IsTracking = false;
                break;
            case MapLoadStatus.Loaded:
                IsTracking = true;
                break;
        }
    }
}

namespace DoveDraft
{
    public partial class Services
    {
        public static IMemoryService Memory => Get<IMemoryService>();
    }
}
