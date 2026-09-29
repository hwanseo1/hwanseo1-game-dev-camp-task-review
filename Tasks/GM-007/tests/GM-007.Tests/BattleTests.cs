using MiniRpg;

namespace GM_007.Tests;

public class BattleTests
{
    // ── 초기값 · 스킬 배열 ──

    [Fact]
    public void 시작_체력은_플레이어_100_몬스터_200이다()
    {
        Battle b = new Battle();
        Assert.Equal(100, b.PlayerHp);
        Assert.Equal(200, b.MonsterHp);
    }

    [Fact]
    public void 플레이어_스킬은_4개_몬스터_스킬은_2개다()
    {
        Assert.Equal(4, Battle.PlayerSkillNames.Length);
        Assert.Equal(4, Battle.PlayerSkillPower.Length);
        Assert.Equal(2, Battle.MonsterSkillNames.Length);
        Assert.Equal("일반 공격", Battle.PlayerSkillNames[0]);
        Assert.Equal("가로 베기", Battle.PlayerSkillNames[1]);
        Assert.Equal("기 모으기", Battle.MonsterSkillNames[1]);
    }

    [Theory]
    [InlineData(0, true)]
    [InlineData(3, true)]
    [InlineData(-1, false)]
    [InlineData(4, false)]
    public void 스킬_번호는_0에서_3까지만_유효하다(int skill, bool expected)
    {
        Assert.Equal(expected, Battle.IsValidPlayerSkill(skill));
    }

    // ── 플레이어 스킬 ──

    [Theory]
    [InlineData(Battle.Attack, 10)]
    [InlineData(Battle.Slash, 50)]
    public void 공격_스킬은_정해진_데미지를_준다(int skill, int damage)
    {
        Battle b = new Battle();
        Assert.Equal(damage, b.UsePlayerSkill(skill));
        Assert.Equal(200 - damage, b.MonsterHp);
    }

    [Fact]
    public void 몬스터_체력은_0_아래로_내려가지_않는다()
    {
        Battle b = new Battle();
        for (int i = 0; i < 5; i++) b.UsePlayerSkill(Battle.Slash);   // 250 데미지 분량

        Assert.Equal(0, b.MonsterHp);
        Assert.True(b.IsMonsterDead);
    }

    [Fact]
    public void 회복은_최대_체력을_넘지_않는다()
    {
        Battle b = new Battle();
        Assert.Equal(0, b.UsePlayerSkill(Battle.Heal));   // 가득 찬 상태
        Assert.Equal(100, b.PlayerHp);

        b.UseMonsterSkill(Battle.MonsterAttack, critical: true);   // -20 → 80
        Assert.Equal(20, b.UsePlayerSkill(Battle.Heal));            // 30이 아니라 20만
        Assert.Equal(100, b.PlayerHp);
    }

    [Fact]
    public void 방어는_그_턴의_몬스터_공격을_절반으로_줄인다()
    {
        Battle b = new Battle();
        b.UsePlayerSkill(Battle.Guard);
        MonsterTurn m = b.UseMonsterSkill(Battle.MonsterAttack, critical: false);

        Assert.True(m.Guarded);
        Assert.Equal(5, m.Damage);
    }

    [Fact]
    public void 방어는_다음_턴까지_이어지지_않는다()
    {
        Battle b = new Battle();
        b.UsePlayerSkill(Battle.Guard);
        b.UseMonsterSkill(Battle.MonsterAttack, false);

        b.UsePlayerSkill(Battle.Attack);
        MonsterTurn m = b.UseMonsterSkill(Battle.MonsterAttack, false);

        Assert.False(m.Guarded);
        Assert.Equal(10, m.Damage);
    }

    // ── 몬스터 데미지 계산 ──

    [Theory]
    [InlineData(false, false, false, 10)]   // 일반
    [InlineData(false, true,  false, 20)]   // 치명타 x2
    [InlineData(true,  false, false, 15)]   // 기 모음 x1.5
    [InlineData(true,  true,  false, 30)]   // 기 모음 + 치명타 (곱함)
    [InlineData(false, false, true,  5)]    // 방어
    [InlineData(true,  true,  true,  15)]   // 전부
    public void 몬스터_공격_데미지_배율(bool charged, bool crit, bool guarded, int expected)
    {
        Assert.Equal(expected, Battle.MonsterAttackDamage(charged, crit, guarded));
    }

    // ── 기 모으기 ──

    [Fact]
    public void 기_모으기_턴에는_데미지가_없다()
    {
        Battle b = new Battle();
        MonsterTurn m = b.UseMonsterSkill(Battle.MonsterCharge, critical: false);

        Assert.Equal(0, m.Damage);
        Assert.Equal(100, b.PlayerHp);
        Assert.True(b.IsCharged);
    }

    [Fact]
    public void 기_모은_다음_일반_공격은_15_데미지이고_기가_소모된다()
    {
        Battle b = new Battle();
        b.UseMonsterSkill(Battle.MonsterCharge, false);
        MonsterTurn m = b.UseMonsterSkill(Battle.MonsterAttack, false);

        Assert.True(m.Charged);
        Assert.Equal(15, m.Damage);
        Assert.False(b.IsCharged);

        MonsterTurn next = b.UseMonsterSkill(Battle.MonsterAttack, false);
        Assert.Equal(10, next.Damage);   // 다음 공격은 원래대로
    }

    [Fact]
    public void 기를_모은_다음_턴에는_반드시_일반_공격을_고른다()
    {
        Battle b = new Battle();
        b.UseMonsterSkill(Battle.MonsterCharge, false);

        for (int seed = 0; seed < 200; seed++)
            Assert.Equal(Battle.MonsterAttack, b.ChooseMonsterSkill(new Random(seed)));
    }

