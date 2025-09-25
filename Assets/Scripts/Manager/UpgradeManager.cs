using UnityEngine;

public class UpgradeManager : MonoBehaviour
{
    [SerializeField] private Player player;

    public void ApplyUpgrade(string upgradeName)
    {
        if (player == null) return;

        switch (upgradeName)
        {
            case "IncreaseMaxHealth":
                player.IncreaseMaxHealth(20);
                break;

            case "IncreaseMoveSpeed":
                player.IncreaseMoveSpeed(1f);
                break;

            case "UnlockSecondaryWeapon":
                player.UnlockSecondaryWeapon();
                break;

            default:
                Debug.LogWarning("Upgrade not found: " + upgradeName);
                break;
        }
        
        GamesManager.Instance.SwitchState(GamesManager.GameState.Playing);
    }
}