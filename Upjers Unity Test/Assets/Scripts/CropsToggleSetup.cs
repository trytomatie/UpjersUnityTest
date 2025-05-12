using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace PlantingGame
{
    [RequireComponent(typeof(ToggleHelper))]
    public class CropsToggleSetup : MonoBehaviour
    {
        public TextMeshProUGUI costText;
        public TextMeshProUGUI nameText;
        public Image icon;

        /// <summary>
        /// Sets up the crop toggle with the given crop data.
        /// </summary>
        /// <param name="cropData"></param>
        public void SetUpCrop(int cropDataIndex, CropData cropData)
        {
            PlantingManager plantingManager = FindFirstObjectByType<PlantingManager>();
            ToggleHelper toggleHelper = GetComponent<ToggleHelper>();
            costText.text = cropData.cost.ToString();
            nameText.text = cropData.cropName;
            icon.sprite = cropData.icon;
            toggleHelper.onToggleIsOn.AddListener(() =>
            {
                // Set the selected crop prefab index in the PlantingManager
                plantingManager.SetSelectedCropPrefabIndex(cropDataIndex);
            });
        }
    }
}

