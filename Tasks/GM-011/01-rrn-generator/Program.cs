using System;
using System.Text;

namespace RrnGenerator;

class Program
{
    const int MaxRegenerate = 3;

    static void Main()
    {
        Console.OutputEncoding = Encoding.UTF8;
        Console.InputEncoding = Encoding.UTF8;

        Random random = new Random();

        Console.WriteLine("=== 주민등록번호 형식 생성기 ===");

        ResidentNumber rrn = new ResidentNumber();
        if (!AskYear(out rrn.Year)) return;
        if (!AskMonth(out rrn.Month)) return;
        if (!AskDay(rrn.Year, rrn.Month, out rrn.Day)) return;

        Console.Write("성별 (남자 1 / 3, 여자 2 / 4) : ");
        string genderLine = Console.ReadLine();
        if (genderLine == null) return;
        rrn.GenderDigit = RrnRules.ResolveGenderDigit(genderLine, random);
        if (genderLine.Trim() != rrn.GenderDigit.ToString())
            Console.WriteLine("1 ~ 4가 아니어서 무작위로 정했습니다 : " + rrn.GenderDigit);

        rrn.Tail = RrnRules.GenerateTail(rrn.GenderDigit, random);
        Console.WriteLine();
        Console.WriteLine("생성된 번호 : " + rrn);

        // 재생성은 뒤 6자리만 새로 뽑는다. 생년월일 · 성별은 처음 입력 그대로.
        for (int count = 1; count <= MaxRegenerate; count++)
        {
            if (!AskYesNo("다시 생성하시겠습니까? (Y / N) [" + (count - 1) + " / " + MaxRegenerate + "] : ", out bool yes)) return;
            if (!yes) break;

            rrn.Tail = RrnRules.GenerateTail(rrn.GenderDigit, random);
            Console.WriteLine("재생성 " + count + " : " + rrn);
        }

        Console.WriteLine("프로그램을 종료합니다.");
    }

    // 4자리는 그대로, 2자리는 1900년대 / 2000년대 중 고르게 한다 (자동 변환 결과를 기본값으로 표시).
    static bool AskYear(out int year)
    {
        year = 0;
        while (true)
        {
            Console.Write("생년 (4자리 또는 2자리) : ");
            string line = Console.ReadLine();
            if (line == null) return false;

            RrnRules.YearKind kind = RrnRules.ParseYear(line.Trim(), out int value);
            switch (kind)
            {
                case RrnRules.YearKind.FourDigit:
                    year = value;
                    return true;
                case RrnRules.YearKind.TwoDigit:
                    return AskCentury(value, out year);
                default:
                    Console.WriteLine("숫자 4자리 또는 2자리로 입력해 주세요.");
                    break;
            }
        }
    }

    static bool AskCentury(int twoDigit, out int year)
    {
        year = 0;
        int auto = RrnRules.AutoCentury(twoDigit);
        int y1900 = 1900 + twoDigit;
        int y2000 = 2000 + twoDigit;
        Console.WriteLine("    1. " + y1900 + (auto == y1900 ? " (자동)" : ""));
        Console.WriteLine("    2. " + y2000 + (auto == y2000 ? " (자동)" : ""));
        while (true)
        {
            Console.Write("선택 (1 / 2, Enter = 자동) : ");
            string line = Console.ReadLine();
            if (line == null) return false;
            switch (line.Trim())
            {
                case "":  year = auto;  return true;
                case "1": year = y1900; return true;
                case "2": year = y2000; return true;
                default:
                    Console.WriteLine("1 또는 2를 입력해 주세요.");
                    break;
            }
        }
    }

    static bool AskMonth(out int month)
    {
        month = 0;
        while (true)
        {
            Console.Write("월 : ");
            string line = Console.ReadLine();
            if (line == null) return false;
            if (RrnRules.TryParseOneOrTwoDigits(line.Trim(), out month) && RrnRules.IsValidMonth(month))
                return true;
            Console.WriteLine("1 ~ 12 사이로 입력해 주세요.");
        }
    }

    static bool AskDay(int year, int month, out int day)
    {
        day = 0;
        int max = RrnRules.DaysInMonth(year, month);
        while (true)
        {
            Console.Write("일 : ");
            string line = Console.ReadLine();
            if (line == null) return false;
            if (RrnRules.TryParseOneOrTwoDigits(line.Trim(), out day) && RrnRules.IsValidDay(year, month, day))
                return true;
            Console.WriteLine(year + "년 " + month + "월은 1 ~ " + max + "일까지입니다.");
        }
    }

    static bool AskYesNo(string prompt, out bool yes)
    {
        yes = false;
        while (true)
        {
            Console.Write(prompt);
            string line = Console.ReadLine();
            if (line == null) return false;
            switch (line.Trim().ToUpper())
            {
                case "Y": yes = true;  return true;
                case "N": yes = false; return true;
                default:
                    Console.WriteLine("Y 또는 N을 입력해 주세요.");
                    break;
            }
        }
    }
}
