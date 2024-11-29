using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundFXManager : MonoBehaviour
{
    public static SoundFXManager Instance;

    [SerializeField] private AudioSource soundFXObject;

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }
    }

    public void PlaySoundFXClip(AudioClip audioClip, Transform spawnTransform, float volume)
    {
        if (audioClip == null)
        {
            Debug.Log("AudioClip is null. Assign a clip to play.");
            return;
        }

        Debug.Log($"Playing sound: {audioClip.name} at volume: {volume}");

        AudioSource audioSource = Instantiate(soundFXObject, spawnTransform.position, Quaternion.identity);

        audioSource.clip = audioClip;

        audioSource.volume = volume;

        audioSource.Play();

        float clipLenght = audioSource.clip.length;

        Destroy(audioSource.gameObject, clipLenght);
    }    
    
    public void PlayRandomSoundFXClip(AudioClip[] audioClip, Transform spawnTransform, float volume)
    {
        int rand = Random.Range(0, audioClip.Length);

        if (audioClip == null)
        {
            Debug.Log("AudioClip is null. Assign a clip to play.");
            return;
        }

        Debug.Log($"Playing sound: {audioClip[rand].name} at volume: {volume}");


        AudioSource audioSource = Instantiate(soundFXObject, spawnTransform.position, Quaternion.identity);

        audioSource.clip = audioClip[rand];

        audioSource.volume = volume;

        audioSource.Play();

        float clipLenght = audioSource.clip.length;

        Destroy(audioSource.gameObject, clipLenght);
    }

    public bool IsSoundPlaying(AudioClip clip)
    {
        // Recherchez un AudioSource actif avec ce clip
        AudioSource[] audioSources = FindObjectsOfType<AudioSource>();
        foreach (AudioSource source in audioSources)
        {
            if (source.clip == clip && source.isPlaying)
            {
                return true; // Le clip est déjà en train d'être joué
            }
        }
        return false; // Le clip n'est pas en train d'être joué
    }

    public void StopSoundFXClip(AudioClip clip)
    {
        // Trouve tous les AudioSources dans la scène
        AudioSource[] audioSources = FindObjectsOfType<AudioSource>();
        foreach (AudioSource source in audioSources)
        {
            // Si l'AudioSource joue ce clip, on l'arrête
            if (source.clip == clip && source.isPlaying)
            {
                source.Stop();
            }
        }
    }

}