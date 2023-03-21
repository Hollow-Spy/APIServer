using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.Net;
using System.Net.Sockets;
using System.Text;
using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

public class NetworkGameObject : MonoBehaviour
{
    [SerializeField] public bool isLocallyOwned;
    [SerializeField] public int uniqueNetworkID;
    [SerializeField] public int localID;
    static int lastAssignedLocalID = 0;
    float Health = 100;
    private void Awake()
    {
       // localID = lastAssignedLocalID++;

        if (isLocallyOwned) localID = lastAssignedLocalID++;


    }


    public byte[] toPlayerShot(int targetID) //convert the relevant info on the gameobject to a packet
    {
        //create a delimited string with the required data
        //note if we put strings in this we might want to check they don’t have a semicolon or use a different delimiter like |
        int Platform = 0; // 0 for unity // 1 unreal
        string returnVal = "Player shot;" + uniqueNetworkID + ";" +
                            targetID + ";" +
                            Platform + ";"
                            ;
        return Encoding.ASCII.GetBytes(returnVal); //we send the information of who shot who and the damage together with the platformer

    }

    public byte[] toPacket() //convert the relevant info on the gameobject to a packet
    {
        //create a delimited string with the required data
        //note if we put strings in this we might want to check they don’t have a semicolon or use a different delimiter like |
        int Platform = 0; // 0 for unity // 1 unreal
        string returnVal = "Object data;" + uniqueNetworkID + ";" +
                            transform.position.x + ";" +
                            transform.position.y + ";" +
                            transform.position.z + ";" +
                            transform.rotation.x + ";" +
                            transform.rotation.y + ";" +
                            transform.rotation.z + ";" +
                            transform.rotation.w + ";" + //sending over rotation infromation and position
                            Platform + ";"
                            ;
        return Encoding.ASCII.GetBytes(returnVal);

    }

    public void fromPacket(string packet) //convert a packet to the relevant data and apply it to the gameobject properties
    {
        string[] values = packet.Split(';');
        if (int.Parse(values[9]) == 1) // in case its unreal
        {
            //we handle the information differently by switching y with z 
            transform.position = new Vector3(float.Parse(values[2]), float.Parse(values[4]), float.Parse(values[3]));
           transform.rotation = new Quaternion(float.Parse(values[5]), float.Parse(values[7]), float.Parse(values[6]), float.Parse(values[8]));
        }
        else
        {//in case of unity
            transform.position = new Vector3(float.Parse(values[2]), float.Parse(values[3]), float.Parse(values[4]));
            transform.rotation = new Quaternion(float.Parse(values[5]), float.Parse(values[6]), float.Parse(values[7]), float.Parse(values[8]));
        }
      

    }

    public void fromPacketShot(string packet) //convert a packet to the relevant data and apply it to the gameobject properties
    {
        string[] values = packet.Split(';');
        if (int.Parse(values[3]) == 1) // in case its unreal
        {
            //we handle the information differently by switching y with z 
          
        }
        else
        {//in case of unity
            Health -= 10;
            Debug.Log("bro " + int.Parse(values[1]) + "Shot " + int.Parse(values[2]) + "for 10 dmg, he has " + Health + "hp left" );
           
            if (Health <= 0)
            {
                Debug.Log("dead");
                gameObject.GetComponent<MeshRenderer>().enabled = false;
              
            }
        }


    }

}
