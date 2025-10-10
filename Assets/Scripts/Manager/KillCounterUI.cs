using UnityEngine;
using TMPro;

public class KillCounterUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI killText; // Reference to text component for kill count
    private int currentKills = 0;
    
    // Update kill count UI
    public void UpdateKillCount(int kills)
    {
        currentKills = kills;
        killText.text = kills.ToString();
    }
}