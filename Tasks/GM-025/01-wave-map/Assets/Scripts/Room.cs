using System.Collections.Generic;
using Unity.AI.Navigation;
using UnityEngine;

// 룸 하나. 플레이어가 들어오면 data.spawnCount만큼 스폰하고, 모두 처치하면 다음 문을 연다(마지막 룸이면 클리어).
// NavMeshSurface는 이 오브젝트에 붙어 있고 자식(바닥 · 벽 · 장애물)만 모아 data.agentTypeName 타입으로 굽는다.
public class Room : MonoBehaviour
{
    public RoomData data;
    public NavMeshSurface surface;    // 이 룸 전용 NavMesh
    public Renderer floor;            // 룸 안에 들어왔는지 판정하는 범위
    public Transform[] spawnPoints;
    public GameObject nextDoor;       // 클리어 시 열 문. 마지막 룸은 비워 둔다
    public Monster monsterPrefab;
    public PlayerController player;

    public bool Started { get; private set; }
    public bool Cleared { get; private set; }
    public int Alive => alive.Count;
    public IReadOnlyList<Monster> Monsters => alive;

    readonly List<Monster> alive = new List<Monster>();

    void Update()
    {
        if (!Started && Contains(player.transform.position)) StartWave();
    }

    public bool Contains(Vector3 p)
    {
        Bounds b = floor.bounds;
        b.Expand(new Vector3(-2f, 20f, -2f)); // 문턱에 걸친 정도로는 시작하지 않게 1m 안쪽부터
        return b.Contains(p);
    }

    public void StartWave()
    {
        Started = true;

        // 데이터의 Agent Type이 구워 둔 NavMesh 타입과 다르면 그 타입으로 다시 굽는다 (데이터만 바꿔도 동작하도록)
        int typeId = Monster.AgentTypeId(data.agentTypeName);
        if (surface.agentTypeID != typeId)
        {
            surface.agentTypeID = typeId;
            surface.BuildNavMesh();
            Debug.Log($"[{name}] Agent Type이 {data.agentTypeName}(으)로 바뀌어 NavMesh를 다시 구움");
        }

        for (int i = 0; i < data.spawnCount; i++)
        {
            // 스폰 수가 지점 수보다 많으면 지점을 다시 쓰되, 겹치지 않게 둘레로 2m씩 벌린다
            Transform point = spawnPoints[i % spawnPoints.Length];
            int lap = i / spawnPoints.Length;
            Vector3 pos = point.position + (lap == 0 ? Vector3.zero : Quaternion.Euler(0f, lap * 90f, 0f) * Vector3.forward * 2f);
            Monster m = Instantiate(monsterPrefab, pos, point.rotation, transform);
            m.name = $"{name}_Monster{i + 1}";
            m.Init(data, this, player, pos);
            alive.Add(m);
        }
        Debug.Log($"[{name}] 웨이브 시작: {data.spawnCount}마리 (HP {data.maxHp}, 공격력 {data.attackDamage}, 속도 {data.moveSpeed}, 크기 {data.scale}, Agent {data.agentTypeName})");
        GameManager.Instance.OnRoomEntered(this);
    }

    public void OnMonsterDead(Monster m)
    {
        alive.Remove(m);
        Debug.Log($"[{name}] {m.name} 처치 (남은 적 {alive.Count})");
        if (alive.Count > 0) return;

        Cleared = true;
        if (nextDoor != null)
        {
            nextDoor.SetActive(false);
            Debug.Log($"[{name}] 모두 처치 → {nextDoor.name} 열림");
        }
        GameManager.Instance.OnRoomCleared(this);
    }
}
