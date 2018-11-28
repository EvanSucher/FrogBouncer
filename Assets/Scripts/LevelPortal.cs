using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelPortal : MonoBehaviour {


    [SerializeField]
    string levelName;
	// Use this for initialization
	void Start ()
    {
        if(levelName == null)
        {
            levelName = "Level1";
        }
		
	}
	
	// Update is called once per frame
	void Update ()
    {
		
	}

    void OnTriggerEnter2D(Collider2D other) // function called when there's a collision with the wall
    {
        //Debug.Log("TOUCHDOWN");
        if (other.gameObject.tag == "Player")
        {
            Debug.Log("TOUCHDOWN");
            SceneManager.LoadScene(levelName);
        }
    }

}
