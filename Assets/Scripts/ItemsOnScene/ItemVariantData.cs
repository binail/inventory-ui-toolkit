using UnityEngine;

namespace ItemsOnScene
{
    public class ItemVariantData
    {
        public string ID { get; }
        public Color Color { get; }

        public ItemVariantData(string id, Color color)
        {
            ID = id;
            Color = color;
        }
    }
}