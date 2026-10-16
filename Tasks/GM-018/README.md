# GM-018

Unity 과제: Struct와 Class 활용 — 기본 문제(대장장이: Q 일반 무기 struct 복사 / W 전설 무기 class 공유 / E `linked = null` 메모리 확인) + 선택형(몬스터 템플릿 `Clone()` Deep Copy, R 테스트 시나리오).

| 폴더 | 내용 | 상태 |
|---|---|---|
| [01-weapons](01-weapons) | `Weapons.cs`(NormalWeapon struct · LegendaryWeapon class) · `Blacksmith.cs`(Q/W/E) · `MonsterTemplate.cs`(BaseStats · SkillSet · MonsterTemplate.Clone) · `MonsterSpawner.cs`(R) | 완료 |

## 열기 · 실행

Unity `6000.3.9f1`. GM-017 프로젝트(Universal 3D 템플릿, Active Input Handling `Both`, URP 머티리얼 업그레이드 반영)를 복사해 스크립트와 씬 오브젝트만 바꿨다.

1. Unity Hub → **Add project from disk** → `01-weapons` 선택
2. `Assets/Scenes/SampleScene` 열기 (빈 오브젝트 `Blacksmith`, `MonsterSpawner`에 각각 스크립트가 붙어 있음)
3. Play → Game 뷰를 클릭해 포커스 → 기본 `Q` / `W` / `E`, 선택형 `R`

## 확인 로그

Unity 과제라 단위 테스트는 붙이지 않았다. Unity 배치 모드에서 임시 에디터 스크립트로 씬을 구성하고 각 키에 해당하는 메서드를 순서대로 호출해 Console 로그를 파일로 남겼다. 임시 스크립트는 커밋하지 않았다.

| 파일 | 내용 |
|---|---|
| `run-00-scene.txt` | 씬의 두 오브젝트에 컴포넌트 각 1개 |
| `run-01-q-w-e.txt` | W 전 E → 안내, Q: 원본 10 / 복사본 50, W: 원본 999 / 연결 999, E: `linked` null · GC 후 Heap 객체 남아 있음 · `original`로 999 접근. 비교: `original`까지 끊으면 GC 후 사라짐 |
| `run-02-clone.txt` | R 2번: 원본 HP 1000 · 1.5배 / 인스턴스1 HP 500 · 9.9배, PASS / PASS |
| `run-03-shallow-compare.txt` | 같은 시나리오를 얕은 복사로: List 그대로 대입 → 원본 9.9배, 새 List에 같은 SkillSet → 여전히 원본 9.9배, `Clone()` → List · SkillSet 모두 다른 객체 |

---

## 01 무기 · 몬스터

### 기본 문제

- `Weapons.cs`: `struct NormalWeapon`, `class LegendaryWeapon` (둘 다 `name`, `power`)
- `Blacksmith.cs`
  - Q `TestStructCopy()`: 지역 변수 `original`(10) → `copy = original` → `copy.power = 50` → 둘 출력
  - W `TestClassShare()`: 필드 `original = new`(100) → `linked = original` → `linked.power = 999` → 둘 출력. `WeakReference`로 그 객체를 추적
  - E `ReleaseLinked()`: `linked = null` → `GC.Collect()` → `WeakReference.IsAlive`로 Heap 객체가 남았는지 출력 → `original`로 접근

### 선택형

- `MonsterTemplate.Clone()`: 새 객체 → `stats`는 대입(구조체라 값 복사) → **새 List를 만들고 SkillSet도 하나씩 `SkillSet.Clone()`으로 새로 만들어 담음**
- `MonsterSpawner.RunCloneTest()`: 시나리오 1~4 → 원본 · 인스턴스1 출력 → HP 1000 · 배율 1.5 유지 PASS/FAIL

**명세에 없거나 해석해서 정한 것**

| 항목 | 정한 내용 |
|---|---|
| 선택형 키 | 원문 "[Keycode.] 키"에 키 이름이 빠짐 → **R** (Q/W/E 다음) |
| MonsterTemplate 이름 | 요구 사항 멤버는 `stats` · `skills`뿐인데 시나리오가 이름("드래곤", "드래곤_인스턴스1")을 씀 → `string name` 추가 |
| 드래곤 ATK | 시나리오에 값 없음 → 넣지 않음 (기본값 0으로 출력) |
| E에서 "Heap 실체가 사라지는지" | 출력 방법이 없어 `WeakReference` + `GC.Collect()`로 실제 확인 |
| W 전에 E | 문구가 없어 정함 (`아직 전설 무기가 없습니다. W를 먼저 누르세요.`) |
| 무기 이름 | 정의 없음 → 강철 검 / 엑스칼리버 |
| 스크립트 구성 | 제출 형식 정의 없음 → 데이터 타입과 MonoBehaviour를 나눠 4개 |
