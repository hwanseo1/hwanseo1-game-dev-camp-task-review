using System;
using System.Collections.Generic;
using System.Text;

namespace WolnamPpong;

class Program
{
    const int Quit = -1, Fold = -2;

    static void Main(string[] args)
    {
        Console.OutputEncoding = Encoding.UTF8;

        // 테스트 재현용: 인자로 시드를 넘기면 같은 결과가 나온다.
        Random rng = new Random();
        int seed;
        if (args.Length > 0 && int.TryParse(args[0], out seed)) rng = new Random(seed);

        Game game = new Game(rng);

        Console.WriteLine("=== 월남뽕 ===");
        Console.WriteLine("소지금 " + Game.Won(Game.StartMoney) + " | 최소 배팅 " + Game.Won(Game.MinBet)
                          + " | 최대 " + Game.MaxRounds + "판");
        Console.WriteLine("가운데 ? 카드가 앞뒤 두 장 사이(초과 ~ 미만)면 배팅금 x2 획득, 아니면 배팅금 차감");
        Console.WriteLine("[치트] 가운데 카드 상시 공개 | 배팅 칸에서 p: 다음 판 ? 카드 / l: 남은 카드 전체");
        Console.WriteLine();

        while (game.CanDeal)
        {
            game.Deal();
            Console.WriteLine("--- " + game.Round + "판 | 소지금 " + Game.Won(game.Money)
                              + " | 남은 카드 " + game.RemainingCount + "장 ---");
            Console.WriteLine(game.First + " / ? / " + game.Second + "    [치트] ? = " + game.Hidden);

            if (game.IsPair)
            {
                int d;
                game.Play(0, out d);
                Console.WriteLine("같은 숫자 → 배팅 없이 패배, " + Game.Won(-d) + " 차감 → 소지금 " + Game.Won(game.Money));
                Console.WriteLine();
                continue;
            }

            int bet = ReadBet(game);
            if (bet == Quit) { Console.WriteLine("게임을 중단합니다. 최종 소지금 " + Game.Won(game.Money)); return; }
            if (bet == Fold)
            {
                Console.WriteLine("폴드 → 이번 판 종료, 소지금 그대로 " + Game.Won(game.Money));
                Console.WriteLine();
                continue;
            }

            int delta;
            Result result = game.Play(bet, out delta);
            string opened = game.First + " / " + game.Hidden + " / " + game.Second;
            if (result == Result.Win)
                Console.WriteLine(opened + " → 사이에 있음! 승리 +" + Game.Won(delta) + " → 소지금 " + Game.Won(game.Money));
            else
                Console.WriteLine(opened + " → 사이에 없음. 패배 -" + Game.Won(-delta) + " → 소지금 " + Game.Won(game.Money));
            Console.WriteLine();
        }

        if (game.IsBankrupt)
            Console.WriteLine("소지금 0원 — 게임 종료");
        else
            Console.WriteLine("카드 부족 (" + Game.MaxRounds + "판 완료) — 남은 1장(" + game.Leftover + ")은 버려집니다.");
        Console.WriteLine("최종 소지금 " + Game.Won(game.Money));
    }

    // 배팅 금액을 반환. 폴드는 Fold, 중단은 Quit. 치트 키는 판을 넘기지 않고 다시 입력받는다.
    static int ReadBet(Game g)
    {
        int min = Game.MinBetFor(g.Money);
        while (true)
        {
            Console.Write("배팅 금액(" + Game.Won(min) + " ~ " + Game.Won(g.Money) + ") | fold | 치트 p/l | 중단 q: ");
            string s = Console.ReadLine();
            if (s == null) return Quit;
            s = s.Trim().ToLower();

            switch (s)
            {
                case "q":
                    return Quit;

                case "fold":
                    return Fold;

                case "p":
                    Card next;
                    if (g.TryPeekNextHidden(out next))
                        Console.WriteLine("[치트] 다음 판 ? 카드: " + next);
                    else
                        Console.WriteLine("[치트] 마지막 판이라 다음 판이 없습니다.");
                    continue;

                case "l":
                    PrintRemaining(g.Remaining());
                    continue;
            }

            int bet;
            if (!int.TryParse(s.Replace(",", ""), out bet))
                Console.WriteLine("숫자, fold, p, l, q 중에서 입력해 주세요.");
            else if (!Game.IsValidBet(bet, g.Money))
                Console.WriteLine(Game.Won(min) + " 이상, 소지금 이하로 입력해 주세요.");
            else
                return bet;
        }
    }

    static void PrintRemaining(List<Card> rest)
    {
        Console.WriteLine("[치트] 남은 카드 " + rest.Count + "장 (이번 판 3장 제외)");
        for (int s = 0; s < 4; s++)
        {
            List<string> line = new List<string>();
            foreach (Card c in rest)
                if ((int)c.Suit == s) line.Add(Game.RankText(c.Number));
            Console.WriteLine("  " + Game.SuitSymbol((Suit)s) + " : " + (line.Count == 0 ? "-" : string.Join(" ", line)));
        }
    }
}
