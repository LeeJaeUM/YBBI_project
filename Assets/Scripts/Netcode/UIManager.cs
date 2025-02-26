using UnityEngine;
using UnityEngine.UI;
using System.Threading.Tasks;
using TMPro;

public class UIManager : MonoBehaviour
{
    public Button hostButton;
    public Button clientButton;
    public TMP_InputField joinCodeInput;

    private async void Start()
    {
        hostButton.onClick.AddListener(async () => await StartHost());
        clientButton.onClick.AddListener(async () => await JoinClient());
    }

    private async Task StartHost()
    {
        string joinCode = await RelayManager.Instance.CreateRelay();
        if (!string.IsNullOrEmpty(joinCode))
        {
            joinCodeInput.text = joinCode;
        }
    }

    private async Task JoinClient()
    {
        string joinCode = joinCodeInput.text;
        if (!string.IsNullOrEmpty(joinCode))
        {
            await RelayManager.Instance.JoinRelay(joinCode);
        }
    }
}
