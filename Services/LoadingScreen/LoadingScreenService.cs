using System;
using DoveDraft;
using Godot;

public partial class LoadingScreenService : CanvasLayer, IService
{
    [Export]
    private int TransitionInTimeMs { get; set; } = 500;

    [Export(PropertyHint.ExpEasing)]
    private float TransitionInEase { get; set; }

    [Export]
    private int TransitionOutTimeMs { get; set; } = 500;

    [Export(PropertyHint.ExpEasing)]
    private float TransitionOutEase { get; set; }

    [Export]
    private float SpinDegreesPerSecond { get; set; } = 360 * 2;

    [Export]
    private Control RootControl { get; set; }

    [Export]
    private Control Spinner { get; set; }

    private bool isLerping = false;
    private float lerpModifier = 1;
    private float lerpWeight = 0;
    private float lerpEase;
    private Color startingColor;
    private Color targetColor;

    //
    //  Godot Methods
    //

    public override void _EnterTree()
    {
        Services.Register<LoadingScreenService>(this);
        Services.Map.LoadStartTimeDelayMs = TransitionInTimeMs;
        Services.Map.OnMapLoadStatusChanged += OnMapLoadStatusChanged;
        Visible = false;
    }

    public override void _ExitTree()
    {
        Services.Map.OnMapLoadStatusChanged -= OnMapLoadStatusChanged;
        Services.Unregister<LoadingScreenService>();
    }

    public override void _Process(double delta)
    {
        if (Visible == false)
        {
            return;
        }

        if (isLerping)
        {
            lerpWeight += (float)delta * lerpModifier;
            RootControl.Modulate = startingColor.Lerp(
                targetColor,
                Mathf.Ease(lerpWeight, lerpEase)
            );

            if (lerpWeight >= 1)
            {
                isLerping = false;

                if (Mathf.IsZeroApprox(RootControl.Modulate.A))
                {
                    Visible = false;
                }
            }
        }

        if (Spinner.Visible)
        {
            Spinner.RotationDegrees += SpinDegreesPerSecond * (float)delta;
        }
    }

    private void OnMapLoadStatusChanged(MapLoadStatus status)
    {
        switch (status)
        {
            case MapLoadStatus.Starting:
                Visible = true;
                startingColor = new Color(1, 1, 1, 0);
                targetColor = new Color(1, 1, 1, 1);
                lerpWeight = 0;
                lerpModifier = 1000.0f / TransitionInTimeMs;
                lerpEase = TransitionInEase;
                isLerping = true;
                break;

            case MapLoadStatus.Loaded:
                startingColor = new Color(1, 1, 1, 1);
                targetColor = new Color(1, 1, 1, 0);
                lerpWeight = 0;
                lerpModifier = 1000.0f / TransitionOutTimeMs;
                lerpEase = TransitionOutEase;
                isLerping = true;
                break;
        }

        Spinner.Visible = status == MapLoadStatus.Loading || status == MapLoadStatus.Starting;
    }
}
