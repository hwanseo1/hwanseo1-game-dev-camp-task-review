using System.Text;

namespace Strings;

// 문자열 기능 5개. "조건문 + 반복문을 많이 사용"이라 Array.Reverse · LINQ 없이 반복문으로 직접 만든다.
public static class StringRules
{
    // 1. ABCD → DCBA
    public static string Reverse(string text)
    {
        StringBuilder sb = new StringBuilder();
        for (int i = text.Length - 1; i >= 0; i--)
            sb.Append(text[i]);
        return sb.ToString();
    }

    // 2. 짝수 번째(1부터 셈) 문자만 거꾸로. ASDF → AFDS
    //    2·4번째(S, F)를 서로 바꾸고 1·3번째(A, D)는 그대로 둔다.
    public static string ReverseEvenPositions(string text)
    {
        char[] chars = text.ToCharArray();
        int left = 1;                                            // 2번째 (인덱스 1)
        int right = chars.Length % 2 == 0 ? chars.Length - 1     // 길이가 짝수면 마지막 문자가 짝수 번째
                                          : chars.Length - 2;
        while (left < right)
        {
            char tmp = chars[left]; chars[left] = chars[right]; chars[right] = tmp;
            left += 2;
            right -= 2;
        }
        return new string(chars);
    }

    // 3. A1B2C3 → 123. 0~9만 숫자로 본다 (char.IsDigit은 전각 '１' 같은 문자도 숫자로 친다).
    public static string DigitsOnly(string text)
    {
        StringBuilder sb = new StringBuilder();
        foreach (char c in text)
            if (c >= '0' && c <= '9') sb.Append(c);
        return sb.ToString();
    }

    // 4. ABCDAA 안의 A → 3. 대소문자를 구분한다.
    public static int CountChar(string text, char target)
    {
        int count = 0;
        foreach (char c in text)
            if (c == target) count++;
        return count;
    }

    // 5. 1234567 - 12345678 → 123456712345678
    //    명세 문장은 "-만 제거"인데 예시 결과에서는 '-' 앞뒤 공백도 사라져 있다. 예시에 맞춰 공백도 뺀다.
    public static string RemoveHyphen(string text)
    {
        StringBuilder sb = new StringBuilder();
        foreach (char c in text)
        {
            if (c == '-' || c == ' ') continue;
            sb.Append(c);
        }
        return sb.ToString();
    }
}
