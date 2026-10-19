namespace VillageProsperity;

// 시장: 주변 집이 많을수록 손님이 늘고, 같은 줄에 다른 시장이 있으면 물류 네트워크 보너스를 받는다.
public class Market : Building
{
    public const int NetworkBaseProsperity = 35;  // 같은 행 · 열에 다른 Market이 있을 때의 기본 번영도
    public const int CustomerBonus = 10;          // 인접 4방향 House 하나당
    public const int ParkViewBonus = 5;           // 인접 8방향 Park 하나당

    public Market() : base("시장", 20, 'M') { }

    public override int GetDailyProsperity(Building[,] village, int row, int col)
    {
        // 1. 기본 번영도 20으로 시작
        int prosperity = BaseProsperity;
        // 2. 같은 행 또는 같은 열에 다른 Market이 있으면 기본 번영도를 35로 변경 (행 · 열 둘 다여도 35)
        if (AnyInSameLine<Market>(village, row, col)) prosperity = NetworkBaseProsperity;
        // 3. 인접 4방향의 House 수만큼 +10
        prosperity += CountAround4<House>(village, row, col) * CustomerBonus;
        // 4. 인접 8방향의 Park 수만큼 +5
        prosperity += CountAround8<Park>(village, row, col) * ParkViewBonus;
        // 5. 최종 번영도 반환
        return prosperity;
    }
}
