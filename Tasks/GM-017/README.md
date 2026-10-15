# GM-017

Unity 과제: 던전 탐험 시스템 (몬스터 대기열 Queue · 이동 경로 Stack, S 정찰 / A 전투 / M 이동 / B 퇴각) + 심화 가중치 랜덤 드랍 (Array + 누적합, Stack Undo 3회, 결과는 Console 로그).

| 폴더 | 내용 | 상태 |
|---|---|---|
| [01-dungeon](01-dungeon) | `DungeonExplorer.cs`: S Enqueue / A Dequeue / M Push / B Pop + Peek · `WeightedLootSystem.cs`: D 누적합 뽑기 + Push / U Pop + 다시 뽑기 (최대 3번) | 완료 |

## 열기 · 실행

Unity `6000.3.9f1`. GM-016 프로젝트(Universal 3D 템플릿, Active Input Handling `Both`, URP 머티리얼 업그레이드 반영)를 복사해 스크립트만 바꿨다.

1. Unity Hub → **Add project from disk** → `01-dungeon` 선택
2. `Assets/Scenes/SampleScene` 열기 (빈 오브젝트 `DungeonExplorer`, `WeightedLootSystem`에 각각 스크립트가 붙어 있음)
3. Play → Game 뷰를 클릭해 포커스 → 던전 `S` / `A` / `M` / `B`, 심화 `D`(뽑기) / `U`(Undo)

Queue · Stack은 Unity가 직렬화하지 않아 인스펙터에 보이지 않는다. 상태는 Console 로그로만 확인한다.

## 확인 로그

Unity 과제라 단위 테스트는 붙이지 않았다. Unity 배치 모드에서 임시 에디터 스크립트로 컴포넌트를 만들고 각 키에 해당하는 메서드를 순서대로 호출해 Console 로그와 Queue/Stack 상태를 파일로 남겼다. 임시 스크립트는 커밋하지 않았다. 난수는 `Random.InitState(17)`로 고정.

| 파일 | 내용 |
|---|---|
| `run-00-scene.txt` | 씬의 두 오브젝트에 컴포넌트 각 1개 |
| `run-01-scout-fight.txt` | 빈 대기열 A → 안내, S 4번 → A 5번: 발견 순서대로 처치(FIFO), 남은 적 3→0, 다시 빈 대기열 안내 |
| `run-02-move-retreat.txt` | 입구에서 B → 안내, M 3번(거리 1→3) → B: 2번째 방 → 1번째 방 → 입구 안내 → 입구 안내 (LIFO) |
| `run-03-draw-undo.txt` | **뽑기 10회 + Undo 5회 시도**: 빈 히스토리 Undo 안내, 성공 3번(남은 Undo 2→1→0, 매번 다시 뽑기), 소진 후 2번 거절 |
| `run-04-boundaries.txt` | r 경계값(699/700, 899/900, 989/990, 999)과 r=0~999 전수: 700/200/90/10개. 비교: `>=`로 쓰면 701/200/90/9 |
| `run-05-distribution.txt` | 뽑기 100만 번: 70.02% / 20.00% / 8.99% / 1.00% |

---

## 01 던전

### `DungeonExplorer.cs`

`Update`는 키 입력만 보고 `Scout` · `Fight` · `MoveDeeper` · `Retreat`를 부른다.

### `WeightedLootSystem.cs`

- `GetTotalWeight()`: `weights`를 반복문으로 더함 (1000 하드코딩 대신)
- `PickIndex(r)`: 앞에서부터 누적하다 `누적합 > r`이 되는 첫 인덱스
- `Draw()`: `r = Random.Range(0, GetTotalWeight())` → `itemNames[PickIndex(r)]` → `lootHistory.Push` → `획득: (아이템) (r=N)`
- `Undo()`: 히스토리 비었는지 → `undoLeft == 0`인지 순서로 검사 → `Pop` + `undoLeft--` → `취소: (아이템) / 남은 Undo: N` → 바로 `Draw()`

**명세에 없거나 해석해서 정한 것**

| 항목 | 정한 내용 |
|---|---|
| Enqueue · [조우 예고] 위치 | 명세는 3단계 [A]에 적었지만, 2단계 제목("몬스터 발견 (Queue)")과 Hint(발견한 순서대로 다가온다)를 따라 **S**에서 Enqueue. A는 Dequeue만 |
| 빈 대기열 A | 문구가 없어 정함 (`[전투] 싸울 몬스터가 없습니다. S로 정찰하세요.`). 확인 없이 Dequeue하면 예외 |
| 총 이동 거리 | `travelHistory.Count` (입구에서 현재 방까지 깊이). 퇴각하면 줄어든다 |
| 입구 안내 시점 | 이미 입구에서 B (Pop 전 빈 스택) **와** 첫 번째 방에서 B (Pop 후 Peek할 방 없음) 둘 다 |
| 시작 위치 | 빈 스택 = 던전 입구. 처음부터 방에 있다는 정의는 없어 넣지 않음 |
| 심화 키 | 명세에 없어 **D**(Draw) / **U**(Undo). 던전 키와 겹치지 않게 |
| Undo 3번 | 명세 예시(`undoLeft = 3`, 성공 시 1씩 감소)대로 **게임 전체 3번**, 다시 채우지 않음 |
| Undo 불가 문구 | 히스토리 없음 / 소진 둘 다 문구가 없어 정함 |
| 스크립트 수 | 제출은 "스크립트 1개(WeightedLootSystem.cs 등)"지만 던전 본문과 심화가 별개 시스템이라 2개로 나눔 |
| 몬스터 · 방 이름 | 정의 없음. 몬스터는 Hint의 늑대·고블린 포함 5종, 방 5종 |
| 로그 조사 | 명세 형식 그대로 `(이름)이` · `(이름)을` · `(방이름)으로` → "늑대이", "무너진 서고으로"처럼 받침 없는 이름에서 어색함 (고치지 않음) |
