using Unity.VisualScripting;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;
using UnityEngine.Events;

public class PlantingManager : MonoBehaviour
{
    public GameObject[] cropPrefabs;
    public GameObject grid;
    public GameObject placementIndicator;
    private bool plantingMode = false;
    private Camera mainCamera;
    private bool hasValidPosition = false;
    private Vector2Int gridPosition = new Vector2Int(int.MaxValue, int.MaxValue);

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        grid.SetActive(false);
        mainCamera = Camera.main;
        TogglePlantingMode(true);
    }

    private void Update()
    {
        if (plantingMode)
        {
            hasValidPosition = GetGridPositionOnScreen();
            if (hasValidPosition)
            {
                print(gridPosition);
                placementIndicator.transform.position = new Vector3(gridPosition.x, 0.05f, gridPosition.y);
            }
        }
    }

    /// <summary>
    /// Toggles the planting mode depending on the current plantingMode state.
    /// </summary>
    public void TogglePlantingMode()
    {
        TogglePlantingMode(!plantingMode);
    }

    /// <summary>
    /// Toggles the planting mode by passed value.
    /// </summary>
    /// <param name="value"></param>
    public void TogglePlantingMode(bool value)
    {
        plantingMode = value;
        if (value)
        {
            grid.SetActive(true);
            placementIndicator.SetActive(true);
        }
        else
        {
            grid.SetActive(false);
            placementIndicator.SetActive(false);
        }
    }

    /// <summary>
    /// Get the grid position on the screen and checks if the position is valid.
    /// </summary>
    /// <returns>is valid position</returns>
    private bool GetGridPositionOnScreen()
    {
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
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



