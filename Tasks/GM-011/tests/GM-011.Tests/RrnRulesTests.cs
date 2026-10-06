using RrnGenerator;

namespace GM_011.Tests;

public class RrnRulesTests
{
    // ── 1. 생년 ──

    [Theory]
    [InlineData("1975", RrnRules.YearKind.FourDigit, 1975)]   // 명세 예시
    [InlineData("2001", RrnRules.YearKind.FourDigit, 2001)]   // 명세 예시
    [InlineData("05", RrnRules.YearKind.TwoDigit, 5)]
    [InlineData("75", RrnRules.YearKind.TwoDigit, 75)]
    [InlineData("00", RrnRules.YearKind.TwoDigit, 0)]
    public void 생년은_4자리와_2자리를_받는다(string input, RrnRules.YearKind kind, int value)
    {
        Assert.Equal(kind, RrnRules.ParseYear(input, out int actual));
        Assert.Equal(value, actual);
    }

    [Theory]
    [InlineData("5")]
    [InlineData("197")]
    [InlineData("19755")]
    [InlineData("19a5")]
    [InlineData("")]
    [InlineData("-5")]
    public void 생년은_2자리_4자리_숫자가_아니면_거부한다(string input)
    {
        Assert.Equal(RrnRules.YearKind.Invalid, RrnRules.ParseYear(input, out _));
    }

    [Theory]
    [InlineData(1, 2001)]    // 명세 예시
    [InlineData(5, 2005)]    // 명세 예시
    [InlineData(75, 1975)]   // 명세 예시
    [InlineData(0, 2000)]    // 경계: 00 ~ 24 → 2000년대
    [InlineData(24, 2024)]
    [InlineData(25, 1925)]   // 경계: 25 ~ 99 → 1900년대
    [InlineData(99, 1999)]
    public void 두자리_연도는_기준에_따라_자동_변환된다(int twoDigit, int expected)
    {
        Assert.Equal(expected, RrnRules.AutoCentury(twoDigit));
    }

    [Fact]
    public void 명세결함_기준연도가_24로_고정이라_2025년생은_1925년으로_바뀐다()
    {
        // 과제 시점(2026년)에 25, 26을 입력하면 자동 변환은 100년 전으로 간다.
        Assert.Equal(1925, RrnRules.AutoCentury(25));
        Assert.Equal(1926, RrnRules.AutoCentury(26));
    }

    // ── 2. 월 · 일 ──

    [Theory]
    [InlineData("06", 6)]   // 명세 예시
    [InlineData("6", 6)]
    [InlineData("12", 12)]
    [InlineData("0", 0)]    // 형식은 통과, 범위 검사에서 걸러짐
    public void 월일은_1자리와_2자리를_받는다(string input, int expected)
    {
        Assert.True(RrnRules.TryParseOneOrTwoDigits(input, out int value));
        Assert.Equal(expected, value);
    }

    [Theory]
    [InlineData("006")]
    [InlineData("")]
    [InlineData("a")]
    [InlineData("-1")]
    public void 월일은_3자리_이상이나_숫자가_아니면_거부한다(string input)
    {
        Assert.False(RrnRules.TryParseOneOrTwoDigits(input, out _));
    }

    [Theory]
    [InlineData(1, true)]
    [InlineData(12, true)]
    [InlineData(0, false)]
    [InlineData(13, false)]
    public void 월은_1부터_12까지만_허용한다(int month, bool expected)
    {
        Assert.Equal(expected, RrnRules.IsValidMonth(month));
    }

    [Theory]
    [InlineData(1, 31)]
    [InlineData(3, 31)]
    [InlineData(5, 31)]
    [InlineData(7, 31)]
    [InlineData(8, 31)]
    [InlineData(10, 31)]
    [InlineData(12, 31)]
    [InlineData(4, 30)]
    [InlineData(6, 30)]
    [InlineData(9, 30)]
    [InlineData(11, 30)]
    [InlineData(2, 28)]
    public void 월마다_최대_일수가_다르다(int month, int expected)
    {
        Assert.Equal(expected, RrnRules.DaysInMonth(2001, month));
    }

    [Theory]
    [InlineData(2004, true)]
    [InlineData(2000, true)]    // 400으로 나누어떨어짐
    [InlineData(1900, false)]   // 100으로 나누어떨어지지만 400은 아님
    [InlineData(2001, false)]
    public void 윤년을_판정한다(int year, bool expected)
    {
        Assert.Equal(expected, RrnRules.IsLeapYear(year));
    }

    [Theory]
    [InlineData(2004, 2, 29, true)]
    [InlineData(2001, 2, 29, false)]
    [InlineData(2001, 2, 28, true)]
    [InlineData(2001, 4, 31, false)]
    [InlineData(2001, 4, 30, true)]
    [InlineData(2001, 1, 31, true)]
    [InlineData(2001, 1, 32, false)]
    [InlineData(2001, 1, 0, false)]
    public void 일은_월과_윤년에_맞게_검사한다(int year, int month, int day, bool expected)
    {
        Assert.Equal(expected, RrnRules.IsValidDay(year, month, day));
    }

