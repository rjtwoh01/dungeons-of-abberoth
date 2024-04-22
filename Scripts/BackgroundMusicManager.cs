using UnityEngine;

public class BackgroundMusicManager : MonoBehaviour
{
    public AudioClip[] backgroundMusicTracks;
    private AudioSource audioSource;
    private int currentTrackIndex = 0;
    public bool darkAmbience = false;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        PlayNextTrack();
    }

    void Update()
    {
        // Check if the current track has finished playing and play the next track
        if (!audioSource.isPlaying)
        {
            PlayNextTrack();
        }
    }

    void PlayNextTrack()
    {
        // Loop through the background music tracks array and play the next track
        audioSource.clip = backgroundMusicTracks[currentTrackIndex];
        if (darkAmbience) audioSource.volume = 0.5f;
        audioSource.Play();

        // Increment the track index and loop back to the start if necessary
        currentTrackIndex = (currentTrackIndex + 1) % backgroundMusicTracks.Length;
    }
}