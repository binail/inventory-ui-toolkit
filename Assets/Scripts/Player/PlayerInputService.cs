using System;
using UnityEngine;

namespace Player
{
    public class PlayerInputService : MonoBehaviour
    {
        public event Action OnInventoryToggle;

        private Vector3 _moveInput;
        
        public Vector3 MoveInput => IsInventoryOpen ? Vector3.zero : _moveInput;

        private bool IsInventoryOpen => Services.Services.Instance.InventoryView.IsOpen;
        
        private const string HORIZONTAL_AXIS = "Horizontal";
        private const string VERTICAL_AXIS = "Vertical";
        
        private void Update()
        {
            float x = Input.GetAxisRaw(HORIZONTAL_AXIS);
            float z = Input.GetAxisRaw(VERTICAL_AXIS);
            _moveInput = new Vector3(x, 0, z).normalized;

            if (Input.GetKeyDown(KeyCode.E) && !IsInventoryOpen)
                TryCollectItem();

            if (Input.GetKeyDown(KeyCode.Tab))
                OnInventoryToggle?.Invoke();
        }

        private void TryCollectItem()
        {
            var itemId = Services.Services.Instance.ItemsOnScene.TryPickUpItem(
                Services.Services.Instance.PlayerMovement.PlayerPosition
            );

            if (itemId != null)
                Services.Services.Instance.InventoryData.OnCollectItem(itemId);
        }
    }
}