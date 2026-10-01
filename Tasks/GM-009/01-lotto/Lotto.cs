using System;
using System.Collections.Generic;

namespace Lotto;

public enum InputError { None, NotNumber, OutOfRange, Duplicate }

public static class LottoRules
{
    public const int Min = 1, Max = 45, Count = 6;

    // 한 칸 입력 검사: 숫자 아님 → 범위 밖 → 이미 고른 숫자 순서로 본다.
    public static InputError Validate(string text, ICollection<int> picked, out int number)
    {
        number = 0;
        if (text == null || !int.TryParse(text.Trim(), out number)) return InputError.NotNumber;
        if (number < Min || number > Max) return InputError.OutOfRange;
        if (picked.Contains(number)) return InputError.Duplicate;
        return InputError.None;
    }

    // 1~45에서 서로 다른 숫자 count개 (Fisher-Yates 앞부분만 섞기).
    public static List<int> Draw(Random rng, int count)
    {
        int[] pool = new int[Max - Min + 1];
        for (int i = 0; i < pool.Length; i++) pool[i] = Min + i;

        List<int> result = new List<int>();
        for (int i = 0; i < count; i++)
        {
            int j = rng.Next(i, pool.Length);
            int tmp = pool[i]; pool[i] = pool[j]; pool[j] = tmp;
            result.Add(pool[i]);
        }
        return result;
    }

    // 당첨번호 6개 + 보너스 1개를 한 번에 7개 뽑아서 보너스가 당첨번호와 겹치지 않게 한다.
    public static List<int> DrawWinning(Random rng, out int bonus)
    {
        List<int> seven = Draw(rng, Count + 1);
        bonus = seven[Count];
        List<int> winning = seven.GetRange(0, Count);
        winning.Sort();
        return winning;
    }

    public static int MatchCount(IEnumerable<int> mine, ICollection<int> winning)
    {
        int count = 0;
        foreach (int n in mine)
            if (winning.Contains(n)) count++;
        return count;
    }

    // 1~5등, 낙첨은 0.
    public static int Rank(ICollection<int> mine, ICollection<int> winning, int bonus)
    {
        int match = MatchCount(mine, winning);
        switch (match)
        {
            case 6: return 1;
            case 5: return mine.Contains(bonus) ? 2 : 3;
            case 4: return 4;
            case 3: return 5;
            default: return 0;
        }
    }

    public static string RankText(int rank)
    {
        switch (rank)
        {
            case 1: return "1등 (당첨번호 6개 일치)";
            case 2: return "2등 (당첨번호 5개 + 보너스 번호 일치)";
            case 3: return "3등 (당첨번호 5개 일치)";
            case 4: return "4등 (당첨번호 4개 일치)";
            case 5: return "5등 (당첨번호 3개 일치)";
            default: return "낙첨";
        }
    }

    public static string ErrorText(InputError error)
    {
        switch (error)
        {
            case InputError.NotNumber: return "숫자만 입력해 주세요.";
            case InputError.OutOfRange: return Min + " ~ " + Max + " 사이의 숫자를 입력해 주세요.";
            case InputError.Duplicate: return "이미 고른 숫자입니다.";
            default: return "";
        }
    }

    public static string Join(IEnumerable<int> numbers)
    {
        List<string> parts = new List<string>();
        foreach (int n in numbers) parts.Add(n.ToString().PadLeft(2));
        return string.Join(" ", parts);
    }
}
