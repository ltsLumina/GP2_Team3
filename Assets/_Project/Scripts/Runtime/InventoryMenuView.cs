using TMPro;
using UnityEngine;
using UnityEngine.UI;
using VInspector;

public class InventoryMenuView : MenuView
{
    public static InventoryMenuView Instance { get; private set; }

    [SerializeField] private Button closeButton;

    [SerializeField]
    private SerializedDictionary<InventorySystem.Slot, Image> slotImages = new();

    [SerializeField]
    private SerializedDictionary<Player.Stats, TextMeshProUGUI> statValues = new();

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

        closeButton.onClick.AddListener(() => MenuViewManager.HideView(this));
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

        foreach (var stat in GameManager.Instance.Player.PlayerStats)
            SetStatValue(stat.Key, stat.Value);
    }

    private void SetStatValue(Player.Stats stat, int value)
    {
        statValues.TryGetValue(stat, out TextMeshProUGUI statValue);
        if (statValue != null)
            statValue.text = value + "%";
    }

    public override void Hide()
    {
        base.Hide();
        isHidden = true;
    }
}
