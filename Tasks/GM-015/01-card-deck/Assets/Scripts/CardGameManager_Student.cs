using UnityEngine;

public class CardGameManager_Student : MonoBehaviour
{
    // 1단계: 카드 덱 구성 (public이라 인스펙터에 Monster Deck으로 보인다)
    [Header("몬스터 덱 (인스펙터에서 확인하세요)")]
    public int[] monsterDeck = { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };

    void Update()
    {
        // [W] 키: 수동 Swap (0번과 마지막 교체)
        if (Input.GetKeyDown(KeyCode.W))
        {
            Debug.Log("🔁 카드 위치를 교체합니다. (0번 <-> 마지막)");
            SwapAt(0, monsterDeck.Length - 1);
        }

        // [S] 키: 전체 Shuffle (Fisher-Yates)
        if (Input.GetKeyDown(KeyCode.S))
        {
            Debug.Log("🔀 덱을 섞습니다! (Fisher-Yates)");
            ShuffleFisherYates();
        }

        // [Space] 키: 대결
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Battle();
        }
    }

    // 4단계: 0번 카드와 보스 공격력 비교
    // 명세는 승리·패배만 있고, 무승부는 샘플 TODO 5의 "승/패/무"를 따랐다.
    public void Battle()
    {
        int myCard = monsterDeck[0];
        int bossPower = Random.Range(1, 11); // 1~10 (최댓값 11은 포함되지 않음)

        // 결과 문구는 명세에 없어 정함
        if (myCard > bossPower)
        {
            Debug.Log($"⚔️ 내 카드 {myCard} vs 보스 {bossPower} → 승리!");
        }
        else if (myCard < bossPower)
        {
            Debug.Log($"⚔️ 내 카드 {myCard} vs 보스 {bossPower} → 패배...");
        }
        else
        {
            Debug.Log($"⚔️ 내 카드 {myCard} vs 보스 {bossPower} → 무승부");
        }

        // 명세 "대결 후에는 다시 섞어야"는 샘플 TODO 6처럼 안내만 하고 자동으로 섞지 않는다.
        Debug.Log("다음 판은 S로 셔플하세요.");
    }

    // 2단계: 배열의 두 위치 값을 교환하는 Swap 로직
    public void SwapAt(int i, int j)
    {
        if (monsterDeck == null) return;
        if (i < 0 || i >= monsterDeck.Length || j < 0 || j >= monsterDeck.Length) return;
        if (i == j) return;

        int temp = monsterDeck[i];
        monsterDeck[i] = monsterDeck[j];
        monsterDeck[j] = temp;
    }

    // 3단계: 균등 셔플(Fisher-Yates)
    // 명세 문장(i를 0부터 끝까지, randomIndex = Random.Range(0, 10))이 아니라 샘플의 Fisher-Yates를 따랐다.
    public void ShuffleFisherYates()
    {
        if (monsterDeck == null || monsterDeck.Length <= 1) return;

        for (int i = monsterDeck.Length - 1; i >= 1; i--)
        {
            int randomIndex = Random.Range(0, i + 1); // 0~i
            SwapAt(i, randomIndex);
        }
    }
}
