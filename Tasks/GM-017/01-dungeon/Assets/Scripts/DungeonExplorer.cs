using System.Collections.Generic;
using UnityEngine;

public class DungeonExplorer : MonoBehaviour
{
    // 1단계: 몬스터 종류와 방 이름 (Array)
    private readonly string[] monsterPool =
    {
        "늑대", "고블린", "스켈레톤", "오크", "박쥐"
    };

    private readonly string[] roomPool =
    {
        "횃불의 방", "거미줄 복도", "무너진 서고", "지하 감옥", "보물 창고"
    };

    // 1단계: 발견했지만 아직 싸우지 않은 몬스터 (FIFO)
    // Queue · Stack은 Unity가 직렬화하지 않아 인스펙터에 보이지 않는다.
    private Queue<string> monsterQueue = new Queue<string>();

    // 1단계: 이동한 방 (LIFO). 맨 위가 현재 방, 비어 있으면 던전 입구.
    private Stack<string> travelHistory = new Stack<string>();

    void Update()
    {
        // [S] 정찰
        if (Input.GetKeyDown(KeyCode.S))
        {
            Scout();
        }

        // [A] 전투
        if (Input.GetKeyDown(KeyCode.A))
        {
            Fight();
        }

        // [M] 이동
        if (Input.GetKeyDown(KeyCode.M))
        {
            MoveDeeper();
        }

        // [B] 퇴각
        if (Input.GetKeyDown(KeyCode.B))
        {
            Retreat();
        }
    }

    // 2단계 [S]: 무작위 몬스터 발견 → Enqueue
    // 명세는 Enqueue와 [조우 예고] 로그를 3단계(A)에 적었지만, A에서 넣고 바로 빼면 대기열이 의미가 없어
    // 2단계 제목("몬스터 발견 (Queue)")과 Hint(발견한 순서대로 다가온다)를 따라 S에서 넣는다.
    public void Scout()
    {
        string monster = monsterPool[Random.Range(0, monsterPool.Length)];
        monsterQueue.Enqueue(monster);
        Debug.Log($"[조우 예고] 저 멀리서 {monster}이 나타났습니다!");
    }

    // 3단계 [A]: 가장 먼저 발견한 몬스터를 Dequeue
    public void Fight()
    {
        if (monsterQueue.Count == 0)
        {
            // 빈 대기열에서 Dequeue하면 InvalidOperationException. 문구는 명세에 없어 정함
            Debug.Log("[전투] 싸울 몬스터가 없습니다. S로 정찰하세요.");
            return;
        }

        string monster = monsterQueue.Dequeue();
        Debug.Log($"[전투] {monster}을 처치했습니다! 남은 적: {monsterQueue.Count}마리");
    }

    // 4단계 [M]: 무작위 방을 Push
    // "총 이동 거리"는 입구에서 현재 방까지 쌓인 방 수(travelHistory.Count)로 했다. 퇴각하면 줄어든다.
    public void MoveDeeper()
    {
        string room = roomPool[Random.Range(0, roomPool.Length)];
        travelHistory.Push(room);
        Debug.Log($"[이동] {room}에 진입했습니다. (총 이동 거리: {travelHistory.Count})");
    }

    // 5단계 [B]: 현재 방을 Pop → 그 밑의 방을 Peek
    public void Retreat()
    {
        // 이미 입구라면 Pop할 방이 없다 (빈 Stack에서 Pop/Peek하면 InvalidOperationException)
        if (travelHistory.Count == 0)
        {
            Debug.Log("던전 입구까지 돌아왔습니다. 더 이상 퇴각할 수 없습니다.");
            return;
        }

        travelHistory.Pop();

        // 첫 번째 방에서 퇴각하면 Pop 후 스택이 비어 Peek할 방이 없다 → 입구 안내
        if (travelHistory.Count == 0)
        {
            Debug.Log("던전 입구까지 돌아왔습니다. 더 이상 퇴각할 수 없습니다.");
            return;
        }

        Debug.Log($"[퇴각] {travelHistory.Peek()}으로 돌아왔습니다.");
    }

    // 로그 확인용: 현재 대기열 · 경로 상태 (맨 앞/맨 위부터)
    public string DescribeState()
    {
        return $"monsterQueue({monsterQueue.Count}) = [{string.Join(", ", monsterQueue)}], " +
               $"travelHistory({travelHistory.Count}) = [{string.Join(", ", travelHistory)}]";
    }
}
