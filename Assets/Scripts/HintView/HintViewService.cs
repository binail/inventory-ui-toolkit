using UnityEngine;
using UnityEngine.UIElements;

namespace HintView
{
    public class HintViewService : MonoBehaviour
    {
        private VisualElement _mainPanel;

        public void Initialize(UIDocument uiDocument)
        {
            _mainPanel = uiDocument.rootVisualElement.Q<VisualElement>("pick-up-hint");
        }

        private void Update()
        {
            if(_mainPanel == null)
                return;
            
            bool canPickUp = Services.Services.Instance.ItemsOnScene
                .CanPlayerPickUp(Services.Services.Instance.PlayerMovement.PlayerPosition);
            
            _mainPanel.style.display = canPickUp ? DisplayStyle.Flex : DisplayStyle.None;
        }
    }
}