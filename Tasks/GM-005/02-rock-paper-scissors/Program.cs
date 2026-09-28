using System;
using System.Text;

namespace RockPaperScissors;

class Program
{
    static Random rng = new Random();

    // 치트 기능: 요구사항에 이름만 있고 동작 정의가 없어 임의로 만들지 않고 미구현으로 남겼다.

    static void Main(string[] args)
    {
        Console.OutputEncoding = Encoding.UTF8;

        // 테스트 재현용: 인자로 시드를 넘기면 같은 결과가 나온다.
        int seed;
        if (args.Length > 0 && int.TryParse(args[0], out seed)) rng = new Random(seed);

        int money = Rps.StartMoney;
        int round = 1;

        Console.WriteLine("=== 가위바위보 배팅 게임 ===");
        Console.WriteLine("소지금 " + Rps.StartMoney + "원 / 최소 배팅 " + Rps.MinBet + "원 / 최대 " + Rps.MaxRound + "판");
        Console.WriteLine("승리 = 판돈 x3 획득 | 무승부 = 판돈 x5 획득 | 패배 = 판돈 x7 손실");
        Console.WriteLine();

        while (round <= Rps.MaxRound && money > 0)
        {
            if (money < Rps.MinBet)
            {
                Console.WriteLine("소지금이 최소 배팅 금액(" + Rps.MinBet + "원)보다 적어 종료합니다.");
                break;
            }

            Console.WriteLine("--- " + round + "/" + Rps.MaxRound + "판 | 소지금 " + money + "원 ---");

            int bet = ReadBet(money);
            if (bet == 0) { Console.WriteLine("게임을 중단합니다."); break; }

            Hand hand;
            if (!ReadHand(out hand)) { Console.WriteLine("게임을 중단합니다."); break; }

            Hand com = (Hand)rng.Next(1, 4);
            Console.WriteLine("나: " + Rps.Name(hand) + "  vs  컴퓨터: " + Rps.Name(com));

            // 기본 골격을 switch로 구성 (정산)
            Result result = Rps.Judge(hand, com);
            int delta = Rps.Delta(result, bet, money);

            switch (result)
            {
                case Result.Win:
                    Console.WriteLine("승리! +" + delta + "원");
                    break;

                case Result.Draw:
                    Console.WriteLine("무승부! +" + delta + "원");
                    break;

                case Result.Lose:
                    Console.WriteLine("패배... -" + (-delta) + "원");
                    break;
            }

            money += delta;
            Console.WriteLine("소지금: " + money + "원");
            Console.WriteLine();
            round++;
        }

        if (money <= 0)
        {
            Console.WriteLine("가진 돈을 전부 잃었습니다. 게임 종료.");
        }
        else
        {
            int diff = money - Rps.StartMoney;
            string sign = diff >= 0 ? "+" : "";
            Console.WriteLine("게임 종료. 최종 소지금 " + money + "원 (" + sign + diff + "원)");
        }
    }

    // 배팅 금액 입력. 0을 반환하면 중단.
    static int ReadBet(int money)
    {
        while (true)
        {
            Console.Write("배팅 금액 (" + Rps.MinBet + "~" + money + "원, 중단 q): ");
            string s = Console.ReadLine();
            if (s == null) return 0;
            s = s.Trim();

            if (s.ToLower() == "q") return 0;

            int bet;
            if (!int.TryParse(s, out bet))
            {
                Console.WriteLine("숫자만 입력해 주세요.");
                continue;
            }

            string error;
            if (!Rps.IsValidBet(bet, money, out error))
            {
                Console.WriteLine(error);
                continue;
            }
            return bet;
        }
    }

    // 손 입력. false를 반환하면 중단.
    static bool ReadHand(out Hand hand)
    {
        while (true)
        {
            Console.Write("가위(1) 바위(2) 보(3) | 중단(q): ");
            string s = Console.ReadLine();
            if (s == null) { hand = Hand.Rock; return false; }

            switch (s.Trim().ToLower())
            {
                case "1": hand = Hand.Scissors; return true;
                case "2": hand = Hand.Rock;     return true;
                case "3": hand = Hand.Paper;    return true;

                case "q":
                    hand = Hand.Rock;
                    return false;

                default:
                    Console.WriteLine("1, 2, 3 중에서 입력해 주세요.");
                    break;
            }
        }
    }
}
