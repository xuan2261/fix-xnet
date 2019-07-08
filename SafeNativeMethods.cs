﻿// Decompiled with JetBrains decompiler
// Type: xNet.SafeNativeMethods
// Assembly: xNet, Version=3.3.3.0, Culture=neutral, PublicKeyToken=null
// MVID: 8FAB7F03-1085-4650-8C57-7A04F40293E8
// Assembly location: C:\Users\Henris\Desktop\Smart Pastebin\xNet.dll

using System;
using System.Runtime.InteropServices;
using System.Security;

namespace xNet
{
  [SuppressUnmanagedCodeSecurity]
  internal static class SafeNativeMethods
  {
    [DllImport("wininet.dll", CharSet = CharSet.Auto)]
    internal static extern bool InternetGetConnectedState(
      ref SafeNativeMethods.InternetConnectionState lpdwFlags,
      int dwReserved);

    [Flags]
    internal enum InternetConnectionState
    {
      INTERNET_CONNECTION_MODEM = 1,
      INTERNET_CONNECTION_LAN = 2,
      INTERNET_CONNECTION_PROXY = 4,
      INTERNET_RAS_INSTALLED = 16, // 0x00000010
      INTERNET_CONNECTION_OFFLINE = 32, // 0x00000020
      INTERNET_CONNECTION_CONFIGURED = 64, // 0x00000040
    }
  }
}
