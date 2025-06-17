using TMPro;
using UnityEngine;

public class EcoTipsDisplay : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI tipText;
    [TextArea]
    public string[] ecoTips;

    void Start()
    {
        if (ecoTips.Length > 0)
        {
            string randomTip = ecoTips[Random.Range(0, ecoTips.Length)];
            tipText.text = randomTip;
        }
    }
}
