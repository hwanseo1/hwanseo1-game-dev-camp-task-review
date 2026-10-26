# GM-025

Unity 과제 1) 웨이브 맵: 룸 3개(적 1 / 2 / 3마리)를 차례로 클리어한다. 룸에 들어가면 적이 나오고, 모두 처치하면 다음 룸 문이 열린다. 뒤 룸일수록 몬스터가 빠르고 · 세고 · 크다. 룸마다 NavMesh를 따로 굽고, 룸 차이는 `RoomData` 값만으로 낸다.

| 폴더 | 내용 | 상태 |
|---|---|---|
| [01-wave-map](01-wave-map) | `RoomData`(ScriptableObject) · `Room`(웨이브 · 문) · `Monster`(추적 · 공격, 모든 룸 같은 AI) · `PlayerController`(WASD · Space) · `GameManager`(HP 표시 · 클리어 · 게임오버 · R 재시작) | 완료 |

## 열기 · 실행

Unity `6000.3.9f1`. GM-024 프로젝트(Universal 3D 템플릿, Active Input Handling `Both`, AI Navigation 2.0.10 패키지 포함)를 복사해 스크립트와 씬을 새로 만들었다.

1. Unity Hub → **Add project from disk** → `01-wave-map` 선택
2. `Assets/Scenes/SampleScene` 열기
3. Play → Game 뷰를 클릭해 포커스

| 키 | 동작 |
|---|---|
| W A S D / 방향키 | 이동 (이동 방향을 바라봄) |
| Space | 앞쪽 근접 공격 (범위 안 몬스터 모두 1 피해, 0.3초 간격) |
| R | 클리어 · 게임오버 후 재시작 |

화면 왼쪽 위에 HP · 현재 룸 · 남은 적 · 안내 문구가 나온다. 맞으면 플레이어가 잠깐 빨개진다.

## 맵

남 → 북으로 룸 1 → 복도(문1) → 룸 2 → 복도(문2) → 룸 3. 복도 바닥은 어느 NavMesh에도 속하지 않아 몬스터는 자기 룸 밖으로 못 나간다.

| 룸 | 크기 | 장애물 | NavMesh (Agent Type) |
|---|---|---|---|
| Room1 | 16 × 16 | 기둥 2개 | `NavMesh-Room1` (Small, 반지름 0.35) |
| Room2 | 20 × 20 | 엇갈린 가림벽 2개(지그재그) + 상자 2개 | `NavMesh-Room2` (Medium, 반지름 0.5) |
| Room3 | 24 × 24 | 가운데 가로벽(가운데 1.2m 틈, 양 끝 3.4m 통로) + 기둥 4개 | `NavMesh-Room3` (Large, 반지름 0.8) |

Room3 가운데 틈은 Large(지름 1.6)가 못 지나가 양 끝으로 돌아온다. 같은 룸을 Small로 구우면 틈으로 바로 지나간다 (`run-00`: 6.0m vs 21.3m).

## 룸 데이터 (`Assets/Data/Room1~3Data.asset`)

| 항목 | Room1 | Room2 | Room3 |
|---|---|---|---|
| 스폰 수 | 1 | 2 | 3 |
| HP | 3 | 4 | 5 |
| 공격력 / 간격 | 5 / 1.5초 | 7 / 1.2초 | 9 / 1.2초 |
| 사거리 | 1.4 | 1.7 | 2.1 |
| 크기(스케일) · 색 | 0.7 · 노랑 | 1.0 · 주황 | 1.6 · 빨강 |
| Agent Type | Small | Medium | Large |
| speed / acceleration / angularSpeed | 2.5 / 8 / 240 | 3.5 / 12 / 360 | 4.5 / 16 / 480 |
| stoppingDistance / avoidancePriority | 1.1 / 60 | 1.3 / 50 | 1.6 / 40 |

## 확인 로그

Unity 과제라 단위 테스트는 붙이지 않았다. 배치 모드에서 임시 에디터 스크립트로 씬 · 데이터 · 프리팹을 만들고 NavMesh 3개를 구웠다. 그다음 배치 모드 **Play**에 들어가 임시 봇이 키 대신 플레이어를 움직이고 공격하게 해서 로그를 남겼다(게임 안 `Debug.Log`도 같이 기록). 임시 스크립트는 커밋하지 않았다.

