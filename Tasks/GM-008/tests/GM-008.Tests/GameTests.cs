using WolnamPpong;

namespace GM_008.Tests;

public class GameTests
{
    static Card C(Suit s, int n) => new Card(s, n);

    // 지정한 카드를 덱 맨 위에 순서대로 놓고, 나머지는 정렬된 순서로 채운다.
    static List<Card> DeckWith(params Card[] top)
    {
        List<Card> rest = Game.NewDeck();
        foreach (Card c in top) Assert.True(rest.Remove(c));
        List<Card> deck = new List<Card>(top);
        deck.AddRange(rest);
        return deck;
    }

    // ── 덱 · 출력 변환 ──

    [Fact]
    public void 덱은_52장이고_중복이_없다()
    {
        List<Card> deck = Game.NewDeck();
        Assert.Equal(52, deck.Count);
        Assert.Equal(52, deck.Distinct().Count());
    }

    [Fact]
    public void 문양마다_1부터_13까지_13장이다()
    {
        foreach (Suit s in Enum.GetValues<Suit>())
        {
            List<int> numbers = Game.NewDeck().Where(c => c.Suit == s).Select(c => c.Number).ToList();
            Assert.Equal(Enumerable.Range(1, 13), numbers);
        }
    }

    [Theory]
    [InlineData(1, "A")]
    [InlineData(11, "J")]
    [InlineData(12, "Q")]
    [InlineData(13, "K")]
    [InlineData(2, "2")]
    [InlineData(10, "10")]
    public void 출력_변환(int number, string text)
    {
        Assert.Equal(text, Game.RankText(number));
    }

    [Fact]
    public void 카드는_문양과_변환된_숫자로_출력된다()
    {
        Assert.Equal("♥ A", C(Suit.Heart, 1).ToString());
        Assert.Equal("♣ K", C(Suit.Club, 13).ToString());
        Assert.Equal("♠ 7", C(Suit.Spade, 7).ToString());
        Assert.Equal("♦ 10", C(Suit.Diamond, 10).ToString());
    }

    [Fact]
    public void 셔플해도_52장_구성은_그대로다()
    {
        List<Card> deck = Game.NewDeck();
        Game.Shuffle(deck, new Random(1));
        Assert.Equal(52, deck.Distinct().Count());
        Assert.NotEqual(Game.NewDeck(), deck);
    }

    // ── 판정 ──

    [Theory]
    [InlineData(1, 13, 7, Result.Win)]
    [InlineData(1, 13, 2, Result.Win)]
    [InlineData(1, 13, 12, Result.Win)]
    [InlineData(1, 13, 1, Result.Lose)]    // 경계값은 미포함
    [InlineData(1, 13, 13, Result.Lose)]
    [InlineData(6, 8, 7, Result.Win)]
    [InlineData(6, 8, 9, Result.Lose)]
    [InlineData(6, 8, 3, Result.Lose)]
    [InlineData(13, 4, 5, Result.Win)]     // 큰 수가 앞이어도 사이면 승리
    [InlineData(11, 1, 13, Result.Lose)]
    public void 가운데_카드가_두_장_사이_초과_미만이면_승리(int first, int second, int third, Result expected)
    {
        Assert.Equal(expected, Game.Judge(C(Suit.Heart, first), C(Suit.Club, second), C(Suit.Spade, third)));
    }

    [Fact]
    public void 문양은_판정에_쓰지_않는다()
    {
        foreach (Suit s in Enum.GetValues<Suit>())
        {
            Assert.Equal(Result.Win,  Game.Judge(C(Suit.Heart, 1), C(Suit.Heart, 13), C(s, 7)));
            Assert.Equal(Result.Lose, Game.Judge(C(Suit.Heart, 1), C(Suit.Heart, 13), C(s, 13)));
        }
    }

    [Theory]
    [InlineData(7, 5)]
    [InlineData(7, 7)]
    [InlineData(1, 13)]
    public void 앞_두_장이_같은_숫자면_가운데와_상관없이_Pair(int number, int third)
    {
        Assert.Equal(Result.Pair, Game.Judge(C(Suit.Heart, number), C(Suit.Club, number), C(Suit.Spade, third)));
    }

    // ── 정산 · 배팅 ──

    [Theory]
    [InlineData(Result.Win, 1000, 10000, 2000)]
    [InlineData(Result.Win, 5000, 10000, 10000)]
    [InlineData(Result.Lose, 3000, 10000, -3000)]
    [InlineData(Result.Pair, 0, 10000, -1000)]
    [InlineData(Result.Pair, 0, 500, -500)]     // 1,000원 미만이면 남은 만큼만
    public void 정산(Result result, int bet, int money, int delta)
    {
        Assert.Equal(delta, Game.Delta(result, bet, money));
    }

