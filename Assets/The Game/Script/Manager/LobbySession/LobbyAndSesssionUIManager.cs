using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using Unity.Netcode;
using System.Threading.Tasks;
using Unity.VisualScripting;
using System;
using UnityEditor.Build;
using System.Security.Cryptography;
using UnityEngine.SceneManagement;
using System.Collections;

public class LobbyAndSesssionUIManager : MonoBehaviour
{
    #region Fields & Properties

    public static LobbyAndSesssionUIManager Instance { get; private set; }

    [Header("플레이어 이미지 리스트")]
    [SerializeField] private Sprite[] playerJobImages;

    [Header("UI 프리팹")]
    [SerializeField] private GameObject _sessionListPrefab; 
    [SerializeField] private GameObject _createSessionPrefab; 
    [SerializeField] private GameObject _sessionButtonPrefab; 
    [SerializeField] private GameObject _inSessionPrefab;

    [Header("다음 씬 이름")]
    [SerializeField] private string _sceneName = "";

    private GameObject _sessionListUI;
    private GameObject _createSessionUI;
    private GameObject _inSessionUI;

    private Button _refreshButton;
    private Transform _sessionListContainer;
    private TMP_InputField _sessionCodeInput;
    private TMP_InputField _passwordInput;
    private Button _joinButton;
    private Button _createSessionUIButton;

    private TMP_InputField _sessionNameInput;
    private Toggle _privateToggle;
    private TMP_InputField _createPasswordInput;
    private Button _createButton;
    private Button _cancelButton;

    private Button _disconnectButton;
    private Button _readyButton;
    private Button _startButton;

    private Canvas _JobSelectCanv;
    private Button[] _JobButtons;

    private Transform _playerPanelCanv;
    private List<PlayerUiPanel> _playerPanels = new List<PlayerUiPanel>();

    private bool _isDelay = false;
    private string _savedJoinCode;
    private int _savedPlayerIndex = -1;
    private int _savedJobIndex = 0;
    private int _maxConnections;
    private string _savedOwnName;
    #endregion

    #region Custom Functions

    private Transform FindDeepChild(Transform parent, string name)
    {
        foreach (Transform child in parent)
        {
            if (child.name == name)
                return child;
            Transform found = FindDeepChild(child, name);
            if (found != null)
                return found;
        }
        return null;
    }


    private void ShowInSessionUI()
    {
        _sessionListUI.SetActive(false);
        _createSessionUI.SetActive(false);
        _inSessionUI.SetActive(true);
        RequestSingleTone.Instance.RequestChatONOFF(true);
    }

    private void ShowCreateSessionUI()
    {
        _sessionListUI.SetActive(false);
        _createSessionUI.SetActive(true);
        _inSessionUI.SetActive(false);
        RequestSingleTone.Instance.RequestChatONOFF(false);
    }


    public void HideCreateSessionUI()
    {
        _createSessionUI.SetActive(false);
        _sessionListUI.SetActive(true);
        _inSessionUI.SetActive(false);
        RequestSingleTone.Instance.RequestChatONOFF(false);
    }

    public void HideAllUi()
    {
        _createSessionUI.SetActive(false);
        _sessionListUI.SetActive(false);
        _inSessionUI.SetActive(false);
        RequestSingleTone.Instance.RequestChatONOFF(false);
    }

    public void ShowJobSelectCanv()
    {
        _JobSelectCanv.gameObject.SetActive(true);
    }

    private IEnumerator ButtonDelay(float delayTime)
    {
        _isDelay = true;
        yield return new WaitForSeconds(delayTime);
        _isDelay = false;
    }


    public void UpdateSessionList()
    {
        ClearSessionList();
        List<FireBaseSessionData> sessions = GameRelayManager.Instance.GetSessionList();

        foreach (var session in sessions)
        {
            if(!session.GetIsStartInSessionData())
            {
                GameObject sessionButton = Instantiate(_sessionButtonPrefab, _sessionListContainer);

                //  서버 이름 찾기
                TMP_Text sessionNameText = FindDeepChild(sessionButton.transform, "SessionNameText").GetComponent<TMP_Text>();
                TMP_Text playerCountText = FindDeepChild(sessionButton.transform, "PlayerCountText").GetComponent<TMP_Text>();

                if (sessionNameText == null || playerCountText == null)
                {
                    Debug.LogError("sessionButtonPrefab 내부에 TMP_Text가 없습니다! 프리팹을 확인하세요.");
                    continue;
                }

                // UI 업데이트
                sessionNameText.text = session.SessionName;
                playerCountText.text = $"{session.CurrentPlayers} / {session.MaxPlayers}";

                //  버튼 이벤트 추가
                Button button = sessionButton.GetComponent<Button>();
                if (button != null)
                {
                    button.onClick.AddListener(() => SelectSession(session));
                }
                else
                {
                    Debug.LogError("sessionButtonPrefab에 Button 컴포넌트가 없습니다!");
                }
            }
        }
    }


