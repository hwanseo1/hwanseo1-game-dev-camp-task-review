using System.Collections.Generic;
using System.Globalization;

namespace GameDatabase;

// 이용 등급 (명세: enum 사용 : All, Teen, Adult)
public enum AgeRating { All, Teen, Adult }

// 게임 한 개의 정보. "클래스 x"라서 구조체로 만든다.
public struct Game
{
    public string Title;
    public AgeRating Rating;
    public int Price;      // 원
    public double Score;   // 10점 만점 (명세에 만점 기준이 없어 "9.0 이상" 예시에 맞춤)

    // 특이사항 (명세: 튜플 사용). 세 항목 각각 해당하는지 여부.
    public (bool DlcSoon, bool BadOptimization, bool ManyBugs) Notes;

    public Game(string title, AgeRating rating, int price, double score,
                (bool DlcSoon, bool BadOptimization, bool ManyBugs) notes)
    {
        Title = title;
        Rating = rating;
        Price = price;
        Score = score;
        Notes = notes;
    }

    public bool HasAnyNote()
    {
        return Notes.DlcSoon || Notes.BadOptimization || Notes.ManyBugs;
    }

    // "DLC 곧 나온다. / 버그 매우 많음"처럼 해당하는 것만 이어 붙인다. 하나도 없으면 "없음".
    public string NotesText()
    {
        List<string> parts = new List<string>();
        if (Notes.DlcSoon) parts.Add("DLC 곧 나온다.");
        if (Notes.BadOptimization) parts.Add("최적화 좋지 않다.");
        if (Notes.ManyBugs) parts.Add("버그 매우 많음");
        return parts.Count == 0 ? "없음" : string.Join(" / ", parts);
    }

    public string PriceText()
    {
        return Price.ToString("N0", CultureInfo.InvariantCulture) + "원";
    }

    public string ScoreText()
    {
        return Score.ToString("0.0", CultureInfo.InvariantCulture);
    }
}

// 게임 5개를 담는 구조체 배열과 선별 작업. 이것도 클래스 대신 구조체.
public struct GameDb
{
    public const double HighScore = 9.0;

    public Game[] Games;

    public GameDb(Game[] games)
    {
        Games = games;
    }

    // 데이터는 코드에서 직접 지정한다 (명세: 입력 x).
    // 평점 · 특이사항은 조건 출력을 확인하기 위한 예시 값이다 (실제 평가 자료가 아님).
    public static GameDb Create()
    {
        Game[] games = new Game[]
        {
            new Game("엘든 링",            AgeRating.Adult, 64800, 9.5, (false, false, false)),
            new Game("스타듀 밸리",         AgeRating.All,   16000, 9.0, (false, false, false)),
            new Game("몬스터 헌터 와일즈",  AgeRating.Teen,  79800, 8.2, (false, true,  false)),
            new Game("사이버펑크 2077",     AgeRating.Adult, 66000, 8.6, (false, true,  true)),
            new Game("팰월드",             AgeRating.Teen,  32000, 7.8, (true,  false, true)),
        };
        return new GameDb(games);
    }

    // 1. 평점 9.0 이상
    public Game[] ScoreAtLeast(double minScore)
    {
        List<Game> result = new List<Game>();
        foreach (Game game in Games)
            if (game.Score >= minScore) result.Add(game);
        return result.ToArray();
    }

    // 2. 특정 이용 등급 (명세 예시는 Adult)
    public Game[] WithRating(AgeRating rating)
    {
        List<Game> result = new List<Game>();
        foreach (Game game in Games)
            if (game.Rating == rating) result.Add(game);
        return result.ToArray();
    }

    // 3. 특이사항이 하나라도 있는 게임
    public Game[] WithAnyNote()
    {
        List<Game> result = new List<Game>();
        foreach (Game game in Games)
            if (game.HasAnyNote()) result.Add(game);
        return result.ToArray();
    }
}
