using UnityEngine;
using UnityEngine.UI;

public class ShopTabs : MonoBehaviour
{
    public GameObject houseTabContent;
    public GameObject businessTabContent;
    public GameObject energyTabContent;
    public GameObject natureTabContent;

    public Button houseTabButton;
    public Button businessTabButton;
    public Button energyTabButton;
    public Button natureTabButton;

    private void Start()
    {
        houseTabButton.onClick.AddListener(() => ShowTab(houseTabContent));
        businessTabButton.onClick.AddListener(() => ShowTab(businessTabContent));
        energyTabButton.onClick.AddListener(() => ShowTab(energyTabContent));
        natureTabButton.onClick.AddListener(() => ShowTab(natureTabContent));

        ShowTab(houseTabContent); // Show default tab
    }

    private void ShowTab(GameObject tab)
    {
        houseTabContent.SetActive(false);
        businessTabContent.SetActive(false);
        energyTabContent.SetActive(false);
        natureTabContent.SetActive(false);

        tab.SetActive(true);
    }
}