using System;

namespace MiniRpg;

/// <summary>몬스터 행동 한 번의 결과.</summary>
public struct MonsterTurn
{
    public int Skill;        // 0 일반 공격 / 1 기 모으기
    public bool Charged;     // 기를 모은 상태에서 공격했는지 (x1.5)
    public bool Critical;    // 치명타 (x2)
    public bool Guarded;     // 플레이어가 방어 중이었는지 (x0.5)
    public bool Blocked;     // 무적 치트로 막혔는지
    public int Damage;       // 실제로 들어간 데미지
}

/// <summary>전투 상태와 규칙. 입출력과 분리해 두어 단위 테스트가 가능하다.</summary>
public class Battle
{
    public const int PlayerMaxHp  = 100;
    public const int MonsterMaxHp = 200;
    public const int CritPercent  = 20;    // 치명타 확률: 명세상 "자유"

    // 플레이어 스킬. 0, 1은 명세 / 2, 3은 "직접 지정"이라 회복과 방어로 정했다.
    public const int Attack = 0, Slash = 1, Heal = 2, Guard = 3;
    public static readonly string[] PlayerSkillNames = { "일반 공격", "가로 베기", "회복", "방어" };
    public static readonly int[]    PlayerSkillPower = { 10, 50, 30, 0 };   // 회복은 회복량

    // 몬스터 스킬
    public const int MonsterAttack = 0, MonsterCharge = 1;
    public static readonly string[] MonsterSkillNames = { "일반 공격", "기 모으기" };
    public static readonly int[]    MonsterSkillPower = { 10, 0 };

    public int PlayerHp { get; private set; } = PlayerMaxHp;
    public int MonsterHp { get; private set; } = MonsterMaxHp;
    public bool IsCharged { get; private set; }     // 몬스터가 기를 모아 둔 상태
    public bool IsGuarding { get; private set; }    // 이번 턴 플레이어가 방어 중
    public bool Invincible { get; set; }            // 치트 1: 무적

    public bool IsPlayerDead => PlayerHp <= 0;
    public bool IsMonsterDead => MonsterHp <= 0;

    public static bool IsValidPlayerSkill(int skill) => skill >= 0 && skill < PlayerSkillNames.Length;

    /// <summary>플레이어 스킬 사용. 공격이면 준 데미지, 회복이면 실제 회복량을 반환한다.</summary>
    public int UsePlayerSkill(int skill)
    {
        IsGuarding = skill == Guard;

        switch (skill)
        {
            case Attack:
            case Slash:
                int damage = Math.Min(PlayerSkillPower[skill], MonsterHp);
                MonsterHp -= damage;
                return damage;

            case Heal:
                int healed = Math.Min(PlayerSkillPower[Heal], PlayerMaxHp - PlayerHp);
                PlayerHp += healed;
                return healed;

            default:   // Guard
                return 0;
        }
    }

    /// <summary>
    /// 몬스터의 다음 스킬을 고른다. 기를 모은 다음 턴에는 기 모으기를 쓸 수 없어 일반 공격만 남는다.
    /// 그 외에는 두 스킬 중 무작위.
    /// </summary>
    public int ChooseMonsterSkill(Random rng)
    {
        if (IsCharged) return MonsterAttack;
        return rng.Next(MonsterSkillNames.Length);
    }

    public static bool RollCritical(Random rng) => rng.Next(100) < CritPercent;

    /// <summary>
    /// 몬스터 공격 데미지. 기 모으기(x1.5)와 치명타(x2)가 겹치면 곱한다 (명세에 겹칠 때 규칙이 없다).
    /// 방어 중이면 절반. 소수점은 버린다.
    /// </summary>
    public static int MonsterAttackDamage(bool charged, bool critical, bool guarded)
    {
        int damage = MonsterSkillPower[MonsterAttack];
        if (charged)  damage = damage * 3 / 2;
        if (critical) damage *= 2;
        if (guarded)  damage /= 2;
        return damage;
    }

    /// <summary>몬스터 스킬 사용. critical은 일반 공격일 때만 의미가 있다.</summary>
    public MonsterTurn UseMonsterSkill(int skill, bool critical)
    {
        MonsterTurn turn = new MonsterTurn { Skill = skill };

        switch (skill)
        {
            case MonsterCharge:
                if (IsCharged) throw new InvalidOperationException("기 모으기는 연속으로 사용할 수 없다.");
                IsCharged = true;
                break;

            default:   // MonsterAttack
                turn.Charged  = IsCharged;
                turn.Critical = critical;
                turn.Guarded  = IsGuarding;
                IsCharged = false;

                if (Invincible)
                {
                    turn.Blocked = true;
                    break;
                }
                turn.Damage = Math.Min(MonsterAttackDamage(turn.Charged, turn.Critical, turn.Guarded), PlayerHp);
                PlayerHp -= turn.Damage;
                break;
        }

        IsGuarding = false;
        return turn;
    }

    // 치트 2: 즉사
    public void KillMonster() => MonsterHp = 0;
    public void KillPlayer() => PlayerHp = 0;
}
