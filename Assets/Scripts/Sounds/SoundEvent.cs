using UnityEngine;

[CreateAssetMenu(fileName = "SoundEvent", menuName = "Audio/Sound Event")]
public class SoundEvent : ScriptableObject
{
    public AudioClip clip;

    [Header("Sound Settings")]
    [Range(0f, 1f)]
    public float volume = 1f;
    [Range(-3f, 3f)]
    public float pitch = 1f;
    [Range(0f, 0.2f)]
    public float pitchRandomness = 0f;
    public bool loop = false;

    public void Play(AudioSource source)
    {
        if (source == null || clip == null) return;

        source.clip = clip;
        source.volume = volume;
        source.pitch = pitch;
        source.loop = loop;
        source.Play();
    }

    public void PlayOneShot(AudioSource source)
    {
        if (source == null || clip == null) return;

        source.volume = volume;
        source.pitch = pitch + Random.Range(-pitchRandomness, pitchRandomness);
        source.PlayOneShot(clip);
    }
}
