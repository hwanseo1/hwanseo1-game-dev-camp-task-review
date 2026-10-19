namespace VillageProsperity;

// 공원: 스스로는 번영도를 만들지 않는다.
// 경관 보너스 +5는 주변 House · Market이 자기 계산에서 인접 8방향의 Park를 세어 더한다.
public class Park : Building
{
    public Park() : base("공원", 0, 'P') { }

    public override int GetDailyProsperity(Building[,] village, int row, int col)
    {
        // 1. 항상 0 반환
        return 0;
    }
}
