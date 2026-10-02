using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public AudioSource attackAudioSource;
    public AudioSource jumpAudioSource;

    public AudioSource runAudioSource;

    public AudioSource dashAudioSource;

    public AudioSource eatcoinAudioSource;

    public void PlayAttackSound()
    {
        attackAudioSource.Play();   
    }

    public void PlayJumpSound()
    {
        jumpAudioSource.Play();
    }

    public void PlayRunSound()
    {
        runAudioSource.Play();
    }

    public void PlayDashSound()
    {
        dashAudioSource.Play();
    }

    public void PlayEatCoinSound()
    {
        eatcoinAudioSource.Play();
    }
}
