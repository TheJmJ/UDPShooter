using UnityEngine;
using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

public class NetCode
{
    // receiving Thread
    Thread receiveThread;

    public UdpClient client; // Host client
    public IPAddress serverIp; // Server IP Struct
    public string message = ""; // The message we've received
    public string hostIp = "localhost"; // host IP Address xxx.xxx.xxx
    public int hostPort = 65001;
    public IPEndPoint hostEP; //Host end point

    public string lastReceivedUDPPacket = "";
    public string allReceivedUDPPackets = ""; // clean up this from time to time!

    public NetCode(string ip = "localhost", int port = 65001)
    {
        hostIp = ip;
        hostPort = port;
        ConnectToServer();
    }

    void ConnectToServer()
    {
        serverIp = IPAddress.Parse(hostIp);
        hostEP = new IPEndPoint(serverIp, hostPort);

        client = new UdpClient();
        client.Connect(hostEP);
        //client.Client.Blocking = false;
        receiveThread = new Thread( new ThreadStart(ReceiveData));
        receiveThread.IsBackground = true;
        receiveThread.Start();
    }

    // receive thread
    private void ReceiveData()
    {
        // client = new UdpClient(hostPort);
        while (true)
        {
            try
            {
                IPEndPoint anyIP = new IPEndPoint(IPAddress.Any, 0);
                byte[] data = client.Receive(ref anyIP);

                string text = Encoding.UTF8.GetString(data);

                // latest UDPpacket
                lastReceivedUDPPacket = text;

                // ....
                //allReceivedUDPPackets = allReceivedUDPPackets + text;

            }
            catch (Exception err)
            {
                Debug.LogError(err.ToString());
            }
        }
    }

    public IEnumerator AskMessage()
    {
        //string dataMsg = "am: " + language.ToString();

        float timeOut = 100; // 500ms
        float timer = 0;

        //char[] charArray = dataMsg.ToCharArray();
        //byte[] byteArr = new byte[charArray.Length];

        //for (int i = 0; i < charArray.Length; i++)
        //{
        //    byteArr[i] = Convert.ToByte(charArray[i]);
        //}

        //client.Send(byteArr, byteArr.Length);

        var result = client.BeginReceive(new AsyncCallback(ReceiveMessage), client);

        while (!result.IsCompleted || timer < timeOut)
        {
            timer += 0.1f;
            yield return new WaitForSeconds(0.1f);
        }
        if (result.IsCompleted)
        {
            try
            {
                IPEndPoint remoteEP = null;
                byte[] receivedData = client.EndReceive(result, ref remoteEP);
                //dataMsg = Convert.ToString(receivedData);
                //mi.UpdateMessage(dataMsg);
                // EndReceive worked and we have received data and remote endpoint
            }
            catch (Exception ex)
            {
                // EndReceive failed and we ended up here
                Debug.LogError(ex);
            }
        }
        else
        {
            // The operation wasn't completed before the timeout and we're off the hook
            //mi.UpdateMessage("TIMED OUT");
            Debug.LogError("Mission failed. We'll get the packets Next Time");
        }

        result.AsyncWaitHandle.Close();
    }


    public void SendMessage(string message)
    {
        string vectorString = message;

        char[] chararray = vectorString.ToCharArray();
        byte[] byteArr = new byte[chararray.Length];

        for (int i = 0; i < chararray.Length; i++)
        {
            byteArr[i] = Convert.ToByte(chararray[i]);
        }

        client.Send(byteArr, byteArr.Length);
        Debug.Log("Sending Message: " + message);
        //client.BeginReceive(new AsyncCallback(ReceiveMessage), client);
    }

    public void ReceiveMessage(IAsyncResult res)
    {
        try
        {
            byte[] received = client.EndReceive(res, ref hostEP);
            //Debug.Log(System.Text.Encoding.UTF8.GetString(received));          // This only works if we've manually written data into the database.
            Debug.Log(Encoding.GetEncoding("Windows-1252").GetString(received)); // This Encoding seems to read the data from SQLite database correctly
            //Debug.Log(Encoding.ASCII.GetString(received));

            //ProcessReceived(received);
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    //void ProcessReceived(byte[] received)
    //{
    //    string data = Encoding.GetEncoding("Windows-1252").GetString(received);

    //    string[] dataArray = data.Split(' ');
    //    if(dataArray[0] == "78")
    //    {
    //        clientID = dataArray[1];
    //    }

    //}

}
