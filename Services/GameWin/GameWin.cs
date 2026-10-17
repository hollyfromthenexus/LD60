using System;
using System.Threading;
using DoveDraft;
using Godot;
using GodotTask;

public partial class GameWin : CanvasLayer
{
    public async GDTask WinRoutine(CancellationToken token)
    {
        Log.For<GameWin>("Game win routine started.");

        await GDTask.Delay(500, token);
    }
}
