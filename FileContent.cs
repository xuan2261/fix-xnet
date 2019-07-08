// Decompiled with JetBrains decompiler
// Type: xNet.FileContent
// Assembly: xNet, Version=3.3.3.0, Culture=neutral, PublicKeyToken=null
// MVID: 8FAB7F03-1085-4650-8C57-7A04F40293E8
// Assembly location: C:\Users\Henris\Desktop\Smart Pastebin\xNet.dll

using System;
using System.IO;

namespace xNet
{
  public class FileContent : StreamContent
  {
    public FileContent(string pathToContent, int bufferSize = 32768)
    {
      if (pathToContent == null)
        throw new ArgumentNullException(nameof (pathToContent));
      if (pathToContent.Length == 0)
        throw ExceptionHelper.EmptyString(nameof (pathToContent));
      if (bufferSize < 1)
        throw ExceptionHelper.CanNotBeLess<int>(nameof (bufferSize), 1);
      this._content = (Stream) new FileStream(pathToContent, FileMode.Open, FileAccess.Read);
      this._bufferSize = bufferSize;
      this._initialStreamPosition = 0L;
      this._contentType = Http.DetermineMediaType(Path.GetExtension(pathToContent));
    }
  }
}
