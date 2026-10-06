using System;
using System.Text;

namespace RrnGenerator;

// 생년월일 · 성별 숫자 · 뒤 6자리를 한 덩어리로 들고 다니려고 구조체로 묶었다 ("구조체를 사용할 수 있다고 판단이 들면").
public struct ResidentNumber
{
    public int Year;
    public int Month;
    public int Day;
    public int GenderDigit;   // 1 ~ 4
    public string Tail;       // 무작위 6자리

    // 123456-1234567 형식. 앞자리는 연도 끝 두 자리 + 월 + 일 (모두 두 자리로 채움).
    public override string ToString()
    {
        return (Year % 100).ToString("D2") + Month.ToString("D2") + Day.ToString("D2")
             + "-" + GenderDigit + Tail;
    }
}

// 입력 검사 · 변환 · 무작위 생성 규칙. Program.cs는 입출력만 맡는다.
public static class RrnRules
{
    // 2자리 연도 자동 변환 기준: 00 ~ 24 → 2000년대, 25 ~ 99 → 1900년대
    public const int CenturyPivot = 25;

    public enum YearKind { Invalid, FourDigit, TwoDigit }

    // 숫자로만 된 4자리 또는 2자리인지 본다. 4자리 범위 제한은 명세에 없어 0000 ~ 9999를 그대로 받는다.
    public static YearKind ParseYear(string input, out int value)
    {
        value = 0;
        if (input == null || !IsDigits(input)) return YearKind.Invalid;
        if (input.Length == 4) { value = int.Parse(input); return YearKind.FourDigit; }
        if (input.Length == 2) { value = int.Parse(input); return YearKind.TwoDigit; }
        return YearKind.Invalid;
    }

    // 명세의 자동 변환 규칙. 사용자 선택 화면에서 "자동" 표시와 Enter 기본값으로 쓴다.
    public static int AutoCentury(int twoDigit)
    {
        return twoDigit < CenturyPivot ? 2000 + twoDigit : 1900 + twoDigit;
    }

    // 월 · 일은 1자리 또는 2자리 숫자만 받는다 (06 → 6).
    public static bool TryParseOneOrTwoDigits(string input, out int value)
    {
        value = 0;
        if (input == null || input.Length < 1 || input.Length > 2 || !IsDigits(input)) return false;
        value = int.Parse(input);
        return true;
    }

    public static bool IsValidMonth(int month)
    {
        return month >= 1 && month <= 12;
    }

    public static bool IsLeapYear(int year)
    {
        return (year % 4 == 0 && year % 100 != 0) || year % 400 == 0;
    }

    // 31일 달 / 30일 달 / 2월(윤년 29, 평년 28)
    public static int DaysInMonth(int year, int month)
    {
        switch (month)
        {
            case 2:
                return IsLeapYear(year) ? 29 : 28;
            case 4:
            case 6:
            case 9:
            case 11:
                return 30;
            default:
                return 31;
        }
    }

    public static bool IsValidDay(int year, int month, int day)
    {
        return day >= 1 && day <= DaysInMonth(year, month);
    }

    // "1" ~ "4"면 그 숫자, 그 밖의 입력(다른 숫자 · 문자 · 빈 값)은 1 ~ 4 중 무작위.
    public static int ResolveGenderDigit(string input, Random random)
    {
        string trimmed = input == null ? "" : input.Trim();
        if (trimmed.Length == 1 && trimmed[0] >= '1' && trimmed[0] <= '4')
            return trimmed[0] - '0';
        return random.Next(1, 5);
    }

    // 뒤 6자리: 0 없음(1 ~ 9), 바로 앞 숫자와 같으면 안 됨.
    // 첫 자리는 성별 숫자와 붙어 출력되므로 성별 숫자와도 겹치지 않게 한다 (1112345 같은 결과 방지).
    public static string GenerateTail(int genderDigit, Random random)
    {
        StringBuilder sb = new StringBuilder();
        int prev = genderDigit;
        for (int i = 0; i < 6; i++)
        {
            // 1 ~ 9 중 prev를 뺀 8개에서 고른다. 다시 뽑기 반복 없이 한 번에 정해진다.
            int digit = random.Next(1, 9);
            if (digit >= prev) digit++;
            sb.Append(digit);
            prev = digit;
        }
        return sb.ToString();
    }

    // 뒷자리 7자리(성별 + 6자리)가 규칙을 지키는지. 테스트와 재생성 확인용.
    public static bool IsValidBackPart(string back)
    {
        if (back == null || back.Length != 7) return false;
        if (back[0] < '1' || back[0] > '4') return false;
        for (int i = 0; i < back.Length; i++)
        {
            if (back[i] < '1' || back[i] > '9') return false;
            if (i > 0 && back[i] == back[i - 1]) return false;
        }
        return true;
    }

    static bool IsDigits(string s)
    {
        if (s.Length == 0) return false;
        foreach (char c in s)
            if (c < '0' || c > '9') return false;
        return true;
    }
}
