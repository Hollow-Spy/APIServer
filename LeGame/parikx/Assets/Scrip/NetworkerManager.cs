using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;


public class NetworkerManager : MonoBehaviour
{
    public struct UdpState
    {
        public UdpClient u;
        public IPEndPoint e;
    }

    static UdpClient client;
    static IPEndPoint ep;
    static UdpState state;


    // Start is called before the first frame update
    void Start()
    {    
         client = new UdpClient();        //10.0.74.1 marku  // me 10.0.74.126
        ep = new IPEndPoint(IPAddress.Parse("10.0.74.126"), 9050); // endpoint where server is listening (testing localy)
        client.Connect(ep);

        string myMessage = "pisamogu ";
        byte[] array = Encoding.ASCII.GetBytes(myMessage);
        client.Send(array, array.Length);


        client.BeginReceive(ReceiveAsyncCallback, state);


    }





    void ReceiveAsyncCallback(IAsyncResult result)
    {

        byte[] receiveBytes = client.EndReceive(result, ref ep); //get the packet
        
        string receiveString = Encoding.ASCII.GetString(receiveBytes); //decode the packet
        Debug.Log("Received " + receiveString + " from " + ep.ToString()); //display the packet
        client.BeginReceive(ReceiveAsyncCallback, state); //self-callback, meaning this loops infinitely
    }



    // Update is called once per frame
    void Update()
    {
 

    }
}