    [Fact]
    public void 기_모으기를_연속으로_쓰면_예외다()
    {
        Battle b = new Battle();
        b.UseMonsterSkill(Battle.MonsterCharge, false);
        Assert.Throws<InvalidOperationException>(() => b.UseMonsterSkill(Battle.MonsterCharge, false));
    }

    [Fact]
    public void 기가_없을_때는_두_스킬이_모두_무작위로_나온다()
    {
        Battle b = new Battle();
        bool[] seen = new bool[2];
        for (int seed = 0; seed < 200; seed++) seen[b.ChooseMonsterSkill(new Random(seed))] = true;

        Assert.True(seen[Battle.MonsterAttack]);
        Assert.True(seen[Battle.MonsterCharge]);
    }

    [Fact]
    public void 치명타_확률은_대략_20퍼센트다()
    {
        Random rng = new Random(0);
        int crits = 0;
        for (int i = 0; i < 10000; i++) if (Battle.RollCritical(rng)) crits++;

        Assert.InRange(crits, 1800, 2200);
    }

    // ── 사망 ──

    [Fact]
    public void 플레이어_체력은_0_아래로_내려가지_않고_사망한다()
    {
        Battle b = new Battle();
        for (int i = 0; i < 6; i++) b.UseMonsterSkill(Battle.MonsterAttack, critical: true);   // 20 x 6 = 120

        Assert.Equal(0, b.PlayerHp);
        Assert.True(b.IsPlayerDead);
    }

    // ── 치트 ──

    [Fact]
    public void 무적이면_데미지를_받지_않는다()
    {
        Battle b = new Battle { Invincible = true };
        b.UseMonsterSkill(Battle.MonsterCharge, false);
        MonsterTurn m = b.UseMonsterSkill(Battle.MonsterAttack, critical: true);   // 원래 30

        Assert.True(m.Blocked);
        Assert.Equal(0, m.Damage);
        Assert.Equal(100, b.PlayerHp);
        Assert.False(b.IsCharged);   // 막혀도 기는 소모된다
    }

    [Fact]
    public void 무적을_끄면_다시_데미지를_받는다()
    {
        Battle b = new Battle { Invincible = true };
        b.UseMonsterSkill(Battle.MonsterAttack, false);
        b.Invincible = false;
        b.UseMonsterSkill(Battle.MonsterAttack, false);

        Assert.Equal(90, b.PlayerHp);
    }

    [Fact]
    public void 즉사_몬스터()
    {
        Battle b = new Battle();
        b.KillMonster();
        Assert.True(b.IsMonsterDead);
        Assert.False(b.IsPlayerDead);
    }

    [Fact]
    public void 즉사_플레이어는_무적이어도_적용된다()
    {
        Battle b = new Battle { Invincible = true };
        b.KillPlayer();
        Assert.True(b.IsPlayerDead);
    }

    // ── 명세 결함 기록 ──

    [Fact]
    public void 명세결함_가로베기가_일반공격보다_항상_낫다()
    {
        // 가로 베기(50)에 쿨타임·비용·명중률 같은 제약이 명세에 없다.
        // 같은 턴을 쓰는데 데미지가 5배라 일반 공격(0번)을 고를 이유가 없다.
        Assert.True(Battle.PlayerSkillPower[Battle.Slash] > Battle.PlayerSkillPower[Battle.Attack]);
        Assert.Equal(4, (Battle.MonsterMaxHp + 49) / Battle.PlayerSkillPower[Battle.Slash]);   // 4턴이면 끝
    }

    [Fact]
    public void 명세결함_가로베기만_쓰면_몬스터가_무엇을_하든_플레이어는_죽지_않는다()
    {
        // 몬스터의 모든 행동(일반/기 모으기) x 치명타 여부 조합을 전부 따라가
        // 플레이어가 가로 베기만 쓸 때 받는 최대 데미지를 구한다.
        // 몬스터는 4턴째 행동 전에 죽으므로 3번만 행동한다.
        // 명세의 "플레이어 체력이 0이 되면 사망 문구와 함께 종료"는 이 전략에서는 일어나지 않는다.
        int worst = WorstDamageWhileSlashing();

        Assert.Equal(60, worst);   // 치명타 20 x 3
        Assert.True(worst < Battle.PlayerMaxHp);
    }

    // 플레이어는 매 턴 가로 베기, 몬스터는 가능한 모든 선택을 해 볼 때 플레이어가 받는 최대 누적 데미지
    static int WorstDamageWhileSlashing()
    {
        return Explore(new List<(int skill, bool crit)>());

        int Explore(List<(int skill, bool crit)> history)
        {
            Battle b = Replay(history);
            if (b.IsMonsterDead || b.IsPlayerDead) return Battle.PlayerMaxHp - b.PlayerHp;

            int worst = Battle.PlayerMaxHp - b.PlayerHp;
            var options = new List<(int, bool)> { (Battle.MonsterAttack, false), (Battle.MonsterAttack, true) };
            if (!b.IsCharged) options.Add((Battle.MonsterCharge, false));

            foreach (var o in options)
            {
                var next = new List<(int, bool)>(history) { o };
                worst = Math.Max(worst, Explore(next));
            }
            return worst;
        }

        static Battle Replay(List<(int skill, bool crit)> history)
        {
            Battle b = new Battle();
            b.UsePlayerSkill(Battle.Slash);
            foreach (var (skill, crit) in history)
            {
                b.UseMonsterSkill(skill, crit);
                if (b.IsPlayerDead) break;
                b.UsePlayerSkill(Battle.Slash);
                if (b.IsMonsterDead) break;
            }
            return b;
        }
    }
}
