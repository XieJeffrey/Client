﻿using System;using System.Collections;using System.Collections.Generic;public class testBase{	public readonly uint heroID;	public readonly string heroName;	public readonly uint hp;	public readonly float rate;}public class testBaseManager: Singleton<testBaseManager>{	private Dictionary<string, testBase> m_dataList = new Dictionary<string, testBase>();	private readonly long version=131534616182678035;	public int Size	{		get { return m_dataList.Count; }	}	public testBase  Get(int index)	{		if (index > -1 && index < m_dataList.Count)		{			int i = 0;			 foreach (var tmp in m_dataList.Values)			 {				 if (index == i)				 {					return tmp;				 }				 i++;			 }		}		  return null;	}	public testBase Find(uint key1,uint key2=0,uint key3=0)	{		 string key = key1.ToString();		 if (key2 != 0) { key += key2.ToString(); }		 if (key3 != 0) { key += key3.ToString(); }		 if (m_dataList.ContainsKey(key))		 {			return m_dataList[key];		 }		 return null;	}	 public bool Load(string path)	 {		 return TableUtility.instance.Load<testBase>(path,ref m_dataList,version.ToString());	}  }
