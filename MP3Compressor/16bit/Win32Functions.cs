

using System;
using System.Runtime.InteropServices;

namespace Yeti.Sys
{
  public enum BeepType
  {
    SimpleBeep = -1,
    SystemAsterisk = 0x00000040,
    SystemExclamation = 0x00000030,
    SystemHand = 0x00000010,
    SystemQuestion = 0x00000020,
    SystemDefault = 0
  }
  /// <summary>
	/// Win32 API functions
	/// </summary>
  public sealed class Win32
  {
    [DllImport("User32.dll", SetLastError=true)]
    public static extern bool MessageBeep(BeepType Type);
	}
}
