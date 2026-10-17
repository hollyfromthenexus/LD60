using System;
using System.Threading;
using DoveDraft;
using Godot;
using GodotTask;

public partial class OutOfMemory : CanvasLayer
{
    [Export]
    private Label Label { get; set; }

    public async GDTask CrashRoutine(CancellationToken token)
    {
        Log.For<OutOfMemory>("Out of memory routine started");
        Label.Visible = false;

        await GDTask.Delay(500, token);

        Label.Visible = true;
        await GDTask.Delay(2000, token);

        Label.Visible = false;
        await GDTask.Delay(200, token);
    }
}
