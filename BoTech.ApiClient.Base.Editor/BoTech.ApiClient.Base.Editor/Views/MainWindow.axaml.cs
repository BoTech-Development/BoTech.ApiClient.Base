

using Avalonia.Platform;
using System;
using System.Runtime.InteropServices;

namespace BoTech.ApiClient.Base.Editor.Views;

public partial class MainWindow : ShadUI.Window
{
    const int DWMWA_WINDOW_CORNER_PREFERENCE = 33;
    [DllImport("dwmapi.dll", PreserveSig = true)]
    static extern int DwmSetWindowAttribute(IntPtr hwnd, int attr, ref int attrValue, int attrSize);

    public MainWindow()
    {
        InitializeComponent();
    }
    /// <summary>
    /// This method applies the rounded corners theme to the main window, when the platform is win11
    /// </summary>
    /// <param name="e"></param>
    protected override void OnOpened(EventArgs e)
    {
        base.OnOpened(e);

        if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            return;

        IntPtr hwnd = IntPtr.Zero;

        // 1) Direkter Cast auf IPlatformHandle (funktioniert bei expliziter Implementierung)
        if (this.PlatformImpl is IPlatformHandle pHandle)
        {
            hwnd = pHandle.Handle;
        }

        // 2) Fallback: Reflection (wenn Handle nicht öffentlich ist)
        if (hwnd == IntPtr.Zero && this.PlatformImpl != null)
        {
            var pi = this.PlatformImpl.GetType().GetProperty("Handle",
                System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic);
            if (pi != null)
            {
                var val = pi.GetValue(this.PlatformImpl);
                if (val is IntPtr ip) hwnd = ip;
                else
                {
                    // evtl. Wrapper-Typ: handle.Handle oder ähnliches
                    var innerPi = val?.GetType().GetProperty("Handle",
                        System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic);
                    if (innerPi != null)
                    {
                        var innerVal = innerPi.GetValue(val);
                        if (innerVal is IntPtr ip2) hwnd = ip2;
                    }
                }
            }
        }

        if (hwnd != IntPtr.Zero)
        {
            int preference = 2; // 0=Default, 1=DoNotRound, 2=Round, 3=RoundSmall
            DwmSetWindowAttribute(hwnd, DWMWA_WINDOW_CORNER_PREFERENCE, ref preference, Marshal.SizeOf<int>());
        }
    }
}