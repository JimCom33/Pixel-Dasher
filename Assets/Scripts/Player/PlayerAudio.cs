using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class PlayerAudio : MonoBehaviour
{
    [SerializeField] private AudioClip attackClip;
    [SerializeField, Range(0f, 1f)] private float attackVolume = 0.7f;

    private AudioSource audioSource;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.playOnAwake = false;
        audioSource.loop = false;
        audioSource.spatialBlend = 0f;
    }

    // Called by an animation event near the start of Attack1.
    public void PlayAttackSound()
    {
        if (attackClip != null)
        {
            audioSource.PlayOneShot(attackClip, attackVolume);
        }
    }
}
