using UnityEngine;

public class PlayAudio : MonoBehaviour
{
  [Header("SFX")]
  [SerializeField] private AudioClip[] hitSounds;

  private AudioSource audioSource;

  void Awake()
  {
    audioSource = gameObject.AddComponent<AudioSource>();
    audioSource.playOnAwake = false;
    audioSource.spatialBlend = 0f;
  }

  public void PlayHit()
  {
    if (hitSounds.Length == 0) return;
    var clip = hitSounds[Random.Range(0, hitSounds.Length)];
    audioSource.PlayOneShot(clip);
  }
}
