# GM-040

Unity 과제 1) 스킬 쿨타임 시스템: Space로 스킬을 쓰면 플레이어(큐브) 색이 바뀌고 5초 쿨타임이 시작된다. 쿨타임 동안은 다시 못 쓰고, 화면 아래 게이지가 차오르며, 1초마다 남은 시간을 로그로 찍는다. C를 누르면 쿨타임이 취소돼 바로 다시 쓸 수 있다.

| 폴더 | 내용 | 상태 |
|---|---|---|
| [01-skill-cooldown](01-skill-cooldown) | `SkillCooldown.cs`: Space 스킬 사용 · 코루틴 쿨타임 5초 · 초 단위 로그 · C 취소 · 게이지(`Image.fillAmount`) | 완료 |

## 열기 · 실행

Unity `6000.3.9f1`. GM-022 프로젝트(Universal 3D 템플릿, Active Input Handling `Both`)를 복사해 스크립트와 씬 오브젝트만 바꿨다.

1. Unity Hub → **Add project from disk** → `01-skill-cooldown` 선택
2. `Assets/Scenes/SampleScene` 열기 (`Player`의 `SkillCooldown`에 `Canvas/CooldownGauge/Fill` · `Label` 연결됨)
3. Play → Game 뷰를 클릭해 포커스

| 키 | 동작 |
|---|---|
| Space | 스킬 사용 (쿨타임 중이면 사용 불가 로그) |
| C | 쿨타임 취소 → 바로 스킬 사용 가능 |

화면 아래 가운데에 게이지와 라벨(`스킬 사용 가능 (Space)` / `쿨타임 3.5초`)이 나오고, 진행 로그는 Console에 나온다.

## 확인 로그

Unity 과제라 단위 테스트는 붙이지 않았다. 배치 모드에서 임시 에디터 스크립트로 씬을 만들고, 배치 모드 **Play**에 들어가 임시 봇이 키 대신 `TryUseSkill()` · `CancelCooldown()`을 호출하게 해서 게임 안 `Debug.Log`와 0.5초마다의 상태(남은 시간 · 게이지 · 색 · 라벨)를 기록했다. 실제 시간(60fps)으로 돌렸다. 임시 스크립트는 커밋하지 않았다.

| 파일 | 내용 |
|---|---|
| `run-00-scene.txt` | 씬 구성: Player(큐브 + `SkillCooldown`), Canvas → CooldownGauge(배경) → Fill(Filled · Horizontal) · Label |
| `run-01-use-and-wait.txt` | Space → 색 파랑, 로그 5 → 4 → 3 → 2 → 1초 남음 → 5.02초에 "스킬 사용 가능" · 색 흰색, 게이지 0.10씩 차오름 → 다시 Space 성공 |
| `run-02-blocked-during-cooldown.txt` | 쿨타임 중 1.2 / 2.5 / 4.6초에 Space → 모두 "사용 불가 (n초 남음)", 끝난 뒤 Space 성공 |
| `run-03-cancel.txt` | 2.3초에 C → 바로 사용 가능(게이지 1.00 · 흰색) → 같은 프레임 Space 성공, 다시 1초 뒤 C → 취소, 사용 가능 상태에서 C → "취소할 쿨타임 없음" |

---

## 01 스킬 쿨타임

### `SkillCooldown.cs`

- `Update()`: Space `GetKeyDown` → `TryUseSkill()`, C `GetKeyDown` → `CancelCooldown()`
- `TryUseSkill()`: 쿨타임 코루틴이 돌고 있으면(`cooldownRoutine != null`) 사용 불가 로그 후 끝. 아니면 색을 `skillColor`로 바꾸고 `StartCoroutine(CooldownRoutine())`
- `CooldownRoutine()`: `remaining = 5`에서 매 프레임(`yield return null`) `Time.deltaTime`만큼 줄인다. 게이지는 매 프레임, 로그는 `Mathf.CeilToInt(remaining)`이 바뀔 때만 → 5 · 4 · 3 · 2 · 1초 남음. 0이 되면 `EndCooldown()`
- `CancelCooldown()`: `StopCoroutine` 후 `EndCooldown()` (남은 시간 0, 색 `readyColor`, 게이지 가득)
- 게이지: `fillAmount = 1 - remaining / cooldownTime` (쓰면 비고, 차오르다 가득 차면 사용 가능)

**명세에 없거나 해석해서 정한 것**

| 항목 | 정한 내용 |
|---|---|
| 스킬 사용 시 어떤 색으로 · 언제 되돌리나 | 정의 없음 → **평소 흰색, 스킬 사용 직후 파랑, 쿨타임이 끝나거나 C로 취소하면 흰색** (사용자 결정). 색만 보고 사용 가능 여부를 알 수 있다 |
| 쿨타임 진행 로그 형식 | "초 단위로"만 있음 → **남은 시간 카운트다운** `쿨타임 5초 남음` → … → `1초 남음` → `스킬 사용 가능` (사용자 결정) |
| C 키 동작 | "쿨타임 취소 → 바로 시전 가능한 구조" → 취소만 하고 **스킬을 대신 쓰지는 않는다**. 사용 가능 상태에서 C는 "취소할 쿨타임 없음" 로그만 |
| 게이지 모양 · 방향 | 정의 없음 → 화면 아래 가로 막대 1개, 쓰면 비고 차오름 + 라벨에 남은 초(소수 1자리) |
| 쿨타임 중 Space | "사용 불가"만 있음 → 아무 동작 없이 `쿨타임 중이라 스킬 사용 불가 (n초 남음)` 로그 |
| 플레이어 도형 | "도형"만 있음 → 큐브 |
| 스킬 효과 | 색 변경 외 정의 없음 → 없음 |
| 코루틴 / UniTask | "코루틴 사용 / 유니테스크 (선택)" → 코루틴만 사용 (UniTask는 외부 패키지라 설치 안내가 없어 넣지 않음) |
