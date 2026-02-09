namespace InventoryData
{
    public class ItemStackData
    {
        public int InventoryCellNumber { get; private set; }
        public string ItemId { get; private set; }
        public int ItemsCount { get; private set; }

        public ItemStackData(int inventoryCellNumber, string itemId)
        {
            InventoryCellNumber = inventoryCellNumber;
            ItemId = itemId;
            ItemsCount = 1;
        }
        
        public void IncreaseCount()
        {
            ItemsCount++;
        }

        public void ChangeCellNumber(int newCellNumber)
        {
            InventoryCellNumber = newCellNumber;
        }
    }
}