using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Option : MonoBehaviour
{
    #region UI References
    [Header("Option UI Settings")]
    [SerializeField]
    Image _optionBg;
    [SerializeField]
    RectTransform _optionRectTransform;

    [Header("Audio Settings")]
    public Slider bgmSlider;
    public Slider sfxSlider;
    public Toggle muteToggle;

    public AudioSource bgmSource;
    public List<AudioSource> sfxSources = new();
    #endregion

    #region Constants & State
    private const string BGM_KEY = "BGMVolume";
    private const string SFX_KEY = "SFXVolume";
    private const string MUTE_KEY = "Mute";

    private bool isMuted = false;
    private const float defaultVolume = 0.5f;
    #endregion

    #region UI Control
    public void ShowOptionUI()
    {
        _optionRectTransform.localPosition = new Vector3(0f, 0f, 0f);
    }

    public void CloseOptionUI()
    {
        _optionRectTransform.localPosition = new Vector3(0f, 3000f, 0f);
    }
    #endregion

    #region Unity Methods
    void Start()
    {
        float savedBgmVolume = PlayerPrefs.GetFloat(BGM_KEY, defaultVolume);
        float savedSfxVolume = PlayerPrefs.GetFloat(SFX_KEY, defaultVolume);
        int savedMute = PlayerPrefs.GetInt(MUTE_KEY, 0);

        isMuted = savedMute == 1;

        bgmSlider.value = savedBgmVolume;
        sfxSlider.value = savedSfxVolume;
        muteToggle.isOn = isMuted;

        ApplyVolume();

        bgmSlider.onValueChanged.AddListener(OnBgmSliderChanged);
        sfxSlider.onValueChanged.AddListener(OnSfxSliderChanged);
        muteToggle.onValueChanged.AddListener(OnMuteToggleChanged);
    }
    #endregion

    #region UI Button Events
    public void OnBgmSliderChanged(float value)
    {
        PlayerPrefs.SetFloat(BGM_KEY, value);
        ApplyVolume();
    }

    public void OnSfxSliderChanged(float value)
    {
        PlayerPrefs.SetFloat(SFX_KEY, value);
        ApplyVolume();
    }

    public void OnMuteToggleChanged(bool isOn)
    {
        isMuted = isOn;
        PlayerPrefs.SetInt(MUTE_KEY, isOn ? 1 : 0);
        ApplyVolume();
    }

    public void ResetAudioSettings()
    {
        bgmSlider.value = defaultVolume;
        sfxSlider.value = defaultVolume;
        muteToggle.isOn = false;

        PlayerPrefs.SetFloat(BGM_KEY, defaultVolume);
        PlayerPrefs.SetFloat(SFX_KEY, defaultVolume);
        PlayerPrefs.SetInt(MUTE_KEY, 0);

        isMuted = false;

        ApplyVolume();
    }
    #endregion

    #region Volume Application
    private void ApplyVolume()
    {
        float bgmVol = isMuted ? 0f : bgmSlider.value;
        float sfxVol = isMuted ? 0f : sfxSlider.value;

        if (bgmSource != null)
            bgmSource.volume = bgmVol;

        foreach (var sfx in sfxSources)
        {
            if (sfx != null)
                sfx.volume = sfxVol;
        }
    }
    #endregion
}
