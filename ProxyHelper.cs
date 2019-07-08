// Decompiled with JetBrains decompiler
// Type: xNet.ProxyHelper
// Assembly: xNet, Version=3.3.3.0, Culture=neutral, PublicKeyToken=null
// MVID: 8FAB7F03-1085-4650-8C57-7A04F40293E8
// Assembly location: C:\Users\Henris\Desktop\Smart Pastebin\xNet.dll

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
