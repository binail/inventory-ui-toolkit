using HintView;
using InventoryData;
using InventoryView;
using ItemsData;
using ItemsOnScene;
using Player;
using UnityEngine;
using UnityEngine.UIElements;

namespace Services
{
    public class Services : MonoBehaviour
    {
        [SerializeField] private ItemsOnSceneService itemsOnScene;
        [SerializeField] private PlayerInputService playerInput;
        [SerializeField] private PlayerMovement playerMovement;
        [SerializeField] private InventoryViewService inventoryView;
        [SerializeField] private HintViewService hintViewService;
        [SerializeField] private UIDocument mainUI;

        public static Services Instance { get; private set; }

        public InventoryDataService InventoryData { get; private set; }
        public ItemsDataService ItemsData { get; private set; }
        public ItemsOnSceneService ItemsOnScene => itemsOnScene;
        public PlayerInputService PlayerInput => playerInput;
        public PlayerMovement PlayerMovement => playerMovement;
        public InventoryViewService InventoryView => inventoryView;

        private void Awake()
        {
            if (Instance != null)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);

            InitServices();
        }

        private void InitServices()
        {
            InventoryData = new InventoryDataService();
            ItemsData = new ItemsDataService();
            ItemsOnScene.Initialize();
            InventoryView.Initialize(mainUI);
            hintViewService.Initialize(mainUI);
        }
    }
}