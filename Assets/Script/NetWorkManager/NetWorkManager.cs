using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NetworkCommsDotNet;
using NetworkCommsDotNet.Tools;
using NetworkCommsDotNet.Connections;
using NetworkCommsDotNet.Connections.TCP;
using NetworkCommsDotNet.DPSBase;
using System.Net;
using System.Net.Sockets;

public class NetworkManager : MonoBehaviour {

	// Use this for initialization
	void Start () {
        ConnectionInfo info = new ConnectionInfo("192.168.5.101", 15771);
        Connection connection =  TCPConnection.GetConnection(info);
        connection.SendObject("ChatMessage", new ChatMessage(NetworkComms.NetworkIdentifier, "Maplezxc", "Test",1));
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    void ReceiveMsg(string packetType)
    {

    }
}
