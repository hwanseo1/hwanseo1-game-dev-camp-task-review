using System;
using System.Text;

namespace MiniRpg;

class Program
{
    static Random rng = new Random();

    static void Main(string[] args)
    {
        Console.OutputEncoding = Encoding.UTF8;

        // 테스트 재현용: 인자로 시드를 넘기면 같은 결과가 나온다.
        int seed;
        if (args.Length > 0 && int.TryParse(args[0], out seed)) rng = new Random(seed);

        Battle battle = new Battle();

        Console.WriteLine("=== 턴제 미니 RPG ===");
        Console.WriteLine("플레이어 HP " + Battle.PlayerMaxHp + " vs 몬스터 HP " + Battle.MonsterMaxHp);
        Console.WriteLine("몬스터 치명타 확률 " + Battle.CritPercent + "% (x2) | 기 모은 다음 공격 x1.5");
        Console.WriteLine();

        int turn = 1;
        while (true)
        {
            PrintStatus(turn, battle);

            // ── 플레이어 턴 ──
            int skill = ReadAction(battle);
            if (skill == -1) { Console.WriteLine("게임을 중단합니다."); return; }

            if (skill >= 0)
            {
                int amount = battle.UsePlayerSkill(skill);
                switch (skill)
                {
                    case Battle.Heal:
                        Console.WriteLine("플레이어의 회복! HP +" + amount);
                        break;

                    case Battle.Guard:
                        Console.WriteLine("플레이어가 방어 자세를 취했다. (이번 턴 받는 데미지 절반)");
                        break;

                    default:
                        Console.WriteLine("플레이어의 " + Battle.PlayerSkillNames[skill] + "! 몬스터에게 " + amount + " 데미지");
                        break;
                }
            }

            if (CheckEnd(battle)) return;

            // ── 몬스터 턴 ──
            int mSkill = battle.ChooseMonsterSkill(rng);
            bool crit = mSkill == Battle.MonsterAttack && Battle.RollCritical(rng);
            MonsterTurn m = battle.UseMonsterSkill(mSkill, crit);
            PrintMonsterTurn(m);

            if (CheckEnd(battle)) return;

            Console.WriteLine();
            turn++;
        }
    }

    static void PrintStatus(int turn, Battle b)
    {
        string state = "";
        if (b.IsCharged) state += " [몬스터 기 모음]";
        if (b.Invincible) state += " [무적]";
        Console.WriteLine("--- " + turn + "턴 | 플레이어 HP " + b.PlayerHp + "/" + Battle.PlayerMaxHp
                          + " | 몬스터 HP " + b.MonsterHp + "/" + Battle.MonsterMaxHp + " ---" + state);
    }

    static void PrintMonsterTurn(MonsterTurn m)
    {
        if (m.Skill == Battle.MonsterCharge)
        {
            Console.WriteLine("몬스터가 기를 모은다... (다음 공격 x1.5)");
            return;
        }

        string tags = "";
        if (m.Charged)  tags += " [기 모음 x1.5]";
        if (m.Critical) tags += " [치명타 x2]";
        if (m.Guarded)  tags += " [방어 x0.5]";

        if (m.Blocked)
            Console.WriteLine("몬스터의 일반 공격!" + tags + " → 무적 상태라 데미지 없음");
        else
            Console.WriteLine("몬스터의 일반 공격!" + tags + " → 플레이어에게 " + m.Damage + " 데미지");
    }

    // 끝났으면 문구를 출력하고 true.
    static bool CheckEnd(Battle b)
    {
        if (b.IsPlayerDead)
        {
            Console.WriteLine();
            Console.WriteLine("플레이어가 쓰러졌습니다... YOU DIED");
            return true;
        }
        if (b.IsMonsterDead)
        {
            // 몬스터 사망 시 처리는 명세에 없다. 즉사 치트가 성립하려면 필요해서 승리 문구 후 종료로 정했다.
            Console.WriteLine();
            Console.WriteLine("몬스터를 쓰러뜨렸습니다! 승리");
            return true;
        }
        return false;
    }

    // 스킬 번호(0~3)를 반환. 즉사 치트를 쓰면 -2(바로 끝 판정), 중단은 -1.
    static int ReadAction(Battle b)
    {
        while (true)
        {
            Console.Write("스킬 0:일반 공격(10) 1:가로 베기(50) 2:회복(+30) 3:방어 | 치트(c) | 중단(q): ");
            string s = Console.ReadLine();
            if (s == null) return -1;
            s = s.Trim().ToLower();

            if (s == "q") return -1;

            if (s == "c")
            {
                if (CheatMenu(b)) return -2;   // 즉사 → 바로 끝 판정
                continue;                      // 무적 전환·취소 → 턴 소모 없이 다시 입력
            }

            int skill;
            if (int.TryParse(s, out skill) && Battle.IsValidPlayerSkill(skill)) return skill;

            Console.WriteLine("0~3 중에서 입력해 주세요.");
        }
    }

    // 즉사를 썼으면 true.
    static bool CheatMenu(Battle b)
    {
        Console.Write("[치트] 1. 무적(" + (b.Invincible ? "ON" : "OFF") + ") / 2. 즉사 / 그 외: 취소 > ");
        string s = Console.ReadLine();
        if (s == null) return false;

        switch (s.Trim())
        {
            case "1":
                b.Invincible = !b.Invincible;
                Console.WriteLine("[치트] 무적 " + (b.Invincible ? "ON" : "OFF"));
                return false;

            case "2":
                Console.Write("[치트] 즉사 대상 1. 몬스터 / 2. 플레이어 / 그 외: 취소 > ");
                string t = Console.ReadLine();
                switch (t == null ? "" : t.Trim())
                {
                    case "1":
                        b.KillMonster();
                        Console.WriteLine("[치트] 몬스터 즉사");
                        return true;
                    case "2":
                        b.KillPlayer();
                        Console.WriteLine("[치트] 플레이어 즉사");
                        return true;
                    default:
                        Console.WriteLine("[치트] 취소");
                        return false;
                }

            default:
                Console.WriteLine("[치트] 취소");
                return false;
        }
    }
}
