using System;
using System.Text;

namespace DaysInMonth;

class Program
{
    // 과제 조건: 유효한 입력 3번이면 종료
    const int MaxCount = 3;

    static void Main()
    {
        Console.OutputEncoding = Encoding.UTF8;

        int count = 0;
        while (count < MaxCount)
        {
            Console.Write("월을 입력하세요 (1~12) [" + (count + 1) + "/" + MaxCount + "]: ");
            string input = Console.ReadLine();

            if (input == null) break;   // 입력 스트림 종료

            int month;
            if (!int.TryParse(input, out month))
            {
                Console.WriteLine("숫자만 입력해 주세요. (카운트되지 않습니다)");
                continue;
            }

            int days = Month.GetDays(month);
            if (days == 0)
            {
                Console.WriteLine(month + "월은 존재하지 않습니다. 1~12 사이로 입력해 주세요. (카운트되지 않습니다)");
                continue;
            }

            Console.WriteLine(month + "월은 " + days + "일까지 있습니다.");
            count++;
        }

        Console.WriteLine("입력 " + MaxCount + "번이 끝났습니다. 프로그램을 종료합니다.");
    }
}
