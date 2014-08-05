using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DBSystem;

public class RandomName
{
    static readonly RandomName sInstance = new RandomName();
    static public RandomName Instance { get { return sInstance; } }

    List<string> surnames = new List<string>();
    List<string> repeatNameM = new List<string>();
    List<string> noRepeatNameM = new List<string>();
    List<string> repeatNameF = new List<string>();
    List<string> noRepeatNameF = new List<string>();

    void ClearOld()
    {
        surnames.Clear();
        repeatNameM.Clear();
        noRepeatNameM.Clear();
        repeatNameF.Clear();
        noRepeatNameF.Clear();
    }

    public void Init()
    {
        ClearOld();
        
        DBC.COMMON_DBC<_DBC_RANDOM_NAME> allNames = 
            CDataBaseSystem.Instance.GetDataBase<_DBC_RANDOM_NAME>((int)DataBaseStruct.DBC_RANDOM_NAME);

        foreach (_DBC_RANDOM_NAME item in allNames.StructDict.Values)
        {
            if (item.surname != null && item.surname != "")
            {
                surnames.Add(item.surname);
            }

            if (item.personalName != null && item.personalName != "")
            {
                if (item.sex == 1)
                {
                    if (item.repeat == 1)
                        repeatNameM.Add(item.personalName);
                    else
                        noRepeatNameM.Add(item.personalName);
                }
                else if (item.sex == 0)
                {
                    if (item.repeat == 1)
                        repeatNameF.Add(item.personalName);
                    else
                        noRepeatNameF.Add(item.personalName);
                }
                else
                {
                    if (item.repeat == 1)
                    {
                        repeatNameF.Add(item.personalName);
                        repeatNameM.Add(item.personalName);
                    }
                    else
                    {
                        noRepeatNameF.Add(item.personalName);
                        noRepeatNameM.Add(item.personalName);
                    }
                }
            }
        }
    }

    public string GetName( bool isMale)
    {
        List<string> repeatName = repeatNameM;
        List<string> noRepeatName = noRepeatNameM;
        if (isMale == false)
        {
            repeatName = repeatNameF;
            noRepeatName = noRepeatNameF;
        }

        string name = "";
        name = surnames[UnityEngine.Random.Range(0, surnames.Count)];

        int index = UnityEngine.Random.Range(0, repeatName.Count + noRepeatName.Count);
        if (index < noRepeatName.Count)
        {
            name += noRepeatName[index];
            return name;
        }
        else
        {
            int repeat = UnityEngine.Random.Range(0, 2);
            if (repeat == 0)// 名字不需要两个 [5/2/2012 Ivan]
            {
                name += repeatName[index - noRepeatName.Count];
                return name;
            }
            else
            {
                name += repeatName[index - noRepeatName.Count];
                name += repeatName[UnityEngine.Random.Range(0, repeatName.Count)];
                return name;
            }
        }
    }
}
