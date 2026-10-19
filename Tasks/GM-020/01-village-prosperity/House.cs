namespace VillageProsperity;

// 집: 같은 주거 구역 · 가까운 시장 · 공원 경관에서 보너스를 받는다.
public class House : Building
{
    public const int NeighborHouseBonus = 5;   // 인접 4방향 House 하나당
    public const int NearMarketBonus = 10;     // 인접 4방향에 Market이 하나 이상
    public const int ParkViewBonus = 5;        // 인접 8방향 Park 하나당

    public House() : base("집", 10, 'H') { }

    public override int GetDailyProsperity(Building[,] village, int row, int col)
    {
        // 1. 기본 번영도 10으로 시작
        int prosperity = BaseProsperity;
        // 2. 인접 4방향의 House 수만큼 +5
        prosperity += CountAround4<House>(village, row, col) * NeighborHouseBonus;
        // 3. 인접 4방향에 Market이 하나 이상 있으면 +10 (여러 개여도 한 번)
        if (CountAround4<Market>(village, row, col) > 0) prosperity += NearMarketBonus;
        // 4. 인접 8방향의 Park 수만큼 +5
        prosperity += CountAround8<Park>(village, row, col) * ParkViewBonus;
        // 5. 최종 번영도 반환
        return prosperity;
    }
}
