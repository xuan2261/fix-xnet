// Decompiled with JetBrains decompiler
// Type: xNet.HttpProxyClient
// Assembly: xNet, Version=3.3.3.0, Culture=neutral, PublicKeyToken=null
// MVID: 8FAB7F03-1085-4650-8C57-7A04F40293E8
// Assembly location: C:\Users\Henris\Desktop\Smart Pastebin\xNet.dll

using System;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace xNet
{
  public class HttpProxyClient : ProxyClient
  {
    private const int BUFFER_SIZE = 50;
    private const int DEFAULT_PORT = 8080;

    public HttpProxyClient()
      : this((string) null)
    {
    }

    public HttpProxyClient(string host)
      : this(host, 8080)
    {
    }

    public HttpProxyClient(string host, int port)
      : this(host, port, string.Empty, string.Empty)
    {
    }

    public HttpProxyClient(string host, int port, string username, string password)
      : base(ProxyType.Http, host, port, username, password)
    {
    }

    public static HttpProxyClient Parse(string proxyAddress)
    {
      return ProxyClient.Parse(ProxyType.Http, proxyAddress) as HttpProxyClient;
    }

    public static bool TryParse(string proxyAddress, out HttpProxyClient result)
    {
      ProxyClient result1;
      if (ProxyClient.TryParse(ProxyType.Http, proxyAddress, out result1))
      {
        result = result1 as HttpProxyClient;
        return true;
      }
      result = (HttpProxyClient) null;
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
      if (destinationPort != 80)
      {
        HttpStatusCode response;
        try
        {
          NetworkStream stream = tcpClient1.GetStream();
          this.SendConnectionCommand(stream, destinationHost, destinationPort);
          response = this.ReceiveResponse(stream);
        }
        catch (Exception ex)
        {
          tcpClient1.Close();
          if (ex is IOException || ex is SocketException)
            throw this.NewProxyException(Resources.ProxyException_Error, ex);
          throw;
        }
        if (response != HttpStatusCode.OK)
        {
          tcpClient1.Close();
          throw new ProxyException(string.Format(Resources.ProxyException_ReceivedWrongStatusCode, (object) response, (object) this.ToString()), (ProxyClient) this, (Exception) null);
        }
      }
      return tcpClient1;
    }

    private string GenerateAuthorizationHeader()
    {
      if (!string.IsNullOrEmpty(this._username) || !string.IsNullOrEmpty(this._password))
        return string.Format("Proxy-Authorization: Basic {0}\r\n", (object) Convert.ToBase64String(Encoding.UTF8.GetBytes(string.Format("{0}:{1}", (object) this._username, (object) this._password))));
      return string.Empty;
    }

    private void SendConnectionCommand(
      NetworkStream nStream,
      string destinationHost,
      int destinationPort)
    {
      StringBuilder stringBuilder = new StringBuilder();
      stringBuilder.AppendFormat("CONNECT {0}:{1} HTTP/1.1\r\n", (object) destinationHost, (object) destinationPort);
      stringBuilder.AppendFormat(this.GenerateAuthorizationHeader());
      stringBuilder.AppendLine();
      byte[] bytes = Encoding.ASCII.GetBytes(stringBuilder.ToString());
      nStream.Write(bytes, 0, bytes.Length);
    }

    private HttpStatusCode ReceiveResponse(NetworkStream nStream)
    {
      byte[] numArray = new byte[50];
      StringBuilder stringBuilder = new StringBuilder();
      this.WaitData(nStream);
      do
      {
        int count = nStream.Read(numArray, 0, 50);
        stringBuilder.Append(Encoding.ASCII.GetString(numArray, 0, count));
      }
      while (nStream.DataAvailable);
      string str1 = stringBuilder.ToString();
      if (str1.Length == 0)
        throw this.NewProxyException(Resources.ProxyException_ReceivedEmptyResponse, (Exception) null);
      string str2 = str1.Substring(" ", "\r\n", StringComparison.Ordinal);
      int length = str2.IndexOf(' ');
      if (length == -1)
        throw this.NewProxyException(Resources.ProxyException_ReceivedWrongResponse, (Exception) null);
      string str3 = str2.Substring(0, length);
      if (str3.Length == 0)
        throw this.NewProxyException(Resources.ProxyException_ReceivedWrongResponse, (Exception) null);
      return (HttpStatusCode) Enum.Parse(typeof (HttpStatusCode), str3);
    }

    private void WaitData(NetworkStream nStream)
    {
      int num1 = 0;
      int num2 = nStream.ReadTimeout < 10 ? 10 : nStream.ReadTimeout;
      while (!nStream.DataAvailable)
      {
        if (num1 >= num2)
          throw this.NewProxyException(Resources.ProxyException_WaitDataTimeout, (Exception) null);
        num1 += 10;
        Thread.Sleep(10);
      }
    }
  }
}