    [Theory]
    [InlineData(999, 10000, false)]
    [InlineData(1000, 10000, true)]
    [InlineData(10000, 10000, true)]
    [InlineData(10001, 10000, false)]
    [InlineData(0, 10000, false)]
    [InlineData(-1000, 10000, false)]
    [InlineData(500, 500, true)]       // 소지금 1,000원 미만: 전액이 최소 배팅
    [InlineData(499, 500, false)]
    [InlineData(1000, 500, false)]
    [InlineData(0, 0, false)]
    public void 배팅_검증(int bet, int money, bool expected)
    {
        Assert.Equal(expected, Game.IsValidBet(bet, money));
    }

    // ── 게임 흐름 ──

    [Fact]
    public void 한_판에_3장씩_앞_가운데_뒤_순서로_나눈다()
    {
        Game g = new Game(DeckWith(C(Suit.Heart, 1), C(Suit.Club, 13), C(Suit.Spade, 7)));
        g.Deal();

        Assert.Equal(C(Suit.Heart, 1), g.First);
        Assert.Equal(C(Suit.Club, 13), g.Second);
        Assert.Equal(C(Suit.Spade, 7), g.Hidden);
        Assert.Equal(1, g.Round);
        Assert.Equal(49, g.RemainingCount);
    }

    [Fact]
    public void 승리하면_배팅금의_2배를_얻는다()
    {
        Game g = new Game(DeckWith(C(Suit.Heart, 1), C(Suit.Club, 13), C(Suit.Spade, 7)));
        g.Deal();

        Assert.Equal(Result.Win, g.Play(3000, out int delta));
        Assert.Equal(6000, delta);
        Assert.Equal(16000, g.Money);
    }

    [Fact]
    public void 패배하면_배팅한_만큼_차감된다()
    {
        Game g = new Game(DeckWith(C(Suit.Heart, 6), C(Suit.Club, 8), C(Suit.Spade, 8)));
        g.Deal();

        Assert.Equal(Result.Lose, g.Play(3000, out _));
        Assert.Equal(7000, g.Money);
    }

    [Fact]
    public void 같은_숫자면_배팅_없이_1000원_차감()
    {
        Game g = new Game(DeckWith(C(Suit.Heart, 7), C(Suit.Club, 7), C(Suit.Spade, 5)));
        g.Deal();

        Assert.True(g.IsPair);
        Assert.Equal(Result.Pair, g.Play(0, out _));
        Assert.Equal(9000, g.Money);
    }

    [Fact]
    public void 잘못된_배팅은_예외다()
    {
        Game g = new Game(DeckWith(C(Suit.Heart, 1), C(Suit.Club, 13), C(Suit.Spade, 7)));
        g.Deal();
        Assert.Throws<ArgumentOutOfRangeException>(() => g.Play(999, out _));
        Assert.Throws<ArgumentOutOfRangeException>(() => g.Play(10001, out _));
    }

    [Fact]
    public void 폴드하면_소지금은_그대로이고_카드는_버려진다()
    {
        Game g = new Game(DeckWith(C(Suit.Heart, 1), C(Suit.Club, 13), C(Suit.Spade, 7)));
        g.Deal();
        // 폴드 = Play를 부르지 않고 다음 판으로
        g.Deal();

        Assert.Equal(10000, g.Money);
        Assert.Equal(2, g.Round);
        Assert.DoesNotContain(C(Suit.Spade, 7), g.Remaining());
    }

    [Fact]
    public void 열일곱_판을_마치면_카드_부족으로_끝나고_1장이_남는다()
    {
        Game g = new Game(new Random(1));
        HashSet<Card> seen = new HashSet<Card>();
        while (g.CanDeal)
        {
            g.Deal();
            Assert.True(seen.Add(g.First));
            Assert.True(seen.Add(g.Hidden));
            Assert.True(seen.Add(g.Second));
        }

        Assert.Equal(17, g.Round);
        Assert.True(g.IsOutOfCards);
        Assert.Equal(51, seen.Count);                 // 사용한 카드는 다시 나오지 않는다
        Assert.DoesNotContain(g.Leftover, seen);      // 남은 1장은 버려진다
        Assert.Throws<InvalidOperationException>(() => g.Deal());
    }

    [Fact]
    public void 소지금이_0원이면_더_나누지_않는다()
    {
        Game g = new Game(DeckWith(C(Suit.Heart, 6), C(Suit.Club, 8), C(Suit.Spade, 8)));
        g.Deal();
        g.Play(10000, out _);

        Assert.Equal(0, g.Money);
        Assert.True(g.IsBankrupt);
        Assert.False(g.CanDeal);
    }

