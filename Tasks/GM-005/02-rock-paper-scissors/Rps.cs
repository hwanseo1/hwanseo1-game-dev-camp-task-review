namespace RockPaperScissors;

public enum Hand { Scissors = 1, Rock = 2, Paper = 3 }

public enum Result { Win, Lose, Draw }

/// <summary>가위바위보 판정과 배팅 정산. 입출력과 분리해 두어 단위 테스트가 가능하다.</summary>
public static class Rps
{
    public const int StartMoney = 10000;   // 초기 소지금
    public const int MinBet     = 1000;    // 최소 배팅 금액
    public const int MaxRound   = 5;       // 최대 판 수

    public static Result Judge(Hand me, Hand com)
    {
        if (me == com) return Result.Draw;

        switch (me)
        {
            case Hand.Scissors: return com == Hand.Paper    ? Result.Win : Result.Lose;
            case Hand.Rock:     return com == Hand.Scissors ? Result.Win : Result.Lose;
            default:            return com == Hand.Rock     ? Result.Win : Result.Lose;  // 보
        }
    }

    public static string Name(Hand h)
    {
        switch (h)
        {
            case Hand.Scissors: return "가위";
            case Hand.Rock:     return "바위";
            default:            return "보";
        }
    }

    /// <summary>
    /// 소지금 변화량. 과제 조건: 승리 +판돈x3 / 무승부 +판돈x5 / 패배 -판돈x7.
    /// 패배 손실이 소지금을 넘으면 소지금만큼만 잃는다(음수 방지).
    /// </summary>
    public static int Delta(Result result, int bet, int money)
    {
        switch (result)
        {
            case Result.Win:
                return bet * 3;

            case Result.Draw:
                return bet * 5;

            default:   // Lose
                int loss = bet * 7;
                if (loss > money) loss = money;
                return -loss;
        }
    }

    /// <summary>정산 후 소지금. 0원 미만으로 내려가지 않는다.</summary>
    public static int Settle(int money, int bet, Result result)
    {
        return money + Delta(result, bet, money);
    }

    /// <summary>배팅 금액이 유효한지. 유효하지 않으면 error에 안내 문구가 담긴다.</summary>
    public static bool IsValidBet(int bet, int money, out string error)
    {
        if (bet < MinBet)
        {
            error = "최소 " + MinBet + "원 이상 배팅해야 합니다.";
            return false;
        }
        if (bet > money)
        {
            error = "소지금(" + money + "원)보다 많이 배팅할 수 없습니다.";
            return false;
        }
        error = null;
        return true;
    }
}
