// Decompiled with JetBrains decompiler
// Type: xNet.MultipartContent
// Assembly: xNet, Version=3.3.3.0, Culture=neutral, PublicKeyToken=null
// MVID: 8FAB7F03-1085-4650-8C57-7A04F40293E8
// Assembly location: C:\Users\Henris\Desktop\Smart Pastebin\xNet.dll

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace xNet
{
  public class MultipartContent : HttpContent, IEnumerable<HttpContent>, IEnumerable
  {
    private List<MultipartContent.Element> elements = new List<MultipartContent.Element>();
    private const int FIELD_TEMPLATE_SIZE = 43;
    private const int FIELD_FILE_TEMPLATE_SIZE = 72;
    private const string FIELD_TEMPLATE = "Content-Disposition: form-data; name=\"{0}\"\r\n\r\n";
    private const string FIELD_FILE_TEMPLATE = "Content-Disposition: form-data; name=\"{0}\"; filename=\"{1}\"\r\nContent-Type: {2}\r\n\r\n";
    [ThreadStatic]
    private static Random rand;
    private readonly string boundary;

    private static Random Rand
    {
      get
      {
        if (MultipartContent.rand == null)
          MultipartContent.rand = new Random();
        return MultipartContent.rand;
      }
    }

    public MultipartContent()
      : this("----------------" + MultipartContent.GetRandomString(16))
    {
    }

    public MultipartContent(string boundary)
    {
      if (boundary == null)
        throw new ArgumentNullException(nameof (boundary));
      if (boundary.Length == 0)
        throw ExceptionHelper.EmptyString(nameof (boundary));
      if (boundary.Length > 70)
        throw ExceptionHelper.CanNotBeGreater<int>(nameof (boundary), 70);
      this.boundary = boundary;
      this._contentType = string.Format("multipart/form-data; boundary={0}", (object) this.boundary);
    }

    public void Add(HttpContent content, string name)
    {
      if (content == null)
        throw new ArgumentNullException(nameof (content));
      if (name == null)
        throw new ArgumentNullException(nameof (name));
      if (name.Length == 0)
        throw ExceptionHelper.EmptyString(nameof (name));
      this.elements.Add(new MultipartContent.Element()
      {
        Name = name,
        Content = content
      });
    }

    public void Add(HttpContent content, string name, string fileName)
    {
      if (content == null)
        throw new ArgumentNullException(nameof (content));
      if (name == null)
        throw new ArgumentNullException(nameof (name));
      if (name.Length == 0)
        throw ExceptionHelper.EmptyString(nameof (name));
      if (fileName == null)
        throw new ArgumentNullException(nameof (fileName));
      content.ContentType = Http.DetermineMediaType(Path.GetExtension(fileName));
      this.elements.Add(new MultipartContent.Element()
      {
        Name = name,
        FileName = fileName,
        Content = content
      });
    }

    public void Add(HttpContent content, string name, string fileName, string contentType)
    {
      if (content == null)
        throw new ArgumentNullException(nameof (content));
      if (name == null)
        throw new ArgumentNullException(nameof (name));
      if (name.Length == 0)
        throw ExceptionHelper.EmptyString(nameof (name));
      if (fileName == null)
        throw new ArgumentNullException(nameof (fileName));
      HttpContent httpContent = content;
      string str = contentType;
      if (str == null)
        throw new ArgumentNullException(nameof (contentType));
      httpContent.ContentType = str;
      this.elements.Add(new MultipartContent.Element()
      {
        Name = name,
        FileName = fileName,
        Content = content
      });
    }

    public override long CalculateContentLength()
    {
      this.ThrowIfDisposed();
      long num = 0;
      foreach (MultipartContent.Element element in this.elements)
      {
        num += element.Content.CalculateContentLength();
        if (element.IsFieldFile())
        {
          num += 72L;
          num += (long) element.Name.Length;
          num += (long) element.FileName.Length;
          num += (long) element.Content.ContentType.Length;
        }
        else
        {
          num += 43L;
          num += (long) element.Name.Length;
        }
        num += (long) (this.boundary.Length + 6);
      }
      return num + (long) (this.boundary.Length + 6);
    }

    public override void WriteTo(Stream stream)
    {
      this.ThrowIfDisposed();
      if (stream == null)
        throw new ArgumentNullException(nameof (stream));
      byte[] bytes1 = Encoding.ASCII.GetBytes("\r\n");
      byte[] bytes2 = Encoding.ASCII.GetBytes("--" + this.boundary + "\r\n");
      foreach (MultipartContent.Element element in this.elements)
      {
        stream.Write(bytes2, 0, bytes2.Length);
        byte[] bytes3 = Encoding.ASCII.GetBytes(!element.IsFieldFile() ? string.Format("Content-Disposition: form-data; name=\"{0}\"\r\n\r\n", (object) element.Name) : string.Format("Content-Disposition: form-data; name=\"{0}\"; filename=\"{1}\"\r\nContent-Type: {2}\r\n\r\n", (object) element.Name, (object) element.FileName, (object) element.Content.ContentType));
        stream.Write(bytes3, 0, bytes3.Length);
        element.Content.WriteTo(stream);
        stream.Write(bytes1, 0, bytes1.Length);
      }
      byte[] bytes4 = Encoding.ASCII.GetBytes("--" + this.boundary + "--\r\n");
      stream.Write(bytes4, 0, bytes4.Length);
    }

    public IEnumerator<HttpContent> GetEnumerator()
    {
      this.ThrowIfDisposed();
      return this.elements.Select<MultipartContent.Element, HttpContent>((Func<MultipartContent.Element, HttpContent>) (e => e.Content)).GetEnumerator();
    }

    protected override void Dispose(bool disposing)
    {
      if (!disposing || this.elements == null)
        return;
      foreach (MultipartContent.Element element in this.elements)
        element.Content.Dispose();
      this.elements = (List<MultipartContent.Element>) null;
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
      this.ThrowIfDisposed();
      return (IEnumerator) this.GetEnumerator();
    }

    public static string GetRandomString(int length)
    {
      StringBuilder stringBuilder = new StringBuilder(length);
      for (int index = 0; index < length; ++index)
      {
        switch (MultipartContent.Rand.Next(3))
        {
          case 0:
            stringBuilder.Append((char) MultipartContent.Rand.Next(48, 58));
            break;
          case 1:
            stringBuilder.Append((char) MultipartContent.Rand.Next(97, 123));
            break;
          case 2:
            stringBuilder.Append((char) MultipartContent.Rand.Next(65, 91));
            break;
        }
      }
      return stringBuilder.ToString();
    }

    private void ThrowIfDisposed()
    {
      if (this.elements == null)
        throw new ObjectDisposedException(nameof (MultipartContent));
    }

    private sealed class Element
    {
      public string Name;
      public string FileName;
      public HttpContent Content;

      public bool IsFieldFile()
      {
        return this.FileName != null;
      }
    }
  }
}
