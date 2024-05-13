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

    bool isChanging = false;

    SettingsManager settingsManager;

    public static AudioManager instance;

    private void Awake()
    {
        settingsManager = FindFirstObjectByType<SettingsManager>();
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
        if (!audioSource.isPlaying && playlist.Length > 0 && !isChanging)
        {
            currentMusicIndex = (currentMusicIndex + 1) % playlist.Length;
            StartCoroutine(ChangeMusic(playlist[currentMusicIndex]));
        }
    }

    public IEnumerator ChangeMusic(AudioClip music)
    {
        isChanging = true;
        float initialVolum = settingsManager.musicVolum;
        float volum = initialVolum;

        while (volum > 0)
        {
            volum -= .3f * Time.deltaTime;
            audioMixer.SetFloat("Music", LinearToDecibel(volum));
            yield return null;
        }

        yield return new WaitForSeconds(.5f);

        audioSource.clip = music;
        audioSource.Play();

        while (volum < initialVolum)
        {
            volum += .3f * Time.deltaTime;
            audioMixer.SetFloat("Music", LinearToDecibel(volum));
            yield return null;
        }

        volum = initialVolum;
        audioMixer.SetFloat("Music", LinearToDecibel(volum));
        isChanging = false;
    }
    float LinearToDecibel(float volum)
    {
        float dB;

        if (volum != 0)
            dB = 20.0f * Mathf.Log10(volum);
        else
            dB = -144.0f;

        return dB;
    }

    /// <summary>
    /// spatial blend : 0 = 2D, 1 = 3D
    /// </summary>
    /// <param name="clip"></param>
    /// <param name="spatialBlend"></param>
    /// <param name="pos"></param>
    public void PlayClipAt(AudioClip clip, float spatialBlend, Vector3 pos)
    {
        AudioSource tmpAudioSource;
        if (soundsGo.Count <= 0) soundsGo.Enqueue(CreateSoundsGO());
        tmpAudioSource = soundsGo.Dequeue();

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
        return tmpAudioSource;
    }
}