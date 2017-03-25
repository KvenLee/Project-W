using UnityEngine;
using System;
using System.IO;
using System.Collections;
using System.Security.Cryptography;
using System.Text;

/*
 * @author:徐琼
 * 2016-1-13
 * 唯一ID生成器
 */
public class UniqueIDHelper
{
    //.获取一个string 的 唯一 键值
    static private string GetUniqueString()
    {
        return Guid.NewGuid().ToString();
    }
    
    //.生成一个唯一ID
    static public string GetUniqueID()
    {
        string input = GetUniqueString();
        
        MD5 md5Hasher = MD5.Create();

        byte[] data = md5Hasher.ComputeHash(Encoding.Default.GetBytes(input));

        StringBuilder sBuilder = new StringBuilder();

        for (int i = 0; i < data.Length; i++)
        {
            sBuilder.Append(data[i].ToString("x2"));
        }
        return sBuilder.ToString();
    }

    static public string GetMD5HashFromFile(string filename)
    {
        try
        {
            using (FileStream file = new FileStream(filename, FileMode.Open))
            {
                System.Security.Cryptography.MD5 md5 = new System.Security.Cryptography.MD5CryptoServiceProvider();
                byte[] retVal = md5.ComputeHash(file);
                file.Close();

                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < retVal.Length; i++)
                {
                    sb.Append(retVal[i].ToString("x2"));
                }
                return sb.ToString();
            }
        }
        catch (Exception e)
        {
            throw new Exception(string.Format("GetMD5HashFromFile() error!Info:\n{0}",e.Message));
        }
    }
}
