using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IgnorePlayerCollision : MonoBehaviour
{
    private GameObject player;
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        Physics.IgnoreCollision(this.gameObject.GetComponent<Collider>(),player.GetComponent<Collider>(),true);
        
    }
}
