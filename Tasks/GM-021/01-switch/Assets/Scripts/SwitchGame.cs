using UnityEngine;

// 스위치 3개 중 정답 찾기: 1/2/3 선택, C 치트, 10라운드 후 정답률
public class SwitchGame : MonoBehaviour
{
    public Renderer[] switches = new Renderer[3];   // 씬의 도형 3개 (Switch1~3)
    public Vector3 lineStart = new Vector3(0f, -2f, 0f); // 선택 라인의 시작점 (플레이어 자리)
    public int totalRounds = 10;
    public float resultDelay = 1f;   // 결과 색을 보여 주는 시간 (그 뒤 다음 라운드)

    int round;          // 현재 라운드 (1부터)
    int answerIndex;    // 정답 인덱스 (0~2)
    int correctCount;
    bool waiting;       // 결과 표시 중 → 입력 무시
    bool finished;

    void Start()
    {
        round = 0;
        correctCount = 0;
        finished = false;
        StartRound();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.C)) ShowCheat();

        if (Input.GetKeyDown(KeyCode.Alpha1)) Choose(0);
        if (Input.GetKeyDown(KeyCode.Alpha2)) Choose(1);
        if (Input.GetKeyDown(KeyCode.Alpha3)) Choose(2);
    }

    public void StartRound()
    {
        round++;
        waiting = false;

        // 라운드 시작 시 회색으로 초기화
        for (int i = 0; i < switches.Length; i++)
        {
            switches[i].material.color = Color.gray;
        }

        // Random.Range(int, int)는 최댓값을 포함하지 않는다 → 0, 1, 2
        answerIndex = Random.Range(0, switches.Length);
        Debug.Log($"[라운드 {round}/{totalRounds}] 정답 스위치를 골라 보세요 (1 / 2 / 3)");
    }

    public void Choose(int index)
    {
        if (finished)
        {
            Debug.Log("게임이 끝났습니다.");
            return;
        }
        if (waiting) return;

        bool correct = index == answerIndex;
        if (correct) correctCount++;

        Color resultColor = correct ? Color.green : Color.red;
        switches[index].material.color = resultColor;

        // Scene 뷰에 선택 라인 표시 (결과 표시 시간 동안 유지)
        Debug.DrawLine(lineStart, switches[index].transform.position, resultColor, resultDelay);

        Debug.Log($"[라운드 {round}] {index + 1}번 선택 → {(correct ? "정답!" : "오답")} (정답: {answerIndex + 1}번)");

        if (round >= totalRounds)
        {
            finished = true;
            float rate = (float)correctCount / totalRounds * 100f;
            Debug.Log($"게임 종료! 정답률: {correctCount}/{totalRounds} ({rate:0}%)");
            return;
        }

        waiting = true;
        Invoke(nameof(StartRound), resultDelay);
    }

    public void ShowCheat()
    {
        if (finished) return;
        Debug.Log($"[치트] 라운드 {round} 정답 인덱스: {answerIndex} ({answerIndex + 1}번 스위치)");
    }

    // 확인용: 현재 상태 조회
    public int AnswerIndex => answerIndex;
    public int Round => round;
    public int CorrectCount => correctCount;
    public bool IsFinished => finished;
}
