using System;
using Godot;

public partial class InventoryIcon : PanelContainer
{
    [Export]
    private TextureRect Display { get; set; }

    public void Initialize(Texture2D texture)
    {
        Display.Texture = texture;
    }
}
