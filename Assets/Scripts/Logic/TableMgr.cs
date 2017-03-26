using Res_Table;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEngine;

public class TableMgr
{
    public const int cIdxHero = 0;
    public const int cIdxSkill = 1;
    public const int cIdxSoldier = 2;
    public const int cIdxItem = 2;
    public Dictionary<int, TableRecordBase> m_tablePathDict = new Dictionary<int, TableRecordBase>()
    {
        {
            cIdxHero,
            new TableRecord<ResHeroList, ResHeroInfo>
            {
                path = "Hero",
                tableIndex = cIdxHero,
            }
        },
        {
            cIdxSkill,
            new TableRecord<ResSkillList, ResSkillInfo>
            {
                path = "Skill",
                tableIndex = cIdxSkill,
            }
        },
        {
            cIdxSoldier,
            new TableRecord<ResSoldierList, ResSoldierInfo>
            {
                path = "Soldier",
                tableIndex = cIdxSoldier,
            }
        },
        {
            cIdxItem,
            new TableRecord<ResItemList, ResItemInfo>
            {
                path = "Item",
                tableIndex = cIdxItem,
            }
        },
    };


    private static Hashtable m_resTable = new Hashtable();
    private static TableMgr m_cTableResMgr;
    public static TableMgr Instance
    {
        get
        {
            if(m_cTableResMgr == null)
                m_cTableResMgr = new TableMgr();
            return m_cTableResMgr;
        }
    }

    public class TableRecordBase
    {
        public virtual CRecordTable Load() { return null; }
    }
    public class TableRecord<T1, T2> : TableRecordBase
    {
        public int tableIndex;
        public string path;
        public CRecordTable recordTable;
        public override CRecordTable Load()
        {
            try
            {
                byte[] buff = (ObjectsManager.LoadObject<TextAsset>(BundleBelong.tables, path)).bytes;
                if(buff != null)
                {
                    recordTable = new CRecordTable();
                    recordTable.Load<T1, T2>(buff);
                }
            }
            catch(System.Exception e)
            {
                Debug.Log(e.ToString());
            }
            return recordTable;
        }
    }

    public void LoadBinary(int nIndex)
    {
        if (!m_resTable.Contains(nIndex))
        {
            TableRecordBase recordBase = m_tablePathDict[nIndex];
            if (recordBase != null)
            {
                CRecordTable recordTable = recordBase.Load();
                m_resTable.Add(nIndex, recordTable);
            }
        }
    }

    public T GetRecord<T>(int tableIndex, uint dataId)
    {
        return GetRecord<T>(tableIndex, (int)dataId);
    }

    public T GetRecord<T>(int tableIndex, ulong dataId)
    {
        return GetRecord<T>(tableIndex, (int)dataId);
    }

    public T GetRecord<T>(int tableIndex, int dataId)
    {
        CRecordTable recordTable = (CRecordTable)m_resTable[tableIndex];
        if (recordTable == null)
        {
            recordTable = m_tablePathDict[tableIndex].Load();
            m_resTable.Add(tableIndex, recordTable);
        }

        if (recordTable != null)
        {
            return (T)recordTable.FindItem(dataId);
        }

        return default(T);
    }

    public T GetRecordByIdx<T>(int tableIndex, int nIndex)
    {
        CRecordTable recordTable = (CRecordTable)m_resTable[tableIndex];
        if (recordTable == null)
        {
            recordTable = m_tablePathDict[tableIndex].Load();
            m_resTable.Add(tableIndex, recordTable);
        }

        if (recordTable != null)
        {
            return (T)recordTable.GetRecord(nIndex);
        }

        return default(T);
    }

    public CRecordTable GetTable(int tableIndex)
    {
        if (m_resTable[tableIndex] != null)
        {
            return (CRecordTable)m_resTable[tableIndex];
        }
        else if (m_tablePathDict.ContainsKey(tableIndex))
        {
            CRecordTable recordTable = m_tablePathDict[tableIndex].Load();
            m_resTable.Add(tableIndex, recordTable);
            return recordTable;
        }
        return null;
    }
}

public class CRecordTable
{
    private Dictionary<int, ProtoBuf.IExtensible> m_cItemMap = new Dictionary<int, ProtoBuf.IExtensible>();
    private List<ProtoBuf.IExtensible> m_itemList = new List<ProtoBuf.IExtensible>();

    public ProtoBuf.IExtensible FindItem(int nKey)
    {
        ProtoBuf.IExtensible t;
        m_cItemMap.TryGetValue(nKey, out t);

        return t;
    }

    public ProtoBuf.IExtensible GetRecord(int nIndex)
    {
        if (nIndex < m_itemList.Count)
        {
            return m_itemList[nIndex];
        }
        return default(ProtoBuf.IExtensible);
    }

    public int Count()
    {
        return m_cItemMap.Count;
    }

    public void Load<T1, T2>(byte[] buff)
    {
        try
        {
            using (Stream stream = new MemoryStream(buff))
            {
                //long fStartTime = DateTime.Now.Ticks;
                //Debug.Log("begin deseralize time = " + DateTime.Now.Ticks.ToString());
                T1 list = ProtoBuf.Serializer.Deserialize<T1>(stream);
                //Debug.Log("end deseralize time tick =" + (DateTime.Now.Ticks - fStartTime).ToString() + typeof(T1).ToString());
                PropertyInfo listProp = list.GetType().GetProperty("list");
                List<T2> recordList = (List<T2>)listProp.GetGetMethod().Invoke(list, null);
                if (recordList != null)
                {
                    for (int i = 0; i < recordList.Count; i++)
                    {
                        T2 item = (T2)recordList[i];
                        PropertyInfo itemProp = recordList[i].GetType().GetProperty("id");
                        int id = System.Convert.ToInt32(itemProp.GetGetMethod().Invoke(recordList[i], null));
                        if (!m_cItemMap.ContainsKey(id))
                        {
                            m_cItemMap.Add(id, item as ProtoBuf.IExtensible);

                            m_itemList.Add(item as ProtoBuf.IExtensible);
                        }
                        else
                        {
                            string table = typeof(T1).ToString();
                            Debug.LogError(string.Format("加载表{0}时出现重复的资源ID：{1}", table, id));
                        }
                    }
                }
            }
        }
        catch (System.Exception ex)
        {
            Debug.LogError(ex.ToString() + " LoadError type is: " + typeof(T1).ToString());
        }
    }
}