    [Fact]
    public void 명세결함_2자리_00은_1900과_2000의_2월_29일_허용이_다르다()
    {
        // 명세에 윤년 언급이 없다. 00을 어느 세기로 고르느냐에 따라 같은 "00년 2월 29일"이 되기도 하고 안 되기도 한다.
        Assert.False(RrnRules.IsValidDay(1900, 2, 29));
        Assert.True(RrnRules.IsValidDay(2000, 2, 29));
    }

    // ── 3. 성별 ──

    [Theory]
    [InlineData("1", 1)]
    [InlineData("2", 2)]
    [InlineData("3", 3)]
    [InlineData("4", 4)]
    [InlineData(" 3 ", 3)]
    public void 성별은_1부터_4까지_입력한_숫자를_쓴다(string input, int expected)
    {
        Assert.Equal(expected, RrnRules.ResolveGenderDigit(input, new Random(0)));
    }

    [Theory]
    [InlineData("0")]
    [InlineData("5")]
    [InlineData("9")]
    [InlineData("12")]
    [InlineData("a")]
    [InlineData("")]
    public void 성별에_다른_값을_넣으면_1부터_4_중_무작위다(string input)
    {
        bool[] seen = new bool[5];
        Random random = new Random(1);
        for (int i = 0; i < 200; i++)
        {
            int g = RrnRules.ResolveGenderDigit(input, random);
            Assert.InRange(g, 1, 4);
            seen[g] = true;
        }
        Assert.True(seen[1] && seen[2] && seen[3] && seen[4]);
    }

    [Fact]
    public void 명세결함_1975년생에_성별_3을_넣어도_막을_규칙이_없다()
    {
        // 실제 주민번호는 1900년대 1/2, 2000년대 3/4. 명세는 "남자 1 / 3"만 있어 생년과 맞는지 검사할 근거가 없다.
        ResidentNumber rrn = new ResidentNumber { Year = 1975, Month = 6, Day = 6, GenderDigit = 3, Tail = "123456" };
        Assert.Equal("750606-3123456", rrn.ToString());
        Assert.True(RrnRules.IsValidBackPart("3123456"));
    }

    // ── 4. 뒤 6자리 ──

    [Fact]
    public void 뒤_6자리는_0이_없고_같은_숫자가_연속되지_않는다()
    {
        Random random = new Random(42);
        for (int i = 0; i < 10000; i++)
        {
            int gender = random.Next(1, 5);
            string tail = RrnRules.GenerateTail(gender, random);
            Assert.Equal(6, tail.Length);
            Assert.True(RrnRules.IsValidBackPart(gender + tail), gender + tail);
        }
    }

    [Fact]
    public void 뒤_6자리는_1부터_9까지_모든_숫자가_나올_수_있다()
    {
        bool[] seen = new bool[10];
        Random random = new Random(7);
        for (int i = 0; i < 1000; i++)
            foreach (char c in RrnRules.GenerateTail(1, random))
                seen[c - '0'] = true;
        Assert.False(seen[0]);
        for (int d = 1; d <= 9; d++)
            Assert.True(seen[d], d + "가 나오지 않음");
    }

    [Fact]
    public void 뒤_6자리_첫_숫자는_성별_숫자와_겹치지_않는다()
    {
        Random random = new Random(3);
        for (int gender = 1; gender <= 4; gender++)
            for (int i = 0; i < 500; i++)
                Assert.NotEqual((char)('0' + gender), RrnRules.GenerateTail(gender, random)[0]);
    }

    [Theory]
    [InlineData("1234567", true)]    // 명세 예시 뒷자리
    [InlineData("1212121", true)]
    [InlineData("1023456", false)]   // 0
    [InlineData("1123456", false)]   // 성별 숫자와 첫 자리 연속
    [InlineData("1234556", false)]   // 연속
    [InlineData("5234567", false)]   // 성별 숫자 범위 밖
    [InlineData("123456", false)]    // 자릿수
    public void 뒷자리_규칙을_검사한다(string back, bool expected)
    {
        Assert.Equal(expected, RrnRules.IsValidBackPart(back));
    }

    // ── 출력 형식 ──

    [Theory]
    [InlineData(1975, 6, 6, 1, "234567", "750606-1234567")]
    [InlineData(2005, 12, 31, 4, "131313", "051231-4131313")]
    [InlineData(2000, 1, 1, 3, "121212", "000101-3121212")]   // 앞자리 0은 생년월일이라 허용
    [InlineData(1905, 2, 9, 2, "989898", "050209-2989898")]
    public void 앞6자리_하이픈_뒤7자리로_출력한다(int y, int m, int d, int g, string tail, string expected)
    {
        ResidentNumber rrn = new ResidentNumber { Year = y, Month = m, Day = d, GenderDigit = g, Tail = tail };
        Assert.Equal(expected, rrn.ToString());
    }

    [Fact]
    public void 명세결함_출력_예시_123456은_34월_56일이라_월일_규칙에_어긋난다()
    {
        // 예시 "123456-1234567"의 앞자리를 생년월일로 읽으면 12년 34월 56일.
        Assert.False(RrnRules.IsValidMonth(34));
    }
}
