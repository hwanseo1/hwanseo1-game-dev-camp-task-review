using System.Reflection;
using VillageProsperity;

namespace GM_020.Tests;

public class ProsperityTests
{
    // "H.M.." 같은 문자열 줄로 마을을 만든다. H 집 · M 시장 · P 공원 · . 빈 칸
    static Building[,] Grid(params string[] rows)
    {
        Building[,] village = new Building[rows.Length, rows[0].Length];
        for (int r = 0; r < rows.Length; r++)
        {
            for (int c = 0; c < rows[r].Length; c++)
            {
                village[r, c] = rows[r][c] switch
                {
                    'H' => new House(),
                    'M' => new Market(),
                    'P' => new Park(),
                    _ => null,
                };
            }
        }
        return village;
    }

    static int At(Building[,] village, int row, int col) => village[row, col]!.GetDailyProsperity(village, row, col);

    // ── 1. 부모 클래스 · 상속 ──

    [Theory]
    [InlineData(typeof(House))]
    [InlineData(typeof(Market))]
    [InlineData(typeof(Park))]
    public void 세_건물은_Building을_상속하고_GetDailyProsperity를_스스로_오버라이드한다(Type type)
    {
        Assert.True(type.IsSubclassOf(typeof(Building)));
        MethodInfo method = type.GetMethod(nameof(Building.GetDailyProsperity))!;
        Assert.Equal(type, method.DeclaringType);
    }

    [Fact]
    public void 부모_클래스의_계산_함수는_추상이라_자식이_반드시_구현한다()
    {
        Assert.True(typeof(Building).IsAbstract);
        Assert.True(typeof(Building).GetMethod(nameof(Building.GetDailyProsperity))!.IsAbstract);
    }

    [Fact]
    public void 이름과_기본_번영도는_집_10_시장_20_공원_0이다()
    {
        Building[] buildings = { new House(), new Market(), new Park() };
        Assert.Equal(new[] { "집", "시장", "공원" }, buildings.Select(b => b.Name));
        Assert.Equal(new[] { 10, 20, 0 }, buildings.Select(b => b.BaseProsperity));
    }

    // ── 2. 집 ──

    [Fact]
    public void 혼자_있는_집은_10이다()
    {
        Assert.Equal(10, At(Grid("...", ".H.", "..."), 1, 1));
    }

    [Theory]
    [InlineData(new[] { ".H.", ".H.", "..." }, 15)]
    [InlineData(new[] { ".H.", "HH.", "..." }, 20)]
    [InlineData(new[] { ".H.", "HHH", "..." }, 25)]
    [InlineData(new[] { ".H.", "HHH", ".H." }, 30)]
    public void 집은_인접_4방향의_집_하나당_5를_더한다(string[] rows, int expected)
    {
        Assert.Equal(expected, At(Grid(rows), 1, 1));
    }

    [Fact]
    public void 집은_대각선의_집에서는_보너스를_받지_않는다()
    {
        Assert.Equal(10, At(Grid("H.H", ".H.", "H.H"), 1, 1));
    }

    [Fact]
    public void 집은_인접_4방향에_시장이_있으면_10을_더한다()
    {
        Assert.Equal(20, At(Grid("...", ".HM", "..."), 1, 1));
    }

    [Fact]
    public void 집의_시장_보너스는_시장이_여러_개여도_한_번이다()
    {
        Assert.Equal(20, At(Grid(".M.", "MHM", ".M."), 1, 1));
    }

    [Fact]
    public void 집은_대각선의_시장에서는_보너스를_받지_않는다()
    {
        Assert.Equal(10, At(Grid("M.M", ".H.", "M.M"), 1, 1));
    }

    [Theory]
    [InlineData(new[] { "P..", ".H.", "..." }, 15)]   // 대각선
    [InlineData(new[] { ".P.", ".H.", "..." }, 15)]   // 상하좌우
    [InlineData(new[] { "P.P", ".H.", "P.P" }, 30)]
    [InlineData(new[] { "PPP", "PHP", "PPP" }, 50)]   // 8칸 모두
    public void 집은_인접_8방향의_공원_하나당_5를_더한다(string[] rows, int expected)
    {
        Assert.Equal(expected, At(Grid(rows), 1, 1));
    }

