using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;
using Utils;

namespace InventoryView
{
    public class InventoryWindow
    {
        private readonly UIDocument _document;
        private readonly VisualTreeAsset _slotTemplate;
        private readonly VisualTreeAsset _itemViewTemplate;
        private readonly VisualElement _mainPanel;

        private VisualElement _draggingItem;
        private VisualElement _originalSlot;
        private Vector2 _dragOffset;
        private List<SlotViewData> _slotsViewData;

        public InventoryWindow(UIDocument document, VisualTreeAsset slotTemplate, VisualTreeAsset itemViewTemplate)
        {
            _document = document;
            _slotTemplate = slotTemplate;
            _itemViewTemplate = itemViewTemplate;
            _mainPanel = _document.rootVisualElement.Q<VisualElement>("inventory-window");

            CreateSlots();
        }

        public void Open(List<SlotViewData> slotsViewData)
        {
            _slotsViewData = slotsViewData;
            _mainPanel.style.display = DisplayStyle.Flex;
            InitSlots(slotsViewData);
        }

        public void Close()
        {
            _mainPanel.style.display = DisplayStyle.None;
        }

        private void StartDrag(PointerDownEvent evt, VisualElement itemView, VisualElement slot)
        {
            if (itemView == null) return;

            if (evt.button == 1)
            {
                DropItem(itemView);
                itemView.style.display = DisplayStyle.None;
                
                return;
            }

            _originalSlot = slot;

            _draggingItem = _itemViewTemplate.CloneTree();
            _draggingItem.userData = itemView.userData;
            _draggingItem.style.position = Position.Absolute;
            _draggingItem.style.top = evt.position.y - evt.localPosition.y;
            _draggingItem.style.left = evt.position.x - evt.localPosition.x;
            InitItemView(_draggingItem);

            _dragOffset = evt.localPosition;

            _mainPanel.parent.Add(_draggingItem);

            itemView.style.display = DisplayStyle.None;

            _document.rootVisualElement.RegisterCallback<PointerMoveEvent>(OnDragMove);
            _document.rootVisualElement.RegisterCallback<PointerUpEvent>(OnDragEnd);
        }

        private void OnDragMove(PointerMoveEvent evt)
        {
            if (_draggingItem == null) return;

            _draggingItem.style.left = evt.position.x - _dragOffset.x;
            _draggingItem.style.top = evt.position.y - _dragOffset.y;
        }

        private void OnDragEnd(PointerUpEvent evt)
        {
            if (_draggingItem == null)
                return;

            bool outsidePanel = !_mainPanel.Q<VisualElement>("inventory-panel").worldBound.Contains(evt.position);

            _draggingItem.RemoveFromHierarchy();
            _draggingItem = null;

            _document.rootVisualElement.UnregisterCallback<PointerMoveEvent>(OnDragMove);
            _document.rootVisualElement.UnregisterCallback<PointerUpEvent>(OnDragEnd);

            if (outsidePanel)
            {
                DropItem(_originalSlot.Q<VisualElement>("item-view"));
            }
            else
            {
                VisualElement targetSlot = null;

                foreach (var slot in _mainPanel.Query<VisualElement>(className: "slot").ToList())
                {
                    var worldRect = slot.worldBound;

                    if (worldRect.Contains(evt.position)
                        && slot.Q<VisualElement>("item-view").userData
                        != _originalSlot.Q<VisualElement>("item-view").userData)
                    {
                        targetSlot = slot;
                        break;
                    }
                }

                bool returnItem = true;
                
                if (targetSlot != null)
                {
                    var targetItem = targetSlot.Q<VisualElement>("item-view");
                    var targetSlotNumber = (int)targetItem.userData;

                    if (_slotsViewData.All(data => data.CellNumber != targetSlotNumber))
                    {
                        targetItem.style.display = DisplayStyle.Flex;

                        var originalItemSlotNumber = (int)_originalSlot.Q<VisualElement>("item-view").userData;
                        _slotsViewData.First(data => data.CellNumber == originalItemSlotNumber)
                            .ChangeCell(targetSlotNumber);
                        InitItemView(targetItem);

                        returnItem = false;
                    }
                }

                if (returnItem)
                {
                    _originalSlot.Q<VisualElement>("item-view").style.display = DisplayStyle.Flex;
                }
            }

            _originalSlot = null;
        }


        private void InitSlots(List<SlotViewData> slotsViewData)
        {
            var slotsContainer = _mainPanel.Q<VisualElement>("slots-container");
            var allSlots = slotsContainer.Query<VisualElement>(className: "slot").ToList();
            
            for (int i = 0; i < DevConstants.INVENTORY_CELLS_COUNT; i++)
            {
                if (i >= allSlots.Count) 
                    continue;

                var slot = allSlots[i];
                var itemView = slot.Q<VisualElement>("item-view");
                
                if (slotsViewData.Any(data => data.CellNumber == i))
                {
                    itemView.style.display = DisplayStyle.Flex;
                    InitItemView(itemView);
                }
                else
                {
                    itemView.style.display = DisplayStyle.None;
                }
            }
        }

        private void DropItem(VisualElement itemView)
        {
            var itemViewData = _slotsViewData.First(data => data.CellNumber == (int)itemView.userData);
            itemViewData.OnDropItem?.Invoke();
            _slotsViewData.Remove(itemViewData);
        }

        private void InitItemView(VisualElement itemElement)
        {
            var viewData = _slotsViewData.FirstOrDefault(data => data.CellNumber == (int)itemElement.userData);
            
            if(viewData == null)
                return;
            
            itemElement.style.backgroundImage = viewData.ItemTexture;
            itemElement.Q<Label>("item-count").text = viewData.CountValue;
        }
        
        private void CreateSlots()
        {
            for (int i = 0; i < DevConstants.INVENTORY_CELLS_COUNT; i++)
            {
                var newSlot = _slotTemplate.CloneTree();
                _mainPanel.Q<VisualElement>("slots-container").Add(newSlot);
                var itemView = newSlot.Q<VisualElement>("item-view");
                itemView.RegisterCallback<PointerDownEvent>(evt => StartDrag(evt, itemView, newSlot));
                itemView.userData = i;
            }
        }
    }
}