using UnityEngine;
using UnityEngine.UI;

public class Option : MonoBehaviour
{
    [Header("Option UI Settings")]
    [SerializeField]
    Image _optionBg;
    [SerializeField]
    RectTransform _optionRectTransform;

    [Header("Audio Settings")]
    public Slider bgmSlider;
    public Slider sfxSlider;
    public Toggle muteToggle;

    #region UI Control
    public void ShowOptionUI()
    {
        _optionRectTransform.localPosition = new Vector3(0f, 0f, 0f);
    }

    public void CloseOptionUI()
    {
        _optionRectTransform.localPosition = new Vector3(0f, 1000f, 0f);
    }
    #endregion

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
