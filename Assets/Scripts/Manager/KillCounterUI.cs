using UnityEngine;
using TMPro;

public class KillCounterUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI killText; // Referência ao componente de texto para exibir as kills
    private int currentKills = 0;

    /// <summary>
    /// Atualiza o contador de kills na UI.
    /// </summary>
    public void UpdateKillCount(int kills)
    {
        currentKills = kills;
        killText.text = kills.ToString();
    }
}