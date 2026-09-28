# Task-02 가위바위보 (배팅 + 치트)

가위바위보로 소지금을 걸고 최대 5판을 진행하는 콘솔 프로그램.

## 동작

- 초기 소지금 **10,000원**, 최소 배팅 **1,000원**
- 판마다 배팅 금액 → 손(가위/바위/보) 입력
- 정산: **승리 = 판돈 x3 획득 / 무승부 = 판돈 x5 획득 / 패배 = 판돈 x7 손실**
- **5판을 마치거나 소지금이 0원이 되면 종료**
- `c` 입력으로 치트 토글 → 그 판 컴퓨터의 손을 미리 보여준다
- `q` 입력으로 중단

## 구현

`Program.cs`

- 기본 골격은 `switch` — 승패 정산, 손 입력 분기, 손 이름 변환 세 곳
- 손과 승패는 `enum Hand` / `enum Result`로 두어 정산 `switch`에서 케이스 누락이 눈에 보이게 함
- 배팅 검증: 최소 금액 미달 / 숫자 아님 / 소지금 초과 → 재입력
- 패배 손실이 소지금보다 클 때는 소지금만큼만 차감 (음수 방지)
- 실행 인자로 시드를 주면 같은 결과 재현 — `.\rps.exe 1`

## 빌드 · 실행

```powershell
& "C:\Program Files\Microsoft Visual Studio\2022\Community\MSBuild\Current\Bin\Roslyn\csc.exe" `
  -nologo -out:rps.exe Program.cs
.\rps.exe        # 시드 지정: .\rps.exe 1
```

## 확인한 것

| 시나리오 | 로그 |
|---|---|
| 5판 완주, 승·패·무 정산 | `screenshots/run-01-five-rounds.txt` |
| 전액 배팅 후 패배 → 소지금 0원 종료 | `screenshots/run-02-bankrupt.txt` |
| 배팅 3종 검증(미달/문자/초과), 손 입력 오류, 치트 ON | `screenshots/run-03-validation-cheat.txt` |
