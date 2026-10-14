# GM-016

Unity 과제: 카드 배틀 덱 매니저 (도감 배열 → 내 덱 List, while 리필 · Fisher-Yates 셔플 · foreach 출력, Space 드로우 / B 버리기 / I 삽입 / R 리필, 결과는 Console 로그).

| 폴더 | 내용 | 상태 |
|---|---|---|
| [01-deck-manager](01-deck-manager) | 시작 시 리필·셔플·출력 / Space 드로우+RemoveAt / B 슬라임 전부 버리기(뒤→앞 for) / I EmergencyHeal 0번 Insert / R 빈 덱일 때 리필+셔플+출력 | 완료 |

## 열기 · 실행

Unity `6000.3.9f1`. GM-015 프로젝트(Universal 3D 템플릿, Active Input Handling `Both`, URP 머티리얼 업그레이드 반영)를 복사해 스크립트만 바꿨다.

1. Unity Hub → **Add project from disk** → `01-deck-manager` 선택
2. `Assets/Scenes/SampleScene` 열기 (빈 오브젝트 `DeckManager`에 스크립트가 붙어 있음)
3. Hierarchy에서 `DeckManager`를 선택해 인스펙터에 **Player Deck**이 보이게 둔다
4. Play → 시작하자마자 섞인 덱 10장이 Console에 찍힘 → Game 뷰를 클릭해 포커스 → `Space` / `B` / `I` / `R`

## 확인 로그

Unity 과제라 단위 테스트는 붙이지 않았다. Unity 배치 모드에서 임시 에디터 스크립트로 컴포넌트를 만들고 각 키에 해당하는 메서드를 순서대로 호출해 Console 로그와 List 상태(인스펙터에 보일 값)를 파일로 남겼다. 임시 스크립트는 커밋하지 않았다. 난수는 `Random.InitState(16)`으로 고정.

| 파일 | 내용 |
|---|---|
| `run-00-scene.txt` | 씬의 `DeckManager` 오브젝트에 컴포넌트 1개 |
| `run-01-start-refill-shuffle.txt` | 시작 시 리필 → 셔플 → 출력. 리필 직후·셔플 직후 Count 10, 12장 상태에서 리필해도 10 |
| `run-02-draw-space.txt` | Space 11번 → 10→0장으로 1씩 줄고, 빈 덱에서는 안내만 (`[0]` 접근 없음) |
| `run-03-insert-i.txt` | I → 11장, 다음 Space에서 EmergencyHeal이 먼저 나옴. 빈 덱에서도 Insert 가능 |
| `run-04-discard-b.txt` | B → 슬라임 1장 제거, 다시 B → 0장. 슬라임 5장 덱(테스트 전용으로 직접 채움)도 전부 제거. 비교: 앞→뒤 for는 2장 남음, foreach 중 Remove는 `InvalidOperationException` |
| `run-05-refill-r.txt` | 카드가 남아 있으면 R 거절, 빈 덱에서 R → 10장 리필·셔플·출력 |
| `run-06-shuffle-distribution.txt` | 셔플 100만 번: 위치별 9.93%~10.07% (이론값 10%), Count가 10이 아닌 경우 0번 |

---

## 01 덱 매니저

`Assets/Scripts/DeckManager.cs` 하나. `Update`는 키 입력만 보고 `DrawCard` · `DiscardAll` · `InsertEmergency` · `RefillAndShuffle`을 부른다.

**명세에 없거나 해석해서 정한 것**

| 항목 | 정한 내용 |
|---|---|
| 게임 시작 시 덱 | 4단계에는 없고 소개 문단("게임 시작 전 … 구성하고 섞은 뒤")에만 있다 → `Start`에서 리필 · 셔플 · 출력 |
| R 조건 | 권장안대로 **덱이 비었을 때만** 리필. 남아 있으면 거절 로그 |
| 리필 인덱스 되돌리기 | 명세대로 넣었지만 Clear 후 10장만 채우므로 실제로는 쓰이지 않는다 (소스 주석) |
| 도감 · 덱 공개 여부 | 도감은 `private readonly` (인스펙터 값이 코드보다 우선하는 문제 회피, GM-015), 덱은 Count 변화를 보려고 `public` |
| 빈 덱 Space, R 거절, B 결과 문구 | 명세에 없어 정함 (`덱이 비었습니다. R로 리필하세요.` 등) |
| PrintDeck 형식 | 선택 사항 중 "번호와 함께" + 첫 줄에 장수 |
| 버릴 카드 · 긴급 카드 | 명세 예시 그대로 "슬라임", "EmergencyHeal" |
