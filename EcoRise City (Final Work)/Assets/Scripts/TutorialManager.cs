using UnityEngine;

public class TutorialManager : MonoBehaviour
{
    [SerializeField] private GameObject tutorialPanel; // Your "Tuto" panel

    private void Start()
    {
        // Check if the tutorial was already shown
        if (PlayerPrefs.GetInt("TutorialShown", 0) == 0)
        {
            // Show tutorial
            tutorialPanel.SetActive(true);
        }
        else
        {
            // Skip tutorial
            tutorialPanel.SetActive(false);
        }
    }

    public void OnContinueButton()
    {
        // Hide tutorial panel
        tutorialPanel.SetActive(false);

        // Save preference so it doesn't show again
        PlayerPrefs.SetInt("TutorialShown", 1);
        PlayerPrefs.Save();
    }
}
