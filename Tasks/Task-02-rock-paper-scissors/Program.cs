using System;
using System.Text;

class Program
{
    const int StartMoney = 10000;   // 초기 소지금
    const int MinBet     = 1000;    // 최소 배팅 금액
    const int MaxRound   = 5;       // 최대 판 수

    enum Hand { Scissors = 1, Rock = 2, Paper = 3 }
    enum Result { Win, Lose, Draw }

    static Random rng = new Random();
    static bool cheat = false;

    static void Main(string[] args)
    {
        Console.OutputEncoding = Encoding.UTF8;

        // 테스트 재현용: 인자로 시드를 넘기면 같은 결과가 나온다.
        int seed;
        if (args.Length > 0 && int.TryParse(args[0], out seed)) rng = new Random(seed);

        int money = StartMoney;
        int round = 1;

        Console.WriteLine("=== 가위바위보 배팅 게임 ===");
        Console.WriteLine("소지금 " + StartMoney + "원 / 최소 배팅 " + MinBet + "원 / 최대 " + MaxRound + "판");
        Console.WriteLine("승리 = 판돈 x3 획득 | 무승부 = 판돈 x5 획득 | 패배 = 판돈 x7 손실");
        Console.WriteLine();

        while (round <= MaxRound && money > 0)
        {
            if (money < MinBet)
            {
                Console.WriteLine("소지금이 최소 배팅 금액(" + MinBet + "원)보다 적어 종료합니다.");
                break;
            }

            Console.WriteLine("--- " + round + "/" + MaxRound + "판 | 소지금 " + money + "원 ---");

            int bet = ReadBet(money);
            if (bet == 0) { Console.WriteLine("게임을 중단합니다."); break; }

            Hand com = (Hand)rng.Next(1, 4);

            Hand hand;
            if (!ReadHand(com, out hand)) { Console.WriteLine("게임을 중단합니다."); break; }

            Console.WriteLine("나: " + Name(hand) + "  vs  컴퓨터: " + Name(com));

            // 기본 골격을 switch로 구성 (정산)
            switch (Judge(hand, com))
            {
                case Result.Win:
                    money += bet * 3;
                    Console.WriteLine("승리! +" + (bet * 3) + "원");
                    break;

                case Result.Draw:
                    money += bet * 5;
                    Console.WriteLine("무승부! +" + (bet * 5) + "원");
                    break;

                case Result.Lose:
                    int loss = bet * 7;
                    if (loss > money) loss = money;   // 소지금보다 많이 잃지 않도록 보정
                    money -= loss;
                    Console.WriteLine("패배... -" + loss + "원");
                    break;
            }

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
            int diff = money - StartMoney;
            string sign = diff >= 0 ? "+" : "";
            Console.WriteLine("게임 종료. 최종 소지금 " + money + "원 (" + sign + diff + "원)");
        }
    }

    // 배팅 금액 입력. 0을 반환하면 중단.
    static int ReadBet(int money)
    {
        while (true)
        {
            Console.Write("배팅 금액 (" + MinBet + "~" + money + "원, 중단 q): ");
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
            if (bet < MinBet)
            {
                Console.WriteLine("최소 " + MinBet + "원 이상 배팅해야 합니다.");
                continue;
            }
            if (bet > money)
            {
                Console.WriteLine("소지금(" + money + "원)보다 많이 배팅할 수 없습니다.");
                continue;
            }
            return bet;
        }
    }

    // 손 입력. false를 반환하면 중단. 치트(c)는 컴퓨터의 손을 미리 보여준다.
    static bool ReadHand(Hand com, out Hand hand)
    {
        while (true)
        {
            Console.Write("가위(1) 바위(2) 보(3) | 치트(c) 중단(q): ");
            string s = Console.ReadLine();
            if (s == null) { hand = Hand.Rock; return false; }

            switch (s.Trim().ToLower())
            {
                case "1": hand = Hand.Scissors; return true;
                case "2": hand = Hand.Rock;     return true;
                case "3": hand = Hand.Paper;    return true;

                case "c":
                    cheat = !cheat;
                    if (cheat) Console.WriteLine("[치트 ON] 컴퓨터의 손: " + Name(com));
                    else       Console.WriteLine("[치트 OFF]");
                    break;

                case "q":
                    hand = Hand.Rock;
                    return false;

                default:
                    Console.WriteLine("1, 2, 3 중에서 입력해 주세요.");
                    break;
            }
        }
    }

    static Result Judge(Hand me, Hand com)
    {
        if (me == com) return Result.Draw;

        switch (me)
        {
            case Hand.Scissors: return com == Hand.Paper    ? Result.Win : Result.Lose;
            case Hand.Rock:     return com == Hand.Scissors ? Result.Win : Result.Lose;
            default:            return com == Hand.Rock     ? Result.Win : Result.Lose;  // 보
        }
    }

    static string Name(Hand h)
    {
        switch (h)
        {
            case Hand.Scissors: return "가위";
            case Hand.Rock:     return "바위";
            default:            return "보";
        }
    }
}
