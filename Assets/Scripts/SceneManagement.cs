using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneManagement : MonoBehaviour
{
    public string _sceneName;

    public void ChangeScene()
    {
        SceneManager.LoadScene(_sceneName);
    }
}
