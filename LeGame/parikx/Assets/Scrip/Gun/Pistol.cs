using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pistol : MonoBehaviour
{
    [SerializeField] Camera cam;
    [SerializeField] int Damage;
    
    private void Update()
    {
        RaycastHit hit;
        

        if (Input.GetMouseButtonDown(0) && Physics.Raycast(cam.transform.position,cam.transform.forward, out hit,Mathf.Infinity))
        {
            if(hit.transform.CompareTag("Player"))
            {
                PlayerShot(hit.transform.GetComponent<NetworkGameObject>().uniqueNetworkID); //if we hit something with a player tag we'll send over our unique id and the damage
            }
          
        }
    }

  
    void PlayerShot(int TargetID)
    {
        NetworkerManager.EventPlayerShot(TargetID); //sends over to networkmanager the data we need to make a shooting event

    }
}