    private void SetJobIndex(int jobIndex)
    {
        _savedJobIndex = jobIndex;
        LobbyAndSesssionFireBaseManager.Instance.SetSessionPlayerJobIndex(_savedJoinCode, _savedPlayerIndex, jobIndex);
        _JobSelectCanv.gameObject.SetActive(false);
    }
    private void ClearSessionList()
    {
        foreach (Transform child in _sessionListContainer)
        {
            Destroy(child.gameObject);
        }
    }

    private void SelectSession(FireBaseSessionData session)
    {
        _sessionCodeInput.text = session.JoinCode;

        if (session.IsPrivate)
        {
            _passwordInput.interactable = true; // 입력 가능
            _passwordInput.text = ""; // 비밀번호 입력칸 초기화
            _passwordInput.placeholder.GetComponent<TMP_Text>().text = "비밀번호 입력"; // 플레이스홀더 변경 (필요 시)
        }
        else
        {
            _passwordInput.interactable = false; // 입력 불가 (회색 처리됨)
            _passwordInput.text = "비밀번호 없음"; // 기본 메시지
        }
    }

    private async void CreateSession()
    {
        string sessionName = _sessionNameInput.text;
        bool isPrivate = _privateToggle.isOn;
        string password = _createPasswordInput.text;

        if (string.IsNullOrWhiteSpace(sessionName))
        {
            Debug.LogError("세션 이름을 입력하세요!");
            return;
        }

        _createButton.interactable = false;

        string joinCode = await GameRelayManager.Instance.CreateRelay(sessionName, isPrivate, password);
  
        if (!string.IsNullOrEmpty(joinCode))
        {
            Debug.Log($"세션 생성 성공. Join Code: {joinCode}");
            ShowInSessionUI();
            UpdateSessionList();
            _readyButton.gameObject.SetActive(false);
            _startButton.gameObject.SetActive(true);
            _savedJoinCode = joinCode;
        }
        else
        {
            Debug.LogError("세션 생성 실패");
            _savedJoinCode = "";
        }

        _createButton.interactable = true;
        
    }
    private async void JoinSession()
    {
        if (_isDelay) return;
        
        string code = _sessionCodeInput.text;
        string password = _passwordInput.text;

        if(await LobbyAndSesssionFireBaseManager.Instance.GetIsStartInFireBase(code))
        {
            Debug.Log("이미 시작된 세션");
            UpdateSessionList();
            return;
        }

        int currentPlayer = await LobbyAndSesssionFireBaseManager.Instance.GetCurrentPlayer(code);
        if (currentPlayer >= _maxConnections) 
        {
            Debug.Log("이미 최대 인원수 입니다.");
            return;
        }

        bool success = await GameRelayManager.Instance.JoinRelay(code, password);
        if (!success)
        {
            Debug.Log("세션 참가 실패");
            return;
        }

        Debug.Log("세션 참가 성공");

        _savedJoinCode = code;
        ShowInSessionUI();

        _readyButton.gameObject.SetActive(true);
        _startButton.gameObject.SetActive(false);

        StartCoroutine(ButtonDelay(1f));
    }

    private async Task<bool> RequestStartGame(string joinCode)
    {
        try
        {
            bool isAllPlayerReady = await LobbyAndSesssionFireBaseManager.Instance.IsAllPlayerReady(joinCode);
            if (!isAllPlayerReady)
            {
                Debug.Log("모든 플레이어가 준비 되지 않음");
                return false;
            }
            Debug.Log("게임시작");

            if (NetworkManager.Singleton.IsHost)
            {
                NetworkManager.Singleton.SceneManager.LoadScene(_sceneName, LoadSceneMode.Single);
            }
            SceneManager.LoadScene(_sceneName);

        }
        catch (Exception ex)
        {
            Debug.LogError($"RequestStartGame도중 예외 발생{ex}");
            return false;
        }

        LobbyAndSesssionFireBaseManager.Instance.SetIsStartInFireBase(_savedJoinCode, true);


        return true;
    }



