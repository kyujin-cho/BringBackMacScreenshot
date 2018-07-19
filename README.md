# Bring back macOS Screenshot Shortcut to Bootcamp

![Screenshot](https://raw.githubusercontent.com/thy2134/BringBackMacScreenshot/master/images/Screen%20Shot%202018-07-20%2002.15.08%20AM.png)

This program binds macOS-like key combination to PrtScn key, which makes Windows users which are running Windows via macOS' BootCamp system easier to capture the screen.   
(한국어 설명은 [이곳](https://github.com/thy2134/BringBackMacScreenshot/blob/master/README.korean.md) 에서 읽을 수 있습니다.)
## Minimum requirements
- Windows 10
- .NET FW 4.6.1

## Start
Click 'Start remapping' button to start service.   
Click 'Stop remapping' when needed.

## Settings
- 'Save to Clipboard' option
    - `Ctrl + Shift + 3`: captures full screen and copies it to clipboard
    - `Ctrl + Alt + Shift + 3`: captures full screen, copies it to clipboard and save screenshot image to given path(changable)
    - `Ctrl + Shift + 4`: captures current focused window and copies it to clipboard
    - `Ctrl + Alt + Shift + 4`: captures current focused window and copies it to clipboard and save screenshot image to given path
- 'Save to Desktop' option
    - `Ctrl + Shift + 3`: captures full screen, copies it to clipboard and save screenshot image to given path(changable)
    - `Ctrl + Alt + Shift + 3`: captures full screen and copies it to clipboard
    - `Ctrl + Shift + 4`: captures current focused window and copies it to clipboard and save screenshot image to given path
    - `Ctrl + Alt + Shift + 4`: captures current focused window and copies it to clipboard
- Start Remapping when program starts: When checked, program automatically starts remapping service when program starts.
- Screenshot Location: Change screenshot save location. Default location is your Desktop folder.

## Install 
### Easy way
- Download self executable program from [Release tab](https://github.com/thy2134/BringBackMacScreenshot/releases).
### Hard way(compile and run)
0. Requirements
- Visual Studio 2017
- Windows 10 SDK
1. clone this repository via `git clone https://github.com/thy2134/BringBackMacScreenshot.git` .
2. Open `BootCampScreenshotRemapper.sln`.
3. Compile.
4. Done!

