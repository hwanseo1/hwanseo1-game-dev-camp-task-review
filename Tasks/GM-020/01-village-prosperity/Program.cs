using System;
using System.Text;
using VillageProsperity;

Console.OutputEncoding = Encoding.UTF8;

Report("마을", Village.CreateSampleVillage());

// 지도 · 건물별 번영도 · 총 번영도를 출력한다.
static void Report(string title, Building[,] village)
{
    Console.WriteLine("=== " + title + " ===");
    PrintMap(village);
    Console.WriteLine();

    for (int row = 0; row < village.GetLength(0); row++)
    {
        for (int col = 0; col < village.GetLength(1); col++)
        {
            Building building = village[row, col];
            if (building == null) continue;
            Console.WriteLine("[" + row + "," + col + "] " + building.Name.PadRight(2) +
                              "  기본 " + building.BaseProsperity.ToString().PadLeft(2) +
                              " → 최종 " + building.GetDailyProsperity(village, row, col).ToString().PadLeft(3));
        }
    }
    Console.WriteLine("총 번영도 : " + Village.TotalProsperity(village));
    Console.WriteLine();
}

static void PrintMap(Building[,] village)
{
    for (int row = 0; row < village.GetLength(0); row++)
    {
        StringBuilder line = new StringBuilder("  ");
        for (int col = 0; col < village.GetLength(1); col++)
        {
            Building building = village[row, col];
            line.Append(building == null ? '.' : building.Symbol);
            line.Append(' ');
        }
        Console.WriteLine(line.ToString().TrimEnd());
    }
    Console.WriteLine("  (H 집 · M 시장 · P 공원 · . 빈 칸)");
}
