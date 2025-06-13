using UnityEngine;

public class BuildingIncome : MonoBehaviour
{
    public int incomePerMinute = 100; // You can set this in the Inspector
    private float incomeTimer;
    private CoinManager coinManager;

    void Start()
    {
        coinManager = FindObjectOfType<CoinManager>();
        incomeTimer = 0f;
    }

    void Update()
    {
        incomeTimer += Time.deltaTime;
        if (incomeTimer >= 60f) // Every 60 seconds
        {
            coinManager.AddCoins(incomePerMinute);
            incomeTimer = 0f;
            Debug.Log("Money earned");
        }
    }
}