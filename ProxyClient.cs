// Decompiled with JetBrains decompiler
// Type: xNet.ProxyClient
// Assembly: xNet, Version=3.3.3.0, Culture=neutral, PublicKeyToken=null
// MVID: BCFC550F-93AE-4DF9-8F50-A984FB298337
// Assembly location: C:\Users\Henris\Desktop\Smart Pastebin\xNet-0bfa2388b222842ad29fcffb3677177a38854ebd\bin\Release\fsdfsd.dll

using System;
using System.Net.Sockets;
using System.Security;
using System.Text;
using System.Threading;

namespace xNet
{
  public abstract class ProxyClient : IEquatable<ProxyClient>
  {
    protected int _port = 1;
    protected int _connectTimeout = 60000;
    protected int _readWriteTimeout = 60000;
    protected ProxyType _type;
    protected string _host;
    protected string _username;
    protected string _password;

    public virtual ProxyType Type
    {
      get
      {
        return this._type;
      }
    }

    public virtual string Host
    {
      get
      {
        return this._host;
      }
      set
      {
        if (value == null)
          throw new ArgumentNullException(nameof (Host));
        if (value.Length == 0)
          throw ExceptionHelper.EmptyString(nameof (Host));
        this._host = value;
      }
    }

    public virtual int Port
    {
      get
      {
        return this._port;
      }
      set
      {
        if (!ExceptionHelper.ValidateTcpPort(value))
          throw ExceptionHelper.WrongTcpPort(nameof (Port));
        this._port = value;
      }
    }

    public virtual string Username
    {
      get
      {
        return this._username;
      }
      set
      {
        if (value != null && value.Length > (int) byte.MaxValue)
          throw new ArgumentOutOfRangeException(nameof (Username), string.Format(Resources.ArgumentOutOfRangeException_StringLengthCanNotBeMore, (object) (int) byte.MaxValue));
        this._username = value;
      }
    }

    public virtual string Password
    {
      get
      {
        return this._password;
      }
      set
      {
        if (value != null && value.Length > (int) byte.MaxValue)
          throw new ArgumentOutOfRangeException(nameof (Password), string.Format(Resources.ArgumentOutOfRangeException_StringLengthCanNotBeMore, (object) (int) byte.MaxValue));
        this._password = value;
      }
    }

    public virtual int ConnectTimeout
    {
      get
      {
        return this._connectTimeout;
      }
      set
      {
        if (value < 0)
          throw ExceptionHelper.CanNotBeLess<int>(nameof (ConnectTimeout), 0);
        this._connectTimeout = value;
      }
    }

    public virtual int ReadWriteTimeout
    {
      get
      {
        return this._readWriteTimeout;
      }
      set
      {
        if (value < 0)
          throw ExceptionHelper.CanNotBeLess<int>(nameof (ReadWriteTimeout), 0);
        this._readWriteTimeout = value;
      }
    }

    protected internal ProxyClient(ProxyType proxyType)
    {
      this._type = proxyType;
    }

    protected internal ProxyClient(ProxyType proxyType, string address, int port)
    {
      this._type = proxyType;
      this._host = address;
      this._port = port;
    }

    protected internal ProxyClient(
      ProxyType proxyType,
      string address,
      int port,
      string username,
      string password)
    {
      this._type = proxyType;
      this._host = address;
      this._port = port;
      this._username = username;
      this._password = password;
    }

    public static ProxyClient Parse(ProxyType proxyType, string proxyAddress)
    {
      if (proxyAddress == null)
        throw new ArgumentNullException(nameof (proxyAddress));
      if (proxyAddress.Length == 0)
        throw ExceptionHelper.EmptyString(nameof (proxyAddress));
      string[] strArray = proxyAddress.Split(':');
      int port = 0;
      string host = strArray[0];
      if (strArray.Length >= 2)
      {
        try
        {
          port = int.Parse(strArray[1]);
        }
        catch (Exception ex)
        {
          if (ex is FormatException || ex is OverflowException)
            throw new FormatException(Resources.InvalidOperationException_ProxyClient_WrongPort, ex);
          throw;
        }
        if (!ExceptionHelper.ValidateTcpPort(port))
          throw new FormatException(Resources.InvalidOperationException_ProxyClient_WrongPort);
      }
      string username = (string) null;
      string password = (string) null;
      if (strArray.Length >= 3)
        username = strArray[2];
      if (strArray.Length >= 4)
        password = strArray[3];
      return ProxyHelper.CreateProxyClient(proxyType, host, port, username, password);
    }

