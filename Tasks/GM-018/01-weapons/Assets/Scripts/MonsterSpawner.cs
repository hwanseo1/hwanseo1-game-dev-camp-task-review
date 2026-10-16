using UnityEngine;

public class MonsterSpawner : MonoBehaviour
{
    void Update()
    {
        // 원문 "[Keycode.] 키"에 키 이름이 빠져 있어 R로 정함 (Q/W/E 다음 키)
        if (Input.GetKeyDown(KeyCode.R))
        {
            RunCloneTest();
        }
    }

    // 선택형 2단계 테스트 시나리오
    public void RunCloneTest()
    {
        // 1. 원본 템플릿. ATK는 시나리오에 값이 없어 넣지 않았다 (기본값 0).
        MonsterTemplate template = new MonsterTemplate();
        template.name = "드래곤";
        template.stats.hp = 1000;
        template.skills.Add(new SkillSet("화염방사", 1.5f));

        // 2. Clone
        MonsterTemplate instance1 = template.Clone();
        instance1.name = "드래곤_인스턴스1";

        // 3. 구조체 수정
        instance1.stats.hp = 500;

        // 4. 참조 타입 내부 수정
        instance1.skills[0].multiplier = 9.9f;

        Log(template);
        Log(instance1);

        bool hpKept = template.stats.hp == 1000;
        bool skillKept = Mathf.Approximately(template.skills[0].multiplier, 1.5f);
        Debug.Log($"[결과 확인] 원본 HP 1000 유지: {(hpKept ? "PASS" : "FAIL")} / 원본 스킬 배율 1.5배 유지: {(skillKept ? "PASS" : "FAIL")}");
    }

    private void Log(MonsterTemplate monster)
    {
        SkillSet skill = monster.skills[0];
        Debug.Log($"[{monster.name}] HP {monster.stats.hp}, ATK {monster.stats.atk}, 스킬 {skill.skillName} {skill.multiplier}배");
    }
}
