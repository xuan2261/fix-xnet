// Decompiled with JetBrains decompiler
// Type: xNet.Socks5ProxyClient
// Assembly: xNet, Version=3.3.3.0, Culture=neutral, PublicKeyToken=null
// MVID: BCFC550F-93AE-4DF9-8F50-A984FB298337
// Assembly location: C:\Users\Henris\Desktop\Smart Pastebin\xNet-0bfa2388b222842ad29fcffb3677177a38854ebd\bin\Release\fsdfsd.dll

using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace xNet
{
  public class Socks5ProxyClient : ProxyClient
  {
    private const int DefaultPort = 1080;
    private const byte VersionNumber = 5;
    private const byte Reserved = 0;
    private const byte AuthMethodNoAuthenticationRequired = 0;
    private const byte AuthMethodGssapi = 1;
    private const byte AuthMethodUsernamePassword = 2;
    private const byte AuthMethodIanaAssignedRangeBegin = 3;
    private const byte AuthMethodIanaAssignedRangeEnd = 127;
    private const byte AuthMethodReservedRangeBegin = 128;
    private const byte AuthMethodReservedRangeEnd = 254;
    private const byte AuthMethodReplyNoAcceptableMethods = 255;
    private const byte CommandConnect = 1;
    private const byte CommandBind = 2;
    private const byte CommandUdpAssociate = 3;
    private const byte CommandReplySucceeded = 0;
    private const byte CommandReplyGeneralSocksServerFailure = 1;
    private const byte CommandReplyConnectionNotAllowedByRuleset = 2;
    private const byte CommandReplyNetworkUnreachable = 3;
    private const byte CommandReplyHostUnreachable = 4;
    private const byte CommandReplyConnectionRefused = 5;
    private const byte CommandReplyTTLExpired = 6;
    private const byte CommandReplyCommandNotSupported = 7;
    private const byte CommandReplyAddressTypeNotSupported = 8;
    private const byte AddressTypeIPV4 = 1;
    private const byte AddressTypeDomainName = 3;
    private const byte AddressTypeIPV6 = 4;

    public Socks5ProxyClient()
      : this((string) null)
    {
    }

    public Socks5ProxyClient(string host)
      : this(host, 1080)
    {
    }

    public Socks5ProxyClient(string host, int port)
      : this(host, port, string.Empty, string.Empty)
    {
    }

    public Socks5ProxyClient(string host, int port, string username, string password)
      : base(ProxyType.Socks5, host, port, username, password)
    {
    }

    public static Socks5ProxyClient Parse(string proxyAddress)
    {
      return ProxyClient.Parse(ProxyType.Socks5, proxyAddress) as Socks5ProxyClient;
    }

    public static bool TryParse(string proxyAddress, out Socks5ProxyClient result)
    {
      ProxyClient result1;
      if (ProxyClient.TryParse(ProxyType.Socks5, proxyAddress, out result1))
      {
        result = result1 as Socks5ProxyClient;
        return true;
      }
      result = (Socks5ProxyClient) null;
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
        NetworkStream stream = tcpClient1.GetStream();
        this.InitialNegotiation(stream);
        this.SendCommand(stream, (byte) 1, destinationHost, destinationPort);
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

    private void InitialNegotiation(NetworkStream nStream)
    {
      byte num = string.IsNullOrEmpty(this._username) || string.IsNullOrEmpty(this._password) ? (byte) 0 : (byte) 2;
      byte[] buffer1 = new byte[3]
      {
        (byte) 5,
        (byte) 1,
        num
      };
      nStream.Write(buffer1, 0, buffer1.Length);
      byte[] buffer2 = new byte[2];
      nStream.Read(buffer2, 0, buffer2.Length);
      byte command = buffer2[1];
      if (num == (byte) 2 && command == (byte) 2)
      {
        this.SendUsernameAndPassword(nStream);
      }
      else
      {
        if (command == (byte) 0)
          return;
        this.HandleCommandError(command);
      }
    }

    private void SendUsernameAndPassword(NetworkStream nStream)
    {
      byte[] numArray1 = string.IsNullOrEmpty(this._username) ? new byte[0] : Encoding.ASCII.GetBytes(this._username);
      byte[] numArray2 = string.IsNullOrEmpty(this._password) ? new byte[0] : Encoding.ASCII.GetBytes(this._password);
      byte[] buffer1 = new byte[numArray1.Length + numArray2.Length + 3];
      buffer1[0] = (byte) 1;
      buffer1[1] = (byte) numArray1.Length;
      numArray1.CopyTo((Array) buffer1, 2);
      buffer1[2 + numArray1.Length] = (byte) numArray2.Length;
      numArray2.CopyTo((Array) buffer1, 3 + numArray1.Length);
      nStream.Write(buffer1, 0, buffer1.Length);
      byte[] buffer2 = new byte[2];
      nStream.Read(buffer2, 0, buffer2.Length);
      if (buffer2[1] != (byte) 0)
        throw this.NewProxyException(Resources.ProxyException_Socks5_FailedAuthOn, (Exception) null);
    }

    private void SendCommand(
      NetworkStream nStream,
      byte command,
      string destinationHost,
      int destinationPort)
    {
      byte addressType = this.GetAddressType(destinationHost);
      byte[] addressBytes = this.GetAddressBytes(addressType, destinationHost);
      byte[] portBytes = this.GetPortBytes(destinationPort);
      byte[] buffer1 = new byte[4 + addressBytes.Length + 2];
      buffer1[0] = (byte) 5;
      buffer1[1] = command;
      buffer1[2] = (byte) 0;
      buffer1[3] = addressType;
      addressBytes.CopyTo((Array) buffer1, 4);
      byte[] numArray = buffer1;
      int index = 4 + addressBytes.Length;
      portBytes.CopyTo((Array) numArray, index);
      nStream.Write(buffer1, 0, buffer1.Length);
      byte[] buffer2 = new byte[(int) byte.MaxValue];
      nStream.Read(buffer2, 0, buffer2.Length);
      byte command1 = buffer2[1];
      if (command1 == (byte) 0)
        return;
      this.HandleCommandError(command1);
    }

    private byte GetAddressType(string host)
    {
      IPAddress address = (IPAddress) null;
      if (!IPAddress.TryParse(host, out address))
        return 3;
      switch (address.AddressFamily)
      {
        case AddressFamily.InterNetwork:
          return 1;
        case AddressFamily.InterNetworkV6:
          return 4;
        default:
          throw new ProxyException(string.Format(Resources.ProxyException_NotSupportedAddressType, (object) host, (object) System.Enum.GetName(typeof (AddressFamily), (object) address.AddressFamily), (object) this.ToString()), (ProxyClient) this, (Exception) null);
      }
    }

    private byte[] GetAddressBytes(byte addressType, string host)
    {
      switch (addressType)
      {
        case 1:
        case 4:
          return IPAddress.Parse(host).GetAddressBytes();
        case 3:
          byte[] numArray = new byte[host.Length + 1];
          numArray[0] = (byte) host.Length;
          Encoding.ASCII.GetBytes(host).CopyTo((Array) numArray, 1);
          return numArray;
        default:
          return (byte[]) null;
      }
    }

    private byte[] GetPortBytes(int port)
    {
      return new byte[2]
      {
        (byte) (port / 256),
        (byte) (port % 256)
      };
    }

    private void HandleCommandError(byte command)
    {
      string str;
      switch (command)
      {
        case 1:
          str = Resources.Socks5_CommandReplyGeneralSocksServerFailure;
          break;
        case 2:
          str = Resources.Socks5_CommandReplyConnectionNotAllowedByRuleset;
          break;
        case 3:
          str = Resources.Socks5_CommandReplyNetworkUnreachable;
          break;
        case 4:
          str = Resources.Socks5_CommandReplyHostUnreachable;
          break;
        case 5:
          str = Resources.Socks5_CommandReplyConnectionRefused;
          break;
        case 6:
          str = Resources.Socks5_CommandReplyTTLExpired;
          break;
        case 7:
          str = Resources.Socks5_CommandReplyCommandNotSupported;
          break;
        case 8:
          str = Resources.Socks5_CommandReplyAddressTypeNotSupported;
          break;
        case byte.MaxValue:
          str = Resources.Socks5_AuthMethodReplyNoAcceptableMethods;
          break;
        default:
          str = Resources.Socks_UnknownError;
          break;
      }
      throw new ProxyException(string.Format(Resources.ProxyException_CommandError, (object) str, (object) this.ToString()), (ProxyClient) this, (Exception) null);
    }
  }
}
