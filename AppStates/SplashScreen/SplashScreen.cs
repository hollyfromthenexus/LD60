using System.Threading;
using DoveDraft;
using Godot;
using GodotTask;

public partial class SplashScreen : CanvasLayer
{
    public async GDTask SplashRoutine(CancellationToken token)
    {
        Log.For<SplashScreenAppState>("Starting splash.");
        Visible = false;

        await GDTask.Delay(1500, token);

        Log.For<SplashScreenAppState>("SPLASH!");
        Visible = true;

        await GDTask.Delay(1500, token);

        Log.For<SplashScreenAppState>("Done.");
        Visible = false;

        await GDTask.Delay(200, token);
    }
}
