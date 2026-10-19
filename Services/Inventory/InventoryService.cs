using System.Collections.Generic;
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
    public event ContentsChangedDelegate OnContentsChanged;

    public IEnumerable<string> AllItems => items;

    private List<string> items = new();

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
        items.Clear();

        if (count > 0)
        {
            OnContentsChanged?.Invoke();
        }
    }

    public void AddItem(string itemType)
    {
        Log.For<InventoryService>($"Adding {itemType}.");
        items.Add(itemType);

        OnContentsChanged?.Invoke();
    }

    public bool RemoveItem(string itemType)
    {
        Log.For<InventoryService>($"Removing {itemType}.");
        bool result = items.Remove(itemType);

        if (result)
        {
            OnContentsChanged?.Invoke();
        }

        return result;
    }
}

namespace DoveDraft
{
    public partial class Services
    {
        public static IInventoryService Inventory => Get<IInventoryService>();
    }
}
