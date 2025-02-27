using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SkillCoolTimeCtrl : MonoBehaviour
{
    public Image _coolTimeImg;
    public Button _baseAtkBtn;
    public TextMeshProUGUI _coolTimeText;
    public float _coolTime = 5.0f;

    private float _timer;
    private bool _isCoolTime = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _baseAtkBtn.onClick.AddListener(OnClickButton);
        _coolTimeText.gameObject.SetActive(false);

        if (_coolTimeImg != null)
        {
            _coolTimeImg.fillAmount = 1;
        }
    }

    // Update is called once per frame
    void Update()
    { 
        if (_isCoolTime)
        {
            if (_coolTimeImg != null)
            {
                _coolTimeImg.fillAmount = 0;
            }

            _timer -= Time.deltaTime;
            _coolTimeText.text = _timer.ToString("F2");

            if (_coolTimeImg != null)
            {
                _coolTimeImg.fillAmount = _timer / _coolTime;
            }

            if (_timer <= 0)
            {
                _isCoolTime = false;
                _coolTimeText.gameObject.SetActive(false);

                if (_coolTimeImg != null)
                {
                    _coolTimeImg.fillAmount = 1;
                }

                _baseAtkBtn.interactable = true;
            }
        }
    }

    void OnClickButton()
    {
        if (!_isCoolTime)
        {
            _isCoolTime = true;
            _timer = _coolTime;
            _coolTimeText.gameObject.SetActive(true);
            _baseAtkBtn.interactable = false;
        }
    }
}
