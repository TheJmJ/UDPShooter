﻿using UnityEngine;
using System.Text;
using System;
using System.Linq;
using System.Collections.Generic;

public class Client : MonoBehaviour
{
    #region Packet Types (Static)
    public static UInt16 PACKET_HI = BitConverter.ToUInt16(Encoding.Unicode.GetBytes("NEW"), 0);
    public static UInt16 PACKET_ACK = BitConverter.ToUInt16(Encoding.Unicode.GetBytes("ACK"), 0);
    public static UInt16 PACKET_BYE = BitConverter.ToUInt16(Encoding.Unicode.GetBytes("BYE"), 0);
    public static UInt16 PACKET_SNAP = BitConverter.ToUInt16(Encoding.Unicode.GetBytes("SNAP"), 0);
    public static UInt16 PACKET_HIT = BitConverter.ToUInt16(Encoding.Unicode.GetBytes("HIT"), 0);
    #endregion

    public GameObject dummyPrefab;
    Dictionary<int, GameObject> dummies = new Dictionary<int, GameObject>();

    NetCode netCode;

    float lastTick = 0;
    float tickRate = 0.1f;
    public int clientID = -1;

    public string ipAddress = "";
    public int port = 0;

    bool isStarted = false;

    [SerializeField]int lastSnapshotSize = 0;

    public void Initiate()
    {
        netCode = new NetCode(ipAddress, port);
        UInt16 packetType = PACKET_HI;
        netCode.SendMessage(packetType.ToString());
        Debug.Log(PACKET_ACK + " " + PACKET_BYE + " " + PACKET_SNAP + " " + PACKET_HIT + " " + PACKET_HI);
        isStarted = true;
    }
    private void Update()
    {
        if (!isStarted) return;

        if (clientID == -1)
        {

        }

        if (Time.time - lastTick > tickRate)
        {
            lastTick = Time.time;

            SendSnapshot();
        }
        if(netCode.lastReceivedUDPPacket != "")
        {
            ProcessPacket(netCode.lastReceivedUDPPacket);
            netCode.lastReceivedUDPPacket = "";
        }
    }

    void ProcessPacket(string text)
    {
        string[] results = text.Split(' ');

        if (results[0] == PACKET_SNAP.ToString())
        {
            int playerID = Int32.Parse(results[1]);
            if (playerID != clientID)
            {
                GameObject dummyGO;
                if (!dummies.TryGetValue(playerID, out dummyGO))
                {
                    dummyGO = Instantiate(dummyPrefab, null);
                    dummies.Add(playerID, dummyGO);
                }
                dummyGO.transform.position = new Vector3(float.Parse(results[2]), float.Parse(results[3]), float.Parse(results[4]));
            }
        }
        else if(results[0] == PACKET_HI.ToString())
        {
            if (clientID < 0)
            {
                clientID = Int32.Parse(results[1]);
                Debug.Log("Received clientID! Setting it to" + results[1]);
            }
            else
            {
                // We already have a clientID. Send own one
                string message = PACKET_HI.ToString() + " " + clientID.ToString();
                Debug.Log("HELLO> " + message);
                netCode.SendMessage(message);
            }
        }
        else
        {
            //Debug.Log(packetType + "!=" + PACKET_SNAP.ToString());
        }

    }

    void SendSnapshot()
    {
        UInt16 packetType = PACKET_SNAP;
        string positionString = transform.position.x.ToString("F3") + " " + transform.position.y.ToString("F3") + " " + transform.position.z.ToString("F3");

        string message = packetType.ToString() + " " + clientID.ToString() + " " + positionString;
        netCode.SendMessage(message);

        lastSnapshotSize = message.Length * sizeof(Char);
    }
}
