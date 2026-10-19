namespace VillageProsperity;

// 모든 건물의 부모 클래스 (명세 1단계).
// 건물 이름 · 기본 번영도 · 최종 번영도 계산 함수를 공통으로 가진다.
public abstract class Building
{
    public string Name { get; }
    public int BaseProsperity { get; }

    // 지도에 찍을 한 글자 (H · M · P)
    public char Symbol { get; }

    protected Building(string name, int baseProsperity, char symbol)
    {
        Name = name;
        BaseProsperity = baseProsperity;
        Symbol = symbol;
    }

    // 자신의 위치(row, col)와 마을 전체 그리드를 보고 하루 최종 번영도를 계산한다.
    // 건물은 배열 안에서 자기 칸을 모르므로 위치를 인자로 받는다.
    public abstract int GetDailyProsperity(Building[,] village, int row, int col);

    // 상하좌우
    static readonly (int Dr, int Dc)[] Around4 = { (-1, 0), (1, 0), (0, -1), (0, 1) };

    // 대각선 포함 8방향
    static readonly (int Dr, int Dc)[] Around8 =
    {
        (-1, -1), (-1, 0), (-1, 1),
        ( 0, -1),          ( 0, 1),
        ( 1, -1), ( 1, 0), ( 1, 1),
    };

    // 인접 4방향에 있는 T 건물 수
    protected static int CountAround4<T>(Building[,] village, int row, int col) where T : Building
    {
        return CountAt<T>(village, row, col, Around4);
    }

    // 인접 8방향(대각선 포함)에 있는 T 건물 수
    protected static int CountAround8<T>(Building[,] village, int row, int col) where T : Building
    {
        return CountAt<T>(village, row, col, Around8);
    }

    // 같은 행 또는 같은 열에 (자기 칸을 뺀) T 건물이 하나라도 있는지. 사이에 무엇이 있든 상관없다.
    protected static bool AnyInSameLine<T>(Building[,] village, int row, int col) where T : Building
    {
        for (int c = 0; c < village.GetLength(1); c++)
        {
            if (c != col && village[row, c] is T) return true;
        }
        for (int r = 0; r < village.GetLength(0); r++)
        {
            if (r != row && village[r, col] is T) return true;
        }
        return false;
    }

    static int CountAt<T>(Building[,] village, int row, int col, (int Dr, int Dc)[] offsets) where T : Building
    {
        int count = 0;
        foreach ((int dr, int dc) in offsets)
        {
            int r = row + dr;
            int c = col + dc;
            // 마을 밖은 건너뛴다 (가장자리 · 모서리 칸)
            if (r < 0 || r >= village.GetLength(0) || c < 0 || c >= village.GetLength(1)) continue;
            if (village[r, c] is T) count++;
        }
        return count;
    }
}
