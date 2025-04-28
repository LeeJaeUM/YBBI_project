using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewPhase", menuName = "Patterns/Phase")]
public class PhasePatternData : ScriptableObject
{
    public int phaseNumber;  // 페이즈 번호
    public List<PatternData> patterns;  // 해당 페이즈에서 사용할 패턴 리스트
}
