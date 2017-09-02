using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerZoneLink : MonoBehaviour
{
    public string zoneName;
    private PlayerZoneControl playerZoneControl;
    private ZonalAudioControl zoneAudioControl;


    void Start()
    {
        playerZoneControl = Camera.main.GetComponent<CameraFollow>().player.GetComponent<PlayerZoneControl>();
        zoneAudioControl = Camera.main.GetComponent<CameraFollow>().gameControl.GetComponent<ZonalAudioControl>();
    }

    void OnTriggerEnter(Collider col)
    {
        if (col.tag == "Player")
        {
            if (playerZoneControl.currentZone != zoneName)
            {
                playerZoneControl.UpdateZone(zoneName);
                zoneAudioControl.zoneChanged = true;
            }
            
        }
    }

}