    [Fact]
    public void 소지금_500원에서_같은_숫자면_0원이_된다()
    {
        Game g = new Game(DeckWith(C(Suit.Heart, 7), C(Suit.Club, 7), C(Suit.Spade, 5)), money: 500);
        g.Deal();
        g.Play(0, out int delta);

        Assert.Equal(-500, delta);
        Assert.Equal(0, g.Money);
        Assert.False(g.CanDeal);
    }

    // ── 치트 ──

    [Fact]
    public void 치트1_다음_판의_가운데_카드를_미리_본다()
    {
        Game g = new Game(new Random(1));
        while (true)
        {
            g.Deal();
            if (!g.TryPeekNextHidden(out Card next)) break;
            g.Deal();
            Assert.Equal(next, g.Hidden);
            if (g.IsOutOfCards) break;
        }
    }

    [Fact]
    public void 치트1_마지막_판에는_다음_판이_없다()
    {
        Game g = new Game(new Random(1));
        while (g.CanDeal) g.Deal();
        Assert.False(g.TryPeekNextHidden(out _));
    }

    [Fact]
    public void 치트2_남은_카드는_이번_판_3장을_뺀_전체다()
    {
        Game g = new Game(new Random(1));
        g.Deal();
        List<Card> rest = g.Remaining();

        Assert.Equal(49, rest.Count);
        Assert.DoesNotContain(g.First, rest);
        Assert.DoesNotContain(g.Hidden, rest);
        Assert.DoesNotContain(g.Second, rest);
        Assert.Equal(rest.OrderBy(c => c.Suit).ThenBy(c => c.Number), rest);   // 문양 → 숫자 순 정렬
    }

    [Fact]
    public void 시드가_같으면_같은_순서로_나온다()
    {
        Game a = new Game(new Random(7)), b = new Game(new Random(7));
        for (int i = 0; i < Game.MaxRounds; i++)
        {
            a.Deal(); b.Deal();
            Assert.Equal(a.Hidden, b.Hidden);
        }
    }

    // ── 명세 결함 ──

    [Fact]
    public void 명세결함_연속된_숫자는_이길_수_있는_카드가_없다()
    {
        // 예: 6 / ? / 7 — "초과 ~ 미만"이라 사이에 들어갈 숫자가 없다. 같은 숫자처럼 무조건 지는 판인데
        // 명세는 같은 숫자만 자동 패배로 다루고, 이 경우는 최소 1,000원을 걸거나 폴드해야 한다.
        for (int n = 1; n <= 12; n++)
            for (int third = 1; third <= 13; third++)
                Assert.Equal(Result.Lose, Game.Judge(C(Suit.Heart, n), C(Suit.Club, n + 1), C(Suit.Spade, third)));
    }

    [Fact]
    public void 명세결함_상시치트와_비용없는_폴드면_같은숫자_판_말고는_돈을_잃지_않는다()
    {
        // 가운데 카드가 항상 보이고 폴드에 비용이 없으니, "이기는 판만 걸고 나머지는 폴드"하면 된다.
        // 시드 1,000개 모두에서 소지금이 줄어든 판은 같은 숫자 판(강제 1,000원)뿐이다.
        for (int seed = 0; seed < 1000; seed++)
        {
            Game g = new Game(new Random(seed));
            while (g.CanDeal)
            {
                g.Deal();
                Result r = Game.Judge(g.First, g.Second, g.Hidden);
                if (r == Result.Lose) continue;   // 폴드

                g.Play(g.Money, out int delta);
                if (r == Result.Pair) Assert.True(delta < 0);
                else Assert.True(delta > 0);
            }
        }
    }

    [Fact]
    public void 명세결함_소지금이_0원도_아니고_최소배팅도_안_되는_상태가_생긴다()
    {
        // 배팅 금액 단위가 없어서 9,500원을 걸고 지면 500원이 남는다.
        // 종료 조건(0원)에 걸리지 않지만 최소 배팅 1,000원은 낼 수 없다.
        Game g = new Game(DeckWith(C(Suit.Heart, 6), C(Suit.Club, 8), C(Suit.Spade, 8)));
        g.Deal();
        g.Play(9500, out _);

        Assert.Equal(500, g.Money);
        Assert.True(g.CanDeal);
        Assert.True(g.Money < Game.MinBet);
        Assert.Equal(500, Game.MinBetFor(g.Money));   // 구현에서는 전액을 최소 배팅으로 정했다
    }
}
