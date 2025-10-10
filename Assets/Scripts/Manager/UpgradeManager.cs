using System;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeManager : MonoBehaviour
{
    [SerializeField] private Player player;
    [SerializeField] private GameObject upgradeUI;
    [SerializeField] private Button shovelButton; 
    [SerializeField] private Button tridentButton;
    private void Update()
    {
        // Disable button if shovel limit is reached
        if (shovelButton != null)
            shovelButton.interactable = player.CurrentShovelCount < player.MaxShovels;
        // Disable button if trident limit is reached
        if (tridentButton != null)
            tridentButton.interactable = !player.HasTrident;
    }

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
            
            case "UnlockTertiaryWeapon":
                player.UnlockTertiaryWeapon();
                break;

            default:
                Debug.LogWarning("[UpgradeManager] Upgrade not found: " + upgradeName);
                break;
        }
        
        GamesManager.Instance.SwitchState(GamesManager.GameState.Playing);
    }
}