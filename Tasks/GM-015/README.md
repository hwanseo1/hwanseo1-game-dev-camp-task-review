# GM-015

Unity 과제: 몬스터 카드 덱 매니저 (public 배열 + 인스펙터 확인, Swap · Fisher-Yates 셔플 · 대결, 결과는 Console 로그).

| 폴더 | 내용 | 상태 |
|---|---|---|
| [01-card-deck](01-card-deck) | W 0번↔마지막 Swap / S Fisher-Yates 셔플 / Space 0번 카드 vs 보스 대결 (승·패·무) | 완료 (셔플은 샘플의 Fisher-Yates를 따름) |

## 열기 · 실행

Unity `6000.3.9f1`. GM-014 프로젝트(Universal 3D 템플릿, Active Input Handling `Both`, URP 머티리얼 업그레이드 반영)를 복사해 스크립트만 바꿨다.

1. Unity Hub → **Add project from disk** → `01-card-deck` 선택
2. `Assets/Scenes/SampleScene` 열기 (빈 오브젝트 `CardGameManager`에 스크립트가 붙어 있음)
3. Hierarchy에서 `CardGameManager`를 선택해 인스펙터에 **Monster Deck**이 보이게 둔다
4. Play → Game 뷰를 클릭해 포커스 → `W` / `S` / `Space` → 인스펙터 숫자와 Console 확인

Active Input Handling이 템플릿 기본값(새 Input System 전용)이면 샘플의 `Input.GetKeyDown`이 예외를 던진다 (GM-013 `run-00` 참고).

## 확인 로그

Unity 과제라 단위 테스트는 붙이지 않았다. Unity 배치 모드에서 임시 에디터 스크립트로 컴포넌트를 만들고 각 키에 해당하는 메서드를 순서대로 호출해 Console 로그와 배열 상태(인스펙터에 보일 값)를 파일로 남겼다. 임시 스크립트는 커밋하지 않았다.

| 파일 | 내용 |
|---|---|
| `run-01-swap-w.txt` | W 3번 → 1과 10이 바뀌고, 두 번 누르면 원래대로 |
| `run-02-shuffle-s.txt` | S 3번 → 매번 10장이 다시 배치됨 (`Random.InitState(15)`로 고정) |
| `run-03-battle-space.txt` | 섞지 않고 Space 6번 → 계속 같은 0번 카드(1)로 대결. 무승부도 나옴. S 뒤에는 0번 카드가 바뀜 |
| `run-04-empty-deck.txt` | 인스펙터에서 배열 크기를 0으로 바꾸면 W·S는 샘플의 검사 덕에 무사, Space는 `IndexOutOfRangeException` |
| `run-05-shuffle-bias.txt` | **명세 3단계 문장대로 만든 셔플은 편향됨** (100만 번: 위치별 7.74%~12.88%, Fisher-Yates는 9.89%~10.07%) |

---

## 01 카드 덱

`Assets/Scripts/CardGameManager_Student.cs` 하나 (샘플 클래스 이름 그대로). `Update`는 키 입력만 보고 `SwapAt` · `ShuffleFisherYates` · `Battle`을 부른다.

**명세에 없거나 해석해서 정한 것**

| 항목 | 정한 내용 |
|---|---|
| 셔플 방식 | 명세 문장(i = 0→끝, `Random.Range(0, 10)`)과 샘플(i = 끝→1, `Random.Range(0, i + 1)`)이 다르다. **샘플의 Fisher-Yates를 따름**. 명세 방식의 편향은 `run-05`로 확인 |
| 무승부 | 명세는 승리·패배만, 샘플 TODO 5는 "승/패/무" → 무승부도 출력 |
| 대결 후 셔플 | 명세 "다시 섞어야 공정"은 샘플 TODO 6처럼 **안내 로그만** 출력하고 자동으로 섞지 않음 |
| 대결 결과 문구 | 없어서 `⚔️ 내 카드 6 vs 보스 2 → 승리!` 형식으로 정함 |
| 뽑은 카드 | 덱에서 빼지 않음 (명세에 제거 언급 없음) → 섞지 않으면 같은 카드로 계속 대결 |
| Space의 빈 덱 검사 | 샘플 TODO에 없어 넣지 않음 (`run-04`) |
