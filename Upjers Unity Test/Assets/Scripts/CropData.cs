using System;
using UnityEngine;

namespace PlantingGame
{
    [CreateAssetMenu(fileName = "CropData", menuName = "ScriptableObjects/CropData", order = 1)]
    public class CropData : ScriptableObject
    {
        public int cost;
        public float growthTime;
        public float sellPriceMultiplier;
        public GameObject cropPrefab;
        public Sprite icon;
    }
}