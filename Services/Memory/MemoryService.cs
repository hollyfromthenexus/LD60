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
    private ProgressBar ProgressBar { get; set; }

    [Export]
    private Label Label { get; set; }

    public bool IsTracking { get; set; } = true;
    public float Capacity { get; private set; } = 1024;
    public float Current { get; private set; } = 128;

    //
    //	Lifecycle Methods
    //

    public override void _EnterTree()
    {
        Services.Register<IMemoryService>(this);
    }

    public override void _ExitTree()
    {
        Services.Unregister<IMemoryService>();
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
        Label.Text = $"MEMORY: {Current:0.00} B / {Capacity:0.00} B";
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

        return player.Velocity.Length() * (float)delta;
    }
}

namespace DoveDraft
{
    public partial class Services
    {
        public static IMemoryService Memory => Get<IMemoryService>();
    }
}
