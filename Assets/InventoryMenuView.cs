using UnityEngine;
using UnityEngine.UI;
using VInspector;

public class InventoryMenuView : MenuView
{
    public static InventoryMenuView Instance { get; private set; }
    
    [SerializeField]
    SerializedDictionary<InventorySystem.Slot, Image> slotImages = new();

    
    // temporary pragma cause isHidden is not used. get rid of it when you use it
#pragma warning disable CS0414 // Field is assigned but its value is never used
    private bool isHidden = false;
#pragma warning restore CS0414 // Field is assigned but its value is never used

    public override void Initialize()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    public void PlaceInventoryItem(InventorySystem.Slot slot, Sprite sprite)
    {
        slotImages.TryGetValue(slot, out Image image);
        image.sprite = sprite;
    }

    public override void Show()
    {
        base.Show();
        isHidden = false;
    }

    public override void Hide()
    {
        base.Hide();
        isHidden = true;
    }
}
