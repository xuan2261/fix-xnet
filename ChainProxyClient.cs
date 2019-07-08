// Decompiled with JetBrains decompiler
// Type: xNet.ChainProxyClient
// Assembly: xNet, Version=3.3.3.0, Culture=neutral, PublicKeyToken=null
// MVID: 8FAB7F03-1085-4650-8C57-7A04F40293E8
// Assembly location: C:\Users\Henris\Desktop\Smart Pastebin\xNet.dll

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;

namespace xNet
{
  public class ChainProxyClient : ProxyClient
  {
    private readonly List<ProxyClient> proxies = new List<ProxyClient>();
    [ThreadStatic]
    private static Random rand;

    private static Random Rand
    {
      get
      {
        if (ChainProxyClient.rand == null)
          ChainProxyClient.rand = new Random();
        return ChainProxyClient.rand;
      }
    }

    public bool EnableShuffle { get; set; }

    public List<ProxyClient> Proxies
    {
      get
      {
        return this.proxies;
      }
    }

    public override string Host
    {
      get
      {
        throw new NotSupportedException();
      }
      set
      {
        throw new NotSupportedException();
      }
    }

    public override int Port
    {
      get
      {
        throw new NotSupportedException();
      }
      set
      {
        throw new NotSupportedException();
      }
    }

    public override string Username
    {
      get
      {
        throw new NotSupportedException();
      }
      set
      {
        throw new NotSupportedException();
      }
    }

    public override string Password
    {
      get
      {
        throw new NotSupportedException();
      }
      set
      {
        throw new NotSupportedException();
      }
    }

    public override int ConnectTimeout
    {
      get
      {
        throw new NotSupportedException();
      }
      set
      {
        throw new NotSupportedException();
      }
    }

    public override int ReadWriteTimeout
    {
      get
      {
        throw new NotSupportedException();
      }
      set
      {
        throw new NotSupportedException();
      }
    }

    public ChainProxyClient(bool enableShuffle = false)
      : base(ProxyType.Chain)
    {
      this.EnableShuffle = enableShuffle;
    }

    public override TcpClient CreateConnection(
      string destinationHost,
      int destinationPort,
      TcpClient tcpClient = null)
    {
      if (this.proxies.Count == 0)
        throw new InvalidOperationException(Resources.InvalidOperationException_ChainProxyClient_NotProxies);
      List<ProxyClient> proxyClientList;
      if (this.EnableShuffle)
      {
        proxyClientList = this.proxies.ToList<ProxyClient>();
        for (int index1 = 0; index1 < proxyClientList.Count; ++index1)
        {
          int index2 = ChainProxyClient.Rand.Next(proxyClientList.Count);
          ProxyClient proxyClient = proxyClientList[index1];
          proxyClientList[index1] = proxyClientList[index2];
          proxyClientList[index2] = proxyClient;
        }
      }
      else
        proxyClientList = this.proxies;
      int index3 = proxyClientList.Count - 1;
      TcpClient tcpClient1 = tcpClient;
      for (int index1 = 0; index1 < index3; ++index1)
        tcpClient1 = proxyClientList[index1].CreateConnection(proxyClientList[index1 + 1].Host, proxyClientList[index1 + 1].Port, tcpClient1);
      return proxyClientList[index3].CreateConnection(destinationHost, destinationPort, tcpClient1);
    }

    public override string ToString()
    {
      StringBuilder stringBuilder = new StringBuilder();
      foreach (ProxyClient proxy in this.proxies)
        stringBuilder.AppendLine(proxy.ToString());
      return stringBuilder.ToString();
    }

    public new virtual string ToExtendedString()
    {
      StringBuilder stringBuilder = new StringBuilder();
      foreach (ProxyClient proxy in this.proxies)
        stringBuilder.AppendLine(proxy.ToExtendedString());
      return stringBuilder.ToString();
    }

    public void AddProxy(ProxyClient proxy)
    {
      if (proxy == null)
        throw new ArgumentNullException(nameof (proxy));
      this.proxies.Add(proxy);
    }

    public void AddHttpProxy(string proxyAddress)
    {
      this.proxies.Add((ProxyClient) HttpProxyClient.Parse(proxyAddress));
    }

    public void AddSocks4Proxy(string proxyAddress)
    {
      this.proxies.Add((ProxyClient) Socks4ProxyClient.Parse(proxyAddress));
    }

    public void AddSocks4aProxy(string proxyAddress)
    {
      this.proxies.Add((ProxyClient) Socks4aProxyClient.Parse(proxyAddress));
    }

    public void AddSocks5Proxy(string proxyAddress)
    {
      this.proxies.Add((ProxyClient) Socks5ProxyClient.Parse(proxyAddress));
    }
  }
}
