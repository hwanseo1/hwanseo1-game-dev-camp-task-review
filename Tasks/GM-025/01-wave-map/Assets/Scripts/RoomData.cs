using UnityEngine;

// 룸 하나의 웨이브 데이터. 로직(Room / Monster)은 같고, 이 값만 바꿔서 룸마다 난이도를 다르게 한다.
[CreateAssetMenu(fileName = "RoomData", menuName = "GM-025/Room Data")]
public class RoomData : ScriptableObject
{
    [Header("웨이브")]
    public int spawnCount = 1;

    [Header("몬스터 스탯")]
    public int maxHp = 3;
    public int attackDamage = 5;
    public float attackInterval = 1.5f;   // 공격 간격(초)
    public float attackRange = 1.4f;      // 플레이어와의 수평 중심 거리가 이 안이면 공격

    [Header("크기")]
    public float scale = 1f;              // 몬스터 루트 스케일. NavMeshAgent 반지름 · 높이도 같이 커진다
    public Color color = Color.white;

    [Header("NavMeshAgent 세팅")]
    public string agentTypeName = "Medium"; // Navigation 창의 Agent Type 이름. 룸 NavMesh도 같은 타입으로 굽는다
    public float moveSpeed = 3f;
    public float acceleration = 8f;
    public float angularSpeed = 240f;
    public float stoppingDistance = 1.2f;
    public int avoidancePriority = 50;    // 낮을수록 우선 (강한 몬스터가 약한 몬스터를 밀어낸다)
}
