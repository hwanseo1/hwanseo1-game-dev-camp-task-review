using UnityEngine;

// 패턴 기억 게임: 큐브가 4색 패턴을 보여 주면 1~4 키로 같은 순서를 입력한다 (총 3라운드)
public class PatternGame : MonoBehaviour
{
    public enum GameState { Showing, Input, RoundClear, Failed, AllClear }

    // 한 프레임의 입력 (Update에서 키를 읽어 Step에 넘긴다)
    public enum PatternKey { None, Red, Green, Blue, Yellow, Next, Restart, Quit }

    public const int PatternLength = 4;
    public const int TotalRounds = 3;

    public float[] showTimePerRound = { 1.5f, 1.0f, 0.5f };   // 색 1개를 유지하는 시간 (라운드별)
    public Color idleColor = Color.gray;                       // 입력 대기 중 큐브 색

    // 0 빨강 / 1 초록 / 2 파랑 / 3 노랑 (= 1 ~ 4번 키)
    static readonly Color[] Palette = { Color.red, Color.green, Color.blue, Color.yellow };
    static readonly string[] ColorNames = { "빨강", "초록", "파랑", "노랑" };

    Renderer rend;

    GameState state;
    int round;              // 1 ~ 3
    int[] pattern = new int[PatternLength];
    int showIndex;          // 지금 보여 주는 패턴 칸
    float showTimer;        // 현재 색을 보여 준 시간
    int inputIndex;         // 플레이어가 맞힌 개수

    void Awake()
    {
        rend = GetComponent<Renderer>();
    }

    void Start()
    {
        StartGame();
    }

    void Update()
    {
        Step(Time.deltaTime, ReadKey());
    }

    static PatternKey ReadKey()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1)) return PatternKey.Red;
        if (Input.GetKeyDown(KeyCode.Alpha2)) return PatternKey.Green;
        if (Input.GetKeyDown(KeyCode.Alpha3)) return PatternKey.Blue;
        if (Input.GetKeyDown(KeyCode.Alpha4)) return PatternKey.Yellow;
        if (Input.GetKeyDown(KeyCode.Space)) return PatternKey.Next;
        if (Input.GetKeyDown(KeyCode.R)) return PatternKey.Restart;
        if (Input.GetKeyDown(KeyCode.Escape)) return PatternKey.Quit;
        return PatternKey.None;
    }

    // 한 프레임 처리: 상태에 따라 타이머를 돌리거나 입력을 받는다
    public void Step(float dt, PatternKey key)
    {
        switch (state)
        {
            case GameState.Showing:
                // 표시 중에는 입력 무시, 타이머로만 다음 색으로 넘어간다
                showTimer += dt;
                if (showTimer >= ShowTime)
                {
                    showTimer -= ShowTime;
                    showIndex++;
                    if (showIndex < PatternLength)
                    {
                        SetColor(Palette[pattern[showIndex]]);
                    }
                    else
                    {
                        state = GameState.Input;
                        SetColor(idleColor);
                        Debug.Log("입력 시작! 1 빨강 / 2 초록 / 3 파랑 / 4 노랑");
                    }
                }
                break;

            case GameState.Input:
                if (key >= PatternKey.Red && key <= PatternKey.Yellow) CheckInput((int)key - 1);
                break;

            case GameState.RoundClear:
                if (key == PatternKey.Next)
                {
                    round++;
                    StartRound();
                }
                break;

            case GameState.Failed:
            case GameState.AllClear:
                if (key == PatternKey.Restart) StartGame();
                else if (key == PatternKey.Quit) QuitGame();
                break;
        }
    }

    void CheckInput(int color)
    {
        SetColor(Palette[color]);

        if (pattern[inputIndex] != color)
        {
            state = GameState.Failed;
            SetColor(Color.red);
            Debug.Log($"틀렸습니다! ({inputIndex + 1}번째: 정답 {ColorNames[pattern[inputIndex]]}, 입력 {ColorNames[color]}) R 재시작 / Esc 종료");
            return;
        }

        inputIndex++;
        if (inputIndex < PatternLength) return;

        SetColor(Color.green);
        if (round < TotalRounds)
        {
            state = GameState.RoundClear;
            Debug.Log($"{round}라운드 성공! Space로 다음 라운드");
        }
        else
        {
            state = GameState.AllClear;
            Debug.Log("3라운드 모두 성공! R 재시작 / Esc 종료");
        }
    }

    void StartGame()
    {
        round = 1;
        StartRound();
    }

    void StartRound()
    {
        MakePattern();
        state = GameState.Showing;
        showIndex = 0;
        showTimer = 0f;
        inputIndex = 0;
        SetColor(Palette[pattern[0]]);
        Debug.Log($"{round}라운드 시작 (색당 {ShowTime}초)");
    }

    // 4색을 중복 없이 섞는다 (Fisher-Yates)
    void MakePattern()
    {
        for (int i = 0; i < PatternLength; i++) pattern[i] = i;
        for (int i = PatternLength - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            (pattern[i], pattern[j]) = (pattern[j], pattern[i]);
        }
    }

    void SetColor(Color c)
    {
        rend.material.color = c;
    }

    void QuitGame()
    {
        Debug.Log("게임 종료");
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    float ShowTime => showTimePerRound[round - 1];

    // 확인용: 현재 상태 조회
    public GameState State => state;
    public int Round => round;
    public int[] Pattern => (int[])pattern.Clone();
    public int InputIndex => inputIndex;
    public float ShowTimer => showTimer;
    public Color CubeColor => rend.material.color;
    public static string ColorName(int color) => ColorNames[color];
}
