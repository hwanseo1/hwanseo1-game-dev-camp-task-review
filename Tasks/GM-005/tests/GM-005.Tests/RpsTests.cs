using RockPaperScissors;

namespace GM_005.Tests;

public class RpsTests
{
    // ── 승패 판정: 3x3 전체 조합 ──

    [Theory]
    [InlineData(Hand.Scissors, Hand.Paper)]
    [InlineData(Hand.Rock, Hand.Scissors)]
    [InlineData(Hand.Paper, Hand.Rock)]
    public void 이기는_조합은_Win이다(Hand me, Hand com)
    {
        Assert.Equal(Result.Win, Rps.Judge(me, com));
    }

    [Theory]
    [InlineData(Hand.Scissors, Hand.Rock)]
    [InlineData(Hand.Rock, Hand.Paper)]
    [InlineData(Hand.Paper, Hand.Scissors)]
    public void 지는_조합은_Lose이다(Hand me, Hand com)
    {
        Assert.Equal(Result.Lose, Rps.Judge(me, com));
    }

    [Theory]
    [InlineData(Hand.Scissors)]
    [InlineData(Hand.Rock)]
    [InlineData(Hand.Paper)]
    public void 같은_손이면_Draw이다(Hand hand)
    {
        Assert.Equal(Result.Draw, Rps.Judge(hand, hand));
    }

    [Theory]
    [InlineData(Hand.Scissors, "가위")]
    [InlineData(Hand.Rock, "바위")]
    [InlineData(Hand.Paper, "보")]
    public void 손_이름을_반환한다(Hand hand, string expected)
    {
        Assert.Equal(expected, Rps.Name(hand));
    }

    // ── 정산 ──

    [Fact]
    public void 승리하면_판돈의_3배를_얻는다()
    {
        Assert.Equal(3000, Rps.Delta(Result.Win, 1000, 10000));
    }

    [Fact]
    public void 무승부면_판돈의_5배를_얻는다()
    {
        Assert.Equal(5000, Rps.Delta(Result.Draw, 1000, 10000));
    }

    [Fact]
    public void 패배하면_판돈의_7배를_잃는다()
    {
        Assert.Equal(-7000, Rps.Delta(Result.Lose, 1000, 10000));
    }

    [Theory]
    [InlineData(10000, 10000, -10000)]   // 전액 배팅: 7배 손실이 소지금을 넘어 소지금만큼만
    [InlineData(5000, 2000, -5000)]      // 14000 손실 → 5000으로 제한
    [InlineData(9000, 1000, -7000)]      // 7000 < 9000 이므로 제한 없음
    public void 패배_손실은_소지금을_넘지_않는다(int money, int bet, int expectedDelta)
    {
        Assert.Equal(expectedDelta, Rps.Delta(Result.Lose, bet, money));
    }

    [Theory]
    [InlineData(10000, 10000)]
    [InlineData(5000, 2000)]
    [InlineData(1000, 1000)]
    public void 정산_후_소지금은_음수가_되지_않는다(int money, int bet)
    {
        int after = Rps.Settle(money, bet, Result.Lose);

        Assert.True(after >= 0, $"소지금 {money}, 배팅 {bet} → {after}");
    }

    [Fact]
    public void 전액_배팅_후_패배하면_소지금이_0원이_된다()
    {
        Assert.Equal(0, Rps.Settle(Rps.StartMoney, Rps.StartMoney, Result.Lose));
    }

    // ── 배팅 검증 ──

    [Theory]
    [InlineData(1000, 10000)]    // 최소 금액
    [InlineData(5000, 10000)]
    [InlineData(10000, 10000)]   // 소지금 전액
    public void 유효한_배팅은_통과한다(int bet, int money)
    {
        Assert.True(Rps.IsValidBet(bet, money, out string error));
        Assert.Null(error);
    }

    [Theory]
    [InlineData(999)]
    [InlineData(500)]
    [InlineData(0)]
    [InlineData(-1000)]
    public void 최소_금액_미달은_거부된다(int bet)
    {
        Assert.False(Rps.IsValidBet(bet, 10000, out string error));
        Assert.Contains("최소", error);
    }

    [Theory]
    [InlineData(10001, 10000)]
    [InlineData(99999, 10000)]
    public void 소지금_초과는_거부된다(int bet, int money)
    {
        Assert.False(Rps.IsValidBet(bet, money, out string error));
        Assert.Contains("소지금", error);
    }

    // ── 명세 모순 기록 ──

    [Fact]
    public void 명세모순_무승부가_승리보다_이득이다()
    {
        // 과제 명세: 승리 = 판돈 x3 / 무승부 = 판돈 x5.
        // 이기는 것보다 비기는 게 이득이라 "매판 같은 손만 내서 비기기"가 최적 전략이 된다.
        // 명세대로 구현했으므로 이 테스트는 통과한다. 통과 자체가 모순의 증거다.
        int bet = 1000;

        int win  = Rps.Delta(Result.Win, bet, Rps.StartMoney);
        int draw = Rps.Delta(Result.Draw, bet, Rps.StartMoney);

        Assert.True(draw > win, $"무승부 {draw}원 > 승리 {win}원");
    }

    [Fact]
    public void 명세모순_최소_배팅으로도_두판이면_파산한다()
    {
        // 패배 x7 손실이 과해서, 최소 배팅(1000원)만 해도 2연패로 소지금이 0원이 된다.
        int money = Rps.StartMoney;                      // 10000

        money = Rps.Settle(money, Rps.MinBet, Result.Lose);   // -7000 → 3000
        Assert.Equal(3000, money);

        money = Rps.Settle(money, Rps.MinBet, Result.Lose);   // -7000 → 3000만 잃어 0원
        Assert.Equal(0, money);
    }
}
