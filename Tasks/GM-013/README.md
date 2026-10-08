# GM-013

Unity 과제: 하루 일과 시뮬레이터 (키 입력 + `for` · `while` · `do-while` · `if`, 결과는 Console 로그).

| 폴더 | 내용 | 상태 |
|---|---|---|
| [01-day-simulator](01-day-simulator) | Space 훈련(for, 3회 제한) / H 탐험(while, 20% 함정, break) / B 보스전(do-while) / R 상태 확인(if, LogWarning) | 완료 (보스 방어막 · "휴식"은 정의가 없어 미구현) |

## 열기 · 실행

Unity `6000.3.9f1`, Universal 3D 템플릿(`com.unity.template.3d-cross-platform-17.0.14`)으로 만들었다.

1. Unity Hub → **Add project from disk** → `01-day-simulator` 선택
2. 처음 열 때 URP 머티리얼 업그레이드 창이 뜨면 업그레이드 (반영해서 커밋해 둠)
3. `Assets/Scenes/SampleScene` 열기 (빈 오브젝트 `DaySimulator`에 스크립트가 붙어 있음)
4. Play → Game 뷰를 클릭해 포커스 → `Space` / `H` / `B` / `R` → Console 확인

**Active Input Handling을 `Both`로 바꿔 두었다.** Unity 6 템플릿 기본값은 `Input System Package (New)`라서, 샘플 코드의 `Input.GetKeyDown`이 호출될 때마다 `InvalidOperationException`을 던진다 (`screenshots/run-00-legacy-input-error.txt`).

## 확인 로그

Unity 과제라 단위 테스트는 붙이지 않았다. 대신 Unity 배치 모드에서 임시 에디터 스크립트로 `DaySimulator`를 만들고, 각 키에 해당하는 메서드를 순서대로 호출해 Console 로그를 파일로 남겼다 (`Random.InitState`로 시드 고정). 임시 스크립트는 커밋하지 않았다.

| 파일 | 내용 |
|---|---|
| `run-00-legacy-input-error.txt` | 템플릿 기본 설정(`activeInputHandler: 1`)에서 `Input.GetKeyDown` → `InvalidOperationException` |
| `run-01-training.txt` | Space 4번: 공격력 10 → 25, 4번째는 "오늘은 너무 지쳤습니다" |
| `run-02-explore-twice.txt` | H 2번: 첫 번째는 진행도 0→100 · 골드 +50, **두 번째는 진행도가 100에 남아 있어 반복 없이 바로 +50** |
| `run-03-boss-lose-then-dead.txt` | 공격력 10으로 보스전: 5번 주고받고 HP 0 패배 → **HP 0인데도 do-while이 한 번 돌아 HP -20** → H 첫 반복에서 "탐험 실패!" → 죽은 뒤에도 훈련 가능 |
| `run-04-boss-win.txt` | 훈련 3번(공격력 25) → 4타에 보스 HP 0, **마지막 타격에도 반격**해 HP 20 → R에서 경고 |
| `run-05-explore-traps.txt` | 함정 2번(HP 100 → 80) 걸리고도 탐험 성공 |

---

## 01 하루 일과 시뮬레이터

`Assets/Scripts/DaySimulator.cs` 하나. `Update`는 키 입력만 보고 `Train` · `Explore` · `FightBoss` · `CheckStatus`를 부른다.

**명세에 없거나 해석해서 정한 것**

| 항목 | 정한 내용 |
|---|---|
| 변수 이름 | 명세의 `trainingCount`를 썼다 (샘플은 `dailyTrainingDone`). `isBossDefeated` · `explorationProgress`도 선언 |
| 훈련 로그 문구 | 명세의 `칼을 휘두릅니다! (N회)`를 썼다 (샘플은 `N번째 연습 휘두르기!`) |
| "하루에 3번" | 하루가 바뀌는 규칙이 없어 `trainingCount`를 되돌리지 않는다 → 게임 전체에서 3번 |
| 탐험 진행도 | 초기화하라는 말이 없어 멤버 변수 그대로 둔다 → 두 번째 H는 바로 성공 (명세 그대로, 리뷰에 지적) |
| 함정 확률 | `Random.Range(0, 100) < 20` |
| 탐험 성공 판정 | 진행도 100 이상이고 HP가 0보다 클 때 골드 +50 |
| 반격 시점 | "공격을 받을 때마다" → 보스 HP를 0으로 만든 타격에도 반격한다 |
| `isBossDefeated` | 보스 HP가 0 이하로 끝나면 `true`. 처치 후 재도전은 막지 않는다 (규칙 없음) |
| 빨간색 로그 | 명세대로 `Debug.LogWarning` (실제로는 노란색. 리뷰에 지적) |
| 사망 | 규칙이 없어 HP 0 이하에서도 모든 키가 동작한다 |

**구현하지 않은 것** (동작 정의가 없음, 소스에 주석)

- 보스 방어막 "첫 타격은 무조건 견뎌냅니다": 상황 설명에만 있고 로직이 없다. 공격력이 최대 25라 첫 타격에 보스(HP 100)가 죽는 일도 없다.
- 5단계 제목의 "휴식": 회복량 · 조건이 없다.
