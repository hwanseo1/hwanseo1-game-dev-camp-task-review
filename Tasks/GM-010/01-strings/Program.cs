using System;
using System.Text;

namespace Strings;

class Program
{
    static void Main()
    {
        Console.OutputEncoding = Encoding.UTF8;
        Console.InputEncoding = Encoding.UTF8;

        Console.WriteLine("=== 문자열 활용 ===");
        Console.WriteLine("1. 문자 거꾸로 뒤집기");
        Console.WriteLine("2. 짝수 번째 문자만 거꾸로");
        Console.WriteLine("3. 숫자만 출력");
        Console.WriteLine("4. 문자 개수 세기");
        Console.WriteLine("5. 주민번호 '-' 제거");

        while (true)
        {
            Console.Write("번호를 선택하세요 (1 ~ 5): ");
            string line = Console.ReadLine();
            if (line == null) return;

            string text;
            switch (line.Trim())
            {
                case "1":
                    text = Ask("문자열: ");
                    if (text == null) return;
                    Console.WriteLine("결과: " + StringRules.Reverse(text));
                    return;
                case "2":
                    text = Ask("문자열: ");
                    if (text == null) return;
                    Console.WriteLine("결과: " + StringRules.ReverseEvenPositions(text));
                    return;
                case "3":
                    text = Ask("문자열: ");
                    if (text == null) return;
                    Console.WriteLine("결과: " + StringRules.DigitsOnly(text));
                    return;
                case "4":
                    text = Ask("문자열 : ");
                    if (text == null) return;
                    char target;
                    if (!AskChar(out target)) return;
                    Console.WriteLine("결과 : " + StringRules.CountChar(text, target));
                    return;
                case "5":
                    text = Ask("주민번호: ");
                    if (text == null) return;
                    Console.WriteLine("결과: " + StringRules.RemoveHyphen(text));
                    return;
                default:
                    Console.WriteLine("1 ~ 5 중에서 입력해 주세요.");
                    break;
            }
        }
    }

    // 입력은 있는 그대로 쓴다 (앞뒤 공백도 문자로 본다).
    static string Ask(string prompt)
    {
        Console.Write(prompt);
        return Console.ReadLine();
    }

    // "문자 하나"이므로 길이가 1이 아니면 다시 받는다.
    static bool AskChar(out char target)
    {
        target = '\0';
        while (true)
        {
            string line = Ask("문자 : ");
            if (line == null) return false;
            if (line.Length == 1)
            {
                target = line[0];
                return true;
            }
            Console.WriteLine("문자 하나만 입력해 주세요.");
        }
    }
}
