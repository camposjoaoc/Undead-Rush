using System;
using System.Collections.Generic;
using UnityEngine;

// Enum para identificar diferentes efeitos sonoros
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

    // Toca o som associado a este SoundInstance
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

    // Lista de instâncias de sons configuradas no Inspector
    [SerializeField] List<SoundInstance> soundInstances = new();

    // Toca o efeito sonoro correspondente ao enum passado
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