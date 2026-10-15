using System.Collections.Generic;
using UnityEngine;

public class WeightedLootSystem : MonoBehaviour
{
    // 제공 데이터 (Array). 같은 인덱스끼리 짝이다.
    private readonly string[] itemNames = { "쓰레기", "일반 검", "희귀 방패", "전설의 보석" };
    private readonly int[] weights = { 700, 200, 90, 10 }; // 총합: 1000

    // 뽑은 아이템 기록 (맨 위 = 가장 최근)
    private Stack<string> lootHistory = new Stack<string>();

    // Undo는 게임 전체에서 3번까지 (명세 예시대로 성공할 때마다 1씩 감소, 다시 채우지 않음)
    private int undoLeft = 3;

    void Update()
    {
        // 명세에 키가 없어 정함. DungeonExplorer의 S/A/M/B와 겹치지 않게 D(Draw) / U(Undo)
        if (Input.GetKeyDown(KeyCode.D))
        {
            Draw();
        }

        if (Input.GetKeyDown(KeyCode.U))
        {
            Undo();
        }
    }

    // 가중치 총합을 반복문으로 계산 (하드코딩한 1000 대신)
    public int GetTotalWeight()
    {
        int total = 0;
        for (int i = 0; i < weights.Length; i++)
        {
            total += weights[i];
        }
        return total;
    }

    // 누적합 > r 이 처음 되는 아이템의 인덱스
    // 즉 weights[0..k-1]의 합 <= r < weights[0..k]의 합 인 k
    public int PickIndex(int r)
    {
        int cumulative = 0;
        for (int i = 0; i < weights.Length; i++)
        {
            cumulative += weights[i];
            if (cumulative > r)
            {
                return i;
            }
        }

        // r이 0 ~ totalWeight-1 범위면 여기까지 오지 않는다
        return weights.Length - 1;
    }

    // 가중치 랜덤으로 1개 뽑아 lootHistory에 Push
    public string Draw()
    {
        int r = Random.Range(0, GetTotalWeight()); // int 버전은 최댓값 제외 → 0 ~ 999
        string item = itemNames[PickIndex(r)];

        lootHistory.Push(item);
        Debug.Log($"획득: {item} (r={r})");
        return item;
    }

    // 최근 획득을 취소하고 즉시 다시 뽑기
    public void Undo()
    {
        if (lootHistory.Count == 0)
        {
            Debug.Log("취소할 아이템이 없습니다. 먼저 뽑기를 하세요.");
            return;
        }

        if (undoLeft == 0)
        {
            Debug.Log("더 이상 Undo할 수 없습니다. (남은 Undo: 0)");
            return;
        }

        string canceled = lootHistory.Pop();
        undoLeft--;
        Debug.Log($"취소: {canceled} / 남은 Undo: {undoLeft}");

        // 다시 뽑기 기회
        Draw();
    }

    // 로그 확인용: 현재 히스토리 (맨 위부터)
    public string DescribeState()
    {
        return $"lootHistory({lootHistory.Count}) = [{string.Join(", ", lootHistory)}], undoLeft = {undoLeft}";
    }
}
