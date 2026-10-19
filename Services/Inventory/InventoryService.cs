using System.Collections.Generic;
using System.Linq;
using DoveDraft;
using Godot;
using static IInventoryService;

public interface IInventoryService : IService
{
    public delegate void ContentsChangedDelegate();

    public event ContentsChangedDelegate OnContentsChanged;

    public IEnumerable<string> AllItems { get; }

    public void Clear();

    public void AddItem(string itemType);

    public bool RemoveItem(string itemType);
}

public partial class InventoryService : CanvasLayer, IInventoryService
{
    private class ItemData
    {
        public string Item { get; set; }
        public InventoryIcon Icon { get; set; }
    }

    public event ContentsChangedDelegate OnContentsChanged;

    public IEnumerable<string> AllItems => items.Select(data => data.Item);

    [Export]
    private PackedScene IconScene { get; set; }

    [Export]
    private Godot.Collections.Dictionary<string, Texture2D> ItemTextures { get; set; } = new();

    [Export]
    private Control IconParent { get; set; }

    private List<ItemData> items = new();

    //
    //  Godot Methods
    //

    public override void _EnterTree()
    {
        Services.Register<IInventoryService>(this);
    }

    public override void _ExitTree()
    {
        Services.Unregister<IInventoryService>();
    }

    //
    //  Public Methods
    //

    public void Clear()
    {
        int count = items.Count;
        foreach (ItemData data in items)
        {
            DestroyItemData(data);
        }
        items.Clear();

        if (count > 0)
        {
            OnContentsChanged?.Invoke();
        }
    }

    public void AddItem(string itemType)
    {
        Log.For<InventoryService>($"Adding {itemType}.");
        items.Add(CreateItemData(itemType));

        OnContentsChanged?.Invoke();
    }

    public bool RemoveItem(string itemType)
    {
        Log.For<InventoryService>($"Removing {itemType}.");

        int dataIndex = items.FindIndex(itemData => itemData.Item == itemType);
        if (dataIndex == -1)
        {
            return false;
        }

        ItemData data = items[dataIndex];
        DestroyItemData(data);

        items.RemoveAt(dataIndex);
        OnContentsChanged?.Invoke();
        return true;
    }

    private ItemData CreateItemData(string itemType)
    {
        InventoryIcon icon = null;
        if (ItemTextures.TryGetValue(itemType, out Texture2D texture))
        {
            icon = IconScene.Instantiate<InventoryIcon>();
            IconParent.AddChild(icon);

            icon.Initialize(texture);
        }

        return new() { Item = itemType, Icon = icon };
    }

    private void DestroyItemData(ItemData data)
    {
        data.Icon.QueueFree();
        data.Icon = null;
    }
}

namespace DoveDraft
{
    public partial class Services
    {
        public static IInventoryService Inventory => Get<IInventoryService>();
    }
}