    [Fact]
    public void 집은_두_칸_떨어진_공원에서는_보너스를_받지_않는다()
    {
        Assert.Equal(10, At(Grid("P.P.P", ".....", "P.H.P", ".....", "P.P.P"), 2, 2));
    }

    [Fact]
    public void 집의_규칙_세_가지는_함께_더해진다()
    {
        // 집 2 (+10) · 시장 (+10) · 공원 2 (+10)
        Assert.Equal(40, At(Grid("PH.", "HHM", "..P"), 1, 1));
    }

    // ── 3. 시장 ──

    [Fact]
    public void 혼자_있는_시장은_20이다()
    {
        Assert.Equal(20, At(Grid("...", ".M.", "..."), 1, 1));
    }

    [Fact]
    public void 시장은_같은_행에_다른_시장이_있으면_기본이_35다_떨어져_있어도()
    {
        Assert.Equal(35, At(Grid(".....", "M...M", "....."), 1, 0));
    }

    [Fact]
    public void 시장은_같은_열에_다른_시장이_있으면_기본이_35다_떨어져_있어도()
    {
        Assert.Equal(35, At(Grid(".M.", "...", "...", "...", ".M."), 0, 1));
    }

    [Fact]
    public void 시장_사이에_다른_건물이_있어도_물류_보너스는_그대로다()
    {
        Assert.Equal(35, At(Grid("M.H.M"), 0, 0));
    }

    [Fact]
    public void 행과_열_모두에_시장이_있어도_기본은_35_한_번이다()
    {
        Assert.Equal(35, At(Grid("M.M", "...", "M.."), 0, 0));
    }

    [Fact]
    public void 시장은_대각선에만_다른_시장이_있으면_물류_보너스가_없다()
    {
        Assert.Equal(20, At(Grid("M..", ".M.", "..M"), 1, 1));
    }

    [Theory]
    [InlineData(new[] { ".H.", ".M.", "..." }, 30)]
    [InlineData(new[] { ".H.", "HMH", ".H." }, 60)]
    [InlineData(new[] { "H.H", ".M.", "H.H" }, 20)]   // 대각선의 집은 안 센다
    public void 시장은_인접_4방향의_집_하나당_10을_더한다(string[] rows, int expected)
    {
        Assert.Equal(expected, At(Grid(rows), 1, 1));
    }

    [Theory]
    [InlineData(new[] { "P..", ".M.", "..." }, 25)]
    [InlineData(new[] { "PPP", "PMP", "PPP" }, 60)]
    public void 시장은_인접_8방향의_공원_하나당_5를_더한다(string[] rows, int expected)
    {
        Assert.Equal(expected, At(Grid(rows), 1, 1));
    }

    [Fact]
    public void 시장의_보너스는_바뀐_기본_35_위에_더해진다()
    {
        // 기본 35 · 집 2 (+20) · 공원 1 (+5)
        Assert.Equal(60, At(Grid("PH.", "HM.", "...", ".M."), 1, 1));
    }

    // ── 4. 공원 ──

    [Theory]
    [InlineData(new object[] { new[] { "...", ".P.", "..." } })]
    [InlineData(new object[] { new[] { "HMH", "MPM", "HPH" } })]
    public void 공원은_주변과_상관없이_항상_0이다(string[] rows)
    {
        Assert.Equal(0, At(Grid(rows), 1, 1));
    }

    [Fact]
    public void 공원은_다른_공원에게_보너스를_주지_않는다()
    {
        Building[,] village = Grid("PPP", "PPP", "PPP");
        Assert.Equal(0, Village.TotalProsperity(village));
    }

    // ── 5. 가장자리 ──

