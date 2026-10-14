using System.Collections.Generic;
using UnityEngine;

public class DeckManager : MonoBehaviour
{
    // 1단계: 도감 (고정 배열)
    // public으로 두면 인스펙터 값이 우선해 크기가 바뀔 수 있어(GM-015) private으로 고정했다.
    private readonly string[] masterCards =
    {
        "용사", "마법사", "궁수", "도둑", "기사", "힐러", "드래곤", "슬라임", "골렘", "암살자"
    };

    // 1단계: 내 덱 (전투 중 줄어들므로 List). 인스펙터에서 Count 변화를 보려고 public으로 뒀다.
    public List<string> playerDeck = new List<string>();

    // B 키로 전부 버릴 카드, I 키로 맨 위에 넣을 카드
    private const string DiscardTarget = "슬라임";
    private const string EmergencyCard = "EmergencyHeal";

    // 소개 문단 "게임 시작 전 … 덱을 구성하고 섞은 뒤"를 따라 시작할 때 한 번 준비한다.
    void Start()
    {
        RefillDeck();
        ShuffleDeck();
        PrintDeck();
    }

    void Update()
    {
        // [Space] 드로우 + 삭제
        if (Input.GetKeyDown(KeyCode.Space))
        {
            DrawCard();
        }

        // [B] 특정 카드 전부 버리기
        if (Input.GetKeyDown(KeyCode.B))
        {
            DiscardAll(DiscardTarget);
        }

        // [I] 긴급 카드 맨 위에 삽입
        if (Input.GetKeyDown(KeyCode.I))
        {
            InsertEmergency();
        }

        // [R] 리필 + 셔플
        if (Input.GetKeyDown(KeyCode.R))
        {
            RefillAndShuffle();
        }
    }

    // 1단계: Clear 후 while로 도감 카드를 Count가 10이 될 때까지 복사
    public void RefillDeck()
    {
        playerDeck.Clear();

        int index = 0;
        while (playerDeck.Count < masterCards.Length)
        {
            playerDeck.Add(masterCards[index]);
            index++;

            // 명세: 인덱스가 0~9를 넘지 않게 0으로 되돌린다.
            // Clear 후 10장만 채우므로 실제로는 마지막 Add 뒤에만 실행되고 다시 쓰이지 않는다.
            if (index >= masterCards.Length)
            {
                index = 0;
            }
        }
    }

    // 2단계: Fisher-Yates (뒤에서 앞으로, j는 0~i)
    public void ShuffleDeck()
    {
        for (int i = playerDeck.Count - 1; i >= 1; i--)
        {
            int j = Random.Range(0, i + 1); // int 버전은 최댓값 제외 → 0~i
            Swap(playerDeck, i, j);
        }
    }

    // 2단계: temp로 두 위치 교환 (분리 필수)
    public void Swap(List<string> list, int a, int b)
    {
        string temp = list[a];
        list[a] = list[b];
        list[b] = temp;
    }

    // 3단계: foreach로 순서대로 출력 (번호 포함)
    // 빈 덱이면 foreach가 아무것도 찍지 않아서 장수를 첫 줄에 함께 출력한다.
    public void PrintDeck()
    {
        Debug.Log($"📋 현재 덱 ({playerDeck.Count}장)");

        int number = 1;
        foreach (string card in playerDeck)
        {
            Debug.Log($"{number}. {card}");
            number++;
        }
    }

    // 4단계 [Space]: [0] 저장 → 출력 → RemoveAt(0)
    public void DrawCard()
    {
        if (playerDeck.Count <= 0)
        {
            // 빈 덱일 때 문구는 명세에 없어 정함
            Debug.Log("덱이 비었습니다. R로 리필하세요.");
            return;
        }

        string drawnCard = playerDeck[0];
        Debug.Log($"🃏 드로우: {drawnCard}");
        playerDeck.RemoveAt(0);
        Debug.Log($"남은 카드: {playerDeck.Count}장");
    }

    // 4단계 [B]: 뒤에서 앞으로 for + RemoveAt (람다식·foreach 삭제 금지)
    public void DiscardAll(string target)
    {
        int removed = 0;
        for (int i = playerDeck.Count - 1; i >= 0; i--)
        {
            if (playerDeck[i] == target)
            {
                playerDeck.RemoveAt(i);
                removed++;
            }
        }

        Debug.Log($"🗑️ {target} {removed}장 버림 (남은 카드: {playerDeck.Count}장)");
    }

    // 4단계 [I]: 맨 앞(0번)에 Insert 후 Count 증가 확인
    public void InsertEmergency()
    {
        int before = playerDeck.Count;
        playerDeck.Insert(0, EmergencyCard);
        Debug.Log($"🚑 {EmergencyCard}을 맨 위에 넣었습니다. ({before}장 → {playerDeck.Count}장)");
    }

    // 4단계 [R]: 권장안대로 덱이 비었을 때만 리필
    public void RefillAndShuffle()
    {
        if (playerDeck.Count > 0)
        {
            // 거절 문구는 명세에 없어 정함
            Debug.Log($"아직 카드가 {playerDeck.Count}장 남아 있어 리필할 수 없습니다.");
            return;
        }

        RefillDeck();
        ShuffleDeck();
        Debug.Log("🔄 덱을 리필하고 섞었습니다.");
        PrintDeck();
    }
}
