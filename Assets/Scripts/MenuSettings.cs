using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Audio;
using System;

public class MenuSettings : MonoBehaviour
{
    public Toggle[] resolutionToggles;
    public int[] screenWidths;

    public Dropdown graphicsDropdown;

    public AudioMixer menuAudioMixer;
    public Slider menuVolumeSlider;
   
    public Slider mouseSensitivitySlider;
    public InputField mouseSensitivityField;
    public float mouseSensitivity;

    private GameObject[] keybindButtons;


    public static MenuSettings instance;

    //Loads saved user settings
    public void Start()
    {
        instance = this;

        //Resolutiom       
        if (PlayerPrefs.HasKey("screen res index"))
        {
            int activeScreenResolutionIndex = PlayerPrefs.GetInt("screen res index");
            for (int i = 0; i < resolutionToggles.Length; i++)
            {
                resolutionToggles[i].isOn = i == activeScreenResolutionIndex;
            }
        }

        //Graphics                                                                          
        if (PlayerPrefs.HasKey("graphics index"))
        {
            int activeGraphicsIndex = PlayerPrefs.GetInt("graphics index");
            QualitySettings.SetQualityLevel(activeGraphicsIndex);
            graphicsDropdown.value = activeGraphicsIndex;          
            graphicsDropdown.RefreshShownValue();
        }

        //Menu music slider
        if (PlayerPrefs.HasKey("menu volume"))
        {
            menuVolumeSlider.value = PlayerPrefs.GetFloat("menu volume");
        }        

        //Mouse sensitivity slider & input field
        if (PlayerPrefs.HasKey("mouse sensitivity"))
        {
            mouseSensitivitySlider.value = PlayerPrefs.GetFloat("mouse sensitivity");
            mouseSensitivityField.text = PlayerPrefs.GetFloat("mouse sensitivity").ToString();
        }         
        
             
    }


    public void Awake()
    {
        keybindButtons = GameObject.FindGameObjectsWithTag("Keybind");
    }


    //Main menu
    public void PlayGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
    public void QuitGame()
    {
        Application.Quit();
    }

 
    //Resolution settings
    public void SetScreenResolution(int i)
    {
        if(i == 3 && resolutionToggles[i].isOn)
        {
            Resolution[] allResolutions = Screen.resolutions;
            Resolution maxResolution = allResolutions[allResolutions.Length - 1];
            Screen.SetResolution(maxResolution.width, maxResolution.height, true);
        }
        else
        {
            if (resolutionToggles[i].isOn)
            {                
                float aspectRatio = 16 / 9f;
                Screen.SetResolution(screenWidths[i], (int)(screenWidths[i] / aspectRatio), false);              
            }
        }
        PlayerPrefs.SetInt("screen res index", i);
        PlayerPrefs.Save();
    }


    //Graphics settings
    public void SetQuality(int qualityIndex)
    {
        QualitySettings.SetQualityLevel(qualityIndex);
        PlayerPrefs.SetInt("graphics index", qualityIndex);
        PlayerPrefs.Save();
    }

    //Menu music settings
    public void SetMenuVolume(float volume)
    {      
        menuAudioMixer.SetFloat("MenuVolume", volume);
        PlayerPrefs.SetFloat("menu volume", volume);
        PlayerPrefs.Save();
    }
   

    //Mouse sensitivity
    //gets sensitivity value from slider (will be used in character control)
    public void SetMouseSensitivity(float sensitivity)
    {
        mouseSensitivity = sensitivity;
        if (mouseSensitivitySlider)
        {
            mouseSensitivitySlider.value = sensitivity;
        }
        if (mouseSensitivityField)
        {
            mouseSensitivityField.text = sensitivity.ToString();
        }
        PlayerPrefs.SetFloat("mouse sensitivity", sensitivity);
        PlayerPrefs.Save();
    }

    //gets sensitivity value from Input Field (will be used in character control)
    public void SetMouseSensitivityFromInputField(string sensitivity)
    {
        mouseSensitivity = float.Parse(sensitivity);
        if (mouseSensitivitySlider)
        {
            mouseSensitivitySlider.value = float.Parse(sensitivity);
        }
        if(mouseSensitivityField)
        {
            mouseSensitivityField.text = sensitivity;
        }
        PlayerPrefs.SetFloat("mouse sensitivity", float.Parse(sensitivity));
        PlayerPrefs.Save();
    }

    //Keybinds
    public void UpdateKeyText(string key, KeyCode code)
    {
        Text tmp = Array.Find(keybindButtons, x => x.name == key).GetComponentInChildren<Text>();
        tmp.text = code.ToString();
    }
}

