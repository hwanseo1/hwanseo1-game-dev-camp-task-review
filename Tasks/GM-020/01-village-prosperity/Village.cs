namespace VillageProsperity;

// 마을 관리자. 칸마다 건물 종류를 묻지 않고, 각 건물이 자기 규칙으로 계산하게 맡긴다.
public static class Village
{
    public const int Size = 5;

    // 2중 반복문으로 전체를 돌며 null이 아닌 칸의 GetDailyProsperity()를 더한다.
    public static int TotalProsperity(Building[,] village)
    {
        int total = 0;
        for (int row = 0; row < village.GetLength(0); row++)
        {
            for (int col = 0; col < village.GetLength(1); col++)
            {
                Building building = village[row, col];
                if (building == null) continue;
                total += building.GetDailyProsperity(village, row, col);
            }
        }
        return total;
    }

    // 명세 4단계의 배치 예시 그대로
    public static Building[,] CreateSpecExample()
    {
        Building[,] village = new Building[Size, Size];
        village[0, 0] = new House();
        village[0, 1] = new Market();
        village[1, 1] = new Park();
        return village;
    }

    // 모든 규칙이 한 번 이상 쓰이도록 정한 예시 마을
    //   H H M . .
    //   H P H . .
    //   . H M H .
    //   . . . . M
    //   P . . . H
    public static Building[,] CreateSampleVillage()
    {
        Building[,] village = new Building[Size, Size];
        village[0, 0] = new House();
        village[0, 1] = new House();
        village[0, 2] = new Market();
        village[1, 0] = new House();
        village[1, 1] = new Park();
        village[1, 2] = new House();
        village[2, 1] = new House();
        village[2, 2] = new Market();
        village[2, 3] = new House();
        village[3, 4] = new Market();
        village[4, 0] = new Park();
        village[4, 4] = new House();
        return village;
    }
}
