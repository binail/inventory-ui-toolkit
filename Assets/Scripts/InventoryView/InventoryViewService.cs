using System.Collections.Generic;
using InventoryData;
using UnityEngine;
using UnityEngine.UIElements;

namespace InventoryView
{
    public class InventoryViewService : MonoBehaviour
    {
        [SerializeField] private VisualTreeAsset slotTemplate;
        [SerializeField] private VisualTreeAsset itemViewTemplate;
        
        private InventoryWindow _window;
        private bool _isOpen;
        
        public bool IsOpen => _isOpen;

        private IReadOnlyList<ItemStackData> ItemStacksData => Services.Services.Instance.InventoryData.ItemStacksData;

        private const string ITEM_TEXTURE_PATH = "ItemIcons/i_{0}_icon";

        public void Initialize(UIDocument mainUI)
        {
            _window = new InventoryWindow(mainUI, slotTemplate, itemViewTemplate);
            _window.Close();
            
            Services.Services.Instance.PlayerInput.OnInventoryToggle -= ToggleWindow;
            Services.Services.Instance.PlayerInput.OnInventoryToggle += ToggleWindow;
        }

        private void OnDestroy()
        {
            Services.Services.Instance.PlayerInput.OnInventoryToggle -= ToggleWindow;
        }

        private void ToggleWindow()
        {
            if (_isOpen)
                _window.Close();
            else
                _window.Open(CreateSlotsViewData());
            
            _isOpen = !_isOpen;
        }

        private List<SlotViewData> CreateSlotsViewData()
        {
            var result = new List<SlotViewData>(ItemStacksData.Count);
            
            foreach (var slotData in ItemStacksData)
            {
                result.Add(new SlotViewData(
                    GetItemTextureById(slotData.ItemId),
                    slotData.ItemsCount.ToString(),
                    slotData.InventoryCellNumber,
                    slotData.ChangeCellNumber,
                    () => OnDropItem(slotData.InventoryCellNumber)
                    ));
            }

            return result;
        }

        private Texture2D  GetItemTextureById(string id)
        {
            var itemData = Services.Services.Instance.ItemsData.GetItemData(id);
            
            return Resources.Load<Texture2D>(string.Format(ITEM_TEXTURE_PATH, itemData.IconName));
        }

        private void OnDropItem(int cellNumber)
        {
            Services.Services.Instance.InventoryData.OnDropItem(cellNumber, out string itemId, out int itemsCount);
            Services.Services.Instance.ItemsOnScene.DropItems(itemId, itemsCount);
        }
    }
}