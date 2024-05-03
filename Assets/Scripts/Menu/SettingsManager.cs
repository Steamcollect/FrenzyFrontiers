using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

public class SettingsManager : MonoBehaviour
{
	public AudioMixer audioMixer;

	public Slider mainVolumSlider, musicSlider, soundSlider;
	public Toggle fullScreenToggle;

	[HideInInspector] public float mainVolum, musicVolum, soundVolum;
	bool isFullScreen;

	LoadAndSaveData dataSystem;

	private void Awake()
	{
		dataSystem = GetComponent<LoadAndSaveData>();
	}

	void Start()
	{
		LoadParameters();
	}

	public void SetMainVolumVolum(float volum)
	{
		float dB;

		if (volum != 0)
			dB = 20.0f * Mathf.Log10(volum);
		else
			dB = -144.0f;

		mainVolum = dB;
		audioMixer.SetFloat("MainVolum", mainVolum);
	}
	public void SetMusicVolum(float volum)
	{
		float dB;

		if (volum != 0)
			dB = 20.0f * Mathf.Log10(volum);
		else
			dB = -144.0f;

		musicVolum = dB;
		audioMixer.SetFloat("Music", musicVolum);
	}
	public void SetSoundVolum(float volum)
	{
		float dB;

		if (volum != 0)
			dB = 20.0f * Mathf.Log10(volum);
		else
			dB = -144.0f;

		soundVolum = dB;
		audioMixer.SetFloat("Sound", dB);
	}
	public void SetFullScreen(bool isFullScreen)
	{
		Screen.fullScreen = isFullScreen;
		this.isFullScreen = isFullScreen;
	}

	void LoadParameters()
	{
		mainVolum = Mathf.Pow(10.0f, dataSystem.parametersData.masterVolum / 20.0f);
		audioMixer.SetFloat("MainVolum", mainVolum);
		musicVolum = Mathf.Pow(10.0f, dataSystem.parametersData.musicVolum / 20.0f);
		audioMixer.SetFloat("Music", musicVolum);
		soundVolum = Mathf.Pow(10.0f, dataSystem.parametersData.soundVolum / 20.0f);
		audioMixer.SetFloat("Sound", soundVolum);
		isFullScreen = dataSystem.parametersData.isFullScreen;

		mainVolumSlider.value = mainVolum;
		musicSlider.value = musicVolum;
		soundSlider.value = soundVolum;
		fullScreenToggle.isOn = isFullScreen;
	}
	public void SaveParameters()
	{
		dataSystem.parametersData.masterVolum = mainVolum;
		dataSystem.parametersData.musicVolum = musicVolum;
		dataSystem.parametersData.soundVolum = soundVolum;
		dataSystem.parametersData.isFullScreen = isFullScreen;

		dataSystem.SaveParametersData();
	}
}
