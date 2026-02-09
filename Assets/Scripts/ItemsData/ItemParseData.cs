using System;
using UnityEngine;

namespace ItemsData
{
    [Serializable]
    public class ItemParseData
    {
        [SerializeField] private string id;
        [SerializeField] private string iconName;
        [SerializeField] private Color cubeColor;
        [SerializeField] private bool stackable;
        [SerializeField] private int maxStack;
        
        public string Id => id;
        public string IconName => iconName;
        public Color CubeColor => cubeColor;
        public bool Stackable => stackable;
        public int MaxStack => maxStack;
    }
}