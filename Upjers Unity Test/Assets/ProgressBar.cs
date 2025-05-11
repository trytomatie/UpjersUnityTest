using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace PlantingGame
{
    public class ProgressBar : MonoBehaviour
    {
        public Image increaseBar;
        public Image fillBar;
        public GameObject progressBarParent;
        public TextMeshProUGUI progressText;
        public GameObject workerAlertParent;
        public TextMeshProUGUI workerAlertText;
        public float fillDuration = 0.5f;
        public float backFillDelay = 0.1f;
        public float backFillSpeed = 0.1f;
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            HideWorkerAlert();
        }

        /// <summary>
        /// Update progress from 0 to 1
        /// </summary>
        /// <param name="progress"></param>
        public void UpdateProgressBar(float progress)
        {
            StartCoroutine(UpdateProgressBarCoroutine(progress));
        }

        /// <summary>
        /// Show a worker alert with a message.
        /// </summary>
        /// <param name="text"></param>
        public void ShowWorkerAlert(string text)
        {
            progressBarParent.SetActive(false);
            workerAlertParent.SetActive(true);
            workerAlertText.text = text;
        }

        /// <summary>
        /// Show a worker alert with a message.
        /// </summary>
        /// <param name="text"></param>
        public void ShowWorkerAlert(string text,Color color)
        {
            workerAlertText.color = color;
            ShowWorkerAlert(text);
        }

        public void HideWorkerAlert()
        {
            progressBarParent.SetActive(true);
            workerAlertParent.SetActive(false);
            fillBar.fillAmount = 0;
            increaseBar.fillAmount = 0;
            progressText.text = "0%";
        }

        IEnumerator UpdateProgressBarCoroutine(float targetProgress)
        {
            float startProgress = fillBar.fillAmount;
            float elapsedTime = 0f;

            while (elapsedTime < fillDuration)
            {
                elapsedTime += Time.deltaTime;
                float progress = Mathf.Lerp(startProgress, targetProgress, elapsedTime / fillDuration);
                increaseBar.fillAmount = progress;
                yield return null;
            }
            yield return new WaitForSeconds(backFillDelay);
            elapsedTime = 0;
            while (elapsedTime < backFillSpeed)
            {
                elapsedTime += Time.deltaTime;
                float progress = Mathf.Lerp(startProgress, targetProgress, elapsedTime / backFillSpeed);
                fillBar.fillAmount = progress;
                yield return null;
            }
        }
    }
}

