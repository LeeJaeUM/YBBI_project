using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using Unity.Netcode;
using System.Threading.Tasks;
using Unity.VisualScripting;
using System;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    [Header("플레이어 이미지 리스트")]
    [SerializeField] private Sprite[] playerJobImages;

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

    private string _savedJoinCode;
    private string _savedPlayerName;

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


    public void HideCreateSessionUI()
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
        _savedJoinCode = joinCode;
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

        _savedJoinCode = code;
        ShowInSessionUI();
    }

    private void ToggleReady()
    {
        Debug.Log("준비 상태 변경");
        NetcodeFireBaseManager.Instance.RequseReversalReadyToggle(_savedJoinCode, _savedPlayerName);
    }

    private void StartGame()
    {
        Debug.Log("게임 시작");
    }


    private async void DisconnectSession()
    {

        Debug.Log("DisconnectSession요청");

        var sessionList = RelayManager.Instance.GetSessionList();
        var session = sessionList.Find(s => s.JoinCode == _savedJoinCode);
        Debug.Log($"나갈 세션 코드 :{_savedJoinCode}");
        
        if (NetworkManager.Singleton.IsHost)
        {
            Debug.Log("호스트가 세션을 종료합니다.");
            // 호스트가 세션을 종료하면 모든 클라이언트 연결이 끊어짐
            
            sessionList.Remove(session);

            NetcodeFireBaseManager.Instance.RemoveSessionFromFirebase(_savedJoinCode);

            // 세션 리스트에서 해당 세션 제거 (호스트가 떠나면 자동 삭제)
            sessionList.RemoveAll(s => s.JoinCode == _savedJoinCode);
        }
        else if (NetworkManager.Singleton.IsClient)
        {
            Debug.Log("클라이언트가 세션에서 나갑니다.");
            bool result = await NetcodeFireBaseManager.Instance.RemovePlayerFromSession(_savedJoinCode, _savedPlayerName);
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
    public void SetSavedPlayerName(string playerName)
    {
        if(_savedPlayerName == null)
        {
            _savedPlayerName = playerName;
        }
        else if(_savedPlayerName != null)
        {
            Debug.Log($"이미 플레이어 이름이 존재함 현재 이름 : {_savedPlayerName}");
        }
        else
        {
            Debug.Log("이름 저장 실패");
        }
    }

    public async Task<bool> UpdatePlayerPanels()
    {
        for (int i = 0; i < 4; i++)
        {
            if(_savedJoinCode != null)
            {
                Debug.Log($"{_savedJoinCode}의 {i}번째 플레이어 판낼 출력 시도");
                string playerName = await NetcodeFireBaseManager.Instance.GetSessionPlayerName(_savedJoinCode, i);
                bool isReady = await NetcodeFireBaseManager.Instance.GetSessionPlayerIsReady(_savedJoinCode, i);
                int playerJobIndex = await NetcodeFireBaseManager.Instance.GetSessionPlayerJobIndex(_savedJoinCode, i);
                Debug.Log($"{i} 번쨰 플레이어의 이름 : {playerName}, 준비상태 {isReady}");
                _playerPanels[i].UpdatePanel(playerName.ToString(), isReady, playerJobIndex, playerJobImages);
                Debug.Log($"_playerPanels[{i}] = {_playerPanels[i].ReturnDataString()}");
                if (_playerPanels[i] == null)
                {
                    _playerPanels[i].ResetPanel();
                }
            }
            else
            {
                Debug.Log("저장된 joinCode가 없음");
                return false;
            }
        }
        return true;
    }

    public class PlayerPanel
    {
        private TextMeshProUGUI _nameText;
        private Toggle _readyToggle;
        private Image _playerJobImage;

        public PlayerPanel(Transform panelTransform)
        {
            _nameText = panelTransform.Find("Name").GetComponent<TextMeshProUGUI>();
            _readyToggle = panelTransform.Find("IsReady").GetComponent<Toggle>();
            _playerJobImage = panelTransform.Find("PlayerJobButton").GetComponent<Image>();
        }

        public void UpdatePanel(string playerName, bool isReady, int playerJobIndex, Sprite[] sprites)
        {
            _nameText.text = playerName;
            _readyToggle.isOn = isReady;

            if (sprites.Length > playerJobIndex)
            {
                _playerJobImage.sprite = sprites[playerJobIndex]; // 인덱스에 따라 이미지 변경
            }
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
}
