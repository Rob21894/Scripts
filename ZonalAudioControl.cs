using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZonalAudioControl : MonoBehaviour {

    [System.Serializable]
    public class ZonalAudio
    {
        public GameObject Zone;
        [HideInInspector] public string ZoneName;

        [Tooltip("Only Add Sources That will be Effected By Zones")]
        // public List<AudioSource> SfxAudioSources = new List<AudioSource>();
        public AudioSource[] SfxAudioSources;
        [HideInInspector] public float[] tempAudioValues;

    }

    [Tooltip("Enter the amount of zones you will have for this level")]
    public ZonalAudio[] zonalAudio;
    public float fadeSpeed;
    public bool zoneChanged = false;
    private GameObject[] allAudioSources;
    private PlayerZoneControl playerZoneControl;
	void Start ()
	{
	    playerZoneControl = Camera.main.GetComponent<CameraFollow>().player.GetComponent<PlayerZoneControl>();
        InitializeClassValues();

	    zoneChanged = true;

	}
	
	// Update is called once per frame
	void Update ()
    {
        if (zoneChanged)
        {
            StopAllCoroutines();
            StartCoroutine(IncreaseZoneAudioSources(zonalAudio[ReturnAudioZoneNumber(playerZoneControl.currentZone)].SfxAudioSources,
                zonalAudio[ReturnAudioZoneNumber(playerZoneControl.currentZone)].tempAudioValues));
            StartCoroutine(DecreaseZoneAudioSources(allAudioSources));

            zoneChanged = false;
        }
		
	}

    public void InitializeClassValues()
    {
        foreach (ZonalAudio t in zonalAudio)
        {
            t.tempAudioValues = new float[t.SfxAudioSources.Length];
            t.ZoneName = t.Zone.GetComponent<PlayerZoneLink>().zoneName;
        }
        foreach (ZonalAudio t in zonalAudio)
        {
            for (int j = 0; j < t.SfxAudioSources.Length; j++)
            {
                t.tempAudioValues[j] = t.SfxAudioSources[j].volume;
                t.SfxAudioSources[j].mute = true;
                t.SfxAudioSources[j].loop = true;
                t.SfxAudioSources[j].playOnAwake = true;
            }
        }
          allAudioSources = GameObject.FindGameObjectsWithTag("ZonalAudioSources");
    }


    public IEnumerator IncreaseZoneAudioSources(AudioSource[] audioSources, float[] targetaudioValues)
    {
        bool turnUp = false;

        while (!turnUp)
        {
            var checkedVolume = checkTargetVolume(audioSources);

            for (int i = 0; i < audioSources.Length; i++)
            {
                audioSources[i].mute = false;
                if (audioSources[i].volume < targetaudioValues[i])
                {
                    audioSources[i].volume += fadeSpeed * Time.deltaTime;
                }

                if (checkedVolume)
                {
                    for (int j = 0; j < audioSources.Length; j++)
                    {
                        audioSources[j].volume = targetaudioValues[j];
                    }

                    turnUp = true;
                }

            }

            yield return null;
        }
    }

    public IEnumerator DecreaseZoneAudioSources(GameObject[] audioSources)
    {
        bool turnDown = false;

        while (!turnDown)
        {
            foreach (var audioSource in audioSources )
            {
                bool match = false;

                for (int i = 0; i < zonalAudio[ReturnAudioZoneNumber(playerZoneControl.currentZone)].SfxAudioSources.Length; i++)
                {
                    if (audioSource.GetComponent<AudioSource>() ==
                        zonalAudio[ReturnAudioZoneNumber(playerZoneControl.currentZone)].SfxAudioSources[i])
                    {
                        match = true;
                        break;
                        
                    }
                }

                if (!match)
                {
                    audioSource.GetComponent<AudioSource>().volume -= fadeSpeed * Time.deltaTime;
                }
            }

           // turnDown = true;
            yield return null;
        }
    }
    bool checkTargetVolume(AudioSource[] IncreaseVolumeSources)
    {
            for (int j = 0; j < IncreaseVolumeSources.Length; j++)
            {
                if (zonalAudio[ReturnAudioZoneNumber(playerZoneControl.currentZone)].SfxAudioSources[j].volume < zonalAudio[ReturnAudioZoneNumber(playerZoneControl.currentZone)].tempAudioValues[j])
                {
                    return false;
                }
            }
        return true;

    }

    public int ReturnAudioZoneNumber(string currentZone)
    {
        for (int i = 0; i < zonalAudio.Length; i++)
        {
            if (currentZone == zonalAudio[i].ZoneName)
            {
                return i;
            }
        }

        return 0;
    }


}