    [Theory]
    [InlineData(0, 0)]
    [InlineData(0, 4)]
    [InlineData(4, 0)]
    [InlineData(4, 4)]
    [InlineData(0, 2)]
    [InlineData(2, 4)]
    public void 가장자리_칸에서도_마을_밖은_건너뛰고_계산한다(int row, int col)
    {
        foreach (Building building in new Building[] { new House(), new Market(), new Park() })
        {
            Building[,] village = new Building[Village.Size, Village.Size];
            village[row, col] = building;
            Assert.Equal(building.BaseProsperity, village[row, col].GetDailyProsperity(village, row, col));
        }
    }

    // ── 6. 마을 전체 ──

    [Fact]
    public void 마을은_5x5_Building_2차원_배열이다()
    {
        Building[,] village = Village.CreateSampleVillage();
        Assert.Equal(Village.Size, village.GetLength(0));
        Assert.Equal(Village.Size, village.GetLength(1));
    }

    [Fact]
    public void 빈_마을의_총_번영도는_0이다()
    {
        Assert.Equal(0, Village.TotalProsperity(new Building[Village.Size, Village.Size]));
    }

    [Fact]
    public void 예시_마을의_칸별_번영도는_손으로_계산한_값과_같다()
    {
        Building[,] village = Village.CreateSampleVillage();
        int[,] expected =
        {
            { 25, 30, 60,  0,  0 },
            { 20,  0, 25,  0,  0 },
            {  0, 25, 70, 20,  0 },
            {  0,  0,  0,  0, 30 },
            {  0,  0,  0,  0, 20 },
        };
        for (int r = 0; r < Village.Size; r++)
        {
            for (int c = 0; c < Village.Size; c++)
            {
                int actual = village[r, c] == null ? 0 : At(village, r, c);
                Assert.True(expected[r, c] == actual, $"[{r},{c}] 예상 {expected[r, c]}, 실제 {actual}");
            }
        }
        Assert.Equal(325, Village.TotalProsperity(village));
    }

    [Fact]
    public void 예시_마을은_물류_보너스가_있는_시장과_없는_시장을_모두_가진다()
    {
        // (0,2) · (2,2)는 같은 열, (3,4)는 혼자
        Building[,] village = Village.CreateSampleVillage();
        Assert.Equal(60, At(village, 0, 2));
        Assert.Equal(30, At(village, 3, 4));   // 20 + 집 1
    }

    [Fact]
    public void 총_번영도를_계산해도_마을_배열은_바뀌지_않는다()
    {
        Building[,] village = Village.CreateSampleVillage();
        Building[] before = village.Cast<Building>().ToArray();
        Village.TotalProsperity(village);
        Assert.Equal(before, village.Cast<Building>().ToArray());
    }

    // 관리자 코드는 건물 종류를 묻지 않으므로 새 건물을 넣어도 고칠 곳이 없다.
    class Fountain : Building
    {
        public Fountain() : base("분수", 7, 'F') { }
        public override int GetDailyProsperity(Building[,] village, int row, int col) => BaseProsperity;
    }

    [Fact]
    public void 관리자는_종류를_묻지_않아_새_건물도_고치지_않고_합산한다()
    {
        Building[,] village = new Building[Village.Size, Village.Size];
        village[2, 2] = new Fountain();
        village[0, 0] = new House();
        Assert.Equal(17, Village.TotalProsperity(village));
    }

    // ── 7. 명세 결함 ──

    [Fact]
    public void 명세결함_인자_없는_GetDailyProsperity로는_같은_건물이라도_칸마다_값이_달라_위치를_넘겨야_한다()
    {
        // 명세 최종 목표는 GetDailyProsperity()를 인자 없이 쓰지만, 같은 집 객체도 놓인 칸에 따라 결과가 다르다.
        Building[,] village = Grid("HM...", ".....", ".....", ".....", "....H");
        Building house = village[0, 0]!;
        Assert.Equal(20, house.GetDailyProsperity(village, 0, 0));
        Assert.Equal(10, house.GetDailyProsperity(village, 4, 4));
    }
}
