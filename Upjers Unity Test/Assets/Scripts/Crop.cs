using System.Collections;
using UnityEngine;
using UnityEngine.Events;
namespace PlantingGame
{
    public class Crop : MonoBehaviour
    {
        public float growthTime = 15f;
        public float cost = 10f;
        public float sellPriceMultiplier = 1.5f;
        public GrowthStage currentGrowthStage = GrowthStage.New;
        public UnityEvent onGrowthCycleUpdate;

        private float _advancmentCompletionTime;
        private PlantingManager _plantingManager;

        private void Start()
        {
            CropSetup();
            _plantingManager = FindFirstObjectByType<PlantingManager>();
            if(_plantingManager == null)
            {
                Debug.LogError("PlantingManager not found in the scene.");
                return;
            }
        }
        /// <summary>
        /// Logic for planting a crop.
        /// </summary>
        private void CropSetup()
        {
            _advancmentCompletionTime = Time.time;
            currentGrowthStage = GrowthStage.New;
            StartCoroutine(AdvanceGrowthCycle());
        }

        IEnumerator AdvanceGrowthCycle()
        {
            while (true)
            {
                switch(currentGrowthStage)
                {
                    case GrowthStage.New:
                        if (CheckGrowthAdvancement())
                        {
                            currentGrowthStage = GrowthStage.Maturing;
                            _advancmentCompletionTime = float.MaxValue; // Reset the advancement completion time
                            // TODO: Call worker here
                        }
                        break;
                    case GrowthStage.Maturing:
                        if (CheckGrowthAdvancement())
                        {
                            currentGrowthStage = GrowthStage.Harvestable;
                            // TODO: Call worker here
                        }
                        break;
                    case GrowthStage.Harvestable:
                        // Nothing to do here, the crop is ready to be harvested.
                        break;
                }
                onGrowthCycleUpdate.Invoke(); // other events could be added here that may affect the growth cycle.
                yield return new WaitForSeconds(1f); // Check every second
            }
        }

        /// <summary>
        /// Checks if the crop is ready to be harvested.
        /// </summary>
        private bool CheckGrowthAdvancement()
        {
            if (_advancmentCompletionTime + growthTime >= Time.time)
            {
                return true;
            }
            return false;
        }


    }

    public enum GrowthStage
    {
        New,
        Maturing,
        Harvestable,
    }
}