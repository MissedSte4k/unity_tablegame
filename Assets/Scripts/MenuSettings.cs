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
    public Slider masterVolumeSlider;
    public float masterVolume;

    public Slider mouseSensitivitySlider;
    public InputField mouseSensitivityField;
    public float mouseSensitivity;

    public Slider FoVSlider;
    public InputField FoVField;
    public float fieldOfView;
   
    public GameObject[] keybindButtons;

    public static MenuSettings Instance;
   
    
    //Loads saved user settings
    public void Start()
    {      
        DontDestroyOnLoad(gameObject);

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

        //master volume slider
        if (PlayerPrefs.HasKey("master volume"))
        {
            masterVolumeSlider.value = PlayerPrefs.GetFloat("master volume");
            masterVolume = PlayerPrefs.GetFloat("master volume");
        }      

        //Mouse sensitivity slider & input field
        if (PlayerPrefs.HasKey("mouse sensitivity"))
        {
            mouseSensitivitySlider.value = PlayerPrefs.GetFloat("mouse sensitivity");
            mouseSensitivityField.text = PlayerPrefs.GetFloat("mouse sensitivity").ToString();
        }

        //FoV slider & input field
        if (PlayerPrefs.HasKey("field of view"))
        {
            FoVSlider.value = PlayerPrefs.GetFloat("field of view");
            FoVField.text = PlayerPrefs.GetFloat("field of view").ToString();
        }
    }

    public void Awake()
    {
        if (Instance == null)
        {
            Instance = FindObjectOfType<MenuSettings>();
        }
    }

    public void Update()
    {
        AudioListener.volume = masterVolume;
    }


    //Main menu
    public void PlayGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 2);
    }
    public void QuitGame()
    {
        Application.Quit();
    }


    //Resolution settings
    public void SetScreenResolution(int i)
    {
        if (i == 3 && resolutionToggles[i].isOn)
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

    //master volume settings
    public void SetMasterVolume(float volume)
    {
        masterVolume = volume;
        PlayerPrefs.SetFloat("master volume", volume);
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
        if (mouseSensitivityField)
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

    //Field of View
    //gets FoV value from slider (will be used in character control)
    public void SetFoV(float fov)
    {
        fieldOfView = fov;
        if (FoVSlider)
        {
            FoVSlider.value = fov;
        }
        if (FoVField)
        {
            FoVField.text = fov.ToString();
        }
        PlayerPrefs.SetFloat("field of view", fov);
        PlayerPrefs.Save();
    }

    //gets FoV value from Input Field (will be used in character control)
    public void SetFoVFromInputField(string fov)
    {
        fieldOfView = float.Parse(fov);
        if (FoVSlider)
        {
            FoVSlider.value = float.Parse(fov);
        }
        if (FoVField)
        {
            FoVField.text = fov;
        }
        PlayerPrefs.SetFloat("field of view", float.Parse(fov));
        PlayerPrefs.Save();
    }
}