# Bring back macOS Screenshot Shortcut to Bootcamp

![Screenshot](https://raw.githubusercontent.com/thy2134/BringBackMacScreenshot/master/images/Screen%20Shot%202018-07-20%2002.15.08%20AM.png)

macOS의 스크린샷 단축키(Cmd + Shift + 3/4)와 비슷한 단축키를 프린트스크린 키에 바인딩해주는 프로그램입니다. 프린트스크린 키가 없는 맥북의 부트캠프 유저들을 위해 제작했습니다.

## 요구사항
- Windows 10
- .NET 프레임워크 4.6.1

## 사용법
'Start remapping' 버튼을 클릭하면 시작됩니다.   
'Stop remapping' 을 누르면 중단됩니다.

## 설정
- 'Save to Clipboard' 옵션
    - `Ctrl + Shift + 3`: 전체 화면을 캡쳐 후 클립보드에 복사합니다
    - `Ctrl + Alt + Shift + 3`: 전체 화면을 캡쳐, 클립보드에 복사 후 지정된 경로에 PNG 파일로 저장합니다
    - `Ctrl + Shift + 4`: 현재 클릭된 창을 캡쳐 후 클립보드에 복사합니다
    - `Ctrl + Alt + Shift + 4`: 현재 클릭된 창을 캡쳐, 클립보드에 복사 후 지정된 경로에 PNG 파일로 저장합니다
- 'Save to Desktop' 옵션
    - `Ctrl + Shift + 3`: 전체 화면을 캡쳐, 클립보드에 복사 후 지정된 경로에 PNG 파일로 저장합니다
    - `Ctrl + Alt + Shift + 3`: 전체 화면을 캡쳐 후 클립보드에 복사합니다
    - `Ctrl + Shift + 4`: 현재 클릭된 창을 캡쳐, 클립보드에 복사 후 지정된 경로에 PNG 파일로 저장합니다
    - `Ctrl + Alt + Shift + 4`: 현재 클릭된 창을 캡쳐 후 클립보드에 복사합니다
- Start Remapping when program starts: 체크 시, 프로그램 실행과 동시에 서비스가 시작됩니다.
- Screenshot Location: 스크린샷 PNG 파일이 저장될 경로를 지정합니다.

## 설치 
### 간단한 방법
- [Release tab](https://github.com/thy2134/BringBackMacScreenshot/releases) 에서 파일을 다운로드 후 실행하세요.
### Hard way(compile and run)
0. 요구사항 
- Visual Studio 2017
- Windows 10 SDK
1. `git clone https://github.com/thy2134/BringBackMacScreenshot.git` 으로 레포지토리를 클론하세요.
2. `BootCampScreenshotRemapper.sln`을 열고,
3. 컴파일 후 실행합니다.

