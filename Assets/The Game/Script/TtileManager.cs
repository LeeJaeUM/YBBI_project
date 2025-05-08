using UnityEngine;
using UnityEngine.SceneManagement;

public class TtileManager : MonoBehaviour
{
    [SerializeField] private GameObject _settingsUI;
    [SerializeField] private string _nextSceneName = "TheGameAuth";  // 다음 씬 이름

    public void LoadNextScene()
    {
        if (!string.IsNullOrEmpty(_nextSceneName))
        {
            SceneManager.LoadScene(_nextSceneName);
        }
        else
        {
            Debug.LogWarning("다음 씬 이름이 설정되지 않았습니다.");
        }
    }

    public void ToggleSettings()
    {
        if (_settingsUI != null)
        {
            _settingsUI.SetActive(!_settingsUI.activeSelf);
        }
    }

    public void ExitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

}
