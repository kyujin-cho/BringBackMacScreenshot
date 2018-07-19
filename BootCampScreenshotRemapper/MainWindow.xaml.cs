using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Diagnostics;
using System.Windows.Interop;
using System.IO;
using Microsoft.Win32;

namespace BootCampScreenshotRemapper {
  /// <summary>
  /// Interaction logic for MainWindow.xaml
  /// </summary>
  public partial class MainWindow : Window {
    [DllImport("user32.dll")]
    public static extern bool RegisterHotKey(IntPtr hWnd, int id, uint fsModifiers, uint vk);

    [DllImport("user32.dll")]
    public static extern bool UnregisterHotKey(IntPtr hWnd, int id);

    [DllImport("user32.dll")]
    static extern IntPtr GetForegroundWindow();

    [DllImport("user32.dll")]
    static extern int GetWindowText(IntPtr hWnd, StringBuilder text, int count);

    [DllImport("user32.dll")]
    public static extern void keybd_event(byte bVk, byte bScan, uint dwFlags, uint dwExtraInfo);

    const int HOTKEY_ID_1 = 31197; //Any number to use to identify the hotkey instance
    const int HOTKEY_ID_2 = 31198; //Any number to use to identify the hotkey instance
    const int HOTKEY_ID_3 = 31199; //Any number to use to identify the hotkey instance
    const int HOTKEY_ID_4 = 31200; //Any number to use to identify the hotkey instance
    const int WM_HOTKEY = 0x0312;
    //Modifiers:
    private const uint MOD_ALT = 0x0001;
    private const uint MOD_CTRL = 0x0002;
    private const uint MOD_SHIFT = 0x0004;
    private const uint MOD_WIN = 0x0008;

    private const uint FLAG_KEYDOWN = 0x0000;
    private const uint FLAG_KEYUP = 0x0002;

    private const byte VK_NUM3 = 0x33;
    private const byte VK_NUM4 = 0x34;
    private const byte VK_SNAPSHOT = 0x002C;
    private const byte VK_ALT = 0x0012;

    private const int CAPTURE_SCREEN = 0;
    private const int CAPTURE_WINDOW = 1;
    private const int CLIPBOARD_IMAGE_TO_FILE = 2;

    enum ServiceType { SaveToClipboard, SaveToDesktop };
    DoWorkEventHandler[] handlers = new DoWorkEventHandler[2];
    RunWorkerCompletedEventHandler SaveClipboardToDesktop;

    private IntPtr _windowHandle;
    private HwndSource _source;
    RegistryKey rk;

    ServiceType type = ServiceType.SaveToClipboard;
    bool startOnStart = false;
    bool changedByReg = false;
    int regChangeCount = 0;
    string ScreenshotLocation;

    public MainWindow() {
      InitializeComponent();
      uint info = 0;
      rk = Registry.CurrentUser.OpenSubKey("Software\\BootCampScreenShotRemapper", true);
      ScreenshotLocation = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
      if (rk == null) {
        rk = Registry.CurrentUser.CreateSubKey("Software\\BootCampScreenShotRemapper");
        rk.SetValue("ServiceType", "SaveToClipboard");
        rk.SetValue("StartOnStart", "false");
        rk.SetValue("ScreenshotLocation", Environment.GetFolderPath(Environment.SpecialFolder.Desktop));
      } else {
        var svcType = rk.GetValue("ServiceType") as string;
        var sons = rk.GetValue("StartOnStart") as string;
        ScreenshotLocation = rk.GetValue("ScreenshotLocation") as string;
        changedByReg = true;
        if (svcType == "SaveToDesktop") {
          type = ServiceType.SaveToDesktop;
          DesktopRadio.IsChecked = true;
          regChangeCount++;
        }
        startOnStart = sons == "true";
        if(startOnStart) {
          StartOnStart.IsChecked = true;
          regChangeCount++;
        }
      }

      SaveLocation.Text = ScreenshotLocation;
      handlers[CAPTURE_SCREEN] = new DoWorkEventHandler((object sender, DoWorkEventArgs e) => {
        keybd_event(VK_SNAPSHOT, 0, FLAG_KEYDOWN, info);
        System.Threading.Thread.Sleep(10);
        keybd_event(VK_SNAPSHOT, 0, FLAG_KEYUP, info);
        System.Threading.Thread.Sleep(50);
      });
      handlers[CAPTURE_WINDOW] = new DoWorkEventHandler((object sender, DoWorkEventArgs e) => {
        keybd_event(VK_ALT, 0, FLAG_KEYDOWN, info);
        System.Threading.Thread.Sleep(10);
        keybd_event(VK_SNAPSHOT, 0, FLAG_KEYDOWN, info);
        System.Threading.Thread.Sleep(10);
        keybd_event(VK_SNAPSHOT, 0, FLAG_KEYUP, info);
        System.Threading.Thread.Sleep(10);
        keybd_event(VK_ALT, 0, FLAG_KEYUP, info);
      });
      SaveClipboardToDesktop = new RunWorkerCompletedEventHandler((object sender, RunWorkerCompletedEventArgs e) => {
        try {
          var currentTime = DateTime.Now.ToString("yyyy-MM-dd hh.mm.ss tt");
          BitmapSource bmpSrc = Clipboard.GetImage();
          string filePath = System.IO.Path.Combine(ScreenshotLocation, "Screen Shot " + currentTime + ".png");
          using (var fileStream = new FileStream(filePath, FileMode.Create)) {
            BitmapEncoder encoder = new PngBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(bmpSrc));
            encoder.Save(fileStream);
          }
        } catch (COMException error) {
          Console.WriteLine(error);
        }
      });
    }
    