| 파일 | 내용 |
|---|---|
| `run-00-layout.txt` | 룸 · 장애물 배치, 룸마다 NavMeshSurface 1개(Agent Type 다름), 다른 룸 / 다른 타입 위치에는 NavMesh 없음, 룸 사이 경로 계산 불가, Room3 틈: Large 21.3m 우회 vs Small 6.0m 직진 |
| `run-01-data-agent.txt` | `RoomData` 표 → 스폰된 몬스터의 HP · 스케일 · NavMeshAgent 값이 데이터와 같음, 실제 반지름 = Agent Type 반지름, 추적 최고 속도 2.5 / 3.5 / 4.5 m/s |
| `run-02-full-clear.txt` | 닫힌 문에 막힘 → 1번 처치 → 문1 열림 → 2번 룸 들어가면 2마리 → 문2 → 3번 룸 3마리 → 클리어. 제자리에서 싸운 봇이 받은 피해 5 / 14 / 54 |
| `run-03-gameover-restart.txt` | 3번 룸에서 가만히 맞기 → HP 0 → 게임오버(플레이어 · 몬스터 정지, 공격 안 됨) → `Restart()` 후 HP 100 · 문 닫힘 · 1번 룸부터 |
| `run-04-data-change.txt` | 코드 그대로 1번 룸 데이터만 바꿈(스폰 4 · HP 10 · 공격력 20 · 크기 1.6 · Large · 속도 6) → NavMesh를 Large로 다시 굽고 4마리가 6 m/s로 추적, 1회 피해 20 |

---

## 01 웨이브 맵

### 구조

- **`RoomData`** (ScriptableObject): 스폰 수, HP · 공격력 · 공격 간격 · 사거리, 크기 · 색, Agent Type 이름과 NavMeshAgent 값(speed · acceleration · angularSpeed · stoppingDistance · avoidancePriority)
- **`Room`**: 플레이어가 바닥 범위 1m 안쪽에 들어오면 `StartWave()` → `spawnCount`만큼 프리팹 하나(`Monster.prefab`)를 스폰해 `Init(data)`. 다 죽으면 `nextDoor.SetActive(false)`, 마지막 룸이면 클리어. 데이터의 Agent Type이 구워 둔 NavMesh 타입과 다르면 웨이브 시작 때 다시 굽는다
- **`Monster`**: `Init`에서 스케일 · 색 · NavMeshAgent 값을 데이터로 채운 뒤 Agent를 켠다(프리팹은 Agent가 꺼진 상태 — 타입을 바꾼 뒤 켜야 그 룸 NavMesh에 올라감). `Update`: `SetDestination(플레이어)`, 사거리 안이면 공격 간격마다 피해. 모든 룸이 이 코드 하나를 쓴다
- **`PlayerController`**: `CharacterController` 이동 + `Physics.OverlapSphere` 근접 공격
- **`GameManager`**: Playing / Clear / GameOver, `OnGUI` 표시, R → 씬 다시 불러오기

**명세에 없거나 해석해서 정한 것**

| 항목 | 정한 내용 |
|---|---|
| 플레이어가 적을 처치하는 방법 | 정의 없음 → **Space 근접 공격** (사용자 결정). 피해 1, 0.3초 간격, 앞쪽 범위 안 몬스터 모두 |
| 플레이어 HP · 사망 | 몬스터 "공격력"만 있음 → **HP 100, 0이면 게임오버, R 재시작** (사용자 결정) |
| 플레이어 이동 방식 | 정의 없음 → **WASD + CharacterController** (사용자 결정). 룸마다 NavMesh가 따로라 NavMeshAgent로는 룸 사이를 못 건넌다 |
| 적이 나오는 시점 | 정의 없음 → **룸에 들어갈 때** (사용자 결정). 1번 룸은 시작 위치가 룸 안이라 바로 시작 |
| "Agent 세팅" | NavMeshAgent 컴포넌트 값 + **룸마다 다른 Agent Type**(Small / Medium / Large, 반지름 · 높이 = 몬스터 크기)로 봤다. 룸 NavMesh도 그 타입으로 굽는다 |
| 난이도 수치 | "증가"만 있음 → 위 데이터 표처럼 정함 |
| 데이터 형식 | ScriptableObject 에셋 3개 |
| 몬스터 AI | "패턴은 동일"만 있음 → 추적 + 사거리 안 근접 공격 |
| 스폰 위치 · 스폰 수 > 지점 수 | 룸마다 스폰 지점(빈 오브젝트). 지점보다 많으면 지점을 다시 쓰되 2m씩 둘레로 벌림 (안 벌리면 한 점에 겹쳐 순간 속도가 튀었다) |
| 문 | 룸 사이 복도에 하나씩. 클리어하면 사라짐, 지나간 뒤 다시 닫지 않음 |
| 클리어 이후 | "CLEAR!" 표시 + R 재시작 |
