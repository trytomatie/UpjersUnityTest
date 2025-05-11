using System.Collections;
using UnityEngine;
using UnityEngine.Events;
namespace PlantingGame
{
    public class Crop : MonoBehaviour
    {
        public bool canGrow = true;
        public GrowthStage currentGrowthStage = GrowthStage.New;
        public GameObject progressBarPrefab;
        public UnityEvent onGrowthCycleUpdate;
        public GameObject[] cropPrefabs;

        private bool _waitingForWorker = false;
        private float _advancmentCompletionTime;
        private PlantingManager _plantingManager;
        private ProgressBar _progressBar;
        private WorkerJobManager _workerJobManager;
        private GameManager _gameManager;
        public CropData cropData;



        private void Start()
        {
            CropSetup();
            Canvas canvas = FindFirstObjectByType<Canvas>();
            _plantingManager = FindFirstObjectByType<PlantingManager>();
            _workerJobManager = FindFirstObjectByType<WorkerJobManager>();
            _gameManager = FindFirstObjectByType<GameManager>();
            if(_plantingManager == null)
            {
                Debug.LogError("PlantingManager not found in the scene.");
                return;
            }
            _progressBar = Instantiate(progressBarPrefab, canvas.transform).GetComponent<ProgressBar>();
            _progressBar.transform.position = Camera.main.WorldToScreenPoint(transform.position + new Vector3(0, 1.5f, 0));
            onGrowthCycleUpdate.AddListener(UpdateProgressBar);
            SetCropModel(currentGrowthStage);
        }

        public void SetCropModel(GrowthStage growthStage)
        {
            if (cropPrefabs.Length == 0)
            {
                Debug.LogError("No crop prefabs assigned.");
                return;
            }
            for (int i = 0; i < cropPrefabs.Length; i++)
            {
                cropPrefabs[i].SetActive(false);
            }
            switch (growthStage)
            {
                case GrowthStage.New:
                    cropPrefabs[0].SetActive(true);
                    break;
                case GrowthStage.Maturing:
                    cropPrefabs[1].SetActive(true);
                    break;
                case GrowthStage.Harvestable:
                    cropPrefabs[1].SetActive(true);
                    break;
            }
        }   

        public void UpdateProgressBar()
        {
            float progress = 1-(_advancmentCompletionTime - Time.time) / cropData.growthTime;
            _progressBar.UpdateProgressBar(progress);
            _progressBar.progressText.text = $"{(int)(progress * 100)}%";
        }
        /// <summary>
        /// Logic for planting a crop.
        /// </summary>
        private void CropSetup()
        {
            _advancmentCompletionTime = Time.time + cropData.growthTime;
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
                            _waitingForWorker = true;
                            _progressBar.ShowWorkerAlert("Water!",new Color32(154,255,255,255));
                            _workerJobManager.AddJob(WorkerJobType.Gardening, this);
                        }
                        break;
                    case GrowthStage.Maturing:
                        if (CheckGrowthAdvancement())
                        {
                            _waitingForWorker = true;
                            _progressBar.ShowWorkerAlert("Harvest!",Color.yellow);
                            _workerJobManager.AddJob(WorkerJobType.Harvesting, this);
                            AdvanceGrowthCycle();
                        }
                        break;
                    case GrowthStage.Harvestable:
                        // Nothing to do here, the crop is ready to be harvested.
                        break;
                }
                onGrowthCycleUpdate.Invoke(); // other events could be added here that may affect the growth cycle.
                yield return new WaitForSeconds(1f);
            }
        }

        /// <summary>
        /// Advances the growth cycle
        /// </summary>
        public void AdvanceGrowthCycle()
        {
            if (currentGrowthStage == GrowthStage.New)
            {
                _waitingForWorker = false;
                _progressBar.HideWorkerAlert();
                currentGrowthStage = GrowthStage.Maturing;
                _advancmentCompletionTime = Time.time + cropData.growthTime;
                SetCropModel(currentGrowthStage);
            }
            else if (currentGrowthStage == GrowthStage.Maturing)
            {
                currentGrowthStage = GrowthStage.Harvestable;
                _waitingForWorker = true;
                SetCropModel(currentGrowthStage);
            }
            else if(currentGrowthStage == GrowthStage.Harvestable)
            {
                _plantingManager.RemoveCrop(new Vector2Int(Mathf.RoundToInt(transform.position.x), Mathf.RoundToInt(transform.position.z)));
                _gameManager.Money += Mathf.RoundToInt(cropData.cost * cropData.sellPriceMultiplier);
                Destroy(_progressBar.gameObject);
                Destroy(gameObject);
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