    private async void StartGame()
    {
        Debug.Log("스타트 버튼 눌림");
        _startButton.interactable = false;
        await RequestStartGame(_savedJoinCode);
        _startButton.interactable = true;
        Debug.Log("스타트 로직 완수");
    }

    private async void ToggleReady()
    {
        Debug.Log("준비 상태 변경");
        LobbyAndSesssionFireBaseManager.Instance.RequseReversalReadyToggle(_savedJoinCode, TheGameAuthManager.Instance.GetPlayerNickName());
        await Task.Delay(300); // Firebase 값 갱신 시간 확보
        await UpdatePlayerPanels();
    }


    private async void DisconnectSession()
    {
        if (_isDelay) return;

        Debug.Log("DisconnectSession요청");

        HideCreateSessionUI();

        var sessionList = GameRelayManager.Instance.GetSessionList();
        var session = sessionList.Find(s => s.JoinCode == _savedJoinCode);


        Debug.Log($"나갈 세션 코드 :{_savedJoinCode}");
        
        if (NetworkManager.Singleton.IsHost)
        {
            Debug.Log("호스트가 세션을 종료합니다.");
            // 호스트가 세션을 종료하면 모든 클라이언트 연결이 끊어짐
            
            sessionList.Remove(session);

            LobbyAndSesssionFireBaseManager.Instance.RemoveSessionFromFirebase(_savedJoinCode);

            // 세션 리스트에서 해당 세션 제거 (호스트가 떠나면 자동 삭제)
            sessionList.RemoveAll(s => s.JoinCode == _savedJoinCode);

        }
        else if (NetworkManager.Singleton.IsClient)
        {
            Debug.Log("클라이언트가 세션에서 나갑니다.");
            NetworkManager.Singleton.Shutdown();

            await LobbyAndSesssionFireBaseManager.Instance.RemovePlayerFromSession(_savedJoinCode, _savedPlayerIndex);
        }



        foreach (var panel in _playerPanels)
        {
            panel.ResetPanel();
        }

        // UI 업데이트

        _savedPlayerIndex = -1;
        _savedJobIndex = 0;
        _savedJoinCode = null;
        _savedPlayerIndex = -1;
        UpdateSessionList();
        RequestSingleTone.Instance.RequestClearChatContent();

        StartCoroutine(ButtonDelay(1f));
    }



    public string GetSavedJoinCode()
    {
        return _savedJoinCode;
    }

    public void SetOwnPlayerIndex(int index)
    {
        _savedPlayerIndex = index;
    }
    public int GetOwnPlayerIndex()
    {
        return _savedPlayerIndex;
    }

    public async Task<bool> UpdatePlayerPanels()
    {
        for (int i = 0; i < _maxConnections; i++)
        {
            if(_savedJoinCode != null)
            {
                Debug.Log($"{_savedJoinCode}의 {i}번째 플레이어 판낼 출력 시도");


                string playerName = await LobbyAndSesssionFireBaseManager.Instance.GetSessionPlayerName(_savedJoinCode, i);

                if (string.IsNullOrEmpty(playerName) || playerName == "Player")
                {
                    Debug.Log($"{i}번 패널: 플레이어 없음 -> 초기화");
                    _playerPanels[i].ResetPanel();
                    continue;
                }

                bool isReady = await LobbyAndSesssionFireBaseManager.Instance.GetSessionPlayerIsReady(_savedJoinCode, i);
                int playerJobIndex = await LobbyAndSesssionFireBaseManager.Instance.GetSessionPlayerJobIndex(_savedJoinCode, i);
                Debug.Log($"{i} 번쨰 플레이어의 이름 : {playerName}, 준비상태 {isReady}");

                _playerPanels[i].UpdatePanel(playerName.ToString(), isReady, playerJobIndex, playerJobImages,i == 0);

            }
            else
            {
                Debug.Log("저장된 joinCode가 없음");
                return false;
            }
        }
        return true;
    }

