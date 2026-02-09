using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ItemsData;
using Player;
using UnityEngine;

namespace ItemsOnScene
{
    public class ItemsOnSceneService : MonoBehaviour
    {
        [SerializeField] private List<Transform> itemsStartPoints;
        [SerializeField] private float pickUpDistance;
        [SerializeField] private float dropDelay;
        [SerializeField] private float dropDistance;
        [SerializeField] private float dropForce;
        [SerializeField] private int startItemsCount;
        [SerializeField] private ItemVariant itemPrefab;

        private List<ItemVariant> _items;

        private PlayerMovement PlayerMovement => Services.Services.Instance.PlayerMovement;
        private ItemsDataService ItemsData => Services.Services.Instance.ItemsData;
        
        public void Initialize()
        {
            _items ??= new List<ItemVariant>(startItemsCount);

            if (itemsStartPoints == null || itemsStartPoints.Count == 0)
            {
                Debug.LogError("No start points assigned!");

                return;
            }

            if (startItemsCount > itemsStartPoints.Count)
            {
                Debug.LogError("StartItemsCount exceeds available start points!");

                return;
            }

            List<int> availablePointNumbers = new List<int>();

            for (int i = 0; i < itemsStartPoints.Count; i++)
            {
                availablePointNumbers.Add(i);
            }

            for (int i = 0; i < startItemsCount; i++)
            {
                if (availablePointNumbers.Count == 0)
                {
                    Debug.LogError("Ran out of available start points!");
                    break;
                }

                int randomIndexInList = Random.Range(0, availablePointNumbers.Count);
                int pointIndex = availablePointNumbers[randomIndexInList];

                Transform spawnPoint = itemsStartPoints[pointIndex];

                if (spawnPoint == null)
                {
                    Debug.LogError($"Start point at index {pointIndex} is null!");
                    break;
                }

                var itemParseData = ItemsData.GetRandomItemData();
                var itemData = new ItemVariantData(itemParseData.Id, itemParseData.CubeColor);
                
                CreateItem(spawnPoint.position, itemData);

                availablePointNumbers.RemoveAt(randomIndexInList);
            }
        }

        public bool CanPlayerPickUp(Vector3 playerPosition)
        {
            return GetItemsInRange(playerPosition).Any();
        }

        public string TryPickUpItem(Vector3 playerPosition)
        {
            var item = GetItemsInRange(playerPosition).FirstOrDefault();
            item?.Disable();
            
            return item?.ItemId;
        }

        private List<ItemVariant> GetItemsInRange(Vector3 playerPosition)
        {
            var result = new List<ItemVariant>();

            if (_items == null)
            {
                return result;
            }

            for (int i = 0; i < _items.Count; i++)
            {
                if (!_items[i].IsActive)
                    continue;

                if ((_items[i].transform.position - playerPosition).magnitude <= pickUpDistance)
                    result.Add(_items[i]);
            }
            
            return result;
        }

        public void DropItems(string itemId, int itemsCount)
        {
            StartCoroutine(DropItemsCoroutine(itemId, itemsCount));
        }

        private ItemVariant CreateItem(Vector3 position, ItemVariantData itemData)
        {
            ItemVariant newItem = Instantiate(itemPrefab, position, Quaternion.identity, transform);
            newItem.Initialize(itemData);
            _items.Add(newItem);
            
            return newItem;
        }

        private IEnumerator DropItemsCoroutine(string itemId, int itemsCount)
        {
            Vector3 playerPosition = PlayerMovement.PlayerPosition;
            Vector3 forward = PlayerMovement.PlayerForward;
            var itemParseData = ItemsData.GetItemData(itemId);
            var itemData = new ItemVariantData(itemParseData.Id, itemParseData.CubeColor);

            for (int i = 0; i < itemsCount; i++)
            {
                Vector3 spawnPosition = playerPosition + forward * dropDistance + Vector3.up;

                ItemVariant item = _items.FirstOrDefault(x => !x.IsActive);

                if (item == null)
                {
                    item = CreateItem(spawnPosition, itemData);
                }
                else
                {
                    item.transform.position = spawnPosition;
                    item.Initialize(itemData);
                }

                item.DropItem(forward + Vector3.up * dropForce);

                yield return new WaitForSeconds(dropDelay);
            }
        }
    }
}
