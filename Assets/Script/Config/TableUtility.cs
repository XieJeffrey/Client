﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

using System.Reflection;

public class TableUtility : Singleton<TableUtility>
{
    public bool Load<T>(string path, ref Dictionary<string, T> outPutData,string version) where T : class
    {
        try
        {
            byte[] data = File.ReadAllBytes(path);
            string tblVersion = ByteUtility.ReadString(ref data);
            if (tblVersion != version)
            {
                Util.LogError("{0} 读取失败，版本错误",path);
                return false ;
            }
            string[] typeStr = ByteUtility.ReadString(ref data).Split('|');
            string[] memberStr = ByteUtility.ReadString(ref data).Split('|');

            while (data.Length > 0)
            {
                T tmp = Activator.CreateInstance<T>();
                Type type = tmp.GetType();
                FieldInfo propertyInfo = null;
                string keyStr = ByteUtility.ReadString(ref data);
                for (int i = 0; i < typeStr.Length; i++)
                {
                    switch (typeStr[i])
                    {
                        case "uint":
                            propertyInfo = type.GetField(memberStr[i]);
                            propertyInfo.SetValue(tmp, ByteUtility.ReadUint(ref data));
                            break;
                        case "string":
                            propertyInfo = type.GetField(memberStr[i]);
                            propertyInfo.SetValue(tmp, ByteUtility.ReadString(ref data));
                            break;
                        case "float":
                            propertyInfo = type.GetField(memberStr[i]);
                            propertyInfo.SetValue(tmp, ByteUtility.ReadFloat(ref data));
                            break;
                        case "short":
                            propertyInfo = type.GetField(memberStr[i]);
                            propertyInfo.SetValue(tmp, ByteUtility.ReadShort(ref data));
                            break;
                    }
                }
                if (!outPutData.ContainsKey(keyStr))
                {
                    outPutData.Add(keyStr, tmp);
                }
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e.StackTrace);
            return false;
        }
        return true;
    }


}