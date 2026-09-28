using System;
using System.Text;

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

            int days = GetDays(month);
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

    // 월별 일수. 1~12 이외는 0을 반환해 호출부에서 예외처리.
    // if-else 체인보다 switch의 case 묶음이 중복 없이 짧아 선택.
    static int GetDays(int month)
    {
        switch (month)
        {
            case 1:
            case 3:
            case 5:
            case 7:
            case 8:
            case 10:
            case 12:
                return 31;

            case 4:
            case 6:
            case 9:
            case 11:
                return 30;

            case 2:
                return 28;      // 과제 조건: 윤년 무시

            default:
                return 0;
        }
    }
}
