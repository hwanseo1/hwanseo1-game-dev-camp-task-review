using UnityEngine;

public class DaySimulator : MonoBehaviour
{
    // 1단계: 변수 선언
    int playerHP = 100;
    int playerAtk = 10;
    int gold = 0;
    int trainingCount = 0;          // 하루가 바뀌는 규칙이 명세에 없어 0으로 되돌리지 않는다
    bool isBossDefeated = false;
    int explorationProgress = 0;    // 탐험마다 0으로 되돌리라는 말이 명세에 없어 그대로 둔다

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Train();
        }

        if (Input.GetKeyDown(KeyCode.H))
        {
            Explore();
        }

        if (Input.GetKeyDown(KeyCode.B))
        {
            FightBoss();
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            CheckStatus();
        }
    }

    // 2단계: [Space] 아침 훈련 (for 문)
    public void Train()
    {
        if (trainingCount < 3)
        {
            Debug.Log("--- 훈련 시작 ---");
            for (int i = 1; i <= 10; i++)
            {
                Debug.Log($"칼을 휘두릅니다! ({i}회)");
            }
            playerAtk += 5;
            trainingCount++;
            Debug.Log($"훈련 완료! 현재 공격력: {playerAtk} (오늘 훈련 {trainingCount}/3)");
        }
        else
        {
            Debug.Log("오늘은 너무 지쳤습니다. 더 이상 훈련할 수 없습니다.");
        }
    }

    // 3단계: [H] 황야 탐험 (while 문)
    public void Explore()
    {
        Debug.Log($"--- 탐험 시작 (진행도 {explorationProgress}) ---");
        while (explorationProgress < 100)
        {
            explorationProgress += 20;

            if (Random.Range(0, 100) < 20)
            {
                playerHP -= 10;
                Debug.Log($"함정에 걸렸습니다! HP -10 (남은 HP: {playerHP})");
            }
            Debug.Log($"탐험 진행도: {explorationProgress}");

            if (playerHP <= 0)
            {
                Debug.Log("탐험 실패!");
                break;
            }
        }

        if (explorationProgress >= 100 && playerHP > 0)
        {
            gold += 50;
            Debug.Log($"탐험 성공! 골드 +50 (보유 골드: {gold})");
        }
    }

    // 4단계: [B] 보스 최후의 결전 (do-while 문)
    // "방어막이 있어 첫 타격은 무조건 견뎌냅니다"는 상황 설명에만 있고 로직이 없어 구현하지 않았다
    public void FightBoss()
    {
        int bossHP = 100;
        Debug.Log($"--- 보스전 시작 (보스 HP: {bossHP}, 내 HP: {playerHP}) ---");
        do
        {
            bossHP -= playerAtk;
            Debug.Log($"보스에게 {playerAtk}의 데미지! (보스 HP: {bossHP})");

            playerHP -= 20;
            Debug.Log($"보스가 반격합니다! 20의 데미지 (내 HP: {playerHP})");
        } while (bossHP > 0 && playerHP > 0);

        if (bossHP <= 0)
        {
            isBossDefeated = true;
            Debug.Log("보스를 처치했습니다!");
        }
        else
        {
            Debug.Log("보스에게 패배했습니다...");
        }
    }

    // 5단계: [R] 상태 확인 (if 문)
    // 제목의 "휴식"은 회복량 등 로직이 없어 구현하지 않았다
    public void CheckStatus()
    {
        Debug.Log($"[현재 상태] HP: {playerHP} | ATK: {playerAtk} | GOLD: {gold}");
        if (playerHP <= 20)
        {
            Debug.LogWarning("휴식이 절실합니다...");
        }
    }
}
