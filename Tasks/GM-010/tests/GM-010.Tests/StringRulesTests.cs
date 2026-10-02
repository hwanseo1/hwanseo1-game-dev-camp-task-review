using Strings;

namespace GM_010.Tests;

public class StringRulesTests
{
    // ── 1. 거꾸로 ──

    [Theory]
    [InlineData("ABCD", "DCBA")]          // 명세 예시
    [InlineData("A", "A")]
    [InlineData("", "")]
    [InlineData("안녕하세요", "요세하녕안")]
    [InlineData("A B", "B A")]
    public void 문자열을_거꾸로_출력한다(string text, string expected)
    {
        Assert.Equal(expected, StringRules.Reverse(text));
    }

    [Fact]
    public void 명세결함_이모지는_char_단위로_뒤집으면_깨진다()
    {
        // 😀는 char 2개(서로게이트 쌍). 반복문으로 char를 뒤집으면 쌍의 순서가 바뀌어 다른 문자가 아니게 된다.
        string reversed = StringRules.Reverse("A😀");
        Assert.NotEqual("😀A", reversed);
        Assert.Equal(3, reversed.Length);
    }

    // ── 2. 짝수 번째만 거꾸로 ──

    [Theory]
    [InlineData("ASDF", "AFDS")]          // 명세 예시: 2·4번째(S, F)만 뒤집힘
    [InlineData("ABCDE", "ADCBE")]        // 홀수 길이: 2·4번째(B, D)
    [InlineData("ABCDEF", "AFCDEB")]      // 2·4·6번째(B, D, F) → F, D, B
    [InlineData("AB", "AB")]              // 짝수 번째가 하나뿐
    [InlineData("A", "A")]
    [InlineData("", "")]
    public void 짝수_번째_문자만_거꾸로_한다(string text, string expected)
    {
        Assert.Equal(expected, StringRules.ReverseEvenPositions(text));
    }

    [Fact]
    public void 명세결함_짝수를_0부터_세면_예시와_다르다()
    {
        // 인덱스 0·2(A, D)를 바꾸면 DSAF가 된다. 예시 AFDS는 1부터 센 경우에만 맞는다.
        Assert.NotEqual("DSAF", StringRules.ReverseEvenPositions("ASDF"));
    }

    // ── 3. 숫자만 ──

    [Theory]
    [InlineData("A1B2C3", "123")]         // 명세 예시
    [InlineData("ABC", "")]
    [InlineData("2026-10-02", "20261002")]
    [InlineData("１２3", "3")]            // 전각 숫자는 0~9가 아님
    public void 숫자만_출력한다(string text, string expected)
    {
        Assert.Equal(expected, StringRules.DigitsOnly(text));
    }

    // ── 4. 문자 개수 ──

    [Theory]
    [InlineData("ABCDAA", 'A', 3)]        // 명세 예시
    [InlineData("ABCDAA", 'a', 0)]        // 대소문자 구분
    [InlineData("ABCDAA", 'Z', 0)]
    [InlineData("A B C", ' ', 2)]         // 공백도 문자
    [InlineData("", 'A', 0)]
    public void 문자열_안의_문자_개수를_센다(string text, char target, int expected)
    {
        Assert.Equal(expected, StringRules.CountChar(text, target));
    }

    // ── 5. 주민번호 ──

    [Theory]
    [InlineData("900101-1234567", "9001011234567")]
    [InlineData("9001011234567", "9001011234567")]
    [InlineData("--12-3", "123")]
    public void 하이픈을_제거한다(string text, string expected)
    {
        Assert.Equal(expected, StringRules.RemoveHyphen(text));
    }

    [Fact]
    public void 명세결함_예시는_하이픈만이_아니라_공백도_지운다()
    {
        // 명세 예시 그대로. '-'만 지우면 "1234567  12345678"(공백 2칸)이 남는다.
        Assert.Equal("123456712345678", StringRules.RemoveHyphen("1234567 - 12345678"));
        Assert.NotEqual("1234567  12345678", StringRules.RemoveHyphen("1234567 - 12345678"));
    }

    [Fact]
    public void 명세결함_예시_주민번호는_실제_자릿수와_다르다()
    {
        // 실제 주민번호는 6자리-7자리 = 13자리. 예시는 7자리-8자리 = 15자리다.
        Assert.Equal(15, StringRules.RemoveHyphen("1234567 - 12345678").Length);
    }
}
