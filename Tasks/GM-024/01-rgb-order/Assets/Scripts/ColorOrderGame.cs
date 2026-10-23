using UnityEngine;

// R → G → B 순서 맞히기: 왼쪽 위 큐브가 맞혀야 할 색, 아래 큐브 3개(1~3번 키) 중 그 색을 고른다
public class ColorOrderGame : MonoBehaviour
{
    // 맞혀야 할 색 단계 (R → G → B → 1점 + R)
    public enum Stage { R, G, B }

    public Renderer targetCube;                         // 왼쪽 위: 현재 단계 색
    public Renderer[] choiceCubes = new Renderer[3];    // 아래: 1 / 2 / 3번 키

    // 0 빨강 / 1 초록 / 2 파랑 (= Stage 값)
    static readonly Color[] Palette = { Color.red, Color.green, Color.blue };
    static readonly string[] ColorNames = { "R", "G", "B" };

    Stage stage;
    int score;
    readonly int[] choices = new int[3];   // 아래 큐브 3개의 색

    void Start()
    {
        ResetGame();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1)) Press(0);
        else if (Input.GetKeyDown(KeyCode.Alpha2)) Press(1);
        else if (Input.GetKeyDown(KeyCode.Alpha3)) Press(2);
    }

    public void ResetGame()
    {
        stage = Stage.R;
        score = 0;
        RollChoices();
        Refresh();
        Debug.Log($"시작! 맞힐 색 {ColorNames[(int)stage]} / 큐브 {ChoiceText()}");
    }

    // index번 큐브(0 ~ 2)를 골랐을 때 단계 전이
    public void Press(int index)
    {
        int picked = choices[index];

        if (picked == (int)stage)
        {
            switch (stage)
            {
                case Stage.R: stage = Stage.G; break;
                case Stage.G: stage = Stage.B; break;
                case Stage.B:
                    score++;
                    stage = Stage.R;
                    Debug.Log($"R → G → B 완성! 1점 획득 (점수 {score})");
                    break;
            }
        }
        else
        {
            Debug.Log($"틀렸습니다! ({index + 1}번 = {ColorNames[picked]}, 정답 {ColorNames[(int)stage]}) R 단계로");
            stage = Stage.R;
        }

        // 다시 뽑는 시점은 명세에 없음 → 키를 누를 때마다 (사용자 결정)
        RollChoices();
        Refresh();
        Debug.Log($"맞힐 색 {ColorNames[(int)stage]} / 큐브 {ChoiceText()} / 점수 {score}");
    }

    // 3개를 R / G / B에서 따로 뽑는다 (중복 허용). 맞힐 색이 하나도 없으면 다시 뽑는다
    void RollChoices()
    {
        do
        {
            for (int i = 0; i < choices.Length; i++) choices[i] = Random.Range(0, Palette.Length);
        }
        while (System.Array.IndexOf(choices, (int)stage) < 0);
    }

    void Refresh()
    {
        targetCube.material.color = Palette[(int)stage];
        for (int i = 0; i < choiceCubes.Length; i++) choiceCubes[i].material.color = Palette[choices[i]];
    }

    string ChoiceText() => $"1 {ColorNames[choices[0]]} · 2 {ColorNames[choices[1]]} · 3 {ColorNames[choices[2]]}";

    // 확인용: 현재 상태 조회
    public Stage CurrentStage => stage;
    public int Score => score;
    public int[] Choices => (int[])choices.Clone();
    public static string ColorName(int color) => ColorNames[color];
}
