// Decompiled with JetBrains decompiler
// Type: xNet.Socks4ProxyClient
// Assembly: xNet, Version=3.3.3.0, Culture=neutral, PublicKeyToken=null
// MVID: 8FAB7F03-1085-4650-8C57-7A04F40293E8
// Assembly location: C:\Users\Henris\Desktop\Smart Pastebin\xNet.dll

using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace xNet
{
  public class Socks4ProxyClient : ProxyClient
  {
    protected internal const int DefaultPort = 1080;
    protected internal const byte VersionNumber = 4;
    protected internal const byte CommandConnect = 1;
    protected internal const byte CommandBind = 2;
    protected internal const byte CommandReplyRequestGranted = 90;
    protected internal const byte CommandReplyRequestRejectedOrFailed = 91;
    protected internal const byte CommandReplyRequestRejectedCannotConnectToIdentd = 92;
    protected internal const byte CommandReplyRequestRejectedDifferentIdentd = 93;

    public Socks4ProxyClient()
      : this((string) null)
    {
    }

    public Socks4ProxyClient(string host)
      : this(host, 1080)
    {
    }

    public Socks4ProxyClient(string host, int port)
      : this(host, port, string.Empty)
    {
    }

    public Socks4ProxyClient(string host, int port, string username)
      : base(ProxyType.Socks4, host, port, username, (string) null)
    {
    }

    public static Socks4ProxyClient Parse(string proxyAddress)
    {
      return ProxyClient.Parse(ProxyType.Socks4, proxyAddress) as Socks4ProxyClient;
    }

    public static bool TryParse(string proxyAddress, out Socks4ProxyClient result)
    {
      ProxyClient result1;
      if (ProxyClient.TryParse(ProxyType.Socks4, proxyAddress, out result1))
      {
        result = result1 as Socks4ProxyClient;
        return true;
      }
      result = (Socks4ProxyClient) null;
      return false;
    }

    public override TcpClient CreateConnection(
      string destinationHost,
      int destinationPort,
      TcpClient tcpClient = null)
    {
      this.CheckState();
      if (destinationHost == null)
        throw new ArgumentNullException(nameof (destinationHost));
      if (destinationHost.Length == 0)
        throw ExceptionHelper.EmptyString(nameof (destinationHost));
      if (!ExceptionHelper.ValidateTcpPort(destinationPort))
        throw ExceptionHelper.WrongTcpPort(nameof (destinationPort));
      TcpClient tcpClient1 = tcpClient ?? this.CreateConnectionToProxy();
      try
      {
        this.SendCommand(tcpClient1.GetStream(), (byte) 1, destinationHost, destinationPort);
      }
      catch (Exception ex)
      {
        tcpClient1.Close();
        if (ex is IOException || ex is SocketException)
          throw this.NewProxyException(Resources.ProxyException_Error, ex);
        throw;
      }
      return tcpClient1;
    }

    protected internal virtual void SendCommand(
      NetworkStream nStream,
      byte command,
      string destinationHost,
      int destinationPort)
    {
      byte[] ipAddressBytes = this.GetIPAddressBytes(destinationHost);
      byte[] portBytes = Socks4ProxyClient.GetPortBytes(destinationPort);
      byte[] numArray1 = string.IsNullOrEmpty(this._username) ? Array.Empty<byte>() : Encoding.ASCII.GetBytes(this._username);
      byte[] buffer1 = new byte[9 + numArray1.Length];
      buffer1[0] = (byte) 4;
      buffer1[1] = command;
      byte[] numArray2 = buffer1;
      portBytes.CopyTo((Array) numArray2, 2);
      byte[] numArray3 = buffer1;
      ipAddressBytes.CopyTo((Array) numArray3, 4);
      numArray1.CopyTo((Array) buffer1, 8);
      buffer1[8 + numArray1.Length] = (byte) 0;
      nStream.Write(buffer1, 0, buffer1.Length);
      byte[] buffer2 = new byte[8];
      nStream.Read(buffer2, 0, buffer2.Length);
      byte command1 = buffer2[1];
      if (command1 == (byte) 90)
        return;
      this.HandleCommandError(command1);
    }

    protected internal byte[] GetIPAddressBytes(string destinationHost)
    {
      IPAddress address;
      if (!IPAddress.TryParse(destinationHost, out address))
      {
        try
        {
          IPAddress[] hostAddresses = Dns.GetHostAddresses(destinationHost);
          if (hostAddresses.Length != 0)
            address = hostAddresses[0];
        }
        catch (Exception ex)
        {
          if (ex is SocketException || ex is ArgumentException)
            throw new ProxyException(string.Format(Resources.ProxyException_FailedGetHostAddresses, (object) destinationHost), (ProxyClient) this, ex);
          throw;
        }
      }
      return address.GetAddressBytes();
    }

    protected internal static byte[] GetPortBytes(int port)
    {
      return new byte[2]
      {
        (byte) (port / 256),
        (byte) (port % 256)
      };
    }

    protected internal void HandleCommandError(byte command)
    {
      string str;
      switch (command)
      {
        case 91:
          str = Resources.Socks4_CommandReplyRequestRejectedOrFailed;
          break;
        case 92:
          str = Resources.Socks4_CommandReplyRequestRejectedCannotConnectToIdentd;
          break;
        case 93:
          str = Resources.Socks4_CommandReplyRequestRejectedDifferentIdentd;
          break;
        default:
          str = Resources.Socks_UnknownError;
          break;
      }
      throw new ProxyException(string.Format(Resources.ProxyException_CommandError, (object) str, (object) this.ToString()), (ProxyClient) this, (Exception) null);
    }
  }
}
