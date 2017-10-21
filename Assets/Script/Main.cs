using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using NetWorkManager;

public class Main : MonoBehaviour
{
    public GameObject Button;
    void Start()
    {
        SocketClient client = new SocketClient();
        client.SendConnect("127.0.0.1",9900);
        client.SendMessage(1, 2, null, "10001");
    }

    // Update is called once per frame
    void Update()
    {       
       
    }
}
