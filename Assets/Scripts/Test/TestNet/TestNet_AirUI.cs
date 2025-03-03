using UnityEngine;
using UnityEngine.UI;

public class TestNet_AirUI : MonoBehaviour
{
    public Slider _airSlider;      // 일반 체력 슬라이더
    public Slider _dangerSlider;   // 위험도 슬라이더
    public TestNet_CharAir _charAir;

    private float _maxAir = 0;
    private float _safeAirZone = 0;

    private void OnEnable()
    {
        _charAir.onChangeAir += ChangeAir;
        _maxAir = _charAir.GetMaxAir();
        _safeAirZone = _maxAir * (2f / 3f); // 안정권 계산
    }
    private void OnDisable()
    {
        _charAir.onChangeAir -= ChangeAir;
    }

    private void ChangeAir(float value)
    {
        // 일반 체력 슬라이더 (0 ~ maxAir)
        _airSlider.value = value / _maxAir;

        // 위험도 슬라이더 (safeAirZone ~ maxAir 사이 값만 표현)
        if (value >= _safeAirZone)
        {
            float dangerValue = (value - _safeAirZone) / (_maxAir - _safeAirZone);
            _dangerSlider.value = dangerValue;
        }
        else
        {
            _dangerSlider.value = 0; // 안전 구간에서는 위험 슬라이더 0
        }
    }
}
