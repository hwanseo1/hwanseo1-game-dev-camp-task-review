using System.Collections.Generic;

// 선택형 1단계: 값 타입 스탯
public struct BaseStats
{
    public int hp;
    public int atk;
}

// 선택형 1단계: 참조 타입 스킬
public class SkillSet
{
    public string skillName;
    public float multiplier;

    public SkillSet(string skillName, float multiplier)
    {
        this.skillName = skillName;
        this.multiplier = multiplier;
    }

    public SkillSet Clone()
    {
        return new SkillSet(skillName, multiplier);
    }
}

public class MonsterTemplate
{
    // 요구 사항 멤버는 stats · skills 두 개지만, 테스트 시나리오가 "이름"을 쓰므로 추가했다.
    public string name;
    public BaseStats stats;
    public List<SkillSet> skills = new List<SkillSet>();

    // Deep Copy: 원본과 Heap을 하나도 공유하지 않는 새 몬스터
    public MonsterTemplate Clone()
    {
        MonsterTemplate clone = new MonsterTemplate();
        clone.name = name;

        // 구조체는 대입만으로 값이 복사된다.
        clone.stats = stats;

        // List를 그대로 대입하면 원본과 같은 List를 가리킨다 (= 보고된 버그).
        // 새 List를 만들고, 안의 SkillSet도 하나씩 새로 만들어 담는다.
        clone.skills = new List<SkillSet>();
        foreach (SkillSet skill in skills)
        {
            clone.skills.Add(skill.Clone());
        }

        return clone;
    }
}
