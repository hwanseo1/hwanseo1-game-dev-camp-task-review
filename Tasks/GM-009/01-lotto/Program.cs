using System;
using System.Collections.Generic;
using System.Text;

namespace Lotto;

class Program
{
    static void Main(string[] args)
    {
        Console.OutputEncoding = Encoding.UTF8;

        // 테스트 재현용: 인자로 시드를 넘기면 같은 결과가 나온다.
        Random rng = new Random();
        int seed;
        if (args.Length > 0 && int.TryParse(args[0], out seed)) rng = new Random(seed);

        Console.WriteLine("=== 로또 당첨기 ===");
        Console.WriteLine("숫자 " + LottoRules.Min + " ~ " + LottoRules.Max + " 중 " + LottoRules.Count
                          + "개 | 당첨번호 6개 + 보너스 번호 1개");
        Console.WriteLine();

        int mode = ReadMode();
        if (mode == 0) return;

        List<int> mine;
        if (mode == 1)
        {
            mine = ReadNumbers();
            if (mine == null) return;
        }
        else
        {
            mine = LottoRules.Draw(rng, LottoRules.Count);
            Console.WriteLine("자동으로 번호를 골랐습니다.");
        }
        mine.Sort();

        int bonus;
        List<int> winning = LottoRules.DrawWinning(rng, out bonus);

        Console.WriteLine();
        Console.WriteLine("내 번호   : " + LottoRules.Join(mine));
        Console.WriteLine("당첨번호  : " + LottoRules.Join(winning) + "  + 보너스 " + bonus);

        List<int> hits = new List<int>();
        foreach (int n in mine)
            if (winning.Contains(n)) hits.Add(n);
        Console.WriteLine("일치      : " + hits.Count + "개" + (hits.Count > 0 ? " (" + string.Join(" ", hits) + ")" : "")
                          + (mine.Contains(bonus) ? " + 보너스 " + bonus : ""));

        int rank = LottoRules.Rank(mine, winning, bonus);
        Console.WriteLine("결과      : " + LottoRules.RankText(rank));
    }

    // 1 = 사용자 입력, 2 = 자동, 0 = 입력 끝(EOF)
    static int ReadMode()
    {
        while (true)
        {
            Console.Write("1. 사용자 입력 / 2. 자동 중에 1개를 선택하십시오: ");
            string line = Console.ReadLine();
            if (line == null) return 0;

            switch (line.Trim())
            {
                case "1": return 1;
                case "2": return 2;
                default:
                    Console.WriteLine("1 또는 2를 입력해 주세요.");
                    break;
            }
        }
    }

    // 한 개씩 입력받고, 잘못된 값이면 그 칸만 다시 받는다.
    static List<int> ReadNumbers()
    {
        List<int> picked = new List<int>();
        while (picked.Count < LottoRules.Count)
        {
            Console.Write((picked.Count + 1) + "번째 숫자 (" + LottoRules.Min + " ~ " + LottoRules.Max + "): ");
            string line = Console.ReadLine();
            if (line == null) return null;

            int number;
            InputError error = LottoRules.Validate(line, picked, out number);
            if (error != InputError.None)
            {
                Console.WriteLine(LottoRules.ErrorText(error));
                continue;
            }
            picked.Add(number);
        }
        return picked;
    }
}
