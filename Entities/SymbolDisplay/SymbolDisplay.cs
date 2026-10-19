using Godot;

[Tool]
[GlobalClass]
public partial class SymbolDisplay : Node3D
{
    [Export]
    private string Symbol { get; set; }

    [Export]
    private StandardMaterial3D NeonMaterial { get; set; }

    [Export]
    private MeshInstance3D Frame { get; set; }

    [Export]
    public Godot.Collections.Dictionary<string, MeshInstance3D> SymbolDisplays { get; set; } =
        new()
        {
            { "circle", null },
            { "spiral", null },
            { "star", null },
            { "timelines", null },
            { "x", null },
        };

    [Export]
    public Godot.Collections.Dictionary<string, Color> SymbolColors { get; set; } =
        new()
        {
            { "circle", Colors.Cyan },
            { "spiral", Colors.Green },
            { "star", Colors.Blue },
            { "timelines", Colors.Yellow },
            { "x", Colors.Magenta },
        };

    public override void _Ready()
    {
        RefreshAppearance();
    }

    public void _func_godot_apply_properties(Godot.Collections.Dictionary entity_properties)
    {
        if (entity_properties.TryGetValue("symbol", out Variant symbol))
        {
            Symbol = symbol.AsString();
        }

        RefreshAppearance();
    }

    private void RefreshAppearance()
    {
        if (string.IsNullOrEmpty(Symbol))
        {
            return;
        }

        NeonMaterial = NeonMaterial.Duplicate() as StandardMaterial3D;
        Frame.SetSurfaceOverrideMaterial(0, NeonMaterial);

        foreach (var kvp in SymbolDisplays)
        {
            kvp.Value.Visible = kvp.Key == Symbol;
            kvp.Value.SetSurfaceOverrideMaterial(0, NeonMaterial);

            NeonMaterial.AlbedoColor = SymbolColors[Symbol];
            NeonMaterial.Emission = SymbolColors[Symbol];
        }
    }
}
