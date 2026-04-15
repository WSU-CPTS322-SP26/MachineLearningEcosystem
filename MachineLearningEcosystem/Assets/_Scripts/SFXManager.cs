using System.Collections;
using UnityEngine;

public class SFXManager : MonoBehaviour
{
    public static SFXManager instance;
    [SerializeField] private GameObject sfxPlayerPrefab;
    private ObjectPool<AudioSource> sfxPlayerPool;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        sfxPlayerPool = new(sfxPlayerPrefab.GetComponent<AudioSource>(), 50, transform);
    }

    // Sound that needs only 2d positioning
    public void PlayOmnicientAudioClip(AudioClip clip, float volume = 1f, float pitch = 1f)
    {
        AudioSource source = sfxPlayerPool.Get();
        source.clip = clip;
        source.volume = volume;
        source.pitch = pitch;
        source.spatialBlend = 0f;
        source.Play();
    }

    // Sound that needs to be a 3d positioning
    public void PlayAudioClip(AudioClip clip, Transform placement, float volume = 1f, float pitch = 1f, float minDist = 0f, float maxDist = 100f)
    {
        AudioSource source = sfxPlayerPool.Get();
        source.transform.position = placement.position;
        source.clip = clip;
        source.volume = volume;
        source.pitch = pitch;
        source.spatialBlend = 1f;
        source.minDistance = minDist;
        source.maxDistance = maxDist;
        source.Play();
    }

    private IEnumerator KillSFXPlayer(AudioSource source)
    {
        while (source.isPlaying)
        {
            yield return null;
        }
        source.clip = null;
        sfxPlayerPool.ReturnToPool(source);
    }
}
