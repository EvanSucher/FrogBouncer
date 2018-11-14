using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamerFollower : MonoBehaviour {

    [SerializeField]
    GameObject player;

    [SerializeField]
    float smoothTimeX;

    [SerializeField]
    float smoothTimeY;

    private Vector2 velocity;

    // Use this for initialization
    void Start() {

    }
	
	// Update is called once per frame
	void FixedUpdate () {
        float posX = Mathf.SmoothDamp(transform.position.x, player.transform.position.x, ref velocity.x, smoothTimeX);
        float posY = Mathf.SmoothDamp(transform.position.y, player.transform.position.y, ref velocity.y, smoothTimeY);

        transform.position = new Vector3(posX, posY, transform.position.z);
    }
}
