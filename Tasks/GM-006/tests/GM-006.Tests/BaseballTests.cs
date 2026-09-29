using NumberBaseball;

namespace GM_006.Tests;

public class BaseballTests
{
    static readonly int[] Answer = { 3, 6, 9 };

    static Score Judge(int a, int b, int c) => Baseball.Judge(Answer, new[] { a, b, c });

    // ── 판정: 정답 3 6 9 기준 ──

    [Theory]
    [InlineData(3, 6, 9, 3, 0, 0)]   // 전부 일치
    [InlineData(3, 9, 6, 1, 2, 0)]   // 자리 두 개 바뀜
    [InlineData(9, 3, 6, 0, 3, 0)]   // 숫자만 전부 일치
    [InlineData(1, 2, 4, 0, 0, 3)]   // 전부 없음
    [InlineData(3, 1, 2, 1, 0, 2)]
    [InlineData(6, 1, 2, 0, 1, 2)]
    [InlineData(3, 6, 0, 2, 0, 1)]
    [InlineData(0, 3, 6, 0, 2, 1)]
    [InlineData(9, 6, 3, 1, 2, 0)]
    public void 자리마다_SBO를_판정한다(int a, int b, int c, int s, int ball, int o)
    {
        Score score = Judge(a, b, c);

        Assert.Equal(s, score.Strike);
        Assert.Equal(ball, score.Ball);
        Assert.Equal(o, score.Out);
    }

    [Theory]
    [InlineData(3, 6, 9)]
    [InlineData(1, 2, 4)]
    [InlineData(9, 3, 6)]
    [InlineData(3, 1, 2)]
    public void S_B_O의_합은_항상_3이다(int a, int b, int c)
    {
        Score score = Judge(a, b, c);
        Assert.Equal(Baseball.Length, score.Strike + score.Ball + score.Out);
    }

    [Fact]
    public void 삼스트라이크만_승리다()
    {
        Assert.True(Judge(3, 6, 9).IsWin);
        Assert.False(Judge(3, 6, 0).IsWin);
        Assert.False(Judge(9, 3, 6).IsWin);   // 3B는 승리가 아니다
    }

    [Fact]
    public void 결과는_SBO_형식으로_표시된다()
    {
        Assert.Equal("1S 2B 0O", Judge(3, 9, 6).ToString());
        Assert.Equal("0S 0B 3O", Judge(1, 2, 4).ToString());
    }

    [Fact]
    public void 영이_첫자리에_와도_판정된다()
    {
        int[] answer = { 0, 1, 2 };
        Assert.True(Baseball.Judge(answer, new[] { 0, 1, 2 }).IsWin);
    }

    // ── 컴퓨터 숫자 생성 ──

    [Fact]
    public void 생성된_숫자는_0에서_9_사이_서로_다른_3개다()
    {
        for (int seed = 0; seed < 1000; seed++)
        {
            int[] answer = Baseball.Generate(new Random(seed));

            Assert.Equal(Baseball.Length, answer.Length);
            Assert.All(answer, d => Assert.InRange(d, 0, 9));
            Assert.Equal(Baseball.Length, answer.Distinct().Count());
        }
    }

    [Fact]
    public void 생성은_모든_숫자를_모든_자리에_낼_수_있다()
    {
        // 0이 첫자리에 안 나오는 식의 치우침이 없는지
        bool[,] seen = new bool[Baseball.Length, 10];
        for (int seed = 0; seed < 2000; seed++)
        {
            int[] answer = Baseball.Generate(new Random(seed));
            for (int i = 0; i < Baseball.Length; i++) seen[i, answer[i]] = true;
        }

        for (int i = 0; i < Baseball.Length; i++)
            for (int d = 0; d < 10; d++)
                Assert.True(seen[i, d], $"{i}번째 자리에 {d}가 한 번도 안 나옴");
    }

    [Fact]
    public void 같은_시드면_같은_숫자가_나온다()
    {
        Assert.Equal(Baseball.Generate(new Random(1)), Baseball.Generate(new Random(1)));
    }

    // ── 입력 파싱 ──

    [Theory]
    [InlineData("1 2 3", new[] { 1, 2, 3 })]
    [InlineData("123", new[] { 1, 2, 3 })]
    [InlineData("0 5 9", new[] { 0, 5, 9 })]
    [InlineData("  3  6 9 ", new[] { 3, 6, 9 })]
    public void 올바른_입력은_숫자_3개가_된다(string input, int[] expected)
    {
        Assert.True(Baseball.TryParseGuess(input, out int[] guess, out string error));
        Assert.Equal(expected, guess);
        Assert.Null(error);
    }

    [Theory]
    [InlineData("")]
    [InlineData("12")]
    [InlineData("1 2")]
    [InlineData("1234")]
    [InlineData("1 2 3 4")]
    [InlineData(null)]
    public void 개수가_3개가_아니면_거부된다(string? input)
    {
        Assert.False(Baseball.TryParseGuess(input, out int[] guess, out string error));
        Assert.Null(guess);
        Assert.Contains("3개", error);
    }

    [Theory]
    [InlineData("a b c")]
    [InlineData("1 2 x")]
    [InlineData("-12")]
    [InlineData("1,2")]
    public void 숫자가_아니면_거부된다(string input)
    {
        Assert.False(Baseball.TryParseGuess(input, out _, out string error));
        Assert.Contains("숫자만", error);
    }

    [Theory]
    [InlineData("3 3 3")]
    [InlineData("1 1 2")]
    [InlineData("1 2 1")]
    [InlineData("122")]
    public void 중복된_숫자는_거부된다(string input)
    {
        Assert.False(Baseball.TryParseGuess(input, out _, out string error));
        Assert.Contains("서로 다른", error);
    }

    // ── 치트 ──

    [Fact]
    public void 치트_정답은_3_6_9다()
    {
        Assert.Equal(new[] { 3, 6, 9 }, Baseball.CheatAnswer);
        Assert.Equal("3 6 9", Baseball.Format(Baseball.CheatAnswer));
    }

    [Fact]
    public void 치트_정답에_3_6_9를_입력하면_3S다()
    {
        Baseball.TryParseGuess("3 6 9", out int[] guess, out _);
        Assert.True(Baseball.Judge(Baseball.CheatAnswer, guess).IsWin);
    }

    // ── 명세 결함 기록 ──

    [Fact]
    public void 명세결함_중복_입력을_허용하면_규칙대로_1S_2B가_나온다()
    {
        // 명세는 플레이어 숫자의 중복 여부를 정하지 않았다.
        // 판정 규칙을 자리마다 그대로 적용하면, 정답 3 6 9에 3 3 3을 넣었을 때
        // 3이 한 개뿐인데도 1S 2B가 된다. 명세만으로는 이 결과를 막을 수 없다.
        // 그래서 입력 단계에서 중복을 거부했다 (중복된_숫자는_거부된다).
        Score score = Judge(3, 3, 3);

        Assert.Equal(1, score.Strike);
        Assert.Equal(2, score.Ball);
        Assert.Equal(0, score.Out);
    }
}
