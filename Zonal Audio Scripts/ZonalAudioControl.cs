using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZonalAudioControl : MonoBehaviour {

    [System.Serializable]
    public class ZonalAudio
    {
        [Tooltip("Renames Element for organisational Purposes")]
        public string ZoneName;
        [HideInInspector] public string _ZoneName;
        public GameObject Zone;

        [Tooltip("Only Add Sources That will be Effected By Zones")]
        // public List<AudioSource> SfxAudioSources = new List<AudioSource>();
        public AudioSource[] SfxAudioSources;
        [HideInInspector] public float[] TempAudioValues;

    }

    [Tooltip("Enter the amount of zones you will have for this level")]
    public ZonalAudio[] zonalAudio;
    public float fadeSpeed;
    public bool ZoneChanged;
    private GameObject[] _allAudioSources;
    private PlayerZoneControl _playerZoneControl;
	void Start ()
	{
	    _playerZoneControl = Camera.main.GetComponent<CameraFollow>().player.GetComponent<PlayerZoneControl>();
        InitializeClassValues();

	    ZoneChanged = true;

	}
	
	// Update is called once per frame
	void Update ()
    {
        if (ZoneChanged)
        {
            StopAllCoroutines();
            StartCoroutine(IncreaseZoneAudioSources(zonalAudio[ReturnAudioZoneNumber(_playerZoneControl.currentZone)].SfxAudioSources,
                zonalAudio[ReturnAudioZoneNumber(_playerZoneControl.currentZone)].TempAudioValues));
            StartCoroutine(DecreaseZoneAudioSources(_allAudioSources));

            ZoneChanged = false;
        }
		
	}

    public void InitializeClassValues()
    {
        foreach (ZonalAudio t in zonalAudio)
        {
            t.TempAudioValues = new float[t.SfxAudioSources.Length];
            t._ZoneName = t.Zone.GetComponent<PlayerZoneLink>().zoneName;
        }
        foreach (ZonalAudio t in zonalAudio)
        {
            for (int j = 0; j < t.SfxAudioSources.Length; j++)
            {
                t.TempAudioValues[j] = t.SfxAudioSources[j].volume;
                t.SfxAudioSources[j].mute = true;
                t.SfxAudioSources[j].loop = true;
                t.SfxAudioSources[j].playOnAwake = true;
            }
        }
          _allAudioSources = GameObject.FindGameObjectsWithTag("ZonalAudioSources");
    }


    public IEnumerator IncreaseZoneAudioSources(AudioSource[] audioSources, float[] targetaudioValues)
    {
        bool turnUp = false;

        while (!turnUp)
        {
            var checkedVolume = CheckTargetVolume(audioSources);

            for (int i = 0; i < audioSources.Length; i++)
            {
                audioSources[i].mute = false;
                if (audioSources[i].volume < targetaudioValues[i])
                {
                    audioSources[i].volume += fadeSpeed * Time.deltaTime;
                }

                if (!checkedVolume) continue;
                for (int j = 0; j < audioSources.Length; j++)
                {
                    audioSources[j].volume = targetaudioValues[j];
                }

                turnUp = true;
            }

            yield return null;
        }
    }

    public IEnumerator DecreaseZoneAudioSources(GameObject[] audioSources)
    {
        var turnDown = false;

        while (!turnDown)
        {
            foreach (var audioSource in audioSources )
            {
                bool match = false;

                for (int i = 0; i < zonalAudio[ReturnAudioZoneNumber(_playerZoneControl.currentZone)].SfxAudioSources.Length; i++)
                {
                    if (audioSource.GetComponent<AudioSource>() ==
                        zonalAudio[ReturnAudioZoneNumber(_playerZoneControl.currentZone)].SfxAudioSources[i])
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
            yield return null;
        }
    }

    public bool CheckTargetVolume(AudioSource[] IncreaseVolumeSources)
    {
            for (int j = 0; j < IncreaseVolumeSources.Length; j++)
            {
                if (zonalAudio[ReturnAudioZoneNumber(_playerZoneControl.currentZone)].SfxAudioSources[j].volume < zonalAudio[ReturnAudioZoneNumber(_playerZoneControl.currentZone)].TempAudioValues[j])
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
            if (currentZone == zonalAudio[i]._ZoneName)
            {
                return i;
            }
        }

        return 0;
    }


}
