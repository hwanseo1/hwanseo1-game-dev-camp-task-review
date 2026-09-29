using System;

namespace NumberBaseball;

/// <summary>한 번 입력에 대한 판정 결과.</summary>
public struct Score
{
    public int Strike;
    public int Ball;
    public int Out;

    public bool IsWin => Strike == Baseball.Length;

    public override string ToString() => Strike + "S " + Ball + "B " + Out + "O";
}

/// <summary>숫자 야구 판정. 입출력과 분리해 두어 단위 테스트가 가능하다.</summary>
public static class Baseball
{
    public const int Length = 3;       // 뽑는 숫자 개수
    public const string CheatKey = "c";

    /// <summary>
    /// 치트 입력 시 그 판의 정답이 되는 숫자.
    /// 원문 "필수 : 치트 → 컴퓨터가 갖고 있는 숫자 : 3 6 9"는 켜는 방법이 정의되지 않았다.
    /// 여기서는 특정 키(c)를 누르면 그 판의 정답을 3 6 9로 바꾸는 것으로 해석했다.
    /// </summary>
    public static readonly int[] CheatAnswer = { 3, 6, 9 };

    /// <summary>
    /// 0~9 중 서로 다른 숫자 3개를 뽑는다.
    /// 중복 허용 여부는 명세에 없다. 치트 예시(3 6 9)가 모두 다른 숫자라서 중복 없이 뽑았다.
    /// </summary>
    public static int[] Generate(Random rng)
    {
        int[] pool = { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 };

        // 앞에서부터 Length개만 섞는다 (Fisher-Yates)
        for (int i = 0; i < Length; i++)
        {
            int j = rng.Next(i, pool.Length);
            int tmp = pool[i];
            pool[i] = pool[j];
            pool[j] = tmp;
        }

        int[] answer = new int[Length];
        Array.Copy(pool, answer, Length);
        return answer;
    }

    /// <summary>
    /// 자리마다 명세 그대로 판정한다.
    /// 숫자와 자리가 같으면 S / 숫자만 있으면 B / 없으면 O. 따라서 S + B + O = 3.
    /// </summary>
    public static Score Judge(int[] answer, int[] guess)
    {
        Score score = new Score();

        for (int i = 0; i < Length; i++)
        {
            if (guess[i] == answer[i])
                score.Strike++;
            else if (Array.IndexOf(answer, guess[i]) >= 0)
                score.Ball++;
            else
                score.Out++;
        }
        return score;
    }

    /// <summary>
    /// 플레이어 입력을 숫자 3개로 바꾼다. "1 2 3"과 "123" 둘 다 받는다.
    /// 유효하지 않으면 error에 안내 문구가 담긴다.
    /// </summary>
    public static bool TryParseGuess(string input, out int[] guess, out string error)
    {
        guess = null;
        string s = (input ?? "").Replace(" ", "");

        if (s.Length != Length)
        {
            error = "숫자 " + Length + "개를 입력해 주세요. (예: 1 2 3)";
            return false;
        }

        int[] result = new int[Length];
        for (int i = 0; i < Length; i++)
        {
            if (s[i] < '0' || s[i] > '9')
            {
                error = "0~9 숫자만 입력해 주세요.";
                return false;
            }
            result[i] = s[i] - '0';

            // 같은 숫자를 두 번 넣으면 B가 부풀려진다 (예: 정답 3 6 9에 3 3 3 → 1S 2B)
            if (Array.IndexOf(result, result[i], 0, i) >= 0)
            {
                error = "서로 다른 숫자를 입력해 주세요.";
                return false;
            }
        }

        guess = result;
        error = null;
        return true;
    }

    public static string Format(int[] digits) => string.Join(" ", digits);
}
