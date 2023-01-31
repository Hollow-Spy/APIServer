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
    public List<NetworkGameObject> netObjects;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(SendNetworkUpdates());

        netObjects = new List<NetworkGameObject>();
        netObjects.AddRange(GameObject.FindObjectsOfType<NetworkGameObject>());
        Debug.Log("starting up");


        client = new UdpClient();        //10.0.74.1 marku  // me 10.0.74.126
        ep = new IPEndPoint(IPAddress.Parse("10.0.74.126"), 9050); // endpoint where server is listening (testing localy)
        client.Connect(ep);

        string myMessage = "pisamogu ";
        byte[] array = Encoding.ASCII.GetBytes(myMessage);
        client.Send(array, array.Length);


        client.BeginReceive(ReceiveAsyncCallback, state);

        RequestUIDs();
    }



    void RequestUIDs()
    {

        List<NetworkGameObject> netObjects = new List<NetworkGameObject>();
        netObjects.AddRange(GameObject.FindObjectsOfType<NetworkGameObject>());
        foreach (NetworkGameObject netObject in netObjects)
        {
            if (netObject.isLocallyOwned && netObject.uniqueNetworkID == 0)
            {
                string myMessage = "I need a UID for local object:" + netObject.localID;
                byte[] array = Encoding.ASCII.GetBytes(myMessage);
                client.Send(array, array.Length);
            }
        }
    }


    void ReceiveAsyncCallback(IAsyncResult result)
    {
       


        byte[] receiveBytes = client.EndReceive(result, ref ep); //get the packet
        
        string receiveString = Encoding.ASCII.GetString(receiveBytes); //decode the packet
        Debug.Log("Received " + receiveString + " from " + ep.ToString()); //display the packet

        if (receiveString.Contains("Assigned UID:"))
        {

            int parseFrom = receiveString.IndexOf(':');
            int parseTo = receiveString.LastIndexOf(';');

            //we need to parse the string from the server back into ints to work with
            int localID = Int32.Parse(betweenStrings(receiveString, ":", ";"));
            int globalID = Int32.Parse(receiveString.Substring(receiveString.IndexOf(";") + 1));

            Debug.Log("Got assignment: " + localID + " local to: " + globalID + " global");

            foreach (NetworkGameObject netObject in netObjects)
            {
                //if the local ID sent by the server matches this game object
                if (netObject.localID == localID)
                {
                    Debug.Log(localID + " : " + globalID);
                    //the global ID becomes the server-provided value
                    netObject.uniqueNetworkID = globalID;
                }
            }
        }

        //continue to loop
      
        client.BeginReceive(ReceiveAsyncCallback, state); //self-callback, meaning this loops infinitely
    }


    public static String betweenStrings(String text, String start, String end)
    {
        int p1 = text.IndexOf(start) + start.Length;
        int p2 = text.IndexOf(end, p1);

        if (end == "") return (text.Substring(p1));
        else return text.Substring(p1, p2 - p1);
    }

    IEnumerator SendNetworkUpdates()
    {
        while (true)
        {
            List<NetworkGameObject> netObjects = new List<NetworkGameObject>();
            netObjects.AddRange(GameObject.FindObjectsOfType<NetworkGameObject>());

            foreach (NetworkGameObject netObject in netObjects)
            {
                if (netObject.isLocallyOwned)
                {
                    client.Send(netObject.toPacket(), netObject.toPacket().Length);
                   
                }
            }

            yield return new WaitForSeconds(0.2f);
        }
    }

    // Update is called once per frame
    void Update()
    {
 

    }
}
