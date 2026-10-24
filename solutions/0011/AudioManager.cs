using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [SerializeField] private AudioSource source;
    [SerializeField] private AudioClip pickup;
    [SerializeField] private AudioClip chestOpen;
    [SerializeField] private AudioClip stoneBreak;
    [SerializeField] private AudioClip portalEnter;
    [SerializeField] private AudioClip uiConfirm;
    [SerializeField] private AudioClip levelComplete;
    [SerializeField] private AudioClip uiError;
    [SerializeField] private AudioClip damage;

    private void Awake()
    {
        if (source == null)
        {
            source = GetComponent<AudioSource>();
        }

        if (source == null)
        {
            source = gameObject.AddComponent<AudioSource>();
        }

        source.playOnAwake = false;
        source.spatialBlend = 0f;
    }

    public void PlayPickup() => Play(pickup);
    public void PlayChestOpen() => Play(chestOpen);
    public void PlayStoneBreak() => Play(stoneBreak);
    public void PlayPortal() => Play(portalEnter);
    public void PlayUiConfirm() => Play(uiConfirm);
    public void PlayLevelComplete() => Play(levelComplete);
    public void PlayUiError() => Play(uiError);
    public void PlayDamage() => Play(damage);

    private void Play(AudioClip clip)
    {
        if (clip == null || source == null)
        {
            return;
        }

        source.PlayOneShot(clip);
    }
}
