using System;
using System.Collections.Generic;
using UnityEngine;

// Enum for identity differents sounds effects
public enum SoundEffects
{
    EnemyDeath,
    EnemyAttack,
    PlayerShoot,
    PlayerDeath,
    LevelUp,
    BackgroundSound,
    MenuBackgroundSound,
    GameOver
}

[Serializable]
public struct SoundInstance
{
    public SoundEffects Effects;
    [SerializeField] AudioSource source;
    
    public void PlaySound()
    {
        source.Play();
    }
}

public class SoundManager : MonoBehaviour
{
    #region Singleton

    // Instância única do SoundManager
    public static SoundManager Instance;

    private void Awake()
    {
        // Garante que só exista um SoundManager na cena
        if (Instance != null && Instance != this)
        {
            Debug.LogError(
                "[SoundManager] Multiple SoundManager instances detected! There should only be one SoundManager in the scene.");
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    #endregion
    
    // Sound Instance List
    [SerializeField] List<SoundInstance> soundInstances = new();
    
    // Plays sound effect accoring to enum
    public void PlaySoundEffect(SoundEffects anEffect)
    {
        for (int i = 0; i < soundInstances.Count; i++)
        {
            if (soundInstances[i].Effects == anEffect)
            {
                soundInstances[i].PlaySound();
                return;
            }
        }
        Debug.LogError("[SoundManager] Sound effect not found: " + anEffect);
    }
}