    protected override void OnSourceInitialized(EventArgs e) {
      base.OnSourceInitialized(e);
      _windowHandle = new WindowInteropHelper(this).Handle;
      _source = HwndSource.FromHwnd(_windowHandle);
      _source.AddHook(HwndHook);
      Console.WriteLine("Source initialized");
      if (startOnStart) {
        StartService();
      }
    }
  
    private IntPtr HwndHook(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled) {
      const int WM_HOTKEY = 0x0312;
      BackgroundWorker keyWorker;
      ServiceType cType;
      if (msg == WM_HOTKEY) {
        Console.WriteLine("Hook called");
        switch (wParam.ToInt32()) {
          case HOTKEY_ID_1:
          case HOTKEY_ID_3:
            cType = wParam.ToInt32() == HOTKEY_ID_1 ? ServiceType.SaveToDesktop : ServiceType.SaveToClipboard;
            keyWorker = new BackgroundWorker();
            if(type == cType) 
              keyWorker.RunWorkerCompleted += SaveClipboardToDesktop;
            
            keyWorker.DoWork += handlers[CAPTURE_SCREEN];
            keyWorker.RunWorkerAsync();
            
            handled = true;
            break;
          case HOTKEY_ID_2:
          case HOTKEY_ID_4:
            cType = wParam.ToInt32() == HOTKEY_ID_2 ? ServiceType.SaveToDesktop : ServiceType.SaveToClipboard;
            keyWorker = new BackgroundWorker();
            if (type == cType)
              keyWorker.RunWorkerCompleted += SaveClipboardToDesktop;

            keyWorker.DoWork += handlers[CAPTURE_WINDOW];
            keyWorker.RunWorkerAsync();

            handled = true;
            break;
        }
      }
      return IntPtr.Zero;
    }
    
    void StartService() {
      RegisterHotKey(_windowHandle, HOTKEY_ID_1, MOD_CTRL | MOD_SHIFT, VK_NUM3);
      RegisterHotKey(_windowHandle, HOTKEY_ID_2, MOD_CTRL | MOD_SHIFT, VK_NUM4);
      RegisterHotKey(_windowHandle, HOTKEY_ID_3, MOD_CTRL | MOD_ALT | MOD_SHIFT, VK_NUM3);
      RegisterHotKey(_windowHandle, HOTKEY_ID_4, MOD_CTRL | MOD_ALT | MOD_SHIFT, VK_NUM4);
      Console.WriteLine("Service started!");
      StartBtn.IsEnabled = false;
      StopBtn.IsEnabled = true;
      ClipboardRadio.IsEnabled = false;
      DesktopRadio.IsEnabled = false;
      StartOnStart.IsEnabled = false;
    }

    private void StartBtn_Click(object sender, RoutedEventArgs e) {
      StartService();
    }

    private void StopBtn_Click(object sender, RoutedEventArgs e) {
      UnregisterHotKey(_windowHandle, HOTKEY_ID_1);
      UnregisterHotKey(_windowHandle, HOTKEY_ID_2);
      Console.WriteLine("Service stopped!");
      StartBtn.IsEnabled = true;
      StopBtn.IsEnabled = false;
      ClipboardRadio.IsEnabled = true;
      DesktopRadio.IsEnabled = true;
      StartOnStart.IsEnabled = true;
    }

    private void GotoGitBtn_Click(object sender, RoutedEventArgs e) {
      Process.Start("https://github.com/thy2134");
    }

    private void ChangeSaveLocationBtn_Click(object sender, RoutedEventArgs e) {
      using (var dialog = new System.Windows.Forms.FolderBrowserDialog()) {
        dialog.SelectedPath = ScreenshotLocation;
        System.Windows.Forms.DialogResult result = dialog.ShowDialog();
        ScreenshotLocation = dialog.SelectedPath;
        SaveLocation.Text = ScreenshotLocation;
        if (rk != null) rk.SetValue("ScreenshotLocation", dialog.SelectedPath);
      }
    }

    private void OnCheckUnchecked(object sender, RoutedEventArgs e) {
      if (changedByReg && regChangeCount > 0) {
        regChangeCount--;
        return;
      } else if (changedByReg) {
        changedByReg = false;
      }
      Console.WriteLine("FOO");
      if(rk != null) {
        switch ((sender as Control).Name) {
          case "ClipboardRadio":
            rk.SetValue("ServiceType", "SaveToClipboard");
            type = ServiceType.SaveToClipboard;
            break;
          case "DesktopRadio":
            rk.SetValue("ServiceType", "SaveToDesktop");
            type = ServiceType.SaveToDesktop;
            break;
          case "StartOnStart":
            rk.SetValue("StartOnStart", (e.RoutedEvent.Name == "Checked").ToString().ToLower());
            break;
        }
      }
    }
  }
}
