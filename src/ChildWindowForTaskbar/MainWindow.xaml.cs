using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;

namespace ChildWindowForTaskbar
{
  public partial class MainWindow : Window
  {
    public MainWindow()
    {
      InitializeComponent();
    }

    protected override void OnSourceInitialized(EventArgs e)
    {
      base.OnSourceInitialized(e);

      var xamlWindow = (Windows.UI.Xaml.Window.Current as object) as IWindowPrivate;
      xamlWindow.TransparentBackground = true;

      var thisWindow = new WindowInteropHelper(this).EnsureHandle();
      var taskbarWindow = FindWindow("Shell_TrayWnd", null);
      var reBarWindow = FindWindowEx(taskbarWindow, IntPtr.Zero, "ReBarWindow32", null);

      SetWindowLong(thisWindow, -16, (GetWindowLong(thisWindow, -16) & ~WS_POPUP) | WS_CHILD);
      SetParent(thisWindow, taskbarWindow);

      var taskbarRect = new RECT();
      GetWindowRect(taskbarWindow, out taskbarRect);

      var reBarRect = new RECT();
      GetWindowRect(reBarWindow, out reBarRect);

      SetWindowPos(thisWindow,
                   IntPtr.Zero,
                   taskbarRect.Left,
                   reBarRect.Top - taskbarRect.Top,
                   taskbarRect.Right - taskbarRect.Left,
                   reBarRect.Bottom - reBarRect.Top, 
                   0);

      HostControl.Width = Height;
    }

    private void RegisterEvents(object sender, EventArgs e)
    {
      (HostControl.Child as UwpIsland.IslandControl).LauchApp += () =>
      {
        var processStartInfo = new ProcessStartInfo
        {
          FileName = "powershell",
          Arguments = $"start shell:AppsFolder\\Microsoft.WindowsCalculator_8wekyb3d8bbwe!App",
          UseShellExecute = false,
          CreateNoWindow = true,
        };

        Process.Start(processStartInfo);
      };
    }

    private const UInt32 WS_POPUP = 0x80000000;
    private const UInt32 WS_CHILD = 0x40000000;

    [DllImport("user32.dll", SetLastError = true)]
    static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

    [DllImport("user32.dll", SetLastError = true)]
    public static extern IntPtr FindWindowEx(IntPtr parentHandle, IntPtr hWndChildAfter, string className, string windowTitle);

    [DllImport("user32.dll", SetLastError = true)]
    static extern IntPtr SetParent(IntPtr hWndChild, IntPtr hWndNewParent);

    [DllImport("user32.dll", EntryPoint = "GetWindowLong")]
    static extern UInt32 GetWindowLong(IntPtr hWnd, int nIndex);

    [DllImport("user32.dll", EntryPoint = "SetWindowLongPtr")]
    static extern IntPtr SetWindowLong(IntPtr hWnd, int nIndex, UInt32 dwNewLong);

    [DllImport("user32.dll", SetLastError = true)]
    static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, UInt32 uFlags);

    [DllImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    static extern bool GetWindowRect(IntPtr hWnd, out RECT lpRect);

    [StructLayout(LayoutKind.Sequential)]
    public struct RECT
    {
      public int Left;
      public int Top;
      public int Right;
      public int Bottom;
    }
  }

  [ComImport]
  [Guid("06636C29-5A17-458D-8EA2-2422D997A922")]
  [InterfaceType(ComInterfaceType.InterfaceIsIInspectable)]
  public interface IWindowPrivate
  {
    bool TransparentBackground { get; set; }
  }
}