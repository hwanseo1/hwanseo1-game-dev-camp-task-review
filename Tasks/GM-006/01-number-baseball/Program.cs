using System;
using System.Text;

namespace NumberBaseball;

class Program
{
    static Random rng = new Random();

    static void Main(string[] args)
    {
        Console.OutputEncoding = Encoding.UTF8;

        // 테스트 재현용: 인자로 시드를 넘기면 같은 결과가 나온다.
        int seed;
        if (args.Length > 0 && int.TryParse(args[0], out seed)) rng = new Random(seed);

        Console.WriteLine("=== 숫자 야구 ===");
        Console.WriteLine("0~9 중 서로 다른 숫자 " + Baseball.Length + "개를 숫자와 자리까지 맞추세요.");
        Console.WriteLine("S = 숫자·자리 일치 | B = 숫자만 일치 | O = 없는 숫자");
        Console.WriteLine();

        int game = 1;
        while (true)
        {
            Console.WriteLine("--- " + game + "번째 게임 ---");
            if (!PlayOne()) break;

            // 원문은 "결과를 보여주며 프로그램 종료" 직후 "다시 시작할지 선택"을 요구한다.
            // 종료하면 선택할 수 없으므로, 게임을 끝낸 뒤 재시작 여부를 묻는 것으로 구현했다.
            if (!AskRestart()) break;
            game++;
            Console.WriteLine();
        }

        Console.WriteLine("프로그램을 종료합니다.");
    }

    // 한 판 진행. 3S로 끝나면 true, 중단하면 false.
    static bool PlayOne()
    {
        int[] answer = Baseball.Generate(rng);
        int tries = 0;

        while (true)
        {
            Console.Write("숫자 " + Baseball.Length + "개 (예: 1 2 3) | 치트(" + Baseball.CheatKey + ") | 중단(q): ");
            string s = Console.ReadLine();
            if (s == null) return false;
            s = s.Trim().ToLower();

            if (s == "q") return false;

            if (s == Baseball.CheatKey)
            {
                answer = (int[])Baseball.CheatAnswer.Clone();
                Console.WriteLine("[치트] 컴퓨터가 갖고 있는 숫자 : " + Baseball.Format(answer));
                continue;
            }

            int[] guess;
            string error;
            if (!Baseball.TryParseGuess(s, out guess, out error))
            {
                Console.WriteLine(error);
                continue;     // 무효 입력은 시도 횟수에 넣지 않는다
            }

            tries++;
            Score score = Baseball.Judge(answer, guess);
            Console.WriteLine("[" + tries + "회] " + Baseball.Format(guess) + " → " + score);

            if (score.IsWin)
            {
                Console.WriteLine();
                Console.WriteLine("3S! 정답 " + Baseball.Format(answer) + " — " + tries + "번 만에 맞췄습니다.");
                return true;
            }
        }
    }

    static bool AskRestart()
    {
        while (true)
        {
            Console.Write("다시 하시겠습니까? 다시(y) / 종료(n): ");
            string s = Console.ReadLine();
            if (s == null) return false;

            switch (s.Trim().ToLower())
            {
                case "y": return true;
                case "n": return false;
                default:
                    Console.WriteLine("y 또는 n을 입력해 주세요.");
                    break;
            }
        }
    }
}
