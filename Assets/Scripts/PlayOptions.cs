using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class PlayOptions : MonoBehaviour
{    
    public int teamIndex;
    public Button[] teamButtons;
    public Sprite[] teamBlueImages;
    public Sprite[] teamRedImages;
    public Button teamNextButton;
    public GameObject teamLayer, blueCharacterLayer, redCharacterLayer;
    public Toggle[] blueCharacterToggles;
    public Button[] blueCharacterButtons;
    public Toggle[] redCharacterToggles;
    public Button[] redCharacterButtons;
    public int characterIndex = 0;
    public Toggle[] primaryBlueKnightWeaponToggles;
    public Toggle[] secondaryBlueKnightWeaponToggles;
    public Toggle[] primaryBlueScoutWeaponToggles;
    public Toggle[] secondaryBlueScoutWeaponToggles;
    public Toggle[] primaryBlueBerserkerWeaponToggles;
    public Toggle[] secondaryBlueBerserkerWeaponToggles;
    public Toggle[] primaryBlueArcherWeaponToggles;
    public Toggle[] secondaryBlueArcherWeaponToggles;
    public Toggle[] primaryRedKnightWeaponToggles;
    public Toggle[] secondaryRedKnightWeaponToggles;
    public Toggle[] primaryRedScoutWeaponToggles;
    public Toggle[] secondaryRedScoutWeaponToggles;
    public Toggle[] primaryRedBerserkerWeaponToggles;
    public Toggle[] secondaryRedBerserkerWeaponToggles;
    public Toggle[] primaryRedArcherWeaponToggles;
    public Toggle[] secondaryRedArcherWeaponToggles;
    public int primaryWeaponIndex = 0;
    public int secondaryWeaponIndex = 0;
    public Button[] primaryBlueKnightWeaponButtons;
    public Button[] secondaryBlueKnightWeaponButtons;
    public Button[] primaryBlueScoutWeaponButtons;
    public Button[] secondaryBlueScoutWeaponButtons;
    public Button[] primaryBlueBerserkerWeaponButtons;
    public Button[] secondaryBlueBerserkerWeaponButtons;
    public Button[] primaryBlueArcherWeaponButtons;
    public Button[] secondaryBlueArcherWeaponButtons;
    public Button[] primaryRedKnightWeaponButtons;
    public Button[] secondaryRedKnightWeaponButtons;
    public Button[] primaryRedScoutWeaponButtons;
    public Button[] secondaryRedScoutWeaponButtons;
    public Button[] primaryRedBerserkerWeaponButtons;
    public Button[] secondaryRedBerserkerWeaponButtons;
    public Button[] primaryRedArcherWeaponButtons;
    public Button[] secondaryRedArcherWeaponButtons;
    public GameObject blueKnightWeaponsLayer, blueScoutWeaponsLayer, blueBerserkerWeaponsLayer, blueArcherWeaponsLayer,
        redKnightWeaponsLayer, redScoutWeaponsLayer, redBerserkerWeaponsLayer, redArcherWeaponsLayer;


    public static PlayOptions Instance;

    // Use this for initialization
    void Start ()
    {
        DontDestroyOnLoad(gameObject);

        teamIndex = -1;
        blueCharacterToggles[0].isOn = true;
        redCharacterToggles[0].isOn = true;

        teamButtons[0].image.sprite = teamBlueImages[0];
        teamButtons[1].image.sprite = teamRedImages[0];
        


        primaryBlueKnightWeaponToggles[0].isOn = true;
        secondaryBlueKnightWeaponToggles[0].isOn = true;
        primaryBlueScoutWeaponToggles[0].isOn = true;
        secondaryBlueScoutWeaponToggles[0].isOn = true;
        primaryBlueBerserkerWeaponToggles[0].isOn = true;
        secondaryBlueBerserkerWeaponToggles[0].isOn = true;
        primaryBlueArcherWeaponToggles[0].isOn = true;
        secondaryBlueArcherWeaponToggles[0].isOn = true;
        primaryRedKnightWeaponToggles[0].isOn = true;
        secondaryRedKnightWeaponToggles[0].isOn = true;
        primaryRedScoutWeaponToggles[0].isOn = true;
        secondaryRedScoutWeaponToggles[0].isOn = true;
        primaryRedBerserkerWeaponToggles[0].isOn = true;
        secondaryRedBerserkerWeaponToggles[0].isOn = true;
        primaryRedArcherWeaponToggles[0].isOn = true;
        secondaryRedArcherWeaponToggles[0].isOn = true;
    }

    public void Awake()
    {
        if (Instance == null)
        {
            Instance = FindObjectOfType<PlayOptions>();
        }
    }

    public void Update()
    {
        ColorBlock nextButtonColors = teamNextButton.colors;
        if (teamIndex == -1)
        {
            nextButtonColors.normalColor = new Color32(51, 105, 46, 255);
            teamNextButton.colors = nextButtonColors;
            teamNextButton.interactable = false;
        }
        else
        {
            teamNextButton.interactable = true;
            nextButtonColors.normalColor = new Color32(24, 156, 14, 255);
            teamNextButton.colors = nextButtonColors;
        }
    }

    //changes button icon when highlighted
    public void TeamButtonPointerEnter(int i)
    {
        if (i == 0)
        {
            if (teamIndex != 0)
            {
                teamButtons[0].image.sprite = teamBlueImages[1];
            }
            else
            {
                teamButtons[0].image.sprite = teamBlueImages[4];
            }
        } 
        else if(i == 1)
        {
            if (teamIndex != 1)
            {
                teamButtons[1].image.sprite = teamRedImages[1];
            }
            else
            {
                teamButtons[1].image.sprite = teamRedImages[4];
            }
        }
    }
    public void TeamButtonPointerExit(int i)
    {
        if (i == 0)
        {
            if (teamIndex != 0)
            {
                teamButtons[0].image.sprite = teamBlueImages[0];
            }
            else
            {
                teamButtons[0].image.sprite = teamBlueImages[3];
            }
        }
        else if (i == 1)
        {
            if (teamIndex != 1)
            {
                teamButtons[1].image.sprite = teamRedImages[0];
            }
            else
            {
                teamButtons[1].image.sprite = teamRedImages[3];
            }
        }
    }
    public void TeamButtonPointerDown(int i)
    {
        if(i == 0)
        {
            if(teamIndex == -1)
            {
                teamButtons[0].image.sprite = teamBlueImages[2];
            }
            else if (teamIndex == 0)
            {
                teamButtons[0].image.sprite = teamBlueImages[5];
            }
            else if (teamIndex == 1)
            {
                teamButtons[0].image.sprite = teamBlueImages[2];
            }
        }
        if (i == 1)
        {
            if (teamIndex == -1)
            {
                teamButtons[1].image.sprite = teamRedImages[2];
            }
            else if (teamIndex == 0)
            {
                teamButtons[1].image.sprite = teamRedImages[2];
            }
            else if (teamIndex == 1)
            {
                teamButtons[1].image.sprite = teamRedImages[5];
            }
        }
    }

    public void ClickTeamButton(int i)
    {        
        if(i == 0)
        {
            if(teamIndex == -1)
            {
                teamButtons[0].image.sprite = teamBlueImages[3];
                teamIndex = i;
            }
            else if (teamIndex == 0)
            {
                teamButtons[0].image.sprite = teamBlueImages[0];
                teamIndex = -1;
            }
            else if (teamIndex == 1)
            {
                teamButtons[0].image.sprite = teamBlueImages[3];
                teamButtons[1].image.sprite = teamRedImages[0];
                teamIndex = i;
            }           
        }
        else if(i == 1)
        {
            if (teamIndex == -1)
            {
                teamButtons[1].image.sprite = teamRedImages[3];
                teamIndex = i;
            }
            else if (teamIndex == 0)
            {
                teamButtons[1].image.sprite = teamRedImages[3];
                teamButtons[0].image.sprite = teamBlueImages[0];

                teamIndex = i;
            }
            else if (teamIndex == 1)
            {
                teamButtons[1].image.sprite = teamRedImages[0];
                teamIndex = -1;
            }
        }
        TeamButtonPointerEnter(i);
    }    

    //Button Next in Choose team layer
    public void ClickNextTeam()
    {
        if(teamIndex == 0)
        {
            blueCharacterLayer.gameObject.SetActive(true);
            teamLayer.gameObject.SetActive(false);
        }
        else if (teamIndex == 1)
        {
            redCharacterLayer.gameObject.SetActive(true);
            teamLayer.gameObject.SetActive(false);
        }
    }

    public void ClickCharacterButton(int i)
    {
        if (teamIndex == 0)
        {
            blueCharacterToggles[i].isOn = true;
            SetCharacterToggle(i);
        }
        else if(teamIndex == 1)
        {
            redCharacterToggles[i].isOn = true;
            SetCharacterToggle(i);
        }           
    }

    public void SetCharacterToggle(int i)
    {
        if(teamIndex == 0)
        {
            if (blueCharacterToggles[i].isOn)
            {
                characterIndex = i;
            }
        }
        else if (teamIndex == 1)
        {
            if (redCharacterToggles[i].isOn)
            {
                characterIndex = i;
            }
        }
    }


    public void ClickPrimaryWeaponButton(int i)
    {
        if (teamIndex == 0)
        {
            if (characterIndex == 0)
            {
                primaryBlueKnightWeaponToggles[i].isOn = true;
                SetWeaponToggle(i);
            }
            else if (characterIndex == 1)
            {
                primaryBlueScoutWeaponToggles[i].isOn = true;
                SetWeaponToggle(i);
            }
            else if (characterIndex == 2)
            {
                primaryBlueBerserkerWeaponToggles[i].isOn = true;
                SetWeaponToggle(i);
            }
            else if (characterIndex == 3)
            {
                primaryBlueArcherWeaponToggles[i].isOn = true;
                SetWeaponToggle(i);
            }
        }
        else if (teamIndex == 1)
        {
            if (characterIndex == 0)
            {
                primaryRedKnightWeaponToggles[i].isOn = true;
                SetWeaponToggle(i);
            }
            else if (characterIndex == 1)
            {
                primaryRedScoutWeaponToggles[i].isOn = true;
                SetWeaponToggle(i);
            }
            else if (characterIndex == 2)
            {
                primaryRedBerserkerWeaponToggles[i].isOn = true;
                SetWeaponToggle(i);
            }
            else if (characterIndex == 3)
            {
                primaryRedArcherWeaponToggles[i].isOn = true;
                SetWeaponToggle(i);
            }
        }
    }

    public void ClickSecondaryWeaponButton(int i)
    {
        if (teamIndex == 0)
        {
            if (characterIndex == 0)
            {
                secondaryBlueKnightWeaponToggles[i].isOn = true;
                SetWeaponToggle(i);
            }
            else if (characterIndex == 1)
            {
                secondaryBlueScoutWeaponToggles[i].isOn = true;
                SetWeaponToggle(i);
            }
            else if (characterIndex == 2)
            {
                secondaryBlueBerserkerWeaponToggles[i].isOn = true;
                SetWeaponToggle(i);
            }
            else if (characterIndex == 3)
            {
                secondaryBlueArcherWeaponToggles[i].isOn = true;
                SetWeaponToggle(i);
            }
        }
        else if (teamIndex == 1)
        {
            if (characterIndex == 0)
            {
                secondaryRedKnightWeaponToggles[i].isOn = true;
                SetWeaponToggle(i);
            }
            else if (characterIndex == 1)
            {
                secondaryRedScoutWeaponToggles[i].isOn = true;
                SetWeaponToggle(i);
            }
            else if (characterIndex == 2)
            {
                secondaryRedBerserkerWeaponToggles[i].isOn = true;
                SetWeaponToggle(i);
            }
            else if (characterIndex == 3)
            {
                secondaryRedArcherWeaponToggles[i].isOn = true;
                SetWeaponToggle(i);
            }
        }
    }

    public void SetWeaponToggle(int i)
    {
        //if team blue
        if (teamIndex == 0)
        {
            //if knight
            if(characterIndex == 0)
            {
                if(primaryBlueKnightWeaponToggles[i].isOn)
                {
                    primaryWeaponIndex = i;
                }
                if (secondaryBlueKnightWeaponToggles[i].isOn)
                {
                    secondaryWeaponIndex = i;
                }
            } //if scout            
            else if (characterIndex == 1)
            {
                if (primaryBlueScoutWeaponToggles[i].isOn)
                {
                    primaryWeaponIndex = i;
                }
                if (secondaryBlueScoutWeaponToggles[i].isOn)
                {
                    secondaryWeaponIndex = i;
                }
            } //if berserker
            else if (characterIndex == 2)
            {
                if (primaryBlueBerserkerWeaponToggles[i].isOn)
                {
                    primaryWeaponIndex = i;
                }
                if (secondaryBlueBerserkerWeaponToggles[i].isOn)
                {
                    secondaryWeaponIndex = i;
                }
            } //if archer
            else if (characterIndex == 3)
            {
                if (primaryBlueArcherWeaponToggles[i].isOn)
                {
                    primaryWeaponIndex = i;
                }
                if (secondaryBlueArcherWeaponToggles[i].isOn)
                {
                    secondaryWeaponIndex = i;
                }
            }            
        } // if team red
        else if (teamIndex == 1)
        {
            //if knight
            if (characterIndex == 0)
            {
                if (primaryRedKnightWeaponToggles[i].isOn)
                {
                    primaryWeaponIndex = i;
                }
                if (secondaryRedKnightWeaponToggles[i].isOn)
                {
                    secondaryWeaponIndex = i;
                }
            } //if scout
            else if (characterIndex == 1)
            {
                if (primaryRedScoutWeaponToggles[i].isOn)
                {
                    primaryWeaponIndex = i;
                }
                if (secondaryRedScoutWeaponToggles[i].isOn)
                {
                    secondaryWeaponIndex = i;
                }
            } //if berserker
            else if (characterIndex == 2)
            {
                if (primaryRedBerserkerWeaponToggles[i].isOn)
                {
                    primaryWeaponIndex = i;
                }
                if (secondaryRedBerserkerWeaponToggles[i].isOn)
                {
                    secondaryWeaponIndex = i;
                }
            } //if archer
            else if (characterIndex == 3)
            {
                if (primaryRedArcherWeaponToggles[i].isOn)
                {
                    primaryWeaponIndex = i;
                }
                if (secondaryRedArcherWeaponToggles[i].isOn)
                {
                    secondaryWeaponIndex = i;
                }
            }
        }
    }

    //Button Next in Character layer
    public void ClickNextCharacter()
    {
        if (teamIndex == 0)
        {
            if (characterIndex == 0)
            {
                blueKnightWeaponsLayer.gameObject.SetActive(true);
            }
            else if (characterIndex == 1)
            {
                blueScoutWeaponsLayer.gameObject.SetActive(true);
            }
            else if (characterIndex == 2)
            {
                blueBerserkerWeaponsLayer.gameObject.SetActive(true);
            }
            else if (characterIndex == 3)
            {
                blueArcherWeaponsLayer.gameObject.SetActive(true);
            }

            blueCharacterLayer.gameObject.SetActive(false);           
        }
        else if (teamIndex == 1)
        {
            if (characterIndex == 0)
            {
                redKnightWeaponsLayer.gameObject.SetActive(true);
            }
            else if (characterIndex == 1)
            {
                redScoutWeaponsLayer.gameObject.SetActive(true);
            }
            else if (characterIndex == 2)
            {
                redBerserkerWeaponsLayer.gameObject.SetActive(true);
            }
            else if (characterIndex == 3)
            {
                redArcherWeaponsLayer.gameObject.SetActive(true);
            }

            redCharacterLayer.gameObject.SetActive(false);
        }
    }

}
