using System;
using System.Text;
using GameDatabase;

// "클래스 x" → class Program을 직접 쓰지 않고 최상위 문(top-level statements)으로 시작한다.
Console.OutputEncoding = Encoding.UTF8;
Console.InputEncoding = Encoding.UTF8;

GameDb db = GameDb.Create();

while (true)
{
    Console.WriteLine();
    Console.WriteLine("=== 게임 데이터베이스 ===");
    Console.WriteLine("1. 모든 게임 정보 출력");
    Console.WriteLine("2. 특정 조건의 게임 출력");
    Console.WriteLine("3. 종료");
    Console.Write("선택 : ");
    string line = Console.ReadLine();
    if (line == null) return;

    switch (line.Trim())
    {
        case "1":
            PrintGames("모든 게임", db.Games);
            break;
        case "2":
            if (!AskFilter(db)) return;
            break;
        case "3":
            Console.WriteLine("프로그램을 종료합니다.");
            return;
        default:
            Console.WriteLine("1 ~ 3 중에서 골라 주세요.");
            break;
    }
}

// 조건을 고르면 선별 결과를 출력하고 메인 메뉴로 돌아간다.
static bool AskFilter(GameDb db)
{
    Console.WriteLine();
    Console.WriteLine("    1. 평점 " + GameDb.HighScore.ToString("0.0") + " 이상 게임");
    Console.WriteLine("    2. Adult 등급 게임");
    Console.WriteLine("    3. 특이사항이 있는 게임");
    while (true)
    {
        Console.Write("조건 선택 : ");
        string line = Console.ReadLine();
        if (line == null) return false;
        switch (line.Trim())
        {
            case "1":
                PrintGames("평점 " + GameDb.HighScore.ToString("0.0") + " 이상", db.ScoreAtLeast(GameDb.HighScore));
                return true;
            case "2":
                PrintGames("Adult 등급", db.WithRating(AgeRating.Adult));
                return true;
            case "3":
                PrintGames("특이사항 있음", db.WithAnyNote());
                return true;
            default:
                Console.WriteLine("1 ~ 3 중에서 골라 주세요.");
                break;
        }
    }
}

static void PrintGames(string heading, Game[] games)
{
    Console.WriteLine();
    Console.WriteLine("--- " + heading + " (" + games.Length + "개) ---");
    if (games.Length == 0)
    {
        Console.WriteLine("해당하는 게임이 없습니다.");
        return;
    }
    for (int i = 0; i < games.Length; i++)
    {
        Game game = games[i];
        Console.WriteLine("[" + (i + 1) + "] " + game.Title);
        Console.WriteLine("    이용 등급 : " + game.Rating);
        Console.WriteLine("    가격      : " + game.PriceText());
        Console.WriteLine("    평점      : " + game.ScoreText());
        Console.WriteLine("    특이사항  : " + game.NotesText());
    }
}
