using DaysInMonth;

namespace GM_005.Tests;

public class MonthTests
{
    [Theory]
    [InlineData(1, 31)]
    [InlineData(2, 28)]   // 과제 조건: 윤년 무시
    [InlineData(3, 31)]
    [InlineData(4, 30)]
    [InlineData(5, 31)]
    [InlineData(6, 30)]
    [InlineData(7, 31)]
    [InlineData(8, 31)]
    [InlineData(9, 30)]
    [InlineData(10, 31)]
    [InlineData(11, 30)]
    [InlineData(12, 31)]
    public void 유효한_월은_일수를_반환한다(int month, int expected)
    {
        Assert.Equal(expected, Month.GetDays(month));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(13)]
    [InlineData(-1)]
    [InlineData(-5)]
    [InlineData(100)]
    [InlineData(int.MaxValue)]
    [InlineData(int.MinValue)]
    public void 범위_밖의_월은_0을_반환한다(int month)
    {
        // 0은 "존재하지 않는 월" 신호. 호출부가 이 값으로 예외처리한다.
        Assert.Equal(0, Month.GetDays(month));
    }

    [Fact]
    public void 일년_합계는_365일이다()
    {
        int total = 0;
        for (int m = 1; m <= 12; m++) total += Month.GetDays(m);

        Assert.Equal(365, total);   // 2월 28일 기준
    }

    [Fact]
    public void 이월은_윤년을_계산하지_않는다()
    {
        // 실제 달력이라면 윤년에 29일이지만, 과제 조건은 28일 고정이다.
        Assert.Equal(28, Month.GetDays(2));
        Assert.NotEqual(29, Month.GetDays(2));
    }
}
