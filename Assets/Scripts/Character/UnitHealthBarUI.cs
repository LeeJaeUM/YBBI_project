using UnityEngine;
using UnityEngine.UI;

public class UnitHealthBarUI : MonoBehaviour
{
    public Slider _airSlider;      // 일반 체력 슬라이더
    public Slider _dangerSlider;   // 위험도 슬라이더
    public UnitHealth _unitHealth;

    private float _maxAir = 0;
    private float _safeAirZone = 0;

    private void OnEnable()
    {
        _unitHealth.onChangeAir += ChangeAir;
        _maxAir = _unitHealth.GetMaxAir();
        _safeAirZone = _maxAir * (2f / 3f); // 안정권 계산
    }
    private void OnDisable()
    {
        _unitHealth.onChangeAir -= ChangeAir;
    }

    private void ChangeAir(float value)
    {
        // 일반 체력 슬라이더: (0 ~ _safeAirZone) 구간을 0~1로 매핑
        _airSlider.value = Mathf.Clamp01(value / _safeAirZone);

        // 위험도 슬라이더: (_safeAirZone ~ _maxAir) 구간을 0~1로 매핑
        if (value > _safeAirZone)
        {
            _dangerSlider.value = Mathf.Clamp01((value - _safeAirZone) / (_maxAir - _safeAirZone));
        }
        else
        {
            _dangerSlider.value = 0; // 안전 구간에서는 위험 슬라이더 0
        }
    }
}
