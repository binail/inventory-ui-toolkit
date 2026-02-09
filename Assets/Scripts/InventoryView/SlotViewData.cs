using System;
using UnityEngine;

namespace InventoryView
{
    public class SlotViewData
    {
        private readonly Action<int> _onChangeCell;
        
        public Texture2D  ItemTexture { get; }
        public string CountValue { get; }
        public int CellNumber { get; private set; }
        public Action OnDropItem { get; }

        public SlotViewData(Texture2D itemTexture,
            string countValue,
            int cellNumber,
            Action<int> onChangeCell,
            Action onDropItem)
        {
            _onChangeCell = onChangeCell;
            ItemTexture = itemTexture;
            CountValue = countValue;
            CellNumber = cellNumber;
            OnDropItem = onDropItem;
        }

        public void ChangeCell(int newCellNumber)
        {
            CellNumber = newCellNumber;
            _onChangeCell?.Invoke(newCellNumber);
        }
    }
}