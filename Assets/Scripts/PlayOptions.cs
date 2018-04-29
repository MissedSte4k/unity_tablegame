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
    public Text[] teamTitles;
    public Button teamNextButton;
    public GameObject teamLayer, blueCharacterLayer, redCharacterLayer;
    public int characterIndex;
    public Sprite[] blueKnightImages;
    public Sprite[] blueScoutImages;
    public Sprite[] blueBerserkerImages;
    public Sprite[] blueArcherImages;
    public Sprite[] redKnightImages;
    public Sprite[] redScoutImages;
    public Sprite[] redBerserkerImages;
    public Sprite[] redArcherImages; 
    public Button[] blueCharacterButtons;    
    public Button[] redCharacterButtons;
    public Text[] blueCharacterTitles;
    public Text[] redCharacterTitles;
    public Button[] characterNextButtons;
   
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
    public GameObject blueKnightWeaponsLayer, blueScoutWeaponsLayer, blueBerserkerWeaponsLayer, blueArcherWeaponsLayer;


    public static PlayOptions Instance;

    // Use this for initialization
    void Start ()
    {
        DontDestroyOnLoad(gameObject);

        teamIndex = -1;       
        teamButtons[0].image.sprite = teamBlueImages[0];
        teamButtons[1].image.sprite = teamRedImages[0];
        teamTitles[0].enabled = false;
        teamTitles[1].enabled = false;

        characterIndex = -1;
        blueCharacterButtons[0].image.sprite = blueKnightImages[0];
        blueCharacterButtons[1].image.sprite = blueScoutImages[0];
        blueCharacterButtons[2].image.sprite = blueBerserkerImages[0];
        blueCharacterButtons[3].image.sprite = blueArcherImages[0];
        redCharacterButtons[0].image.sprite = redKnightImages[0];
        redCharacterButtons[1].image.sprite = redScoutImages[0];
        redCharacterButtons[2].image.sprite = redBerserkerImages[0];
        redCharacterButtons[3].image.sprite = redArcherImages[0];
        for(int i = 0; i < 4; i++)
        {
            blueCharacterTitles[i].enabled = false;
            redCharacterTitles[i].enabled = false;
        }               
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
        // "Next" button colors in choose team layer
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

       
        if (teamIndex == 0)
        {
            if(characterIndex == -1)
            {
                nextButtonColors.normalColor = new Color32(51, 105, 46, 255);
                characterNextButtons[0].colors = nextButtonColors;
                characterNextButtons[0].interactable = false;
            }
            else
            {
                characterNextButtons[0].interactable = true;
                nextButtonColors.normalColor = new Color32(24, 156, 14, 255);
                characterNextButtons[0].colors = nextButtonColors;
            }
        }
        else if (teamIndex == 1)
        {
            if (characterIndex == -1)
            {
                nextButtonColors.normalColor = new Color32(51, 105, 46, 255);
                characterNextButtons[1].colors = nextButtonColors;
                characterNextButtons[1].interactable = false;
            }
            else
            {
                characterNextButtons[1].interactable = true;
                nextButtonColors.normalColor = new Color32(24, 156, 14, 255);
                characterNextButtons[1].colors = nextButtonColors;
            }
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
            teamTitles[0].enabled = true;
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
            teamTitles[1].enabled = true;
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
            teamTitles[0].enabled = false;
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
            teamTitles[1].enabled = false;
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
            teamTitles[0].enabled = true;
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
            teamTitles[1].enabled = true;
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
        characterIndex = -1;
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


    //changes button icon when highlighted
    public void CharacterButtonPointerEnter(int i)
    {
        if(teamIndex == 0)
        {
            if(i == 0)
            {
                if (characterIndex != 0)
                {
                    blueCharacterButtons[0].image.sprite = blueKnightImages[1];
                }
                else
                {
                    blueCharacterButtons[0].image.sprite = blueKnightImages[4];
                }
            }
            else if (i == 1)
            {
                if (characterIndex != 1)
                {
                    blueCharacterButtons[1].image.sprite = blueScoutImages[1];
                }
                else
                {
                    blueCharacterButtons[1].image.sprite = blueScoutImages[4];
                }
            }
            else if (i == 2)
            {
                if (characterIndex != 2)
                {
                    blueCharacterButtons[2].image.sprite = blueBerserkerImages[1];
                }
                else
                {
                    blueCharacterButtons[2].image.sprite = blueBerserkerImages[4];
                }
            }
            else if (i == 3)
            {
                if (characterIndex != 3)
                {
                    blueCharacterButtons[3].image.sprite = blueArcherImages[1];
                }
                else
                {
                    blueCharacterButtons[3].image.sprite = blueArcherImages[4];
                }
            }
            blueCharacterTitles[i].enabled = true;
        }
        else if(teamIndex == 1)
        {
            if (i == 0)
            {
                if (characterIndex != 0)
                {
                    redCharacterButtons[0].image.sprite = redKnightImages[1];
                }
                else
                {
                    redCharacterButtons[0].image.sprite = redKnightImages[4];
                }
            }
            else if (i == 1)
            {
                if (characterIndex != 1)
                {
                    redCharacterButtons[1].image.sprite = redScoutImages[1];
                }
                else
                {
                    redCharacterButtons[1].image.sprite = redScoutImages[4];
                }
            }
            else if (i == 2)
            {
                if (characterIndex != 2)
                {
                    redCharacterButtons[2].image.sprite = redBerserkerImages[1];
                }
                else
                {
                    redCharacterButtons[2].image.sprite = redBerserkerImages[4];
                }
            }
            else if (i == 3)
            {
                if (characterIndex != 3)
                {
                    redCharacterButtons[3].image.sprite = redArcherImages[1];
                }
                else
                {
                    redCharacterButtons[3].image.sprite = redArcherImages[4];
                }
            }
            redCharacterTitles[i].enabled = true;
        }                
    }

    public void CharacterButtonPointerExit(int i)
    {
        if(teamIndex == 0)
        {
            if (i == 0)
            {
                if(characterIndex != 0)
                {
                    blueCharacterButtons[0].image.sprite = blueKnightImages[0];
                }
                else
                {
                    blueCharacterButtons[0].image.sprite = blueKnightImages[3];
                }
            }
            else if (i == 1)
            {
                if (characterIndex != 1)
                {
                    blueCharacterButtons[1].image.sprite = blueScoutImages[0];
                }
                else
                {
                    blueCharacterButtons[1].image.sprite = blueScoutImages[3];
                }
            }
            else if (i == 2)
            {
                if (characterIndex != 2)
                {
                    blueCharacterButtons[2].image.sprite = blueBerserkerImages[0];
                }
                else
                {
                    blueCharacterButtons[2].image.sprite = blueBerserkerImages[3];
                }
            }
            else if (i == 3)
            {
                if (characterIndex != 3)
                {
                    blueCharacterButtons[3].image.sprite = blueArcherImages[0];
                }
                else
                {
                    blueCharacterButtons[3].image.sprite = blueArcherImages[3];
                }
            }
            blueCharacterTitles[i].enabled = false;
        }
        else if (teamIndex == 1)
        {
            if (i == 0)
            {
                if (characterIndex != 0)
                {
                    redCharacterButtons[0].image.sprite = redKnightImages[0];
                }
                else
                {
                    redCharacterButtons[0].image.sprite = redKnightImages[3];
                }
            }
            else if (i == 1)
            {
                if (characterIndex != 1)
                {
                    redCharacterButtons[1].image.sprite = redScoutImages[0];
                }
                else
                {
                    redCharacterButtons[1].image.sprite = redScoutImages[3];
                }
            }
            else if (i == 2)
            {
                if (characterIndex != 2)
                {
                    redCharacterButtons[2].image.sprite = redBerserkerImages[0];
                }
                else
                {
                    redCharacterButtons[2].image.sprite = redBerserkerImages[3];
                }
            }
            else if (i == 3)
            {
                if (characterIndex != 3)
                {
                    redCharacterButtons[3].image.sprite = redArcherImages[0];
                }
                else
                {
                    redCharacterButtons[3].image.sprite = redArcherImages[3];
                }
            }
            redCharacterTitles[i].enabled = false;
        }                
    }
    // character button icon when mouse pressed down
    public void CharacterButtonPointerDown(int i)
    {
        if(teamIndex == 0)
        {
            if (i == 0)
            {
                if(characterIndex == -1)
                {
                    blueCharacterButtons[0].image.sprite = blueKnightImages[2];
                }
                else if (characterIndex == 0)
                {
                    blueCharacterButtons[0].image.sprite = blueKnightImages[5];
                }
                else if (characterIndex == 1)
                {
                    blueCharacterButtons[0].image.sprite = blueKnightImages[2];
                }
                else if (characterIndex == 2)
                {
                    blueCharacterButtons[0].image.sprite = blueKnightImages[2];
                }
                else if (characterIndex == 3)
                {
                    blueCharacterButtons[0].image.sprite = blueKnightImages[2];
                }
            }
            else if (i == 1)
            {
                if (characterIndex == -1)
                {
                    blueCharacterButtons[1].image.sprite = blueScoutImages[2];
                }
                else if (characterIndex == 0)
                {
                    blueCharacterButtons[1].image.sprite = blueScoutImages[2];
                }
                else if (characterIndex == 1)
                {
                    blueCharacterButtons[1].image.sprite = blueScoutImages[5];
                }
                else if (characterIndex == 2)
                {
                    blueCharacterButtons[1].image.sprite = blueScoutImages[2];
                }
                else if (characterIndex == 3)
                {
                    blueCharacterButtons[1].image.sprite = blueScoutImages[2];
                }
            }
            else if (i == 2)
            {
                if (characterIndex == -1)
                {
                    blueCharacterButtons[2].image.sprite = blueBerserkerImages[2];
                }
                else if (characterIndex == 0)
                {
                    blueCharacterButtons[2].image.sprite = blueBerserkerImages[2];
                }
                else if (characterIndex == 1)
                {
                    blueCharacterButtons[2].image.sprite = blueBerserkerImages[2];
                }
                else if (characterIndex == 2)
                {
                    blueCharacterButtons[2].image.sprite = blueBerserkerImages[5];
                }
                else if (characterIndex == 3)
                {
                    blueCharacterButtons[2].image.sprite = blueBerserkerImages[2];
                }
            }
            else if (i == 3)
            {
                if (characterIndex == -1)
                {
                    blueCharacterButtons[3].image.sprite = blueArcherImages[2];
                }
                else if (characterIndex == 0)
                {
                    blueCharacterButtons[3].image.sprite = blueArcherImages[2];
                }
                else if (characterIndex == 1)
                {
                    blueCharacterButtons[3].image.sprite = blueArcherImages[2];
                }
                else if (characterIndex == 2)
                {
                    blueCharacterButtons[3].image.sprite = blueArcherImages[2];
                }
                else if (characterIndex == 3)
                {
                    blueCharacterButtons[3].image.sprite = blueArcherImages[5];
                }
            }
            blueCharacterTitles[i].enabled = true;
        }
        else if (teamIndex == 1)
        {
            if (i == 0)
            {
                if (characterIndex == -1)
                {
                    redCharacterButtons[0].image.sprite = redKnightImages[2];
                }
                else if (characterIndex == 0)
                {
                    redCharacterButtons[0].image.sprite = redKnightImages[5];
                }
                else if (characterIndex == 1)
                {
                    redCharacterButtons[0].image.sprite = redKnightImages[2];
                }
                else if (characterIndex == 2)
                {
                    redCharacterButtons[0].image.sprite = redKnightImages[2];
                }
                else if (characterIndex == 3)
                {
                    redCharacterButtons[0].image.sprite = redKnightImages[2];
                }
            }
            else if (i == 1)
            {
                if (characterIndex == -1)
                {
                    redCharacterButtons[1].image.sprite = redScoutImages[2];
                }
                else if (characterIndex == 0)
                {
                    redCharacterButtons[1].image.sprite = redScoutImages[2];
                }
                else if (characterIndex == 1)
                {
                    redCharacterButtons[1].image.sprite = redScoutImages[5];
                }
                else if (characterIndex == 2)
                {
                    redCharacterButtons[1].image.sprite = redScoutImages[2];
                }
                else if (characterIndex == 3)
                {
                    redCharacterButtons[1].image.sprite = redScoutImages[2];
                }
            }
            else if (i == 2)
            {
                if (characterIndex == -1)
                {
                    redCharacterButtons[2].image.sprite = redBerserkerImages[2];
                }
                else if (characterIndex == 0)
                {
                    redCharacterButtons[2].image.sprite = redBerserkerImages[2];
                }
                else if (characterIndex == 1)
                {
                    redCharacterButtons[2].image.sprite = redBerserkerImages[2];
                }
                else if (characterIndex == 2)
                {
                    redCharacterButtons[2].image.sprite = redBerserkerImages[5];
                }
                else if (characterIndex == 3)
                {
                    redCharacterButtons[2].image.sprite = redBerserkerImages[2];
                }
            }
            else if (i == 3)
            {
                if (characterIndex == -1)
                {
                    redCharacterButtons[3].image.sprite = redArcherImages[2];
                }
                else if (characterIndex == 0)
                {
                    redCharacterButtons[3].image.sprite = redArcherImages[2];
                }
                else if (characterIndex == 1)
                {
                    redCharacterButtons[3].image.sprite = redArcherImages[2];
                }
                else if (characterIndex == 2)
                {
                    redCharacterButtons[3].image.sprite = redArcherImages[2];
                }
                else if (characterIndex == 3)
                {
                    redCharacterButtons[3].image.sprite = redArcherImages[5];
                }
            }
            redCharacterTitles[i].enabled = true;
        }                
    }

    public void ClickCharacterButton(int i)
    {
        if (teamIndex == 0)
        {
            if (i == 0)
            {
                if(characterIndex == -1)
                {
                    blueCharacterButtons[0].image.sprite = blueKnightImages[3];
                    characterIndex = i;
                }
                else if (characterIndex == 0)
                {
                    blueCharacterButtons[0].image.sprite = blueKnightImages[0];
                    characterIndex = -1;
                }
                else if (characterIndex == 1)
                {
                    blueCharacterButtons[0].image.sprite = blueKnightImages[3];
                    blueCharacterButtons[1].image.sprite = blueScoutImages[0];
                    characterIndex = i;
                }
                else if (characterIndex == 2)
                {
                    blueCharacterButtons[0].image.sprite = blueKnightImages[3];
                    blueCharacterButtons[2].image.sprite = blueBerserkerImages[0];
                    characterIndex = i;
                }
                else if (characterIndex == 3)
                {
                    blueCharacterButtons[0].image.sprite = blueKnightImages[3];
                    blueCharacterButtons[3].image.sprite = blueArcherImages[0];
                    characterIndex = i;
                }
            }            
            else if (i == 1)
            {
                if (characterIndex == -1)
                {
                    blueCharacterButtons[1].image.sprite = blueScoutImages[3];
                    characterIndex = i;
                }
                else if (characterIndex == 0)
                {
                    blueCharacterButtons[1].image.sprite = blueScoutImages[3];
                    blueCharacterButtons[0].image.sprite = blueKnightImages[0];
                    characterIndex = i;
                }
                else if (characterIndex == 1)
                {
                    blueCharacterButtons[1].image.sprite = blueScoutImages[0];                   
                    characterIndex = -1;
                }
                else if (characterIndex == 2)
                {
                    blueCharacterButtons[1].image.sprite = blueScoutImages[3];
                    blueCharacterButtons[2].image.sprite = blueBerserkerImages[0];
                    characterIndex = i;
                }
                else if (characterIndex == 3)
                {
                    blueCharacterButtons[1].image.sprite = blueScoutImages[3];
                    blueCharacterButtons[3].image.sprite = blueArcherImages[0];
                    characterIndex = i;
                }
            }
            else if (i == 2)
            {
                if (characterIndex == -1)
                {
                    blueCharacterButtons[2].image.sprite = blueBerserkerImages[3];
                    characterIndex = i;
                }
                else if (characterIndex == 0)
                {
                    blueCharacterButtons[2].image.sprite = blueBerserkerImages[3];
                    blueCharacterButtons[0].image.sprite = blueKnightImages[0];
                    characterIndex = i;
                }
                else if (characterIndex == 1)
                {
                    blueCharacterButtons[2].image.sprite = blueBerserkerImages[3];
                    blueCharacterButtons[1].image.sprite = blueScoutImages[0];
                    characterIndex = i;
                }
                else if (characterIndex == 2)
                {                    
                    blueCharacterButtons[2].image.sprite = blueBerserkerImages[0];
                    characterIndex = -1;
                }
                else if (characterIndex == 3)
                {
                    blueCharacterButtons[2].image.sprite = blueBerserkerImages[3];
                    blueCharacterButtons[3].image.sprite = blueArcherImages[0];
                    characterIndex = i;
                }
            }
            else if (i == 3)
            {
                if (characterIndex == -1)
                {
                    blueCharacterButtons[3].image.sprite = blueArcherImages[3];
                    characterIndex = i;
                }
                else if (characterIndex == 0)
                {
                    blueCharacterButtons[3].image.sprite = blueArcherImages[3];                   
                    blueCharacterButtons[0].image.sprite = blueKnightImages[0];
                    characterIndex = i;
                }
                else if (characterIndex == 1)
                {
                    blueCharacterButtons[3].image.sprite = blueArcherImages[3];
                    blueCharacterButtons[1].image.sprite = blueScoutImages[0];
                    characterIndex = i;
                }
                else if (characterIndex == 2)
                {
                    blueCharacterButtons[3].image.sprite = blueArcherImages[3];
                    blueCharacterButtons[2].image.sprite = blueBerserkerImages[0];
                    characterIndex = i;
                }
                else if (characterIndex == 3)
                {
                    blueCharacterButtons[3].image.sprite = blueArcherImages[0];                    
                    characterIndex = -1;
                }
            }
        }
        else if (teamIndex == 1)
        {
            if (i == 0)
            {
                if (characterIndex == -1)
                {
                    redCharacterButtons[0].image.sprite = redKnightImages[3];
                    characterIndex = i;
                }
                else if (characterIndex == 0)
                {
                    redCharacterButtons[0].image.sprite = redKnightImages[0];
                    characterIndex = -1;
                }
                else if (characterIndex == 1)
                {
                    redCharacterButtons[0].image.sprite = redKnightImages[3];
                    redCharacterButtons[1].image.sprite = redScoutImages[0];
                    characterIndex = i;
                }
                else if (characterIndex == 2)
                {
                    redCharacterButtons[0].image.sprite = redKnightImages[3];
                    redCharacterButtons[2].image.sprite = redBerserkerImages[0];
                    characterIndex = i;
                }
                else if (characterIndex == 3)
                {
                    redCharacterButtons[0].image.sprite = redKnightImages[3];
                    redCharacterButtons[3].image.sprite = redArcherImages[0];
                    characterIndex = i;
                }
            }
            else if (i == 1)
            {
                if (characterIndex == -1)
                {
                    redCharacterButtons[1].image.sprite = redScoutImages[3];
                    characterIndex = i;
                }
                else if (characterIndex == 0)
                {
                    redCharacterButtons[1].image.sprite = redScoutImages[3];
                    redCharacterButtons[0].image.sprite = redKnightImages[0];
                    characterIndex = i;
                }
                else if (characterIndex == 1)
                {
                    redCharacterButtons[1].image.sprite = redScoutImages[0];
                    characterIndex = -1;
                }
                else if (characterIndex == 2)
                {
                    redCharacterButtons[1].image.sprite = redScoutImages[3];
                    redCharacterButtons[2].image.sprite = redBerserkerImages[0];
                    characterIndex = i;
                }
                else if (characterIndex == 3)
                {
                    redCharacterButtons[1].image.sprite = redScoutImages[3];
                    redCharacterButtons[3].image.sprite = redArcherImages[0];
                    characterIndex = i;
                }
            }
            else if (i == 2)
            {
                if (characterIndex == -1)
                {
                    redCharacterButtons[2].image.sprite = redBerserkerImages[3];
                    characterIndex = i;
                }
                else if (characterIndex == 0)
                {
                    redCharacterButtons[2].image.sprite = redBerserkerImages[3];
                    redCharacterButtons[0].image.sprite = redKnightImages[0];
                    characterIndex = i;
                }
                else if (characterIndex == 1)
                {
                    redCharacterButtons[2].image.sprite = redBerserkerImages[3];
                    redCharacterButtons[1].image.sprite = redScoutImages[0];
                    characterIndex = i;
                }
                else if (characterIndex == 2)
                {
                    redCharacterButtons[2].image.sprite = redBerserkerImages[0];
                    characterIndex = -1;
                }
                else if (characterIndex == 3)
                {
                    redCharacterButtons[2].image.sprite = redBerserkerImages[3];
                    redCharacterButtons[3].image.sprite = redArcherImages[0];
                    characterIndex = i;
                }
            }
            else if (i == 3)
            {
                if (characterIndex == -1)
                {
                    redCharacterButtons[3].image.sprite = redArcherImages[3];
                    characterIndex = i;
                }
                else if (characterIndex == 0)
                {
                    redCharacterButtons[3].image.sprite = redArcherImages[3];
                    redCharacterButtons[0].image.sprite = redKnightImages[0];
                    characterIndex = i;
                }
                else if (characterIndex == 1)
                {
                    redCharacterButtons[3].image.sprite = redArcherImages[3];
                    redCharacterButtons[1].image.sprite = redScoutImages[0];
                    characterIndex = i;
                }
                else if (characterIndex == 2)
                {
                    redCharacterButtons[3].image.sprite = redArcherImages[3];
                    redCharacterButtons[2].image.sprite = redBerserkerImages[0];
                    characterIndex = i;
                }
                else if (characterIndex == 3)
                {
                    redCharacterButtons[3].image.sprite = redArcherImages[0];
                    characterIndex = -1;
                }
            }
        }
      
        CharacterButtonPointerEnter(i);      
    }
}
