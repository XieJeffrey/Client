using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public interface IDataUtil {
    public  string storeKey {
        get;
    }
    public  void Save();

    public  void Load();


    public void Init();        

    public void EveryDataTask();

}

