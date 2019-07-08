// Decompiled with JetBrains decompiler
// Type: xNet.ProxyHelper
// Assembly: xNet, Version=3.3.3.0, Culture=neutral, PublicKeyToken=null
// MVID: BCFC550F-93AE-4DF9-8F50-A984FB298337
// Assembly location: C:\Users\Henris\Desktop\Smart Pastebin\xNet-0bfa2388b222842ad29fcffb3677177a38854ebd\bin\Release\fsdfsd.dll

using System;

namespace xNet
{
  internal static class ProxyHelper
  {
    public static ProxyClient CreateProxyClient(
      ProxyType proxyType,
      string host = null,
      int port = 0,
      string username = null,
      string password = null)
    {
      switch (proxyType)
      {
        case ProxyType.Http:
          if (port != 0)
            return (ProxyClient) new HttpProxyClient(host, port, username, password);
          return (ProxyClient) new HttpProxyClient(host);
        case ProxyType.Socks4:
          if (port != 0)
            return (ProxyClient) new Socks4ProxyClient(host, port, username);
          return (ProxyClient) new Socks4ProxyClient(host);
        case ProxyType.Socks4a:
          if (port != 0)
            return (ProxyClient) new Socks4aProxyClient(host, port, username);
          return (ProxyClient) new Socks4aProxyClient(host);
        case ProxyType.Socks5:
          if (port != 0)
            return (ProxyClient) new Socks5ProxyClient(host, port, username, password);
          return (ProxyClient) new Socks5ProxyClient(host);
        default:
          throw new InvalidOperationException();
      }
    }
  }
}
