using System.Collections.Generic;
using System.Linq;
using Utils;

namespace InventoryData
{
    public class InventoryDataService
    {
        private readonly List<ItemStackData> _itemStacksData = new();

        public IReadOnlyList<ItemStackData> ItemStacksData => _itemStacksData;
        
        public void OnCollectItem(string itemId)
        {
            var itemData = Services.Services.Instance.ItemsData.GetItemData(itemId);
            
            if (itemData == null)
                return;

            if (itemData.Stackable)
            {
                var existingStack = _itemStacksData
                    .FirstOrDefault(stack => stack.ItemId == itemData.Id && stack.ItemsCount < itemData.MaxStack);

                if (existingStack != null)
                {
                    existingStack.IncreaseCount();
                    return;
                }
            }

            if (_itemStacksData.Count >= DevConstants.INVENTORY_CELLS_COUNT)
            {
                return;
            }

            int newCellNumber = 0;
            
            for (int i = 0; i < DevConstants.INVENTORY_CELLS_COUNT; i++)
            {
                if (_itemStacksData.All(stack => stack.InventoryCellNumber != i))
                    newCellNumber = i;
            }

            var newStack = new ItemStackData(newCellNumber, itemData.Id);
            _itemStacksData.Add(newStack);
        }

        public void OnDropItem(int cellNumber, out string itemId, out int itemsCount)
        {
            var stackData = _itemStacksData.First(data => data.InventoryCellNumber == cellNumber);
            itemId = stackData.ItemId;
            itemsCount = stackData.ItemsCount;
            
            _itemStacksData.Remove(stackData);
        }
    }
}