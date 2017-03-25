using UnityEngine;
using System.Collections;
using System.IO;
using System.Collections.Generic;
using System.Net;
using System.Text;

/*******************************
 *  Author: Andrea Chow.
 *  Time:   2015.08.31
 *  Function: 资源检测更新工具
 * *****************************/

public class ResourcesUpdate : CGameSystem
{
    private string prefab = "UIResourcesUpdate";
    public ResourcesUpdateUICtrl CtrlUI { get; private set; }

    public static string version = "";

    WWW assetbundle = null;
    string assetbundlename = string.Empty;
    int currentDownCount = -1;
    int downLoadCount = 0;

    public bool loaded = false;

    public override void SysEnter()
    {
        base.SysEnter();
        ObjectsManager.UnloadAll();
        CUility.GlobalRelease();

        if(CtrlUI == null)
        {
            CtrlUI = UIManager.Instance.GetUI<ResourcesUpdateUICtrl>(prefab);
            CtrlUI.UIEnter();
        }

        StartCoroutine("start");
        StartCoroutine("load");
    }

    public override void SysLeave()
    {
        if(CtrlUI != null)
        {
            CtrlUI.UILeave();
            CtrlUI = null;
            UIManager.Instance.ReleaseUI(prefab);
            CUility.GlobalRelease();
        }
        base.SysLeave();
    }

    IEnumerator load()
    {
        while (true)
        {
            string des = string.Empty;
            if (assetbundle != null)
            {
                des = "正在解压游戏资源 " + assetbundlename + " " + (currentDownCount + 1) + "/" + downLoadCount + " 请耐心等待";
            }
            else
            {
                des = "检查资源更新...... ";
            }
            if(CtrlUI !=null)
            {
                CtrlUI.m_TxtDes.text = des;
            }
            yield return null;
        }
    }

