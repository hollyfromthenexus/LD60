using System.Text;
using Godot;

[Tool]
[GlobalClass]
public partial class NumberDecal : Node3D
{
    [Export]
    private int Digit { get; set; }

    [Export]
    private int Index { get; set; }

    [Export]
    private Label3D DigitLabel { get; set; }

    [Export]
    private Label3D IndexLabel { get; set; }

    public override void _Ready()
    {
        if (Engine.IsEditorHint())
        {
            return;
        }

        DigitLabel.Text = Digit.ToString();

        StringBuilder sb = new StringBuilder();
        sb.Append("[ ");
        for (int i = 0; i < 4; i++)
        {
            sb.Append(Index == i ? "X " : "_ ");
        }
        sb.Append("]");
        IndexLabel.Text = sb.ToString();
    }

    public void _func_godot_apply_properties(Godot.Collections.Dictionary entity_properties)
    {
        if (entity_properties.TryGetValue("digit", out Variant digit))
        {
            Digit = digit.AsInt32();
        }

        if (entity_properties.TryGetValue("index", out Variant index))
        {
            Index = index.AsInt32();
        }
    }
}
