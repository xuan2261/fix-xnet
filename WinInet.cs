// Decompiled with JetBrains decompiler
// Type: xNet.WinInet
// Assembly: xNet, Version=3.3.3.0, Culture=neutral, PublicKeyToken=null
// MVID: 8FAB7F03-1085-4650-8C57-7A04F40293E8
// Assembly location: C:\Users\Henris\Desktop\Smart Pastebin\xNet.dll

using Microsoft.Win32;
using System;
using System.IO;
using System.Security;

namespace xNet
{
  public static class WinInet
  {
    private const string PATH_TO_INTERNET_OPTIONS = "Software\\Microsoft\\Windows\\CurrentVersion\\Internet Settings";

    public static bool InternetConnected
    {
      get
      {
        SafeNativeMethods.InternetConnectionState lpdwFlags = (SafeNativeMethods.InternetConnectionState) 0;
        return SafeNativeMethods.InternetGetConnectedState(ref lpdwFlags, 0);
      }
    }

    public static bool InternetThroughModem
    {
      get
      {
        return WinInet.EqualConnectedState(SafeNativeMethods.InternetConnectionState.INTERNET_CONNECTION_MODEM);
      }
    }

    public static bool InternetThroughLan
    {
      get
      {
        return WinInet.EqualConnectedState(SafeNativeMethods.InternetConnectionState.INTERNET_CONNECTION_LAN);
      }
    }

    public static bool InternetThroughProxy
    {
      get
      {
        return WinInet.EqualConnectedState(SafeNativeMethods.InternetConnectionState.INTERNET_CONNECTION_PROXY);
      }
    }

    public static bool IEProxyEnable
    {
      get
      {
        try
        {
          return WinInet.GetIEProxyEnable();
        }
        catch (IOException ex)
        {
          return false;
        }
        catch (SecurityException ex)
        {
          return false;
        }
        catch (ObjectDisposedException ex)
        {
          return false;
        }
        catch (UnauthorizedAccessException ex)
        {
          return false;
        }
      }
      set
      {
        try
        {
          WinInet.SetIEProxyEnable(value);
        }
        catch (IOException ex)
        {
        }
        catch (SecurityException ex)
        {
        }
        catch (ObjectDisposedException ex)
        {
        }
        catch (UnauthorizedAccessException ex)
        {
        }
      }
    }

    public static HttpProxyClient IEProxy
    {
      get
      {
        string ieProxy;
        try
        {
          ieProxy = WinInet.GetIEProxy();
        }
        catch (IOException ex)
        {
          return (HttpProxyClient) null;
        }
        catch (SecurityException ex)
        {
          return (HttpProxyClient) null;
        }
        catch (ObjectDisposedException ex)
        {
          return (HttpProxyClient) null;
        }
        catch (UnauthorizedAccessException ex)
        {
          return (HttpProxyClient) null;
        }
        HttpProxyClient result;
        HttpProxyClient.TryParse(ieProxy, out result);
        return result;
      }
      set
      {
        try
        {
          if (value != null)
            WinInet.SetIEProxy(value.ToString());
          else
            WinInet.SetIEProxy(string.Empty);
        }
        catch (SecurityException ex)
        {
        }
        catch (ObjectDisposedException ex)
        {
        }
        catch (UnauthorizedAccessException ex)
        {
        }
      }
    }

    public static bool GetIEProxyEnable()
    {
      using (RegistryKey registryKey = Registry.CurrentUser.OpenSubKey("Software\\Microsoft\\Windows\\CurrentVersion\\Internet Settings"))
      {
        object obj = registryKey.GetValue("ProxyEnable");
        if (obj == null)
          return false;
        return (int) obj != 0;
      }
    }

    public static void SetIEProxyEnable(bool enabled)
    {
      using (RegistryKey subKey = Registry.CurrentUser.CreateSubKey("Software\\Microsoft\\Windows\\CurrentVersion\\Internet Settings"))
        subKey.SetValue("ProxyEnable", (object) (enabled ? 1 : 0));
    }

    public static string GetIEProxy()
    {
      using (RegistryKey registryKey = Registry.CurrentUser.OpenSubKey("Software\\Microsoft\\Windows\\CurrentVersion\\Internet Settings"))
        return registryKey.GetValue("ProxyServer") as string ?? string.Empty;
    }

    public static void SetIEProxy(string host, int port)
    {
      if (host == null)
        throw new ArgumentNullException(nameof (host));
      if (host.Length == 0)
        throw ExceptionHelper.EmptyString(nameof (host));
      if (!ExceptionHelper.ValidateTcpPort(port))
        throw ExceptionHelper.WrongTcpPort(nameof (port));
      WinInet.SetIEProxy(host + ":" + port.ToString());
    }

    public static void SetIEProxy(string hostAndPort)
    {
      using (RegistryKey subKey = Registry.CurrentUser.CreateSubKey("Software\\Microsoft\\Windows\\CurrentVersion\\Internet Settings"))
        subKey.SetValue("ProxyServer", (object) (hostAndPort ?? string.Empty));
    }

    private static bool EqualConnectedState(SafeNativeMethods.InternetConnectionState expected)
    {
      SafeNativeMethods.InternetConnectionState lpdwFlags = (SafeNativeMethods.InternetConnectionState) 0;
      SafeNativeMethods.InternetGetConnectedState(ref lpdwFlags, 0);
      return (uint) (lpdwFlags & expected) > 0U;
    }
  }
}
