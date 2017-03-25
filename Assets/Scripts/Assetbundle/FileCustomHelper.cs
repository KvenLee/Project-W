using System.IO;

/***********************
* 类功能说明: 文件帮助
* 日期：
* 作者： 周 星
*
**********************/

public static class FileCustomHelper
{
    /// <summary>
    /// 创建文件夹
    /// </summary>
    /// <param name="folderpath"></param>
    public static void CreateFolder(string folderpath)
    {
        if (!Directory.Exists(folderpath))
        {
            DirectoryInfo dir = new DirectoryInfo(folderpath);
            dir.Create();

#if UNITY_IOS
            UnityEngine.iOS.Device.SetNoBackupFlag(folderpath);
#endif

            UnityEngine.Debug.Log(folderpath + " 目录创建成功!");
        }
        else
        {
            UnityEngine.Debug.Log("目录已存在！");
        }
    }

    /// <summary>
    /// 删除指定文件夹
    /// </summary>
    public static void DeleteFolder(string folderpath)
    {
        if (Directory.Exists(folderpath))
        {
            Directory.Delete(folderpath, true);
            UnityEngine.Debug.Log(folderpath + " 目录删除成功!");
        }
        else
        {
            UnityEngine.Debug.Log("目录不存在！");
        }
    }

    /// <summary>
    /// 删除指定文件夹内容
    /// </summary>
    /// <returns></returns>
    public static bool DeleteFolderContent(string folderpath)
    {
        //if (Directory.Exists(folderpath))
        //{
        //    foreach (string file in Directory.GetFileSystemEntries(folderpath))
        //    {
        //        if (File.Exists(file))
        //        {
        //            FileInfo fi = new FileInfo(file);
        //            if (fi.Attributes.ToString().IndexOf("ReadOnly") != -1)
        //                fi.Attributes = FileAttributes.Normal;
        //            File.Delete(file);
        //        }
        //        else
        //        {
        //            DirectoryInfo di = new DirectoryInfo(file);
        //            if (di.GetFiles().Length != 0)
        //                DeleteFolderContent(di.FullName);
        //            Directory.Delete(file);
        //        }
        //    }
        //    return true;
        //}
        //return false;

        if (Directory.Exists(folderpath))
        {
            string[] d = Directory.GetFileSystemEntries(folderpath);
            for (int i = 0; i < d.Length; i++)
            {
                if (File.Exists(d[i]))
                    File.Delete(d[i]);
                else
                    DeleteFolderContent(d[i]);
            }
            Directory.Delete(folderpath, true);
        }
        return true;
    }

    /// <summary>
    /// 创建文件
    /// </summary>
    /// <param name="filepath"></param>
    /// <returns></returns>
    public static FileStream CreateFile(string filepath, FileMode filemode, FileAccess access)
    {
        FileStream file = new FileStream(filepath, filemode, access);
        if (File.Exists(filepath))
            return file;
        else
            return null;
    }

    /// <summary>
    /// 删除文件
    /// </summary>
    /// <param name="filepath"></param>
    public static void DeleteFile(string filepath)
    {
        if (File.Exists(filepath))
        {
            File.Delete(filepath);
        }
    }

    /// <summary>
    /// 检测指定文件是否存在
    /// </summary>
    public static bool CheckIfExist(string filePath)
    {
        return File.Exists(filePath);
    }
}
