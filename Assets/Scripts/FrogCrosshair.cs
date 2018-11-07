using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FrogCrosshair : MonoBehaviour {

    [SerializeField]
    GameObject frog;

    [SerializeField]
    GameObject mouseCrosshair;

    [SerializeField]
    float frogCrosshairDist;

    Vector3 AimVector;

	// Use this for initialization
	void Start () {
        frogCrosshairDist = frogCrosshairDist * 0.1f;
	}
	
	// Update is called once per frame
	void Update ()
    {
        AimVector = mouseCrosshair.transform.position - frog.transform.position;
        AimVector = AimVector.normalized*frogCrosshairDist;
        this.transform.position = frog.transform.position + AimVector;
	}

    public bool isPointingRight()
    {
        float comparison = this.transform.position.x - frog.transform.position.x;
        if (comparison > 0)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public Vector2 GetNormalizedAimVector()
    {
        Vector2 tempVector = new Vector2(AimVector.x, AimVector.y);
        return AimVector;
    }
}
