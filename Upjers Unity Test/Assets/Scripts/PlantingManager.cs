using System.Collections.Generic;
using Unity.VisualScripting;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
namespace PlantingGame
{
    public class PlantingManager : MonoBehaviour
    {
        public GameObject[] cropPrefabs;
        public GameObject grid;
        public GameObject placementIndicator;
        public GameObject selectedCellIndictaor;
        public UnityEvent<bool> onPlantingModeToggled;
        private bool _plantingMode = false;
        private Camera _mainCamera;
        private Vector2Int _indicatorGridPosition = new Vector2Int(int.MaxValue, int.MaxValue);
        private Vector2Int _selectedGridPosition = new Vector2Int(int.MaxValue, int.MaxValue);
        private Dictionary<Vector2Int,Crop> _crops = new Dictionary<Vector2Int,Crop>();
        private int _selectedCropPrefabIndex = 0;
        private GameManager _gameManager;

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            _gameManager = FindFirstObjectByType<GameManager>();
            TogglePlantingMode(false);
            _mainCamera = Camera.main;
            GameManager.inputActions.Game.PlantCrop.performed += ctx => SelectGridCell();

        }

        private void Update()
        {
            PlaceIndictatorForPC();
        }
        /// <summary>
        /// Place an indicator for the planting mode on PC. not needed for mobile.
        /// </summary>
        private void PlaceIndictatorForPC()
        {
#if UNITY_STANDALONE_WIN || UNITY_STANDALONE_OSX || UNITY_STANDALONE_LINUX
            if (_plantingMode)
            {
                bool hasValidPosition = GetGridPositionOnScreen(out _indicatorGridPosition);
                if (hasValidPosition)
                {
                    placementIndicator.SetActive(true);
                    placementIndicator.transform.position = new Vector3(_indicatorGridPosition.x, 0.05f, _indicatorGridPosition.y);
                }
                else
                {
                    placementIndicator.SetActive(false);
                }
            }
#endif
        }

        /// <summary>
        /// Toggles the planting mode depending on the current plantingMode state.
        /// </summary>
        public void TogglePlantingMode()
        {
            TogglePlantingMode(!_plantingMode);
        }

        /// <summary>
        /// Toggles the planting mode by passed value.
        /// </summary>
        /// <param name="value"></param>
        public void TogglePlantingMode(bool value)
        {
            _plantingMode = value;
            onPlantingModeToggled.Invoke(value);
            if (value)
            {
                grid.SetActive(true);
                GameManager.inputActions.Game.PlantCrop.Enable();

            }
            else
            {
                grid.SetActive(false);
                GameManager.inputActions.Game.PlantCrop.Disable();
            }
        }

        /// <summary>
        /// Selects the grid cell and sets the position of the selected Cell indicator.
        /// </summary>
        /// <returns></returns>
        public bool SelectGridCell()
        {
            bool hasValidPosition = GetGridPositionOnScreen(out _selectedGridPosition);
            if (hasValidPosition)
            {
                PlaceOnSelectedCell();
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Places the selected crop prefab on the selected cell.
        /// </summary>
        public void PlaceOnSelectedCell()
        {
            Crop selectedPrefabCrop = cropPrefabs[_selectedCropPrefabIndex].GetComponent<Crop>();
            // Check if the selected cell is valid
            if (_selectedGridPosition.x == int.MaxValue
                || _selectedGridPosition.y == int.MaxValue
                || _crops.ContainsKey(_selectedGridPosition)
                || _gameManager.Money < selectedPrefabCrop.cost)
            {
                return;
            }
            GameObject go = Instantiate(cropPrefabs[_selectedCropPrefabIndex], new Vector3(_selectedGridPosition.x, 0.01f, _selectedGridPosition.y), Quaternion.identity);
            Crop crop = go.GetComponent<Crop>();
            _gameManager.Money -= (int)crop.cost;
            _crops.Add(_selectedGridPosition, crop);
        }

        /// <summary>
        /// Sets the selected crop prefab index.
        /// </summary>
        /// <param name="index"></param>
        public void SetSelectedCropPrefabIndex(int index)
        {
            if (index < 0 || index >= cropPrefabs.Length)
            {
                Debug.LogError("Invalid crop prefab index.");
                return;
            }
            _selectedCropPrefabIndex = index;
        }

        /// <summary>
        /// Remove Crop from crop dictionary
        /// </summary>
        /// <param name="gridPosition"></param>
        public void RemoveCrop(Vector2Int gridPosition)
        {
            if (_crops.ContainsKey(gridPosition))
            {
                _crops.Remove(gridPosition);
            }
        }

        /// <summary>
        /// Get the grid position on the screen and checks if the position is valid.
        /// </summary>
        /// <returns>is valid position</returns>
        private bool GetGridPositionOnScreen(out Vector2Int gridPosition)
        {
            gridPosition = new Vector2Int(int.MaxValue, int.MaxValue);
            if (_gameManager.isOverUIElement)
            {
                return false;
            }
            Ray ray = _mainCamera.ScreenPointToRay(Input.mousePosition);
            Physics.Raycast(ray, out RaycastHit hit);
            if (hit.collider != null)
            {
                Vector3 hitPoint = hit.point;

                gridPosition = new Vector2Int(Mathf.RoundToInt(hitPoint.x), Mathf.RoundToInt(hitPoint.z));
                return true; // Hit detected
            }
            return false; // No hit detected
        }

        /// <summary>
        /// Toogle Grid UI
        /// </summary>
        private void ToggleGridUI(bool value)
        {
            grid.SetActive(value);
        }


        private void OnValidate()
        {
            // Check if each crop prefab has a Crop component
            foreach (GameObject cropPrefab in cropPrefabs)
            {
                if (cropPrefab.GetComponent<Crop>() == null)
                {
                    Debug.LogError($"The crop prefab ({cropPrefab.name}) must have a Crop component attached.", cropPrefab);

                }
            }
        }
    }
}



