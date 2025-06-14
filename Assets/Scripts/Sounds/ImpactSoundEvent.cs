using UnityEngine;

[CreateAssetMenu(fileName = "ImpactSoundEvent", menuName = "Audio/Impact Sound Event")]
public class ImpactSoundEvent : ScriptableObject
{
    public AudioClip clip;

    [Header("Sound Settings")]
    [Range(0f, 1f)]
    public float baseVolume = 1f;
    [Range(-3f, 3f)]
    public float basePitch = 1f;
    [Range(0f, 0.2f)]
    public float pitchRandomness = 0f;

    [Header("Impact Settings")]
    /// <summary>
    /// min relative velocity to trigger sound
    /// </summary>
    public float impactVelocityThreshold = 0.1f;
    /// <summary>
    /// min relative velocity for volume scaling
    /// </summary>
    public float impactVelocityMin = 0.1f;
    /// <summary>
    /// max relative velocity for volume scaling
    /// </summary>
    public float impactVelocityMax = 10f;


    public float GetScaledVolume(float impactVelocity)
    {
        float clampedVelocity = Mathf.Clamp(impactVelocity, impactVelocityMin, impactVelocityMax);
        float normalizedVelocity = Mathf.InverseLerp(impactVelocityMin, impactVelocityMax, clampedVelocity);
        return baseVolume * normalizedVelocity;
    }

    public float GetRandomizedPitch()
    {
        return basePitch + Random.Range(-pitchRandomness, pitchRandomness);
    }

    public void PlayOneShot(AudioSource source, float impactRelativeVelocity)
    {
        if (source == null || clip == null) return;
        if (impactRelativeVelocity < impactVelocityThreshold) return;

        source.volume = GetScaledVolume(impactRelativeVelocity);
        source.pitch = (pitchRandomness == 0f) ? basePitch : GetRandomizedPitch();
        source.PlayOneShot(clip);
    }
}
