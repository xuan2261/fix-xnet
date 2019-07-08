// Decompiled with JetBrains decompiler
// Type: xNet.Socks4aProxyClient
// Assembly: xNet, Version=3.3.3.0, Culture=neutral, PublicKeyToken=null
// MVID: 8FAB7F03-1085-4650-8C57-7A04F40293E8
// Assembly location: C:\Users\Henris\Desktop\Smart Pastebin\xNet.dll

using System;
using System.Net.Sockets;
using System.Text;

namespace xNet
{
  public class Socks4aProxyClient : Socks4ProxyClient
  {
    public Socks4aProxyClient()
      : this((string) null)
    {
    }

    public Socks4aProxyClient(string host)
      : this(host, 1080)
    {
    }

    public Socks4aProxyClient(string host, int port)
      : this(host, port, string.Empty)
    {
    }

    public Socks4aProxyClient(string host, int port, string username)
      : base(host, port, username)
    {
      this._type = ProxyType.Socks4a;
    }

    public static Socks4aProxyClient Parse(string proxyAddress)
    {
      return ProxyClient.Parse(ProxyType.Socks4a, proxyAddress) as Socks4aProxyClient;
    }

    public static bool TryParse(string proxyAddress, out Socks4aProxyClient result)
    {
      ProxyClient result1;
      if (ProxyClient.TryParse(ProxyType.Socks4a, proxyAddress, out result1))
      {
        result = result1 as Socks4aProxyClient;
        return true;
      }
      result = (Socks4aProxyClient) null;
      return false;
    }

    protected internal override void SendCommand(
      NetworkStream nStream,
      byte command,
      string destinationHost,
      int destinationPort)
    {
      byte[] portBytes = Socks4ProxyClient.GetPortBytes(destinationPort);
      byte[] numArray1 = new byte[4]
      {
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 1
      };
      byte[] numArray2 = string.IsNullOrEmpty(this._username) ? Array.Empty<byte>() : Encoding.ASCII.GetBytes(this._username);
      byte[] bytes = Encoding.ASCII.GetBytes(destinationHost);
      byte[] buffer1 = new byte[10 + numArray2.Length + bytes.Length];
      buffer1[0] = (byte) 4;
      buffer1[1] = command;
      portBytes.CopyTo((Array) buffer1, 2);
      numArray1.CopyTo((Array) buffer1, 4);
      numArray2.CopyTo((Array) buffer1, 8);
      buffer1[8 + numArray2.Length] = (byte) 0;
      bytes.CopyTo((Array) buffer1, 9 + numArray2.Length);
      buffer1[9 + numArray2.Length + bytes.Length] = (byte) 0;
      nStream.Write(buffer1, 0, buffer1.Length);
      byte[] buffer2 = new byte[8];
      nStream.Read(buffer2, 0, 8);
      byte command1 = buffer2[1];
      if (command1 == (byte) 90)
        return;
      this.HandleCommandError(command1);
    }
  }
}
