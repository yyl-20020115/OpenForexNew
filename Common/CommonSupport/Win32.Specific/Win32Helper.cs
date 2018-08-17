using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

namespace CommonSupport
{
#if WINDOWS
    public class Win32Helper
    {
        [DllImport("user32.dll")]
        public static extern int SendMessage(
              IntPtr hWnd,      // handle to destination window
              int Msg,       // message
              IntPtr wParam,  // first message parameter
              IntPtr lParam   // second message parameter
              );
    }
#endif
}
