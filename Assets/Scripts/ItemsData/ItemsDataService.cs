using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ItemsData
{
    public class ItemsDataService
    {
        private readonly IReadOnlyList<ItemParseData> _itemsData;
        
        private const string ITEMS_DATA_PATH = "ItemsData";
        
        public ItemsDataService()
        {
            TextAsset jsonText = Resources.Load<TextAsset>(ITEMS_DATA_PATH);
            ItemDatabase database = JsonUtility.FromJson<ItemDatabase>(jsonText.text);

            _itemsData = new List<ItemParseData>(database.items);
        }

        public ItemParseData GetRandomItemData()
        {
            int index = Random.Range(0, _itemsData.Count);
            return _itemsData[index];
        }

        public ItemParseData GetItemData(string itemId)
        {
            return _itemsData.FirstOrDefault(item => item.Id == itemId);
        }
    }
}