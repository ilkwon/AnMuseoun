using System;
using UnityEngine;
using UnityEngine.Rendering;

public class SoundManager : Singleton<SoundManager>
{
  [Header("BGM")]
  [SerializeField] private AudioClip[] bgmClips;
  private AudioSource bgmSource;

  [Header("SFX")]
  [SerializeField] private AudioClip[] hitSounds;
  [SerializeField] private AudioClip[] enemyDeathSounds;
  [SerializeField] private AudioClip[] attackWhooshSounds;

  [Header("볼륨")]
  [SerializeField] private float bgmVolume = 0.5f;
  [SerializeField] private float sfxVolume = 1f;
  // Start is called once before the first execution of Update after the MonoBehaviour is created

  protected override void Awake()
  {
    base.Awake();
    bgmSource = gameObject.AddComponent<AudioSource>();
    bgmSource.loop = true;
    bgmSource.playOnAwake = false;
    bgmSource.volume = bgmVolume;
  }
  public void PlayBGM(int index)
  {
    if (index < 0 || index >= bgmClips.Length) return;
    bgmSource.clip = bgmClips[index];
    bgmSource.Play();
  }

  public void StopBGM()
  {
    bgmSource.Stop();
  }

   public void PlayHit()
    {
        PlayRandomSFX(hitSounds);
    }

    public void PlayEnemyDeath()
    {
        PlayRandomSFX(enemyDeathSounds);
    }

    public void PlayAttackWhoosh()
    {
        PlayRandomSFX(attackWhooshSounds);
    }

  private void PlayRandomSFX(AudioClip[] clips)
  {
    if (clips == null || clips.Length == 0) return;
    var clip = clips[UnityEngine.Random.Range(0, clips.Length)];
    AudioSource.PlayClipAtPoint(clip, Camera.main.transform.position, sfxVolume);
  }
}
