using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerZoneControl : MonoBehaviour
{

    public string currentZone;
	// Use this for initialization
	void Start ()
    {
        if (currentZone == null)
        {
            
        }
		
	}
	
	// Update is called once per frame
	void Update ()
    {
		
	}

    public void UpdateZone(string zone)
    {
        currentZone = zone;
    }
}
