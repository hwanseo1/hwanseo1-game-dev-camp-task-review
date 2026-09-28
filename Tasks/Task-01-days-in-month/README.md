# Task-01 일수 출력기

월 번호를 입력하면 그 달의 일수를 출력하는 콘솔 프로그램.

## 동작

- `1`~`12` 입력 → `N월은 XX일까지 있습니다.` 출력 후 재입력
- 범위 밖 숫자 / 숫자가 아닌 입력 → 안내 문구만 출력하고 **입력 횟수에 넣지 않음**
- 유효한 입력 **3번**을 채우면 종료
- 2월은 28일 고정 (윤년 계산 없음)

## 구현

`Program.cs` — 월별 일수는 `switch`의 case 묶음으로 처리했다. 31일인 달 7개와 30일인 달 4개를 각각 한 덩어리로 묶으면 `if` 체인보다 중복 없이 짧고, 분기 대상이 정수 하나뿐이라 `switch`가 자연스럽다. 1~12가 아니면 `0`을 반환해 호출부에서 예외처리한다.

## 빌드 · 실행

.NET SDK 없이 Visual Studio에 포함된 Roslyn 컴파일러로 빌드했다.

```powershell
& "C:\Program Files\Microsoft Visual Studio\2022\Community\MSBuild\Current\Bin\Roslyn\csc.exe" `
  -nologo -out:days.exe Program.cs
.\days.exe
```

## 확인한 것

| 입력 | 결과 |
|---|---|
| `1,3,5,7,8,10,12` | 31일 |
| `4,6,9,11` | 30일 |
| `2` | 28일 |
| `0`, `13`, `-5` | 존재하지 않는 월 안내, 카운트 증가 없음 |
| `abc` | 숫자 아님 안내, 카운트 증가 없음 |

1~12월 합계가 365일로 맞는 것까지 확인했다. 실행 로그: `screenshots/run-01-validation.txt`
