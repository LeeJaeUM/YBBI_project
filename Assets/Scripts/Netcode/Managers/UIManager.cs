using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using Unity.Netcode;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    [Header("UI 프리팹")]
    [SerializeField] private GameObject _sessionListPrefab; 
    [SerializeField] private GameObject _createSessionPrefab; 
    [SerializeField] private GameObject _sessionButtonPrefab; 
    [SerializeField] private GameObject _inSessionPrefab;


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


    private Transform _playerPanelCanv;
    private List<PlayerPanel> _playerPanels = new List<PlayerPanel>();

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
    }

    private void Start()
    {
        // UI 프리팹을 인스턴스화하고 초기화
        _sessionListUI = Instantiate(_sessionListPrefab, transform);
        _createSessionUI = Instantiate(_createSessionPrefab, transform);
        _createSessionUI.SetActive(false); // 처음엔 세션 생성 UI 숨김
        _inSessionUI = Instantiate(_inSessionPrefab, transform);
        _inSessionUI.SetActive(false);


        // UI 오브젝트에서 필요한 요소 찾기 (세션 리스트 UI)
        _refreshButton = _sessionListUI.transform.Find("RefreshButton").GetComponent<Button>();
        _sessionListContainer = _sessionListUI.transform.Find("SessionList/Viewport/Content").GetComponent<Transform>();
        _sessionCodeInput = _sessionListUI.transform.Find("SessionCodeInput").GetComponent<TMP_InputField>();
        _passwordInput = _sessionListUI.transform.Find("PasswordInput").GetComponent<TMP_InputField>();
        _joinButton = _sessionListUI.transform.Find("JoinButton").GetComponent<Button>();
        _createSessionUIButton = _sessionListUI.transform.Find("CreateSessionButton").GetComponent<Button>();

        // UI 오브젝트에서 필요한 요소 찾기 (세션 생성 UI)
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
            _playerPanels.Add(new PlayerPanel(panel));
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


        _sessionListUI.SetActive(true);
        _createSessionUI.SetActive(false);
    }

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
    }

    private void ShowCreateSessionUI()
    {
        _sessionListUI.SetActive(false);
        _createSessionUI.SetActive(true);
        _inSessionUI.SetActive(false);
    }


    private void HideCreateSessionUI()
    {
        _createSessionUI.SetActive(false);
        _sessionListUI.SetActive(true);
        _inSessionUI.SetActive(false);
    }


    public void UpdateSessionList()
    {
        ClearSessionList();
        List<SessionData> sessions = RelayManager.Instance.GetSessionList();

        foreach (var session in sessions)
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
    private void ClearSessionList()
    {
        foreach (Transform child in _sessionListContainer)
        {
            Destroy(child.gameObject);
        }
    }

    private void SelectSession(SessionData session)
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

        string joinCode = await RelayManager.Instance.CreateRelay(sessionName, isPrivate, password);
        if (!string.IsNullOrEmpty(joinCode))
        {
            Debug.Log($"세션 생성 성공. Join Code: {joinCode}");
            ShowInSessionUI();
            UpdateSessionList();
        }
        else
        {
            Debug.LogError("세션 생성 실패");
        }
    }
    private async void JoinSession()
    {
        string code = _sessionCodeInput.text;
        string password = _passwordInput.text;

        bool success = await RelayManager.Instance.JoinRelay(code, password);
        if (!success)
        {
            Debug.Log("세션 참가 실패");
            return;
        }
        Debug.Log("세션 참가 성공");

        ShowInSessionUI();
    }

    private void ToggleReady()
    {
        Debug.Log("준비 상태 변경");
        
    }

    private void StartGame()
    {
        Debug.Log("게임 시작");
    }

    private async void DisconnectSession()
    {
        Debug.Log("DisconnectSession요청");
        string joinCode = _sessionCodeInput.text;
        var sessionList = RelayManager.Instance.GetSessionList();
        var session = sessionList.Find(s => s.JoinCode == joinCode);

        RelayManager.Instance.GetSessionIdByJoinCode(joinCode);
        

        if (NetworkManager.Singleton.IsHost)
        {
            Debug.Log("호스트가 세션을 종료합니다.");
            // 호스트가 세션을 종료하면 모든 클라이언트 연결이 끊어짐
            
            sessionList.Remove(session);

            RelayManager.Instance.RemoveSessionFromFirebase(joinCode);

            // 세션 리스트에서 해당 세션 제거 (호스트가 떠나면 자동 삭제)
            sessionList.RemoveAll(s => s.JoinCode == joinCode);
        }
        else if (NetworkManager.Singleton.IsClient)
        {
            Debug.Log("클라이언트가 세션에서 나갑니다.");

        }
        NetworkManager.Singleton.Shutdown();


        foreach (var panel in _playerPanels)
        {
            panel.ResetPanel();
        }

        
        // UI 업데이트
        HideCreateSessionUI();
        UpdateSessionList();
    }

    public void UpdatePlayerPanels(List<PlayerData> players)
    {
        for (int i = 0; i < _playerPanels.Count; i++)
        {
            if (i < players.Count)
            {
                _playerPanels[i].UpdatePanel(players[i].PlayerID.ToString(), players[i].IsReady.Value);
            }
            else
            {
                _playerPanels[i].ResetPanel();
            }
        }
    }

    public class PlayerPanel
    {
        private TextMeshProUGUI _idText;
        private Toggle _readyToggle;

        public PlayerPanel(Transform panelTransform)
        {
            _idText = panelTransform.Find("ID").GetComponent<TextMeshProUGUI>();
            _readyToggle = panelTransform.Find("IsReady").GetComponent<Toggle>();
        }

        public void UpdatePanel(string id, bool isReady)
        {
            _idText.text = id;
            _readyToggle.isOn = isReady;
        }

        public void ResetPanel()
        {
            _idText.text = "ID";
            _readyToggle.isOn = false;
        }
    }
}
