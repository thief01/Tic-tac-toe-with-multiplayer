using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

public class Connector : MonoBehaviour
{
    public static Connector instance;

    private UdpClient client;
    private Thread connectorThread;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void InitSocket(int port)
    {
        client = new UdpClient(port);
        connectorThread = new Thread(Receiver);
        connectorThread.Start();
    }

    public void Receiver()
    {
        IPEndPoint ip = new IPEndPoint(IPAddress.Any, 0);
        while (true)
        {
            byte[] data = client.Receive(ref ip);
            ReceivedData(Encoding.ASCII.GetString(data), ip);
        }
    }

    public void SerializeAndSend<T>(T t, IPEndPoint ip)
    {
        string json = JsonUtility.ToJson(t);
        Send(ip, json);
    }

    public void Send(IPEndPoint ip, string data)
    {
        byte[] datas = Encoding.ASCII.GetBytes(data);
        client.Send(datas, datas.Length, ip);
    }

    protected virtual void ReceivedData(string data, IPEndPoint ip)
    {

    }
}
