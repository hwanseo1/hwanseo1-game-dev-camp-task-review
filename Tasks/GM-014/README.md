# GM-014

Unity 과제: 인벤토리 · 필드 아이템 관리 (키 입력 + 배열 순회 · 검색 · 수정, 결과는 Console 로그).

| 폴더 | 내용 | 상태 |
|---|---|---|
| [01-inventory](01-inventory) | I 인벤토리 출력(for) / G 필드 아이템 줍기(빈칸 검색 + break) / U 1번 슬롯 사용 + 앞으로 당기기(심화) / S 독사과 모두 버리기 | 완료 ("가방이 가득 찼습니다!"는 도달 불가) |

## 열기 · 실행

Unity `6000.3.9f1`. GM-013 프로젝트(Universal 3D 템플릿, Active Input Handling `Both`, URP 머티리얼 업그레이드 반영)를 복사해 스크립트만 바꿨다.

1. Unity Hub → **Add project from disk** → `01-inventory` 선택
2. `Assets/Scenes/SampleScene` 열기 (빈 오브젝트 `InventoryManager`에 스크립트가 붙어 있음)
3. Play → Game 뷰를 클릭해 포커스 → `I` / `G` / `U` / `S` → Console 확인

Active Input Handling이 템플릿 기본값(새 Input System 전용)이면 샘플의 `Input.GetKeyDown`이 예외를 던진다 (GM-013 `run-00` 참고).

## 확인 로그

Unity 과제라 단위 테스트는 붙이지 않았다. Unity 배치 모드에서 임시 에디터 스크립트로 `InventoryManager`를 만들고, `Start`를 부른 뒤 각 키에 해당하는 메서드를 순서대로 호출해 Console 로그를 파일로 남겼다. 임시 스크립트는 커밋하지 않았다.

| 파일 | 내용 |
|---|---|
| `run-01-pickup-all.txt` | 빈 인벤토리 → G 5번으로 1~5번 슬롯 채움 → 6번째 G는 "필드에 더 이상 아이템이 없습니다." |
| `run-02-discard-apple.txt` | 독사과(4번 슬롯) 버리기 → 다시 S는 아무 출력 없음 → 다음 G의 방패가 빈 4번 슬롯으로 |
| `run-03-use-and-shift.txt` | U로 물약 사용 → 뒤 아이템이 한 칸씩 앞으로 → 빈 1번 슬롯에서 U는 아무 출력 없음 |
| `run-04-full-bag-unreachable.txt` | 가방이 가득 찬 순간 필드도 비어 있어 **"가방이 가득 찼습니다!"가 나오지 않음** |
| `run-05-spec-only-index-error.txt` | 샘플의 필드 검사 없이 명세 문장만 따라 만들면 G 5번 → U → G에서 `IndexOutOfRangeException` (임시 스크립트 안에서 재현) |

---

## 01 인벤토리 관리

`Assets/Scripts/InventoryManager.cs` 하나. `Update`는 키 입력만 보고 `ShowInventory` · `PickUp` · `UseFirstSlot` · `DiscardPoisonApple`을 부른다.

**명세에 없거나 해석해서 정한 것**

| 항목 | 정한 내용 |
|---|---|
| G 검사 순서 | 샘플대로 "필드가 비었나"를 먼저, 그다음 빈칸 검색. 빈칸을 못 찾으면 "가방이 가득 찼습니다!" |
| "꽉 찼다" 판정 | `bool isPicked`로 루프 안에서 넣었는지 기록하고, 루프 뒤에 `false`면 출력 |
| 획득 로그 | 문구가 없어 `물약(을)를 주웠습니다! (1번 슬롯)`로 정함 |
| U 심화 (앞으로 당기기) | 구현함. 사용 후 `inventory[i] = inventory[i + 1]`로 당기고 마지막 칸을 "비어있음"으로 |
| U, 1번 슬롯이 빈 경우 | 동작 정의가 없어 아무것도 출력하지 않음 |
| S 버리기 로그 | 문구가 없어 `독사과를 버렸습니다! (4번 슬롯)`로 정함 |
| S, 독사과가 없는 경우 | 동작 정의가 없어 아무것도 출력하지 않음 |
| 버린 칸 | S는 당기지 않는다 (심화는 U에만 있음) → 중간에 빈칸이 생기고, 다음 G가 그 칸을 채운다 |