    public static bool TryParse(ProxyType proxyType, string proxyAddress, out ProxyClient result)
    {
      result = (ProxyClient) null;
      if (string.IsNullOrEmpty(proxyAddress))
        return false;
      string[] strArray = proxyAddress.Split(':');
      int result1 = 0;
      string host = strArray[0];
      if (strArray.Length >= 2 && (!int.TryParse(strArray[1], out result1) || !ExceptionHelper.ValidateTcpPort(result1)))
        return false;
      string username = (string) null;
      string password = (string) null;
      if (strArray.Length >= 3)
        username = strArray[2];
      if (strArray.Length >= 4)
        password = strArray[3];
      try
      {
        result = ProxyHelper.CreateProxyClient(proxyType, host, result1, username, password);
      }
      catch (InvalidOperationException ex)
      {
        return false;
      }
      return true;
    }

    public abstract TcpClient CreateConnection(
      string destinationHost,
      int destinationPort,
      TcpClient tcpClient = null);

    public override string ToString()
    {
      return string.Format("{0}:{1}", (object) this._host, (object) this._port);
    }

    public virtual string ToExtendedString()
    {
      StringBuilder stringBuilder = new StringBuilder();
      stringBuilder.AppendFormat("{0}:{1}", (object) this._host, (object) this._port);
      if (!string.IsNullOrEmpty(this._username))
      {
        stringBuilder.AppendFormat(":{0}", (object) this._username);
        if (!string.IsNullOrEmpty(this._password))
          stringBuilder.AppendFormat(":{0}", (object) this._password);
      }
      return stringBuilder.ToString();
    }

    public override int GetHashCode()
    {
      if (string.IsNullOrEmpty(this._host))
        return 0;
      return this._host.GetHashCode() ^ this._port;
    }

    public bool Equals(ProxyClient proxy)
    {
      return proxy != null && this._host != null && (this._host.Equals(proxy._host, StringComparison.OrdinalIgnoreCase) && this._port == proxy._port);
    }

    public override bool Equals(object obj)
    {
      ProxyClient proxy = obj as ProxyClient;
      if (proxy == null)
        return false;
      return this.Equals(proxy);
    }

    protected TcpClient CreateConnectionToProxy()
    {
      TcpClient tcpClient = (TcpClient) null;
      tcpClient = new TcpClient();
      Exception connectException = (Exception) null;
      ManualResetEventSlim connectDoneEvent = new ManualResetEventSlim();
      try
      {
        tcpClient.BeginConnect(this._host, this._port, (AsyncCallback) (ar =>
        {
          if (tcpClient.Client == null)
            return;
          try
          {
            tcpClient.EndConnect(ar);
          }
          catch (Exception ex)
          {
            connectException = ex;
          }
          connectDoneEvent.Set();
        }), (object) tcpClient);
      }
      catch (Exception ex)
      {
        tcpClient.Close();
        if (ex is SocketException || ex is SecurityException)
          throw this.NewProxyException(Resources.ProxyException_FailedConnect, ex);
        throw;
      }
      if (!connectDoneEvent.Wait(this._connectTimeout))
      {
        tcpClient.Close();
        throw this.NewProxyException(Resources.ProxyException_ConnectTimeout, (Exception) null);
      }
      if (connectException != null)
      {
        tcpClient.Close();
        if (connectException is SocketException)
          throw this.NewProxyException(Resources.ProxyException_FailedConnect, connectException);
        throw connectException;
      }
      if (!tcpClient.Connected)
      {
        tcpClient.Close();
        throw this.NewProxyException(Resources.ProxyException_FailedConnect, (Exception) null);
      }
      tcpClient.SendTimeout = this._readWriteTimeout;
      tcpClient.ReceiveTimeout = this._readWriteTimeout;
      return tcpClient;
    }

    protected void CheckState()
    {
      if (string.IsNullOrEmpty(this._host))
        throw new InvalidOperationException(Resources.InvalidOperationException_ProxyClient_WrongHost);
      if (!ExceptionHelper.ValidateTcpPort(this._port))
        throw new InvalidOperationException(Resources.InvalidOperationException_ProxyClient_WrongPort);
      if (this._username != null && this._username.Length > (int) byte.MaxValue)
        throw new InvalidOperationException(Resources.InvalidOperationException_ProxyClient_WrongUsername);
      if (this._password != null && this._password.Length > (int) byte.MaxValue)
        throw new InvalidOperationException(Resources.InvalidOperationException_ProxyClient_WrongPassword);
    }

    protected ProxyException NewProxyException(
      string message,
      Exception innerException = null)
    {
      return new ProxyException(string.Format(message, (object) this.ToString()), this, innerException);
    }
  }
}
