# game-dev-camp-task-review

Unity(C#) 학습용 Task를 직접 수행한 결과물 모음입니다.

## 구성

| 경로 | 내용 |
|---|---|
| `Tasks/Task-NN-<이름>/` | Task 하나당 폴더. Unity 프로젝트와 결과 화면 |
| `tools/` | 작업 보조 스크립트 |

## 환경

- Unity 6000.3.x / 6000.5.x
- Visual Studio 2022, C#

## 스크린샷 저장

`Win+Shift+S`로 영역을 복사한 뒤:

```powershell
.\tools\save-clip.ps1 Task-01-<이름> error-nullref
```

`Tasks/Task-01-<이름>/screenshots/01-error-nullref.png` 으로 저장됩니다 (번호 자동 증가).

## Unity 로그 확인

```powershell
# 최근 200줄
Get-Content "$env:LOCALAPPDATA\Unity\Editor\Editor.log" -Tail 200

# 컴파일 에러만 추려서 저장
Select-String -Path "$env:LOCALAPPDATA\Unity\Editor\Editor.log" -Pattern "error|Exception|CS[0-9]{4}" |
  Set-Content "Tasks\Task-01-<이름>\screenshots\editor-errors.txt" -Encoding utf8
```

- `Editor.log` — 현재 세션 / `Editor-prev.log` — 직전 세션

## 배치 모드 빌드 로그

에디터를 띄우지 않고 컴파일·빌드 에러만 확인할 때:

```powershell
& "C:\Program Files\Unity\Hub\Editor\<버전>\Editor\Unity.exe" `
  -batchmode -quit -projectPath "Tasks\Task-01-<이름>" `
  -logFile "Tasks\Task-01-<이름>\screenshots\build.log"
```
