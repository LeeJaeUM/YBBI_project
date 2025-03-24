using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerPanel
{
    private TextMeshProUGUI _nameText;
    private Toggle _readyToggle;
    private Button _JobSelectButton;
    private Image _playerJobImage;
    private Image _toggleImage;

    public PlayerPanel(Transform panelTransform)
    {
        _nameText = panelTransform.Find("Name").GetComponent<TextMeshProUGUI>();
        _readyToggle = panelTransform.Find("IsReady").GetComponent<Toggle>();
        _JobSelectButton = panelTransform.Find("PlayerJobButton").GetComponent<Button>();
        _toggleImage = panelTransform.Find("IsReady").GetComponent<Image>();
        _playerJobImage = panelTransform.Find("PlayerJobButton").GetComponent<Image>();

        _JobSelectButton.onClick.AddListener(UIManager.Instance.ShowJobSelectCanv);
    }

    public void UpdatePanel(string playerName, bool isReady, int playerJobIndex, Sprite[] sprites, bool isHost)
    {
        _nameText.text = playerName;
        _readyToggle.isOn = isReady;
        if (sprites.Length > playerJobIndex)
        {
            _playerJobImage.sprite = sprites[playerJobIndex]; // 인덱스에 따라 이미지 변경
        }
        if (!isHost)
        {
            ToggleImageChange(isReady);
        }
    }


    public void ToggleImageChange(bool isReady)
    {
        _toggleImage.color = isReady ? Color.green : Color.white;
    }
    public string GetNameFromPanel()
    {
        return _nameText.text;
    }
    public void ResetPanel()
    {
        _nameText.text = "ID";
        _readyToggle.isOn = false;
        _playerJobImage.sprite = null;
    }
    public string ReturnDataString()
    {
        return $"{_nameText.text}, {_readyToggle.isOn}";
    }
}