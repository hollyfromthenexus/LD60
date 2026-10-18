using DoveDraft;
using Godot;

public interface IMemoryService : IService
{
    public bool Visible { get; set; }

    public bool IsTracking { get; }

    public int Capacity { get; }

    public float Current { get; }

    public void AddMemory(int memory);

    public void EatMemory(float memory);

    public void SetIsTracking(string key, bool isTracking, int priority);

    public void ClearIsTracking(string key);

    public void Reset();
}

public partial class MemoryService : CanvasLayer, IMemoryService
{
    [Export]
    private string OutOfMemoryAppStateName { get; set; } = "OutOfMemory";

    [Export]
    private int InitialCapacity { get; set; } = 1024;

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

    public bool IsTracking => isTrackingPref.Value;

    public int Capacity { get; private set; }
    public float Current { get; private set; }

    //
    //	Private Data
    //

    private PrioritizedPref<string, bool> isTrackingPref = new(false);

    private Vector3 previousRotation;

    //
    //	Lifecycle Methods
    //

    public override void _EnterTree()
    {
        Reset();

        Services.Pause.OnPauseChanged += OnPauseChanged;
        Services.Map.OnMapLoadStatusChanged += OnMapLoadStatusChanged;
        Services.Register<IMemoryService>(this);
    }

    public override void _Ready()
    {
        isTrackingPref.OnValueChanged += OnIsTrackingChanged;
    }

    public override void _ExitTree()
    {
        Services.Unregister<IMemoryService>();
        Services.Map.OnMapLoadStatusChanged -= OnMapLoadStatusChanged;
        Services.Pause.OnPauseChanged -= OnPauseChanged;
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
    //	Public Methods
    //

    public void AddMemory(int memory)
    {
        Capacity += memory;
    }

    public void EatMemory(float memory)
    {
        Current += memory;
    }

    public void SetIsTracking(string key, bool isTracking, int priority) =>
        isTrackingPref.SetPreference(key, isTracking, priority);

    public void ClearIsTracking(string key) => isTrackingPref.ClearPreference(key);

    public void Reset()
    {
        isTrackingPref.ClearAllPreferences();
        Capacity = InitialCapacity;
        Current = 0;

        previousRotation = Services.Player?.Current?.HeadNode.Rotation ?? Vector3.Zero;
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
            isTrackingPref.ClearAllPreferences();
            OutOfMemory();
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

    private void OutOfMemory()
    {
        Log.WarnFor<MemoryService>("OUT OF MEM...!!!");
        Services.App.TransitionToState(OutOfMemoryAppStateName);
    }

    private void OnIsTrackingChanged(bool newValue, string ownerKey)
    {
        previousRotation = Services.Player?.Current?.HeadNode.Rotation ?? Vector3.Zero;
    }

    /// <summary>
    /// Pause service prefers that memory tracking stops.
    /// </summary>
    /// <param name="isPaused"></param>
    private void OnPauseChanged(bool isPaused)
    {
        if (isPaused)
        {
            SetIsTracking(nameof(PauseService), false, 15);
        }
        else
        {
            ClearIsTracking(nameof(PauseService));
        }
    }

    /// <summary>
    /// Map service prefers that memory tracking stops.
    /// </summary>
    /// <param name="status"></param>
    private void OnMapLoadStatusChanged(MapLoadStatus status)
    {
        switch (status)
        {
            case MapLoadStatus.Starting:
                SetIsTracking(nameof(MapService), false, 20);
                break;
            case MapLoadStatus.Loaded:
                ClearIsTracking(nameof(MapService));
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
