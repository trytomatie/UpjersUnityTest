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
        public bool canGrow = true;
        public GrowthStage currentGrowthStage = GrowthStage.New;
        public GameObject progressBarPrefab;
        public UnityEvent onGrowthCycleUpdate;
        private bool _waitingForWorker = false;
        private float _advancmentCompletionTime;
        private PlantingManager _plantingManager;
        private ProgressBar _progressBar;


        private void Start()
        {
            CropSetup();
            Canvas canvas = FindFirstObjectByType<Canvas>();
            _plantingManager = FindFirstObjectByType<PlantingManager>();
            if(_plantingManager == null)
            {
                Debug.LogError("PlantingManager not found in the scene.");
                return;
            }
            _progressBar = Instantiate(progressBarPrefab, canvas.transform).GetComponent<ProgressBar>();
            _progressBar.transform.position = Camera.main.WorldToScreenPoint(transform.position + new Vector3(0, 1.5f, 0));
            onGrowthCycleUpdate.AddListener(UpdateProgressBar);

        }

        public void UpdateProgressBar()
        {
            float progress = 1-(_advancmentCompletionTime - Time.time) / growthTime;
            _progressBar.UpdateProgressBar(progress);
            _progressBar.progressText.text = $"{(int)(progress * 100)}%";
        }
        /// <summary>
        /// Logic for planting a crop.
        /// </summary>
        private void CropSetup()
        {
            _advancmentCompletionTime = Time.time + growthTime;
            currentGrowthStage = GrowthStage.New;
            StartCoroutine(CheckGrowthCycle());
        }

        IEnumerator CheckGrowthCycle()
        {
            while (canGrow)
            {
                switch(currentGrowthStage)
                {
                    case GrowthStage.New:
                        if (CheckGrowthAdvancement())
                        {
                            // TODO: Call worker here
                            _waitingForWorker = true;
                            _progressBar.ShowWorkerAlert("Water!",new Color32(154,255,255,255));
                        }
                        break;
                    case GrowthStage.Maturing:
                        if (CheckGrowthAdvancement())
                        {
                            CheckGrowthCycle();
                            // TODO: Call worker here
                            _progressBar.ShowWorkerAlert("Harvest!",Color.yellow);
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

        public void AdvanceGrowthCycle()
        {
            if (currentGrowthStage == GrowthStage.New)
            {
                _waitingForWorker = false;
                _progressBar.HideWorkerAlert();
                currentGrowthStage = GrowthStage.Maturing;
                _advancmentCompletionTime = Time.time + growthTime;
            }
            else if (currentGrowthStage == GrowthStage.Maturing)
            {
                currentGrowthStage = GrowthStage.Harvestable;
                _waitingForWorker = true;
            }
        }

        /// <summary>
        /// Checks if the crop is ready to be harvested.
        /// </summary>
        private bool CheckGrowthAdvancement()
        {
            if (_advancmentCompletionTime <= Time.time && !_waitingForWorker)
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