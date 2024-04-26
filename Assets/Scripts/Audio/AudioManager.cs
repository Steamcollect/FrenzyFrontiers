using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

[RequireComponent(typeof(AudioSource))]
public class AudioManager : MonoBehaviour
{
    public AudioClip[] playlist;
    int currentMusicIndex = -1;
    AudioSource audioSource;

    public Queue<AudioSource> soundsGo = new Queue<AudioSource>();

    public AudioMixer audioMixer;
    public AudioMixerGroup soundMixerGroup;

    public static AudioManager instance;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        instance = this;
    }

    private void Start()
    {
        for (int i = 0; i < 30; i++)
        {
            soundsGo.Enqueue(CreateSoundsGO());
        }
    }

    private void Update()
    {
        if (!audioSource.isPlaying && playlist.Length > 0)
        {
            currentMusicIndex = (currentMusicIndex + 1) % playlist.Length;
            audioSource.clip = playlist[currentMusicIndex];
            audioSource.Play();
        }
    }

    public void PlayClipAt(AudioClip clip, float spatialBlend, Vector3 pos)
    {
        AudioSource tmpAudioSource;
        if (soundsGo.Count <= 0) tmpAudioSource = CreateSoundsGO();
        else tmpAudioSource = soundsGo.Dequeue();

        tmpAudioSource.transform.position = pos;
        tmpAudioSource.spatialBlend = spatialBlend;
        tmpAudioSource.clip = clip;
        tmpAudioSource.Play();
        StartCoroutine(AddAudioSourceToQueue(tmpAudioSource));
    }
    IEnumerator AddAudioSourceToQueue(AudioSource current)
    {
        yield return new WaitForSeconds(current.clip.length);
        soundsGo.Enqueue(current);
    }

    AudioSource CreateSoundsGO()
    {
        AudioSource tmpAudioSource = new GameObject("Audio Go").AddComponent<AudioSource>();
        tmpAudioSource.transform.SetParent(transform);
        tmpAudioSource.outputAudioMixerGroup = soundMixerGroup;
        soundsGo.Enqueue(tmpAudioSource);

        return tmpAudioSource;
    }
}