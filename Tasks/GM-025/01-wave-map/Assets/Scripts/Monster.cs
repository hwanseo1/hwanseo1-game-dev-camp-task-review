using UnityEngine;
using UnityEngine.AI;

// 모든 룸이 같은 AI를 쓴다: 플레이어를 쫓아가고, 사거리 안이면 공격 간격마다 공격한다.
// 룸마다 다른 것은 Init에 들어오는 RoomData 값뿐이다.
[RequireComponent(typeof(NavMeshAgent))]
public class Monster : MonoBehaviour
{
    public Renderer body;

    public RoomData Data { get; private set; }
    public int Hp { get; private set; }
    public bool IsDead => Hp <= 0;

    NavMeshAgent agent;
    PlayerController target;
    Room room;
    float attackTimer;

    public void Init(RoomData data, Room owner, PlayerController player, Vector3 position)
    {
        Data = data;
        room = owner;
        target = player;
        Hp = data.maxHp;

        transform.localScale = Vector3.one * data.scale;
        body.material.color = data.color;

        agent = GetComponent<NavMeshAgent>();
        agent.agentTypeID = AgentTypeId(data.agentTypeName);
        agent.speed = data.moveSpeed;
        agent.acceleration = data.acceleration;
        agent.angularSpeed = data.angularSpeed;
        agent.stoppingDistance = data.stoppingDistance;
        agent.avoidancePriority = data.avoidancePriority;

        // 프리팹은 Agent가 꺼진 채로 둔다. 타입을 바꾼 뒤 켜야 그 타입의 NavMesh(= 이 룸 NavMesh)에 올라간다.
        transform.position = position;
        agent.enabled = true;
        attackTimer = data.attackInterval;
    }

    void Update()
    {
        if (IsDead || target == null || !agent.isOnNavMesh) return;
        if (!GameManager.Instance.IsPlaying) { agent.isStopped = true; return; }

        agent.isStopped = false;
        agent.SetDestination(target.transform.position);

        attackTimer -= Time.deltaTime;
        if (attackTimer <= 0f && FlatDistance(target.transform.position) <= Data.attackRange)
        {
            attackTimer = Data.attackInterval;
            target.TakeDamage(Data.attackDamage, this);
        }
    }

    public void TakeDamage(int damage)
    {
        if (IsDead) return;
        Hp -= damage;
        Debug.Log($"[{room.name}] {name} 피격 -{damage} → HP {Mathf.Max(Hp, 0)}/{Data.maxHp}");
        if (IsDead)
        {
            room.OnMonsterDead(this);
            Destroy(gameObject);
        }
    }

    float FlatDistance(Vector3 p)
    {
        Vector3 d = p - transform.position;
        d.y = 0f;
        return d.magnitude;
    }

    public static int AgentTypeId(string agentTypeName)
    {
        for (int i = 0; i < NavMesh.GetSettingsCount(); i++)
        {
            int id = NavMesh.GetSettingsByIndex(i).agentTypeID;
            if (NavMesh.GetSettingsNameFromID(id) == agentTypeName) return id;
        }
        Debug.LogError($"Agent Type \"{agentTypeName}\" 없음 (Navigation 창 Agents 탭 확인)");
        return 0;
    }
}
