using UnityEngine;

namespace ItemsOnScene
{
    public class ItemVariant : MonoBehaviour
    {
        [SerializeField] private MeshRenderer meshRenderer;
        [SerializeField] private Rigidbody itemRigidbody;

        private ItemVariantData _data;
        private Material _material;

        public bool IsActive => gameObject.activeSelf;
        public string ItemId => _data.ID;
        
        public void Initialize(ItemVariantData data)
        {
            _data = data;
            gameObject.SetActive(true);

            _material ??= CreateMaterial();
            _material.color = data.Color;
        }

        public void Disable()
        {
            gameObject.SetActive(false);
        }

        public void DropItem(Vector3 force)
        {
            itemRigidbody.AddForce(force, ForceMode.Impulse);
        }

        private Material CreateMaterial()
        {
            var newMaterial = new Material(meshRenderer.sharedMaterial);
            meshRenderer.material = newMaterial;
            
            return newMaterial;
        }
    }
}