using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public GameObject Player;
    public InputField ipaddressField;
    public InputField portField;

    public void Awake()
    {
        ipaddressField.text = "192.168.100.38";
        portField.text = "27015";
    }

    public void StartGame()
    {
        GameObject go = Instantiate(Player);
        Client clientScript = go.GetComponent<Client>();

        clientScript.ipAddress = (ipaddressField.text != "") ? ipaddressField.text : "192.168.100.38";
        clientScript.port = (portField.text != "") ? int.Parse(portField.text) : 27015;

        if (clientScript.port > 65535) clientScript.port = 65535;

        clientScript.Initiate();
    }
}