    public void ResetAllPlayerPanels()
    {
        foreach (var panel in _playerPanels)
        {
            panel.ResetPanel();
        }
    }

    public void UpdateSinglePlayerPanel(int index, string name, bool isReady, int jobIndex, bool isHost)
    {
        if (index < 0 || index >= _playerPanels.Count) return;
        _playerPanels[index].UpdatePanel(name, isReady, jobIndex, playerJobImages, isHost);
    }
    public void ResetSinglePlayerPanel(int index)
    {
        if (index < 0 || index >= _playerPanels.Count) return;
        _playerPanels[index].ResetPanel();
    }
    #endregion

    #region Unity Built-in Functions


    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        _JobButtons = new Button[4];
    }

    public void DestroySingleton()
    {
        if (Instance != null)
        {
            Destroy(Instance.gameObject);
            Instance = null;
        }
    }


    private void Start()
    {

        _maxConnections = LobbyAndSesssionFireBaseManager.Instance.GetMaxConnection();
        // UI 프리팹을 인스턴스화하고 초기화
        _sessionListUI = Instantiate(_sessionListPrefab, transform);
        _createSessionUI = Instantiate(_createSessionPrefab, transform);
        _createSessionUI.SetActive(false); // 처음엔 세션 생성 UI 숨김
        _inSessionUI = Instantiate(_inSessionPrefab, transform);
        _inSessionUI.SetActive(false);

        _refreshButton = _sessionListUI.transform.Find("RefreshButton").GetComponent<Button>();
        _sessionListContainer = _sessionListUI.transform.Find("SessionList/Viewport/Content").GetComponent<Transform>();
        _sessionCodeInput = _sessionListUI.transform.Find("SessionCodeInput").GetComponent<TMP_InputField>();
        _passwordInput = _sessionListUI.transform.Find("PasswordInput").GetComponent<TMP_InputField>();
        _joinButton = _sessionListUI.transform.Find("JoinButton").GetComponent<Button>();
        _createSessionUIButton = _sessionListUI.transform.Find("CreateSessionButton").GetComponent<Button>();

        _sessionNameInput = FindDeepChild(_createSessionUI.transform, "SessionNameInput").GetComponent<TMP_InputField>();
        _privateToggle = FindDeepChild(_createSessionUI.transform, "PrivateToggle").GetComponent<Toggle>();
        _createPasswordInput = FindDeepChild(_createSessionUI.transform, "PasswordInput").GetComponent<TMP_InputField>();
        _createButton = FindDeepChild(_createSessionUI.transform, "CreateButton").GetComponent<Button>();
        _cancelButton = _createSessionUI.transform.Find("CancleButton").GetComponent<Button>();
        _disconnectButton = _inSessionUI.transform.Find("Disconnect").GetComponent<Button>();
        _readyButton = _inSessionUI.transform.Find("Ready").GetComponent<Button>();
        _startButton = _inSessionUI.transform.Find("Start").GetComponent<Button>();

        _playerPanelCanv = _inSessionUI.transform.Find("PlayerPanelCanv");
        foreach (Transform panel in _playerPanelCanv)
        {
            _playerPanels.Add(new PlayerUiPanel(panel));
        }

        _JobSelectCanv = _inSessionUI.transform.Find("JobSelectCanv").GetComponent<Canvas>();
        for (int i = 0; i < 4; i++)
        {
            _JobButtons[i] = FindDeepChild(_JobSelectCanv.transform, $"Job ({i})").GetComponent<Button>();
        }


        // 버튼 이벤트 연결
        _refreshButton.onClick.AddListener(UpdateSessionList);
        _joinButton.onClick.AddListener(JoinSession);
        _createSessionUIButton.onClick.AddListener(ShowCreateSessionUI);
        _createButton.onClick.AddListener(CreateSession);
        _cancelButton.onClick.AddListener(HideCreateSessionUI);
        _disconnectButton.onClick.AddListener(DisconnectSession);
        _readyButton.onClick.AddListener(ToggleReady);
        _startButton.onClick.AddListener(StartGame);

        for (int i = 0; i < 4; i++)
        {
            int index = i;
            _JobButtons[index].onClick.AddListener(() => SetJobIndex(index + 1));
        }



        _sessionListUI.SetActive(true);
        _createSessionUI.SetActive(false);
        _JobSelectCanv.gameObject.SetActive(false);
    }

    #endregion
}
