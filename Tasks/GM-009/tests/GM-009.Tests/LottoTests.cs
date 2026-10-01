using Lotto;

namespace GM_009.Tests;

public class LottoTests
{
    static readonly int[] Winning = { 1, 2, 3, 4, 5, 6 };
    const int Bonus = 7;

    // ── 입력 검사 ──

    [Theory]
    [InlineData("1", 1)]
    [InlineData("45", 45)]
    [InlineData(" 23 ", 23)]
    public void 범위_안의_숫자는_통과한다(string text, int expected)
    {
        int n;
        Assert.Equal(InputError.None, LottoRules.Validate(text, new List<int>(), out n));
        Assert.Equal(expected, n);
    }

    [Theory]
    [InlineData("0")]
    [InlineData("46")]
    [InlineData("-3")]
    [InlineData("100")]
    public void 범위_밖_숫자는_거부한다(string text)
    {
        Assert.Equal(InputError.OutOfRange, LottoRules.Validate(text, new List<int>(), out _));
    }

    [Theory]
    [InlineData("abc")]
    [InlineData("")]
    [InlineData("3.5")]
    [InlineData("1 2")]
    [InlineData("칠")]
    [InlineData(null)]
    public void 숫자가_아닌_값은_거부한다(string? text)
    {
        Assert.Equal(InputError.NotNumber, LottoRules.Validate(text!, new List<int>(), out _));
    }

    [Fact]
    public void 이미_고른_숫자는_거부한다()
    {
        Assert.Equal(InputError.Duplicate, LottoRules.Validate("5", new List<int> { 3, 5 }, out _));
    }

    [Fact]
    public void 숫자_아님_검사가_범위_검사보다_먼저다()
    {
        Assert.Equal(InputError.NotNumber, LottoRules.Validate("99a", new List<int>(), out _));
    }

    // ── 추첨 ──

    [Fact]
    public void 자동_번호는_1에서_45_사이_서로_다른_6개다()
    {
        for (int seed = 0; seed < 1000; seed++)
        {
            List<int> nums = LottoRules.Draw(new Random(seed), 6);
            Assert.Equal(6, nums.Count);
            Assert.Equal(6, nums.Distinct().Count());
            Assert.All(nums, n => Assert.InRange(n, 1, 45));
        }
    }

    [Fact]
    public void 당첨번호는_정렬된_6개이고_보너스는_당첨번호와_겹치지_않는다()
    {
        for (int seed = 0; seed < 1000; seed++)
        {
            int bonus;
            List<int> winning = LottoRules.DrawWinning(new Random(seed), out bonus);
            Assert.Equal(6, winning.Distinct().Count());
            Assert.Equal(winning.OrderBy(n => n), winning);
            Assert.InRange(bonus, 1, 45);
            Assert.DoesNotContain(bonus, winning);
        }
    }

    [Fact]
    public void 추첨은_1부터_45까지_모든_숫자가_나온다()
    {
        HashSet<int> seen = new HashSet<int>();
        Random rng = new Random(1);
        for (int i = 0; i < 1000; i++) seen.UnionWith(LottoRules.Draw(rng, 6));
        Assert.Equal(Enumerable.Range(1, 45), seen.OrderBy(n => n));
    }

    // ── 판정 ──

    [Theory]
    [InlineData(new[] { 1, 2, 3, 4, 5, 6 }, 1)]
    [InlineData(new[] { 1, 2, 3, 4, 5, 7 }, 2)]
    [InlineData(new[] { 1, 2, 3, 4, 5, 45 }, 3)]
    [InlineData(new[] { 1, 2, 3, 4, 44, 45 }, 4)]
    [InlineData(new[] { 1, 2, 3, 43, 44, 45 }, 5)]
    [InlineData(new[] { 1, 2, 42, 43, 44, 45 }, 0)]
    [InlineData(new[] { 40, 41, 42, 43, 44, 45 }, 0)]
    public void 일치_개수로_등수를_정한다(int[] mine, int expected)
    {
        Assert.Equal(expected, LottoRules.Rank(mine, Winning, Bonus));
    }

    [Fact]
    public void 순서가_달라도_같은_등수다()
    {
        Assert.Equal(1, LottoRules.Rank(new[] { 6, 5, 4, 3, 2, 1 }, Winning, Bonus));
        Assert.Equal(2, LottoRules.Rank(new[] { 7, 5, 1, 3, 2, 4 }, Winning, Bonus));
    }

    [Theory]
    [InlineData(new[] { 1, 2, 3, 4, 7, 45 }, 4)]
    [InlineData(new[] { 1, 2, 3, 7, 44, 45 }, 5)]
    [InlineData(new[] { 1, 2, 7, 43, 44, 45 }, 0)]
    public void 보너스는_2등에서만_쓰인다(int[] mine, int expected)
    {
        Assert.Equal(expected, LottoRules.Rank(mine, Winning, Bonus));
    }

    [Theory]
    [InlineData(1, "1등")]
    [InlineData(2, "2등")]
    [InlineData(5, "5등")]
    [InlineData(0, "낙첨")]
    public void 등수_문구(int rank, string prefix)
    {
        Assert.StartsWith(prefix, LottoRules.RankText(rank));
    }

    // ── 명세 결함 ──

    // 명세에 "보너스는 당첨번호와 다른 숫자"라는 규칙이 없다. 겹치게 뽑으면
    // 당첨번호 5개만 맞았는데(그중 하나가 보너스와 같은 숫자) 2등이 된다. 3등과 구분되지 않는다.
    [Fact]
    public void 명세결함_보너스가_당첨번호와_겹치면_5개만_맞아도_2등이_된다()
    {
        int[] winning = { 1, 2, 3, 4, 5, 6 };
        int bonusSameAsWinning = 5;
        int[] mine = { 1, 2, 3, 4, 5, 45 };

        Assert.Equal(5, LottoRules.MatchCount(mine, winning));
        Assert.Equal(2, LottoRules.Rank(mine, winning, bonusSameAsWinning));
    }

    // "중복 숫자 입력 x"는 사용자 입력에만 걸린다. 자동 버전과 당첨번호 추첨에는 중복 규칙이 없다.
    // 중복을 허용해 뽑으면 같은 숫자 하나로 일치 개수가 두 번 세어진다.
    [Fact]
    public void 명세결함_자동번호에_중복을_허용하면_한_숫자가_두_번_맞는다()
    {
        int[] autoWithDuplicate = { 1, 1, 1, 40, 41, 42 };
        Assert.Equal(3, LottoRules.MatchCount(autoWithDuplicate, Winning));
        Assert.Equal(5, LottoRules.Rank(autoWithDuplicate, Winning, Bonus));
    }
}
