using UnityEngine;
using System.Collections;
using System.IO;
using ICSharpCode.SharpZipLib;
using ICSharpCode.SharpZipLib.GZip;

/// <summary>
/// Gzip压缩解压帮助类
/// </summary>

public static class GzipHelper
{
    /// <summary>
    /// Gzip压缩
    /// </summary>
    /// <param name="bytes"></param>
    /// <returns></returns>
    public static byte[] GzipCompress(byte[] bytes)
    {
        using(MemoryStream ms = new MemoryStream())
        {
            using(GZipOutputStream gzip = new GZipOutputStream(ms))
            {
                gzip.Write(bytes, 0, bytes.Length);
                gzip.Close();
                byte[] press = ms.ToArray();
                return press;
            }
        }
    }

    /// <summary>
    /// Gzip解压
    /// </summary>
    /// <param name="bytes"></param>
    /// <returns></returns>
    public static byte[] GzipDecompress(byte[] bytes)
    {
        using(GZipInputStream gzip = new GZipInputStream(new MemoryStream(bytes)))
        {
            using(MemoryStream ms = new MemoryStream())
            {
                int count = 0;

                byte[] data = new byte[bytes.Length];
                while((count = gzip.Read(data, 0, data.Length)) != 0)
                {
                    ms.Write(data, 0, count);
                }
                return ms.ToArray();
            }
        }
    }
}