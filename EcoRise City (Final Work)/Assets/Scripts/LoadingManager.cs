using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class LoadingManager : MonoBehaviour
{
    [SerializeField] private Slider progressBar;
    [SerializeField] private string nextSceneName;

    void Start()
    {
        StartCoroutine(LoadSceneAsync());
    }

    IEnumerator LoadSceneAsync()
    {
        float minimumTime = 5f; // minimum display time in seconds
        float timer = 0f;

        AsyncOperation operation = SceneManager.LoadSceneAsync(nextSceneName);
        operation.allowSceneActivation = false; // Prevent auto-switching when done

        while (!operation.isDone)
        {
            // Update progress bar
            float progress = Mathf.Clamp01(operation.progress / 0.9f);
            if (progressBar != null)
                progressBar.value = progress;

            // Increment timer
            timer += Time.deltaTime;

            // If loading is done and 5 seconds have passed, activate the scene
            if (operation.progress >= 0.9f && timer >= minimumTime)
            {
                operation.allowSceneActivation = true;
            }

            yield return null;
        }
    }
}