    IEnumerator start()
    {
        GetAddressIP();
        version = Application.version;
        string[] versionDiv = version.Split('.');

        UnityEngine.Debug.Log("当前版本号：" + version);
        for (int i = 0; i < versionDiv.Length; i++)
            UnityEngine.Debug.Log(versionDiv[i]);

        yield return null;

        UnityEngine.Debug.Log(AssetPath.cachedPath);

        //====================
        //创建文件夹
        FileCustomHelper.CreateFolder(AssetPath.cachedPath);

        yield return null;
        //====================

        //1.下载服务器资源配置表
        Dictionary<string, Hash128> downloadHashDic = new Dictionary<string, Hash128>();
        string downloadPath = AssetPath.downloadPath + "assetbundleConfig.assetbundle";

        WWW assetbundleConfig = new WWW(downloadPath);
        yield return assetbundleConfig;

        string[] content = assetbundleConfig.text.Split('\n');
        for (int index = 0; index < content.Length; index++)
        {
            string line = content[index];
            if (string.IsNullOrEmpty(line))
                continue;

            string[] elements = line.Split(',');
            downloadHashDic.Add(elements[0], Hash128.Parse(elements[1]));
        }

        //2.加载本地资源配置表
        Dictionary<string, Hash128> localHashDic = new Dictionary<string, Hash128>();
        string localPath = AssetPath.cachedPath + "/" + "assetbundleConfig.assetbundle";
        if (File.Exists(localPath))
        {
            using (StreamReader fs = new StreamReader(localPath, System.Text.Encoding.Default))
            {
                string line = string.Empty;

                while (!string.IsNullOrEmpty(line = fs.ReadLine()))
                {
                    string[] str = line.Split(',');
                    localHashDic.Add(str[0], Hash128.Parse(str[1]));
                }
            }
        }
        else
        {
            UnityEngine.Debug.Log("文件 " + localPath + "不存在！");
        }

        //3.对比本地与服务器表，确定需要更新的数据
        List<string> needDownload = new List<string>();
        List<string> needDelete = new List<string>();

        foreach (KeyValuePair<string, Hash128> kv in downloadHashDic)
        {
            //新增的assetbundle
            if (!localHashDic.ContainsKey(kv.Key))
            {
                needDownload.Add(kv.Key);
            }
            else
            {
                //修改的assetbundle
                if (!localHashDic[kv.Key].Equals(kv.Value))
                    needDownload.Add(kv.Key);
            }
        }

        foreach (KeyValuePair<string, Hash128> kv in localHashDic)
        {
            if (!downloadHashDic.ContainsKey(kv.Key))
            {
                needDelete.Add(kv.Key);
            }
        }

        //4.更新数据

        #region update assetbundle---

        //==== download assetbundle when client is different from server~~

        UnityEngine.Debug.Log("---------需要下载的资源个数 " + needDownload.Count);

        downLoadCount = needDownload.Count;
        //WebRequest req = null; 

        for (int i = 0; i < downLoadCount; i++)
        {
            currentDownCount = i;
            assetbundlename = needDownload[i];

            downloadPath = AssetPath.downloadPath + assetbundlename;

//             req = WebRequest.Create(downloadPath);
//             req.Method = "GET";
//             using (StreamReader reader = new StreamReader(req.GetResponse().GetResponseStream(), Encoding.GetEncoding("gb2312")))
//             {
//                 Encoding.UTF8.GetBytes(reader.ReadToEnd()); 
//             }

            assetbundle = new WWW(downloadPath);
            yield return assetbundle;

            if (assetbundle.isDone)
            {
                byte[] bytes = assetbundle.bytes;

                //最后一个资源为gameassetbundle不需要解压操作！
                //if (i < needDownload.Count - 1)
                {
                    bytes = GzipHelper.GzipDecompress(assetbundle.bytes);
                }

                string path = AssetPath.cachedPath + "/" + assetbundlename;
                using (FileStream fs = new FileStream(path, FileMode.Create, FileAccess.ReadWrite))
                {
                    fs.Write(bytes, 0, bytes.Length);
                    fs.Flush();
                    fs.Close(); //FileSteam Close()和Dispose()方法效用一致
                }
                UnityEngine.Debug.Log("====" + assetbundlename + " " + bytes.Length + " bytes");
            }
            yield return null;
        }

        #endregion

        //5.更新本地资源配置表
        using (FileStream fs = new FileStream(localPath, FileMode.Create, FileAccess.ReadWrite))
        {
            string fileContent = string.Empty;
            foreach (KeyValuePair<string, Hash128> kv in downloadHashDic)
            {
                fileContent += kv.Key + "," + kv.Value.ToString() + "\n";
            }
            byte[] hashBytes = System.Text.Encoding.Default.GetBytes(fileContent);
            fs.Write(hashBytes, 0, hashBytes.Length);
            fs.Flush();
            fs.Close();
        }

        assetbundle = null;
        StopCoroutine("load");

        loaded = true;
        CUility.GlobalRelease();
        
        yield return null;

        //初始化游戏数据
        SensitiveWordHelper.Instance.SensitiveWordsLoad();

        //7.加载登录场景
        yield return null;

        MaskTransfer.Instance.OnShow2(null, delegate()
        {
            MainGamePageSys.Instance.EnterMain();
        });

        //         LoadingSys.Instance.Goto(string.Empty, delegate()
        //         {
        //             CGameRoot.SwitchState(SystemEnum.Login);
        //         });
    }

    void GetAddressIP()
    {
        ///获取本地的IP地址
        string AddressIP = string.Empty;
        foreach (System.Net.IPAddress _IPAddress in System.Net.Dns.GetHostEntry(System.Net.Dns.GetHostName()).AddressList)
        {
            if (_IPAddress.AddressFamily.ToString() == "InterNetwork")
            {
                AddressIP = _IPAddress.ToString();
            }
        }
        ip = AddressIP;
    }
    public string ip;
}



