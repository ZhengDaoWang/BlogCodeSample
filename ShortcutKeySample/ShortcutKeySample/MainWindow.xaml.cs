using System.Runtime.InteropServices;
using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;
using System.ComponentModel;
using System.Diagnostics;

namespace ShortcutKeySample
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        /// <summary>
        /// CTRL+5事件Id
        /// </summary>
        private const int Ctrl5KeyEventId = 9000;

        public MainWindow()
        {
            InitializeComponent();
            KeyDown += MainWindow_KeyDown;
        }

        private void MainWindow_KeyDown(object sender, KeyEventArgs e)
        {
            if (Keyboard.Modifiers == ModifierKeys.Control && (e.Key == Key.D5 || e.Key == Key.NumPad5))
            {
                Debug.WriteLine("WPF的KeyDown事件监听CTRL+5成功"); ;
            }
        }

        [DllImport("user32.dll")]
        public static extern bool RegisterHotKey(IntPtr hWnd, int id, uint fsModifiers, uint vk);

        [DllImport("user32.dll")]
        public static extern bool UnregisterHotKey(IntPtr hWnd, int id);

        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);

            var handle = new WindowInteropHelper(this).Handle;
            var source = HwndSource.FromHwnd(handle);
            source?.AddHook(HwndHook);
            //真正注册快捷键监听处理: 同时注册数字键和小键盘的CTRL+5
            RegisterHotKey(handle, Ctrl5KeyEventId, (uint)ModifierKeys.Control, (uint)KeyInterop.VirtualKeyFromKey(Key.D5));
            RegisterHotKey(handle, Ctrl5KeyEventId, (uint)ModifierKeys.Control, (uint)KeyInterop.VirtualKeyFromKey(Key.NumPad5));
        }

        private IntPtr HwndHook(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            const int wmHotkey = 0x0312;

            switch (msg)
            {
                case wmHotkey:
                    switch (wParam.ToInt32())
                    {
                        case Ctrl5KeyEventId:
                            Debug.WriteLine("Win32监听CTRL+5成功");
                            break;
                    }
                    break;
            }

            return IntPtr.Zero;
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            base.OnClosing(e);

            var handle = new WindowInteropHelper(this).Handle;
            //关闭窗口后取消注册
            UnregisterHotKey(handle, Ctrl5KeyEventId);
        }

        private void Ctrl5Command_OnExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            Debug.WriteLine("WPF的XAML绑定命令监听CTRL+5成功");
        }
    }

    public static class Commands
    {
        public static ICommand Ctrl5Command { get; } = new RoutedCommand();
    }
}
