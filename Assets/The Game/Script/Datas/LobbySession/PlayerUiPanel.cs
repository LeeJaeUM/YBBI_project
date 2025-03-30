using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerUiPanel
{
    private TextMeshProUGUI _nameText;
    private Toggle _readyToggle;
    private Button _JobSelectButton;
    private Image _playerJobImage;
    private Image _toggleImage;

    public PlayerUiPanel(Transform panelTransform)
    {
        _nameText = panelTransform.Find("Name")?.GetComponent<TextMeshProUGUI>();
        _readyToggle = panelTransform.Find("IsReady")?.GetComponent<Toggle>();
        _JobSelectButton = panelTransform.Find("PlayerJobButton")?.GetComponent<Button>();
        _toggleImage = panelTransform.Find("IsReady")?.GetComponent<Image>();
        _playerJobImage = panelTransform.Find("PlayerJobButton")?.GetComponent<Image>();

        if (_JobSelectButton != null)
        {
            _JobSelectButton.onClick.AddListener(LobbyAndSesssionUIManager.Instance.ShowJobSelectCanv);
        }
    }

    public void UpdatePanel(string playerName, bool isReady, int playerJobIndex, Sprite[] sprites, bool isHost)
    {
        if (_nameText != null)
            _nameText.text = playerName;

        if (_readyToggle != null)
            _readyToggle.isOn = isReady;

        if (_playerJobImage != null && sprites != null && playerJobIndex >= 0 && playerJobIndex < sprites.Length)
            _playerJobImage.sprite = sprites[playerJobIndex];

        if (!isHost)
        {
            ToggleImageChange(isReady);
        }
    }

    public void ToggleImageChange(bool isReady)
    {
        if (_toggleImage != null)
            _toggleImage.color = isReady ? Color.green : Color.white;
    }

    public string GetNameFromPanel()
    {
        return _nameText != null ? _nameText.text : "Unknown";
    }

    public void ResetPanel()
    {
        if (_nameText != null)
            _nameText.text = "ID";

        if (_readyToggle != null)
            _readyToggle.isOn = false;

        if (_playerJobImage != null && _playerJobImage.gameObject != null)
            _playerJobImage.sprite = null;
    }

    public string ReturnDataString()
    {
        return $"{_nameText?.text ?? "Unknown"}, {_readyToggle?.isOn.ToString() ?? "false"}";
    }
}


