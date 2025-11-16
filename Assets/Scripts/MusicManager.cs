using UnityEngine;
using System.Collections;

public class MusicManager : MonoBehaviour
{
    public AudioSource CurrentTrack; // The audio source that plays music
    public AudioClip DefaultTrack;   // Level/default music
    public float fadeDur = 1f;

    private bool isFading = false;

    private void Start()
    {
        if (DefaultTrack != null)
        {
            CurrentTrack.clip = DefaultTrack;
            CurrentTrack.volume = 1f;
            CurrentTrack.Play();
        }
    }

    // Public method to switch music
    public void PlayMusic(AudioClip newClip)
    {
        // If it's already playing this clip, do nothing
        if (CurrentTrack.clip == newClip) return;

        // Start the fade and switch coroutine
        StartCoroutine(FadeToNewTrack(newClip));
    }

    private IEnumerator FadeToNewTrack(AudioClip newClip)
    {
        isFading = true;

        // Fade out current track
        float startVol = CurrentTrack.volume;
        while (CurrentTrack.volume > 0f)
        {
            CurrentTrack.volume -= startVol * Time.deltaTime / fadeDur;
            yield return null;
        }
        CurrentTrack.Stop();

        // Switch to new clip
        CurrentTrack.clip = newClip;
        CurrentTrack.Play();

        // Fade in new track
        while (CurrentTrack.volume < 1f)
        {
            CurrentTrack.volume += Time.deltaTime / fadeDur;
            yield return null;
        }
        CurrentTrack.volume = 1f;
        isFading = false;
    }

    // Optional: shortcuts for room activation
    public void RoomActivated(AudioClip roomClip)
    {
        if (!isFading)
            PlayMusic(roomClip);
    }

    public void RoomDeactivated()
    {
        if (!isFading && DefaultTrack != null)
            PlayMusic(DefaultTrack);
    }
}