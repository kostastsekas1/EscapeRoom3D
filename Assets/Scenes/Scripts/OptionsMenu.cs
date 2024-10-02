using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class OptionsMenu : MonoBehaviour
{

    public AudioMixer mixer;

    Resolution[] resolutions;
    public Dropdown resolutiondDropdown;
    public Dropdown QualityPreset;
    public Slider audioslider;
    public int currenresindex = 0;
    public int qualityindex = 2;
    public float mastervolume = 0;
    public bool isFullscreen = true;


    private void Start()
    {
        resolutions = Screen.resolutions;

        resolutiondDropdown.ClearOptions();
        List<string>listofresolutions= new List<string>();
        
        for(int i = 0;i < resolutions.Length; i++) 
        { 
            string option = resolutions[i].width + " x " + resolutions[i].height;
            listofresolutions.Add(option);

            if (resolutions[i].width==Screen.currentResolution.width && resolutions[i].height == Screen.currentResolution.height)
            {
                currenresindex= i;
            }
          
        }
        
        resolutiondDropdown.AddOptions(listofresolutions);
        resolutiondDropdown.value = currenresindex; 
        resolutiondDropdown.RefreshShownValue();

        mixer.GetFloat("MasterVolume", out float value);
        audioslider.value = value;
        mastervolume= value;
    }

    public void volumeSet(float volume)
    {
        mixer.SetFloat("MasterVolume", volume);
    }

    public void QualitySet(int quality)
    {

        QualitySettings.SetQualityLevel(quality);
        qualityindex = quality;
    }

    public void setFullscreen(bool isfullscreen)
    {
        Screen.fullScreen= isfullscreen;
        isFullscreen= isfullscreen;
    }

    public void setResolution(int resolutionindex) 
    { 
        Resolution resolution = resolutions[resolutionindex];
        Screen.SetResolution(resolution.width, resolution.height,Screen.fullScreen);
    }
       
}

