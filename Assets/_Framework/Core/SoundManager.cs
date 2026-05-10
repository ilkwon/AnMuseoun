using System;
using UnityEngine;
using UnityEngine.Rendering;

public class SoundManager : Singleton<SoundManager>
{
  [Header("BGM")]
  [SerializeField] private AudioClip[] bgmClips;
  private AudioSource bgmSource;

  [Header("SFX")]
  
  [SerializeField] private AudioClip[] enemyDeathSounds;
  [SerializeField] private AudioClip[] attackWhooshSounds;
  [SerializeField] private AudioClip[] hitNormalSounds;
  [SerializeField] private AudioClip[] hitCriticalSounds;

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

  public void PlayHit(bool isCritical = false)
  {        
    float volume = isCritical ? 1f : 0.7f;
    float pitch = isCritical ? 1.2f : 1f;

    if (isCritical)
      PlayRandomSFX(hitCriticalSounds, volume: volume, pitch: pitch, spatialBlend: 0.5f);
    else
      PlayRandomSFX(hitNormalSounds, volume: volume, pitch: pitch, spatialBlend: 0.5f);
  }

  //---------------------------------------------------------------------------
  public void PlayEnemyDeath()
  {
    PlayRandomSFX(enemyDeathSounds);
  }

  //---------------------------------------------------------------------------
  public void PlayAttackWhoosh()
  {
    PlayRandomSFX(attackWhooshSounds);
  }

  //---------------------------------------------------------------------------
  private void PlayRandomSFX(AudioClip[] clips, float volume = 1f, float pitch = 1f, float spatialBlend = 0f)
  {
    if (clips == null || clips.Length == 0) return;
    var clip = clips[UnityEngine.Random.Range(0, clips.Length)];
    
    PlaySFX(clip, volume: volume, pitch: pitch, spatialBlend: spatialBlend);
  }

  //---------------------------------------------------------------------------
  private void PlaySFX(AudioClip clip, float volume = 1f, float pitch = 1f,float spatialBlend = 0f)
  {
    if (clip == null) return;
    var go = new GameObject("SFX_" + clip.name);
    go.transform.position = Camera.main.transform.position;
    var source = go.AddComponent<AudioSource>();
    source.clip = clip;
    source.volume = volume * sfxVolume;
    source.pitch = pitch;
    source.spatialBlend = spatialBlend;
    source.Play();
    Destroy(go, clip.length + 0.1f);    
  }

  //---------------------------------------------------------------------------
}
