using GameDatabase;

namespace GM_012.Tests;

public class GameDbTests
{
    static readonly (bool, bool, bool) NoNote = (false, false, false);

    static string[] Titles(Game[] games) => games.Select(g => g.Title).ToArray();

    // ── 1. 데이터 ──

    [Fact]
    public void 게임은_5개를_코드에서_지정한다()
    {
        Assert.Equal(5, GameDb.Create().Games.Length);
    }

    [Fact]
    public void 다섯_게임의_타이틀은_모두_다르다()
    {
        Game[] games = GameDb.Create().Games;
        Assert.Equal(games.Length, games.Select(g => g.Title).Distinct().Count());
    }

    [Fact]
    public void 이용_등급_세_가지가_모두_쓰인다()
    {
        Game[] games = GameDb.Create().Games;
        Assert.Contains(games, g => g.Rating == AgeRating.All);
        Assert.Contains(games, g => g.Rating == AgeRating.Teen);
        Assert.Contains(games, g => g.Rating == AgeRating.Adult);
    }

    [Fact]
    public void 평점은_0에서_10_사이이고_가격은_양수다()
    {
        foreach (Game game in GameDb.Create().Games)
        {
            Assert.InRange(game.Score, 0.0, 10.0);
            Assert.True(game.Price > 0, game.Title);
        }
    }

    [Fact]
    public void 조건_세_가지_모두_결과가_있고_전체보다_적다()
    {
        // 예시 데이터가 선별 작업을 실제로 보여 주는지 (전부 걸리거나 하나도 안 걸리면 확인이 안 된다)
        GameDb db = GameDb.Create();
        foreach (Game[] result in new[] { db.ScoreAtLeast(GameDb.HighScore), db.WithRating(AgeRating.Adult), db.WithAnyNote() })
            Assert.InRange(result.Length, 1, db.Games.Length - 1);
    }

    // ── 2. 조건 선별 ──

    [Fact]
    public void 평점_9점_이상은_엘든링과_스타듀밸리다()
    {
        Assert.Equal(new[] { "엘든 링", "스타듀 밸리" }, Titles(GameDb.Create().ScoreAtLeast(9.0)));
    }

    [Theory]
    [InlineData(9.0, true)]    // 경계: "이상"이므로 포함
    [InlineData(8.9, false)]
    [InlineData(10.0, true)]
    [InlineData(0.0, false)]
    public void 평점_경계_9점은_포함한다(double score, bool expected)
    {
        GameDb db = new GameDb(new[] { new Game("테스트", AgeRating.All, 1000, score, NoNote) });
        Assert.Equal(expected, db.ScoreAtLeast(9.0).Length == 1);
    }

    [Fact]
    public void Adult_등급은_엘든링과_사이버펑크다()
    {
        Assert.Equal(new[] { "엘든 링", "사이버펑크 2077" }, Titles(GameDb.Create().WithRating(AgeRating.Adult)));
    }

    [Theory]
    [InlineData(AgeRating.All, 1)]
    [InlineData(AgeRating.Teen, 2)]
    [InlineData(AgeRating.Adult, 2)]
    public void 등급별_게임_수는_All_1_Teen_2_Adult_2다(AgeRating rating, int count)
    {
        Assert.Equal(count, GameDb.Create().WithRating(rating).Length);
    }

    [Fact]
    public void 특이사항_있는_게임은_세_개다()
    {
        Assert.Equal(new[] { "몬스터 헌터 와일즈", "사이버펑크 2077", "팰월드" }, Titles(GameDb.Create().WithAnyNote()));
    }

    [Fact]
    public void 선별해도_원래_순서를_유지하고_원본은_그대로다()
    {
        GameDb db = GameDb.Create();
        string[] before = Titles(db.Games);
        db.WithAnyNote();
        db.ScoreAtLeast(9.0);
        Assert.Equal(before, Titles(db.Games));
    }

    [Fact]
    public void 빈_데이터베이스는_빈_결과를_준다()
    {
        GameDb db = new GameDb(Array.Empty<Game>());
        Assert.Empty(db.ScoreAtLeast(9.0));
        Assert.Empty(db.WithRating(AgeRating.Adult));
        Assert.Empty(db.WithAnyNote());
    }

    // ── 3. 특이사항 (튜플) ──

    [Theory]
    [InlineData(false, false, false, false, "없음")]
    [InlineData(true, false, false, true, "DLC 곧 나온다.")]
    [InlineData(false, true, false, true, "최적화 좋지 않다.")]
    [InlineData(false, false, true, true, "버그 매우 많음")]
    [InlineData(true, false, true, true, "DLC 곧 나온다. / 버그 매우 많음")]
    [InlineData(true, true, true, true, "DLC 곧 나온다. / 최적화 좋지 않다. / 버그 매우 많음")]
    public void 특이사항은_해당하는_것만_표시한다(bool dlc, bool opt, bool bugs, bool hasAny, string text)
    {
        Game game = new Game("테스트", AgeRating.All, 1000, 5.0, (dlc, opt, bugs));
        Assert.Equal(hasAny, game.HasAnyNote());
        Assert.Equal(text, game.NotesText());
    }

    [Fact]
    public void 튜플_항목은_이름으로_꺼낼_수_있다()
    {
        Game game = new Game("테스트", AgeRating.All, 1000, 5.0, (DlcSoon: false, BadOptimization: true, ManyBugs: false));
        Assert.False(game.Notes.DlcSoon);
        Assert.True(game.Notes.BadOptimization);
        Assert.False(game.Notes.ManyBugs);
    }

    // ── 4. 출력 형식 ──

    [Theory]
    [InlineData(64800, "64,800원")]
    [InlineData(1000, "1,000원")]
    [InlineData(0, "0원")]
    public void 가격은_천_단위_쉼표로_표시한다(int price, string expected)
    {
        Assert.Equal(expected, new Game("테스트", AgeRating.All, price, 5.0, NoNote).PriceText());
    }

    [Theory]
    [InlineData(9.0, "9.0")]
    [InlineData(9.5, "9.5")]
    [InlineData(10, "10.0")]
    public void 평점은_소수점_한_자리로_표시한다(double score, string expected)
    {
        Assert.Equal(expected, new Game("테스트", AgeRating.All, 1000, score, NoNote).ScoreText());
    }

    // ── 5. 구조체 ──

    [Fact]
    public void 게임은_구조체라_복사본을_바꿔도_원본이_그대로다()
    {
        GameDb db = GameDb.Create();
        Game copy = db.Games[0];
        copy.Score = 0.0;
        Assert.Equal(9.5, db.Games[0].Score);
        Assert.True(typeof(Game).IsValueType);
        Assert.True(typeof(GameDb).IsValueType);
    }

    [Fact]
    public void 명세결함_평점_만점_기준이_없어_5점_만점이면_9점_이상_조건이_항상_비어_있다()
    {
        // 명세는 "평점"만 있고 만점이 없다. 5점 만점 데이터를 넣으면 예시 조건 "9.0 이상"은 어떤 게임도 고르지 못한다.
        GameDb fivePointScale = new GameDb(new[]
        {
            new Game("A", AgeRating.All, 1000, 5.0, NoNote),
            new Game("B", AgeRating.All, 1000, 4.8, NoNote),
        });
        Assert.Empty(fivePointScale.ScoreAtLeast(GameDb.HighScore));
    }
}
