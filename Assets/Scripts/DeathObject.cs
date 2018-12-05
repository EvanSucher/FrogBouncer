using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathObject : MonoBehaviour {

    [SerializeField]
    FrogController frog;

    // Use this for initialization
    void Start ()
    {
        //This code Automatically finds the player and sets the frog controller
        GameObject[] playersList = GameObject.FindGameObjectsWithTag("Player");
        frog = playersList[0].GetComponent<FrogController>();

    }

    void OnTriggerEnter2D(Collider2D other) // function called when there's a collision with the wall
    {
        if (other.gameObject.tag == "Player")
        {
            // Kills the player
            frog.DeathReset();
        }
    }
}
