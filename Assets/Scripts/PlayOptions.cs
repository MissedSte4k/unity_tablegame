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
    public GameObject KnightWeaponsLayer, ScoutWeaponsLayer, BerserkerWeaponsLayer, ArcherWeaponsLayer;

    public int primaryWeaponIndex;
    public int secondaryWeaponIndex;
    public Button[] primaryKnightWeaponButtons;
    public Button[] secondaryKnightWeaponButtons;
    public Button[] primaryScoutWeaponButtons;
    public Button[] secondaryScoutWeaponButtons;
    public Button[] primaryBerserkerWeaponButtons;
    public Button[] secondaryBerserkerWeaponButtons;
    public Button[] primaryArcherWeaponButtons;
    public Button[] secondaryArcherWeaponButtons;
    public Sprite[] primaryKnightWeaponImages;
    public Sprite[] secondaryKnightWeaponImages;
    public Sprite[] primaryScoutWeaponImages;
    public Sprite[] secondaryScoutWeaponImages;
    public Sprite[] primaryBerserkerWeaponImages;
    public Sprite[] secondaryBerserkerWeaponImages;
    public Sprite[] primaryArcherWeaponImages;
    public Sprite[] secondaryArcherWeaponImages;
    public Text[] primaryKnightWeaponTitles;
    public Text[] secondaryKnightWeaponTitles;
    public Text[] primaryScoutWeaponTitles;
    public Text[] secondaryScoutWeaponTitles;
    public Text[] primaryBerserkerWeaponTitles;
    public Text[] secondaryBerserkerWeaponTitles;
    public Text[] primaryArcherWeaponTitles;
    public Text[] secondaryArcherWeaponTitles;
    public Button[] weaponPlayButtons;
    
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

        primaryWeaponIndex = -1;
        secondaryWeaponIndex = -1;
        int k = 0;
        for(int i = 0; i < 5; i++)
        {
            k = i * 5;
            primaryKnightWeaponButtons[i].image.sprite = primaryKnightWeaponImages[k];
            secondaryKnightWeaponButtons[i].image.sprite = secondaryKnightWeaponImages[k];
            primaryScoutWeaponButtons[i].image.sprite = primaryScoutWeaponImages[k];
            secondaryScoutWeaponButtons[i].image.sprite = secondaryScoutWeaponImages[k];
            primaryBerserkerWeaponButtons[i].image.sprite = primaryBerserkerWeaponImages[k];           
            primaryArcherWeaponButtons[i].image.sprite = primaryArcherWeaponImages[k];
            secondaryArcherWeaponButtons[i].image.sprite = secondaryArcherWeaponImages[k];

            primaryKnightWeaponTitles[i].enabled = false;
            secondaryKnightWeaponTitles[i].enabled = false;
            primaryScoutWeaponTitles[i].enabled = false;
            secondaryScoutWeaponTitles[i].enabled = false;
            primaryBerserkerWeaponTitles[i].enabled = false;            
            primaryArcherWeaponTitles[i].enabled = false;
            secondaryArcherWeaponTitles[i].enabled = false;
        }
        //berserker has 4 available secondary weapons
        for (int i = 0; i < 4; i++)
        {
            k = i * 5;
            secondaryBerserkerWeaponButtons[i].image.sprite = secondaryBerserkerWeaponImages[k];

            secondaryBerserkerWeaponTitles[i].enabled = false;
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

        ColorBlock playButtonColors = teamNextButton.colors;
        if(characterIndex == 0)
        {
            if(primaryWeaponIndex == -1 || secondaryWeaponIndex == -1)
            {
                playButtonColors.normalColor = new Color32(51, 105, 46, 255);
                weaponPlayButtons[0].colors = playButtonColors;
                weaponPlayButtons[0].interactable = false;
            }
            else
            {
                weaponPlayButtons[0].interactable = true;
                playButtonColors.normalColor = new Color32(24, 156, 14, 255);
                weaponPlayButtons[0].colors = playButtonColors;
            }
        }
        else if (characterIndex == 1)
        {
            if (primaryWeaponIndex == -1 || secondaryWeaponIndex == -1)
            {
                playButtonColors.normalColor = new Color32(51, 105, 46, 255);
                weaponPlayButtons[1].colors = playButtonColors;
                weaponPlayButtons[1].interactable = false;
            }
            else
            {
                weaponPlayButtons[1].interactable = true;
                playButtonColors.normalColor = new Color32(24, 156, 14, 255);
                weaponPlayButtons[1].colors = playButtonColors;
            }
        }
        else if (characterIndex == 2)
        {
            if (primaryWeaponIndex == -1 || secondaryWeaponIndex == -1)
            {
                playButtonColors.normalColor = new Color32(51, 105, 46, 255);
                weaponPlayButtons[2].colors = playButtonColors;
                weaponPlayButtons[2].interactable = false;
            }
            else
            {
                weaponPlayButtons[2].interactable = true;
                playButtonColors.normalColor = new Color32(24, 156, 14, 255);
                weaponPlayButtons[2].colors = playButtonColors;
            }
        }
        else if (characterIndex == 3)
        {
            if (primaryWeaponIndex == -1 || secondaryWeaponIndex == -1)
            {
                playButtonColors.normalColor = new Color32(51, 105, 46, 255);
                weaponPlayButtons[3].colors = playButtonColors;
                weaponPlayButtons[3].interactable = false;
            }
            else
            {
                weaponPlayButtons[3].interactable = true;
                playButtonColors.normalColor = new Color32(24, 156, 14, 255);
                weaponPlayButtons[3].colors = playButtonColors;
            }
        }

    }

    //----------------------------------------------------------------------------------------------------------------------------------------

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

    public void ClickBackTeamButton()
    {      
        int k = 0;
        for (int i = 0; i < 5; i++)
        {
            k = i * 5;
            primaryKnightWeaponButtons[i].image.sprite = primaryKnightWeaponImages[k];
            secondaryKnightWeaponButtons[i].image.sprite = secondaryKnightWeaponImages[k];
            primaryScoutWeaponButtons[i].image.sprite = primaryScoutWeaponImages[k];
            secondaryScoutWeaponButtons[i].image.sprite = secondaryScoutWeaponImages[k];
            primaryBerserkerWeaponButtons[i].image.sprite = primaryBerserkerWeaponImages[k];
            primaryArcherWeaponButtons[i].image.sprite = primaryArcherWeaponImages[k];
            secondaryArcherWeaponButtons[i].image.sprite = secondaryArcherWeaponImages[k];                
        }
        //berserker has 4 available secondary weapons
        for (int i = 0; i < 4; i++)
        {
            k = i * 5;
            secondaryBerserkerWeaponButtons[i].image.sprite = secondaryBerserkerWeaponImages[k];
        }       
    }

    //----------------------------------------------------------------------------------------------------------------------------------------

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

    //Button Next in Choose character layer
    public void ClickNextCharacter()
    {
        if (teamIndex == 0)
        {
            if (characterIndex == 0)
            {
                KnightWeaponsLayer.gameObject.SetActive(true);
                blueCharacterLayer.gameObject.SetActive(false);
            }
            else if (characterIndex == 1)
            {
                ScoutWeaponsLayer.gameObject.SetActive(true);
                blueCharacterLayer.gameObject.SetActive(false);
            }
            else if (characterIndex == 2)
            {
                BerserkerWeaponsLayer.gameObject.SetActive(true);
                blueCharacterLayer.gameObject.SetActive(false);
            }
            else if (characterIndex == 3)
            {
                ArcherWeaponsLayer.gameObject.SetActive(true);
                blueCharacterLayer.gameObject.SetActive(false);
            }
        }
        else if (teamIndex == 1)
        {
            if (characterIndex == 0)
            {
                KnightWeaponsLayer.gameObject.SetActive(true);
                redCharacterLayer.gameObject.SetActive(false);
            }
            else if (characterIndex == 1)
            {
                ScoutWeaponsLayer.gameObject.SetActive(true);
                redCharacterLayer.gameObject.SetActive(false);
            }
            else if (characterIndex == 2)
            {
                BerserkerWeaponsLayer.gameObject.SetActive(true);
                redCharacterLayer.gameObject.SetActive(false);
            }
            else if (characterIndex == 3)
            {
                ArcherWeaponsLayer.gameObject.SetActive(true);
                redCharacterLayer.gameObject.SetActive(false);
            }
        }
    }

    //----------------------------------------------------------------------------------------------------------------------------------------

    public void PrimaryWeaponButtonPointerEnter(int i)
    {
        //Knight
        if(characterIndex == 0)
        {
            if(i == 0)
            {
                if(primaryWeaponIndex != 0)
                {
                    primaryKnightWeaponButtons[0].image.sprite = primaryKnightWeaponImages[1];
                }
                else
                {
                    primaryKnightWeaponButtons[0].image.sprite = primaryKnightWeaponImages[3];
                }                
            }
            else if (i == 1)
            {
                if (primaryWeaponIndex != 1)
                {
                    primaryKnightWeaponButtons[1].image.sprite = primaryKnightWeaponImages[6];
                }
                else
                {
                    primaryKnightWeaponButtons[1].image.sprite = primaryKnightWeaponImages[8];
                }
            }
            else if (i == 2)
            {
                if (primaryWeaponIndex != 2)
                {
                    primaryKnightWeaponButtons[2].image.sprite = primaryKnightWeaponImages[11];
                }
                else
                {
                    primaryKnightWeaponButtons[2].image.sprite = primaryKnightWeaponImages[13];
                }
            }
            else if (i == 3)
            {
                if (primaryWeaponIndex != 3)
                {
                    primaryKnightWeaponButtons[3].image.sprite = primaryKnightWeaponImages[16];
                }
                else
                {
                    primaryKnightWeaponButtons[3].image.sprite = primaryKnightWeaponImages[18];
                }
            }
            else if (i == 4)
            {
                if (primaryWeaponIndex != 4)
                {
                    primaryKnightWeaponButtons[4].image.sprite = primaryKnightWeaponImages[21];
                }
                else
                {
                    primaryKnightWeaponButtons[4].image.sprite = primaryKnightWeaponImages[23];
                }
            }
            primaryKnightWeaponTitles[i].enabled = true;
        } //Scout
        else if (characterIndex == 1)
        {
            if (i == 0)
            {
                if (primaryWeaponIndex != 0)
                {
                    primaryScoutWeaponButtons[0].image.sprite = primaryScoutWeaponImages[1];
                }
                else
                {
                    primaryScoutWeaponButtons[0].image.sprite = primaryScoutWeaponImages[3];
                }
            }
            else if (i == 1)
            {
                if (primaryWeaponIndex != 1)
                {
                    primaryScoutWeaponButtons[1].image.sprite = primaryScoutWeaponImages[6];
                }
                else
                {
                    primaryScoutWeaponButtons[1].image.sprite = primaryScoutWeaponImages[8];
                }
            }
            else if (i == 2)
            {
                if (primaryWeaponIndex != 2)
                {
                    primaryScoutWeaponButtons[2].image.sprite = primaryScoutWeaponImages[11];
                }
                else
                {
                    primaryScoutWeaponButtons[2].image.sprite = primaryScoutWeaponImages[13];
                }
            }
            else if (i == 3)
            {
                if (primaryWeaponIndex != 3)
                {
                    primaryScoutWeaponButtons[3].image.sprite = primaryScoutWeaponImages[16];
                }
                else
                {
                    primaryScoutWeaponButtons[3].image.sprite = primaryScoutWeaponImages[18];
                }
            }
            else if (i == 4)
            {
                if (primaryWeaponIndex != 4)
                {
                    primaryScoutWeaponButtons[4].image.sprite = primaryScoutWeaponImages[21];
                }
                else
                {
                    primaryScoutWeaponButtons[4].image.sprite = primaryScoutWeaponImages[23];
                }
            }
            primaryScoutWeaponTitles[i].enabled = true;
        } //Berserker
        else if (characterIndex == 2)
        {
            if (i == 0)
            {
                if (primaryWeaponIndex != 0)
                {
                    primaryBerserkerWeaponButtons[0].image.sprite = primaryBerserkerWeaponImages[1];
                }
                else
                {
                    primaryBerserkerWeaponButtons[0].image.sprite = primaryBerserkerWeaponImages[3];
                }
            }
            else if (i == 1)
            {
                if (primaryWeaponIndex != 1)
                {
                    primaryBerserkerWeaponButtons[1].image.sprite = primaryBerserkerWeaponImages[6];
                }
                else
                {
                    primaryBerserkerWeaponButtons[1].image.sprite = primaryBerserkerWeaponImages[8];
                }
            }
            else if (i == 2)
            {
                if (primaryWeaponIndex != 2)
                {
                    primaryBerserkerWeaponButtons[2].image.sprite = primaryBerserkerWeaponImages[11];
                }
                else
                {
                    primaryBerserkerWeaponButtons[2].image.sprite = primaryBerserkerWeaponImages[13];
                }
            }
            else if (i == 3)
            {
                if (primaryWeaponIndex != 3)
                {
                    primaryBerserkerWeaponButtons[3].image.sprite = primaryBerserkerWeaponImages[16];
                }
                else
                {
                    primaryBerserkerWeaponButtons[3].image.sprite = primaryBerserkerWeaponImages[18];
                }
            }
            else if (i == 4)
            {
                if (primaryWeaponIndex != 4)
                {
                    primaryBerserkerWeaponButtons[4].image.sprite = primaryBerserkerWeaponImages[21];
                }
                else
                {
                    primaryBerserkerWeaponButtons[4].image.sprite = primaryBerserkerWeaponImages[23];
                }
            }
            primaryBerserkerWeaponTitles[i].enabled = true;
        } // Archer
        else if (characterIndex == 3)
        {
            if (i == 0)
            {
                if (primaryWeaponIndex != 0)
                {
                    primaryArcherWeaponButtons[0].image.sprite = primaryArcherWeaponImages[1];
                }
                else
                {
                    primaryArcherWeaponButtons[0].image.sprite = primaryArcherWeaponImages[3];
                }
            }
            else if (i == 1)
            {
                if (primaryWeaponIndex != 1)
                {
                    primaryArcherWeaponButtons[1].image.sprite = primaryArcherWeaponImages[6];
                }
                else
                {
                    primaryArcherWeaponButtons[1].image.sprite = primaryArcherWeaponImages[8];
                }
            }
            else if (i == 2)
            {
                if (primaryWeaponIndex != 2)
                {
                    primaryArcherWeaponButtons[2].image.sprite = primaryArcherWeaponImages[11];
                }
                else
                {
                    primaryArcherWeaponButtons[2].image.sprite = primaryArcherWeaponImages[13];
                }
            }
            else if (i == 3)
            {
                if (primaryWeaponIndex != 3)
                {
                    primaryArcherWeaponButtons[3].image.sprite = primaryArcherWeaponImages[16];
                }
                else
                {
                    primaryArcherWeaponButtons[3].image.sprite = primaryArcherWeaponImages[18];
                }
            }
            else if (i == 4)
            {
                if (primaryWeaponIndex != 4)
                {
                    primaryArcherWeaponButtons[4].image.sprite = primaryArcherWeaponImages[21];
                }
                else
                {
                    primaryArcherWeaponButtons[4].image.sprite = primaryArcherWeaponImages[23];
                }
            }
            primaryArcherWeaponTitles[i].enabled = true;
        }
    }

    public void SecondaryWeaponButtonPointerEnter(int i)
    {
        //Knight
        if (characterIndex == 0)
        {
            if (i == 0)
            {
                if (secondaryWeaponIndex != 0)
                {
                    secondaryKnightWeaponButtons[0].image.sprite = secondaryKnightWeaponImages[1];
                }
                else
                {
                    secondaryKnightWeaponButtons[0].image.sprite = secondaryKnightWeaponImages[3];
                }
            }
            else if (i == 1)
            {
                if (secondaryWeaponIndex != 1)
                {
                    secondaryKnightWeaponButtons[1].image.sprite = secondaryKnightWeaponImages[6];
                }
                else
                {
                    secondaryKnightWeaponButtons[1].image.sprite = secondaryKnightWeaponImages[8];
                }
            }
            else if (i == 2)
            {
                if (secondaryWeaponIndex != 2)
                {
                    secondaryKnightWeaponButtons[2].image.sprite = secondaryKnightWeaponImages[11];
                }
                else
                {
                    secondaryKnightWeaponButtons[2].image.sprite = secondaryKnightWeaponImages[13];
                }
            }
            else if (i == 3)
            {
                if (secondaryWeaponIndex != 3)
                {
                    secondaryKnightWeaponButtons[3].image.sprite = secondaryKnightWeaponImages[16];
                }
                else
                {
                    secondaryKnightWeaponButtons[3].image.sprite = secondaryKnightWeaponImages[18];
                }
            }
            else if (i == 4)
            {
                if (secondaryWeaponIndex != 4)
                {
                    secondaryKnightWeaponButtons[4].image.sprite = secondaryKnightWeaponImages[21];
                }
                else
                {
                    secondaryKnightWeaponButtons[4].image.sprite = secondaryKnightWeaponImages[23];
                }
            }
            secondaryKnightWeaponTitles[i].enabled = true;
        } //Scout
        else if (characterIndex == 1)
        {
            if (i == 0)
            {
                if (secondaryWeaponIndex != 0)
                {
                    secondaryScoutWeaponButtons[0].image.sprite = secondaryScoutWeaponImages[1];
                }
                else
                {
                    secondaryScoutWeaponButtons[0].image.sprite = secondaryScoutWeaponImages[3];
                }
            }
            else if (i == 1)
            {
                if (secondaryWeaponIndex != 1)
                {
                    secondaryScoutWeaponButtons[1].image.sprite = secondaryScoutWeaponImages[6];
                }
                else
                {
                    secondaryScoutWeaponButtons[1].image.sprite = secondaryScoutWeaponImages[8];
                }
            }
            else if (i == 2)
            {
                if (secondaryWeaponIndex != 2)
                {
                    secondaryScoutWeaponButtons[2].image.sprite = secondaryScoutWeaponImages[11];
                }
                else
                {
                    secondaryScoutWeaponButtons[2].image.sprite = secondaryScoutWeaponImages[13];
                }
            }
            else if (i == 3)
            {
                if (secondaryWeaponIndex != 3)
                {
                    secondaryScoutWeaponButtons[3].image.sprite = secondaryScoutWeaponImages[16];
                }
                else
                {
                    secondaryScoutWeaponButtons[3].image.sprite = secondaryScoutWeaponImages[18];
                }
            }
            else if (i == 4)
            {
                if (secondaryWeaponIndex != 4)
                {
                    secondaryScoutWeaponButtons[4].image.sprite = secondaryScoutWeaponImages[21];
                }
                else
                {
                    secondaryScoutWeaponButtons[4].image.sprite = secondaryScoutWeaponImages[23];
                }
            }
            secondaryScoutWeaponTitles[i].enabled = true;
        } //Berserker
        else if (characterIndex == 2)
        {
            if (i == 0)
            {
                if (secondaryWeaponIndex != 0)
                {
                    secondaryBerserkerWeaponButtons[0].image.sprite = secondaryBerserkerWeaponImages[1];
                }
                else
                {
                    secondaryBerserkerWeaponButtons[0].image.sprite = secondaryBerserkerWeaponImages[3];
                }
            }
            else if (i == 1)
            {
                if (secondaryWeaponIndex != 1)
                {
                    secondaryBerserkerWeaponButtons[1].image.sprite = secondaryBerserkerWeaponImages[6];
                }
                else
                {
                    secondaryBerserkerWeaponButtons[1].image.sprite = secondaryBerserkerWeaponImages[8];
                }
            }
            else if (i == 2)
            {
                if (secondaryWeaponIndex != 2)
                {
                    secondaryBerserkerWeaponButtons[2].image.sprite = secondaryBerserkerWeaponImages[11];
                }
                else
                {
                    secondaryBerserkerWeaponButtons[2].image.sprite = secondaryBerserkerWeaponImages[13];
                }
            }
            else if (i == 3)
            {
                if (secondaryWeaponIndex != 3)
                {
                    secondaryBerserkerWeaponButtons[3].image.sprite = secondaryBerserkerWeaponImages[16];
                }
                else
                {
                    secondaryBerserkerWeaponButtons[3].image.sprite = secondaryBerserkerWeaponImages[18];
                }
            }
            secondaryBerserkerWeaponTitles[i].enabled = true;
        } // Archer
        else if (characterIndex == 3)
        {
            if (i == 0)
            {
                if (secondaryWeaponIndex != 0)
                {
                    secondaryArcherWeaponButtons[0].image.sprite = secondaryArcherWeaponImages[1];
                }
                else
                {
                    secondaryArcherWeaponButtons[0].image.sprite = secondaryArcherWeaponImages[3];
                }
            }
            else if (i == 1)
            {
                if (secondaryWeaponIndex != 1)
                {
                    secondaryArcherWeaponButtons[1].image.sprite = secondaryArcherWeaponImages[6];
                }
                else
                {
                    secondaryArcherWeaponButtons[1].image.sprite = secondaryArcherWeaponImages[8];
                }
            }
            else if (i == 2)
            {
                if (secondaryWeaponIndex != 2)
                {
                    secondaryArcherWeaponButtons[2].image.sprite = secondaryArcherWeaponImages[11];
                }
                else
                {
                    secondaryArcherWeaponButtons[2].image.sprite = secondaryArcherWeaponImages[13];
                }
            }
            else if (i == 3)
            {
                if (secondaryWeaponIndex != 3)
                {
                    secondaryArcherWeaponButtons[3].image.sprite = secondaryArcherWeaponImages[16];
                }
                else
                {
                    secondaryArcherWeaponButtons[3].image.sprite = secondaryArcherWeaponImages[18];
                }
            }
            else if (i == 4)
            {
                if (secondaryWeaponIndex != 4)
                {
                    secondaryArcherWeaponButtons[4].image.sprite = secondaryArcherWeaponImages[21];
                }
                else
                {
                    secondaryArcherWeaponButtons[4].image.sprite = secondaryArcherWeaponImages[23];
                }
            }
            secondaryArcherWeaponTitles[i].enabled = true;
        }
    }

    public void PrimaryWeaponButtonPointerExit(int i)
    {
        //Knight
        if (characterIndex == 0)
        {
            if (i == 0)
            {
                if (primaryWeaponIndex != 0)
                {
                    primaryKnightWeaponButtons[0].image.sprite = primaryKnightWeaponImages[0];
                }
                else
                {
                    primaryKnightWeaponButtons[0].image.sprite = primaryKnightWeaponImages[2];
                }
            }
            else if (i == 1)
            {
                if (primaryWeaponIndex != 1)
                {
                    primaryKnightWeaponButtons[1].image.sprite = primaryKnightWeaponImages[5];
                }
                else
                {
                    primaryKnightWeaponButtons[1].image.sprite = primaryKnightWeaponImages[7];
                }
            }
            else if (i == 2)
            {
                if (primaryWeaponIndex != 2)
                {
                    primaryKnightWeaponButtons[2].image.sprite = primaryKnightWeaponImages[10];
                }
                else
                {
                    primaryKnightWeaponButtons[2].image.sprite = primaryKnightWeaponImages[12];
                }
            }
            else if (i == 3)
            {
                if (primaryWeaponIndex != 3)
                {
                    primaryKnightWeaponButtons[3].image.sprite = primaryKnightWeaponImages[15];
                }
                else
                {
                    primaryKnightWeaponButtons[3].image.sprite = primaryKnightWeaponImages[17];
                }
            }
            else if (i == 4)
            {
                if (primaryWeaponIndex != 4)
                {
                    primaryKnightWeaponButtons[4].image.sprite = primaryKnightWeaponImages[20];
                }
                else
                {
                    primaryKnightWeaponButtons[4].image.sprite = primaryKnightWeaponImages[22];
                }
            }
            primaryKnightWeaponTitles[i].enabled = false;
        } //Scout
        else if (characterIndex == 1)
        {
            if (i == 0)
            {
                if (primaryWeaponIndex != 0)
                {
                    primaryScoutWeaponButtons[0].image.sprite = primaryScoutWeaponImages[0];
                }
                else
                {
                    primaryScoutWeaponButtons[0].image.sprite = primaryScoutWeaponImages[2];
                }
            }
            else if (i == 1)
            {
                if (primaryWeaponIndex != 1)
                {
                    primaryScoutWeaponButtons[1].image.sprite = primaryScoutWeaponImages[5];
                }
                else
                {
                    primaryScoutWeaponButtons[1].image.sprite = primaryScoutWeaponImages[7];
                }
            }
            else if (i == 2)
            {
                if (primaryWeaponIndex != 2)
                {
                    primaryScoutWeaponButtons[2].image.sprite = primaryScoutWeaponImages[10];
                }
                else
                {
                    primaryScoutWeaponButtons[2].image.sprite = primaryScoutWeaponImages[12];
                }
            }
            else if (i == 3)
            {
                if (primaryWeaponIndex != 3)
                {
                    primaryScoutWeaponButtons[3].image.sprite = primaryScoutWeaponImages[15];
                }
                else
                {
                    primaryScoutWeaponButtons[3].image.sprite = primaryScoutWeaponImages[17];
                }
            }
            else if (i == 4)
            {
                if (primaryWeaponIndex != 4)
                {
                    primaryScoutWeaponButtons[4].image.sprite = primaryScoutWeaponImages[20];
                }
                else
                {
                    primaryScoutWeaponButtons[4].image.sprite = primaryScoutWeaponImages[22];
                }
            }
            primaryScoutWeaponTitles[i].enabled = false;
        } //Berserker
        else if (characterIndex == 2)
        {
            if (i == 0)
            {
                if (primaryWeaponIndex != 0)
                {
                    primaryBerserkerWeaponButtons[0].image.sprite = primaryBerserkerWeaponImages[0];
                }
                else
                {
                    primaryBerserkerWeaponButtons[0].image.sprite = primaryBerserkerWeaponImages[2];
                }
            }
            else if (i == 1)
            {
                if (primaryWeaponIndex != 1)
                {
                    primaryBerserkerWeaponButtons[1].image.sprite = primaryBerserkerWeaponImages[5];
                }
                else
                {
                    primaryBerserkerWeaponButtons[1].image.sprite = primaryBerserkerWeaponImages[7];
                }
            }
            else if (i == 2)
            {
                if (primaryWeaponIndex != 2)
                {
                    primaryBerserkerWeaponButtons[2].image.sprite = primaryBerserkerWeaponImages[10];
                }
                else
                {
                    primaryBerserkerWeaponButtons[2].image.sprite = primaryBerserkerWeaponImages[12];
                }
            }
            else if (i == 3)
            {
                if (primaryWeaponIndex != 3)
                {
                    primaryBerserkerWeaponButtons[3].image.sprite = primaryBerserkerWeaponImages[15];
                }
                else
                {
                    primaryBerserkerWeaponButtons[3].image.sprite = primaryBerserkerWeaponImages[17];
                }
            }
            else if (i == 4)
            {
                if (primaryWeaponIndex != 4)
                {
                    primaryBerserkerWeaponButtons[4].image.sprite = primaryBerserkerWeaponImages[20];
                }
                else
                {
                    primaryBerserkerWeaponButtons[4].image.sprite = primaryBerserkerWeaponImages[22];
                }
            }
            primaryBerserkerWeaponTitles[i].enabled = false;
        } // Archer
        else if (characterIndex == 3)
        {
            if (i == 0)
            {
                if (primaryWeaponIndex != 0)
                {
                    primaryArcherWeaponButtons[0].image.sprite = primaryArcherWeaponImages[0];
                }
                else
                {
                    primaryArcherWeaponButtons[0].image.sprite = primaryArcherWeaponImages[2];
                }
            }
            else if (i == 1)
            {
                if (primaryWeaponIndex != 1)
                {
                    primaryArcherWeaponButtons[1].image.sprite = primaryArcherWeaponImages[5];
                }
                else
                {
                    primaryArcherWeaponButtons[1].image.sprite = primaryArcherWeaponImages[7];
                }
            }
            else if (i == 2)
            {
                if (primaryWeaponIndex != 2)
                {
                    primaryArcherWeaponButtons[2].image.sprite = primaryArcherWeaponImages[10];
                }
                else
                {
                    primaryArcherWeaponButtons[2].image.sprite = primaryArcherWeaponImages[12];
                }
            }
            else if (i == 3)
            {
                if (primaryWeaponIndex != 3)
                {
                    primaryArcherWeaponButtons[3].image.sprite = primaryArcherWeaponImages[15];
                }
                else
                {
                    primaryArcherWeaponButtons[3].image.sprite = primaryArcherWeaponImages[17];
                }
            }
            else if (i == 4)
            {
                if (primaryWeaponIndex != 4)
                {
                    primaryArcherWeaponButtons[4].image.sprite = primaryArcherWeaponImages[20];
                }
                else
                {
                    primaryArcherWeaponButtons[4].image.sprite = primaryArcherWeaponImages[22];
                }
            }
            primaryArcherWeaponTitles[i].enabled = false;
        }
    }

    public void SecondaryWeaponButtonPointerExit(int i)
    {
        //Knight
        if (characterIndex == 0)
        {
            if (i == 0)
            {
                if (secondaryWeaponIndex != 0)
                {
                    secondaryKnightWeaponButtons[0].image.sprite = secondaryKnightWeaponImages[0];
                }
                else
                {
                    secondaryKnightWeaponButtons[0].image.sprite = secondaryKnightWeaponImages[2];
                }
            }
            else if (i == 1)
            {
                if (secondaryWeaponIndex != 1)
                {
                    secondaryKnightWeaponButtons[1].image.sprite = secondaryKnightWeaponImages[5];
                }
                else
                {
                    secondaryKnightWeaponButtons[1].image.sprite = secondaryKnightWeaponImages[7];
                }
            }
            else if (i == 2)
            {
                if (secondaryWeaponIndex != 2)
                {
                    secondaryKnightWeaponButtons[2].image.sprite = secondaryKnightWeaponImages[10];
                }
                else
                {
                    secondaryKnightWeaponButtons[2].image.sprite = secondaryKnightWeaponImages[12];
                }
            }
            else if (i == 3)
            {
                if (secondaryWeaponIndex != 3)
                {
                    secondaryKnightWeaponButtons[3].image.sprite = secondaryKnightWeaponImages[15];
                }
                else
                {
                    secondaryKnightWeaponButtons[3].image.sprite = secondaryKnightWeaponImages[17];
                }
            }
            else if (i == 4)
            {
                if (secondaryWeaponIndex != 4)
                {
                    secondaryKnightWeaponButtons[4].image.sprite = secondaryKnightWeaponImages[20];
                }
                else
                {
                    secondaryKnightWeaponButtons[4].image.sprite = secondaryKnightWeaponImages[22];
                }
            }
            secondaryKnightWeaponTitles[i].enabled = false;
        } //Scout
        else if (characterIndex == 1)
        {
            if (i == 0)
            {
                if (secondaryWeaponIndex != 0)
                {
                    secondaryScoutWeaponButtons[0].image.sprite = secondaryScoutWeaponImages[0];
                }
                else
                {
                    secondaryScoutWeaponButtons[0].image.sprite = secondaryScoutWeaponImages[2];
                }
            }
            else if (i == 1)
            {
                if (secondaryWeaponIndex != 1)
                {
                    secondaryScoutWeaponButtons[1].image.sprite = secondaryScoutWeaponImages[5];
                }
                else
                {
                    secondaryScoutWeaponButtons[1].image.sprite = secondaryScoutWeaponImages[7];
                }
            }
            else if (i == 2)
            {
                if (secondaryWeaponIndex != 2)
                {
                    secondaryScoutWeaponButtons[2].image.sprite = secondaryScoutWeaponImages[10];
                }
                else
                {
                    secondaryScoutWeaponButtons[2].image.sprite = secondaryScoutWeaponImages[12];
                }
            }
            else if (i == 3)
            {
                if (secondaryWeaponIndex != 3)
                {
                    secondaryScoutWeaponButtons[3].image.sprite = secondaryScoutWeaponImages[15];
                }
                else
                {
                    secondaryScoutWeaponButtons[3].image.sprite = secondaryScoutWeaponImages[17];
                }
            }
            else if (i == 4)
            {
                if (secondaryWeaponIndex != 4)
                {
                    secondaryScoutWeaponButtons[4].image.sprite = secondaryScoutWeaponImages[20];
                }
                else
                {
                    secondaryScoutWeaponButtons[4].image.sprite = secondaryScoutWeaponImages[22];
                }
            }
            secondaryScoutWeaponTitles[i].enabled = false;
        } //Berserker
        else if (characterIndex == 2)
        {
            if (i == 0)
            {
                if (secondaryWeaponIndex != 0)
                {
                    secondaryBerserkerWeaponButtons[0].image.sprite = secondaryBerserkerWeaponImages[0];
                }
                else
                {
                    secondaryBerserkerWeaponButtons[0].image.sprite = secondaryBerserkerWeaponImages[2];
                }
            }
            else if (i == 1)
            {
                if (secondaryWeaponIndex != 1)
                {
                    secondaryBerserkerWeaponButtons[1].image.sprite = secondaryBerserkerWeaponImages[5];
                }
                else
                {
                    secondaryBerserkerWeaponButtons[1].image.sprite = secondaryBerserkerWeaponImages[7];
                }
            }
            else if (i == 2)
            {
                if (secondaryWeaponIndex != 2)
                {
                    secondaryBerserkerWeaponButtons[2].image.sprite = secondaryBerserkerWeaponImages[10];
                }
                else
                {
                    secondaryBerserkerWeaponButtons[2].image.sprite = secondaryBerserkerWeaponImages[12];
                }
            }
            else if (i == 3)
            {
                if (secondaryWeaponIndex != 3)
                {
                    secondaryBerserkerWeaponButtons[3].image.sprite = secondaryBerserkerWeaponImages[15];
                }
                else
                {
                    secondaryBerserkerWeaponButtons[3].image.sprite = secondaryBerserkerWeaponImages[17];
                }
            }
            secondaryBerserkerWeaponTitles[i].enabled = false;
        } // Archer
        else if (characterIndex == 3)
        {
            if (i == 0)
            {
                if (secondaryWeaponIndex != 0)
                {
                    secondaryArcherWeaponButtons[0].image.sprite = secondaryArcherWeaponImages[0];
                }
                else
                {
                    secondaryArcherWeaponButtons[0].image.sprite = secondaryArcherWeaponImages[2];
                }
            }
            else if (i == 1)
            {
                if (secondaryWeaponIndex != 1)
                {
                    secondaryArcherWeaponButtons[1].image.sprite = secondaryArcherWeaponImages[5];
                }
                else
                {
                    secondaryArcherWeaponButtons[1].image.sprite = secondaryArcherWeaponImages[7];
                }
            }
            else if (i == 2)
            {
                if (secondaryWeaponIndex != 2)
                {
                    secondaryArcherWeaponButtons[2].image.sprite = secondaryArcherWeaponImages[10];
                }
                else
                {
                    secondaryArcherWeaponButtons[2].image.sprite = secondaryArcherWeaponImages[12];
                }
            }
            else if (i == 3)
            {
                if (secondaryWeaponIndex != 3)
                {
                    secondaryArcherWeaponButtons[3].image.sprite = secondaryArcherWeaponImages[15];
                }
                else
                {
                    secondaryArcherWeaponButtons[3].image.sprite = secondaryArcherWeaponImages[17];
                }
            }
            else if (i == 4)
            {
                if (secondaryWeaponIndex != 4)
                {
                    secondaryArcherWeaponButtons[4].image.sprite = secondaryArcherWeaponImages[20];
                }
                else
                {
                    secondaryArcherWeaponButtons[4].image.sprite = secondaryArcherWeaponImages[22];
                }
            }
            secondaryArcherWeaponTitles[i].enabled = false;
        }
    }

    public void primaryWeaponButtonPointerDown(int i)
    {
        if(characterIndex == 0)
        {
            if (i == 0)
            {
                primaryKnightWeaponButtons[0].image.sprite = primaryKnightWeaponImages[4];
            }
            else if(i == 1)
            {
                primaryKnightWeaponButtons[1].image.sprite = primaryKnightWeaponImages[9];
            }
            else if (i == 2)
            {
                primaryKnightWeaponButtons[2].image.sprite = primaryKnightWeaponImages[14];
            }
            else if (i == 3)
            {
                primaryKnightWeaponButtons[3].image.sprite = primaryKnightWeaponImages[19];
            }
            else if (i == 4)
            {
                primaryKnightWeaponButtons[4].image.sprite = primaryKnightWeaponImages[24];
            }
            primaryKnightWeaponTitles[i].enabled = true;
        }
        else if (characterIndex == 1)
        {
            if (i == 0)
            {
                primaryScoutWeaponButtons[0].image.sprite = primaryScoutWeaponImages[4];
            }
            else if (i == 1)
            {
                primaryScoutWeaponButtons[1].image.sprite = primaryScoutWeaponImages[9];
            }
            else if (i == 2)
            {
                primaryScoutWeaponButtons[2].image.sprite = primaryScoutWeaponImages[14];
            }
            else if (i == 3)
            {
                primaryScoutWeaponButtons[3].image.sprite = primaryScoutWeaponImages[19];
            }
            else if (i == 4)
            {
                primaryScoutWeaponButtons[4].image.sprite = primaryScoutWeaponImages[24];
            }
            primaryScoutWeaponTitles[i].enabled = true;
        }
        else if (characterIndex == 2)
        {
            if (i == 0)
            {
                primaryBerserkerWeaponButtons[0].image.sprite = primaryBerserkerWeaponImages[4];
            }
            else if (i == 1)
            {
                primaryBerserkerWeaponButtons[1].image.sprite = primaryBerserkerWeaponImages[9];
            }
            else if (i == 2)
            {
                primaryBerserkerWeaponButtons[2].image.sprite = primaryBerserkerWeaponImages[14];
            }
            else if (i == 3)
            {
                primaryBerserkerWeaponButtons[3].image.sprite = primaryBerserkerWeaponImages[19];
            }
            else if (i == 4)
            {
                primaryBerserkerWeaponButtons[4].image.sprite = primaryBerserkerWeaponImages[24];
            }
            primaryBerserkerWeaponTitles[i].enabled = true;
        }
        else if (characterIndex == 3)
        {
            if (i == 0)
            {
                primaryArcherWeaponButtons[0].image.sprite = primaryArcherWeaponImages[4];
            }
            else if (i == 1)
            {
                primaryArcherWeaponButtons[1].image.sprite = primaryArcherWeaponImages[9];
            }
            else if (i == 2)
            {
                primaryArcherWeaponButtons[2].image.sprite = primaryArcherWeaponImages[14];
            }
            else if (i == 3)
            {
                primaryArcherWeaponButtons[3].image.sprite = primaryArcherWeaponImages[19];
            }
            else if (i == 4)
            {
                primaryArcherWeaponButtons[4].image.sprite = primaryArcherWeaponImages[24];
            }
            primaryArcherWeaponTitles[i].enabled = true;
        }
    }

    public void secondaryWeaponButtonPointerDown(int i)
    {
        if (characterIndex == 0)
        {
            if (i == 0)
            {
                secondaryKnightWeaponButtons[0].image.sprite = secondaryKnightWeaponImages[4];
            }
            else if (i == 1)
            {
                secondaryKnightWeaponButtons[1].image.sprite = secondaryKnightWeaponImages[9];
            }
            else if (i == 2)
            {
                secondaryKnightWeaponButtons[2].image.sprite = secondaryKnightWeaponImages[14];
            }
            else if (i == 3)
            {
                secondaryKnightWeaponButtons[3].image.sprite = secondaryKnightWeaponImages[19];
            }
            else if (i == 4)
            {
                secondaryKnightWeaponButtons[4].image.sprite = secondaryKnightWeaponImages[24];
            }
            secondaryKnightWeaponTitles[i].enabled = true;
        }
        else if (characterIndex == 1)
        {
            if (i == 0)
            {
                secondaryScoutWeaponButtons[0].image.sprite = secondaryScoutWeaponImages[4];
            }
            else if (i == 1)
            {
                secondaryScoutWeaponButtons[1].image.sprite = secondaryScoutWeaponImages[9];
            }
            else if (i == 2)
            {
                secondaryScoutWeaponButtons[2].image.sprite = secondaryScoutWeaponImages[14];
            }
            else if (i == 3)
            {
                secondaryScoutWeaponButtons[3].image.sprite = secondaryScoutWeaponImages[19];
            }
            else if (i == 4)
            {
                secondaryScoutWeaponButtons[4].image.sprite = secondaryScoutWeaponImages[24];
            }
            secondaryScoutWeaponTitles[i].enabled = true;
        }
        else if (characterIndex == 2)
        {
            if (i == 0)
            {
                secondaryBerserkerWeaponButtons[0].image.sprite = secondaryBerserkerWeaponImages[4];
            }
            else if (i == 1)
            {
                secondaryBerserkerWeaponButtons[1].image.sprite = secondaryBerserkerWeaponImages[9];
            }
            else if (i == 2)
            {
                secondaryBerserkerWeaponButtons[2].image.sprite = secondaryBerserkerWeaponImages[14];
            }
            else if (i == 3)
            {
                secondaryBerserkerWeaponButtons[3].image.sprite = secondaryBerserkerWeaponImages[19];
            }
            else if (i == 4)
            {
                secondaryBerserkerWeaponButtons[4].image.sprite = secondaryBerserkerWeaponImages[24];
            }
            secondaryBerserkerWeaponTitles[i].enabled = true;
        }
        else if (characterIndex == 3)
        {
            if (i == 0)
            {
                secondaryArcherWeaponButtons[0].image.sprite = secondaryArcherWeaponImages[4];
            }
            else if (i == 1)
            {
                secondaryArcherWeaponButtons[1].image.sprite = secondaryArcherWeaponImages[9];
            }
            else if (i == 2)
            {
                secondaryArcherWeaponButtons[2].image.sprite = secondaryArcherWeaponImages[14];
            }
            else if (i == 3)
            {
                secondaryArcherWeaponButtons[3].image.sprite = secondaryArcherWeaponImages[19];
            }
            else if (i == 4)
            {
                secondaryArcherWeaponButtons[4].image.sprite = secondaryArcherWeaponImages[24];
            }
            secondaryArcherWeaponTitles[i].enabled = true;
        }
    }

    public void ClickPrimaryWeaponButton(int i)
    {
        if(characterIndex == 0)
        {
            if(i == 0)
            {
                if(primaryWeaponIndex == -1)
                {
                    primaryKnightWeaponButtons[0].image.sprite = primaryKnightWeaponImages[2];
                    primaryWeaponIndex = i;
                }
                else if (primaryWeaponIndex == 0)
                {
                    primaryKnightWeaponButtons[0].image.sprite = primaryKnightWeaponImages[0];
                    primaryWeaponIndex = -1;
                }
                else if (primaryWeaponIndex == 1)
                {
                    primaryKnightWeaponButtons[0].image.sprite = primaryKnightWeaponImages[2];
                    primaryKnightWeaponButtons[1].image.sprite = primaryKnightWeaponImages[5];
                    primaryWeaponIndex = i;
                }
                else if (primaryWeaponIndex == 2)
                {
                    primaryKnightWeaponButtons[0].image.sprite = primaryKnightWeaponImages[2];
                    primaryKnightWeaponButtons[2].image.sprite = primaryKnightWeaponImages[10];
                    primaryWeaponIndex = i;
                }
                else if (primaryWeaponIndex == 3)
                {
                    primaryKnightWeaponButtons[0].image.sprite = primaryKnightWeaponImages[2];
                    primaryKnightWeaponButtons[3].image.sprite = primaryKnightWeaponImages[15];
                    primaryWeaponIndex = i;
                }
                else if (primaryWeaponIndex == 4)
                {
                    primaryKnightWeaponButtons[0].image.sprite = primaryKnightWeaponImages[2];
                    primaryKnightWeaponButtons[4].image.sprite = primaryKnightWeaponImages[20];
                    primaryWeaponIndex = i;
                }
            }
            else if (i == 1)
            {
                if (primaryWeaponIndex == -1)
                {
                    primaryKnightWeaponButtons[1].image.sprite = primaryKnightWeaponImages[7];
                    primaryWeaponIndex = i;
                }
                else if (primaryWeaponIndex == 0)
                {
                    primaryKnightWeaponButtons[1].image.sprite = primaryKnightWeaponImages[7];
                    primaryKnightWeaponButtons[0].image.sprite = primaryKnightWeaponImages[0];
                    primaryWeaponIndex = i;
                }
                else if (primaryWeaponIndex == 1)
                {                   
                    primaryKnightWeaponButtons[1].image.sprite = primaryKnightWeaponImages[5];
                    primaryWeaponIndex = -1;
                }
                else if (primaryWeaponIndex == 2)
                {
                    primaryKnightWeaponButtons[1].image.sprite = primaryKnightWeaponImages[7];
                    primaryKnightWeaponButtons[2].image.sprite = primaryKnightWeaponImages[10];
                    primaryWeaponIndex = i;
                }
                else if (primaryWeaponIndex == 3)
                {
                    primaryKnightWeaponButtons[1].image.sprite = primaryKnightWeaponImages[7];
                    primaryKnightWeaponButtons[3].image.sprite = primaryKnightWeaponImages[15];
                    primaryWeaponIndex = i;
                }
                else if (primaryWeaponIndex == 4)
                {
                    primaryKnightWeaponButtons[0].image.sprite = primaryKnightWeaponImages[2];
                    primaryKnightWeaponButtons[4].image.sprite = primaryKnightWeaponImages[20];
                    primaryWeaponIndex = i;
                }
            }
            else if (i == 2)
            {
                if (primaryWeaponIndex == -1)
                {
                    primaryKnightWeaponButtons[2].image.sprite = primaryKnightWeaponImages[12];
                    primaryWeaponIndex = i;
                }
                else if (primaryWeaponIndex == 0)
                {
                    primaryKnightWeaponButtons[2].image.sprite = primaryKnightWeaponImages[12];
                    primaryKnightWeaponButtons[0].image.sprite = primaryKnightWeaponImages[0];
                    primaryWeaponIndex = i;
                }
                else if (primaryWeaponIndex == 1)
                {
                    primaryKnightWeaponButtons[2].image.sprite = primaryKnightWeaponImages[12];
                    primaryKnightWeaponButtons[1].image.sprite = primaryKnightWeaponImages[5];
                    primaryWeaponIndex = i;
                }
                else if (primaryWeaponIndex == 2)
                {                   
                    primaryKnightWeaponButtons[2].image.sprite = primaryKnightWeaponImages[10];
                    primaryWeaponIndex = -1;
                }
                else if (primaryWeaponIndex == 3)
                {
                    primaryKnightWeaponButtons[2].image.sprite = primaryKnightWeaponImages[12];
                    primaryKnightWeaponButtons[3].image.sprite = primaryKnightWeaponImages[15];
                    primaryWeaponIndex = i;
                }
                else if (primaryWeaponIndex == 4)
                {
                    primaryKnightWeaponButtons[2].image.sprite = primaryKnightWeaponImages[12];
                    primaryKnightWeaponButtons[4].image.sprite = primaryKnightWeaponImages[20];
                    primaryWeaponIndex = i;
                }
            }
            else if (i == 3)
            {
                if (primaryWeaponIndex == -1)
                {
                    primaryKnightWeaponButtons[3].image.sprite = primaryKnightWeaponImages[17];
                    primaryWeaponIndex = i;
                }
                else if (primaryWeaponIndex == 0)
                {
                    primaryKnightWeaponButtons[3].image.sprite = primaryKnightWeaponImages[17];
                    primaryKnightWeaponButtons[0].image.sprite = primaryKnightWeaponImages[0];
                    primaryWeaponIndex = i;
                }
                else if (primaryWeaponIndex == 1)
                {
                    primaryKnightWeaponButtons[3].image.sprite = primaryKnightWeaponImages[17];
                    primaryKnightWeaponButtons[1].image.sprite = primaryKnightWeaponImages[5];
                    primaryWeaponIndex = i;
                }
                else if (primaryWeaponIndex == 2)
                {
                    primaryKnightWeaponButtons[3].image.sprite = primaryKnightWeaponImages[17];
                    primaryKnightWeaponButtons[2].image.sprite = primaryKnightWeaponImages[10];
                    primaryWeaponIndex = i;
                }
                else if (primaryWeaponIndex == 3)
                {
                    primaryKnightWeaponButtons[3].image.sprite = primaryKnightWeaponImages[15];
                    primaryWeaponIndex = -1;
                }
                else if (primaryWeaponIndex == 4)
                {
                    primaryKnightWeaponButtons[3].image.sprite = primaryKnightWeaponImages[17];
                    primaryKnightWeaponButtons[4].image.sprite = primaryKnightWeaponImages[20];
                    primaryWeaponIndex = i;
                }
            }
            else if (i == 4)
            {
                if (primaryWeaponIndex == -1)
                {
                    primaryKnightWeaponButtons[4].image.sprite = primaryKnightWeaponImages[22];
                    primaryWeaponIndex = i;
                }
                else if (primaryWeaponIndex == 0)
                {
                    primaryKnightWeaponButtons[4].image.sprite = primaryKnightWeaponImages[22];
                    primaryKnightWeaponButtons[0].image.sprite = primaryKnightWeaponImages[0];
                    primaryWeaponIndex = i;
                }
                else if (primaryWeaponIndex == 1)
                {
                    primaryKnightWeaponButtons[4].image.sprite = primaryKnightWeaponImages[22];
                    primaryKnightWeaponButtons[1].image.sprite = primaryKnightWeaponImages[5];
                    primaryWeaponIndex = i;
                }
                else if (primaryWeaponIndex == 2)
                {
                    primaryKnightWeaponButtons[4].image.sprite = primaryKnightWeaponImages[22];
                    primaryKnightWeaponButtons[2].image.sprite = primaryKnightWeaponImages[10];
                    primaryWeaponIndex = i;
                }
                else if (primaryWeaponIndex == 3)
                {
                    primaryKnightWeaponButtons[4].image.sprite = primaryKnightWeaponImages[22];
                    primaryKnightWeaponButtons[3].image.sprite = primaryKnightWeaponImages[15];
                    primaryWeaponIndex = i;
                }
                else if (primaryWeaponIndex == 4)
                {
                    primaryKnightWeaponButtons[4].image.sprite = primaryKnightWeaponImages[20];
                    primaryWeaponIndex = -1;
                }
            }
        }
        else if (characterIndex == 1)
        {
            if (i == 0)
            {
                if (primaryWeaponIndex == -1)
                {
                    primaryScoutWeaponButtons[0].image.sprite = primaryScoutWeaponImages[2];
                    primaryWeaponIndex = i;
                }
                else if (primaryWeaponIndex == 0)
                {
                    primaryScoutWeaponButtons[0].image.sprite = primaryScoutWeaponImages[0];
                    primaryWeaponIndex = -1;
                }
                else if (primaryWeaponIndex == 1)
                {
                    primaryScoutWeaponButtons[0].image.sprite = primaryScoutWeaponImages[2];
                    primaryScoutWeaponButtons[1].image.sprite = primaryScoutWeaponImages[5];
                    primaryWeaponIndex = i;
                }
                else if (primaryWeaponIndex == 2)
                {
                    primaryScoutWeaponButtons[0].image.sprite = primaryScoutWeaponImages[2];
                    primaryScoutWeaponButtons[2].image.sprite = primaryScoutWeaponImages[10];
                    primaryWeaponIndex = i;
                }
                else if (primaryWeaponIndex == 3)
                {
                    primaryScoutWeaponButtons[0].image.sprite = primaryScoutWeaponImages[2];
                    primaryScoutWeaponButtons[3].image.sprite = primaryScoutWeaponImages[15];
                    primaryWeaponIndex = i;
                }
                else if (primaryWeaponIndex == 4)
                {
                    primaryScoutWeaponButtons[0].image.sprite = primaryScoutWeaponImages[2];
                    primaryScoutWeaponButtons[4].image.sprite = primaryScoutWeaponImages[20];
                    primaryWeaponIndex = i;
                }
            }
            else if (i == 1)
            {
                if (primaryWeaponIndex == -1)
                {
                    primaryScoutWeaponButtons[1].image.sprite = primaryScoutWeaponImages[7];
                    primaryWeaponIndex = i;
                }
                else if (primaryWeaponIndex == 0)
                {
                    primaryScoutWeaponButtons[1].image.sprite = primaryScoutWeaponImages[7];
                    primaryScoutWeaponButtons[0].image.sprite = primaryScoutWeaponImages[0];
                    primaryWeaponIndex = i;
                }
                else if (primaryWeaponIndex == 1)
                {
                    primaryScoutWeaponButtons[1].image.sprite = primaryScoutWeaponImages[5];
                    primaryWeaponIndex = -1;
                }
                else if (primaryWeaponIndex == 2)
                {
                    primaryScoutWeaponButtons[1].image.sprite = primaryScoutWeaponImages[7];
                    primaryScoutWeaponButtons[2].image.sprite = primaryScoutWeaponImages[10];
                    primaryWeaponIndex = i;
                }
                else if (primaryWeaponIndex == 3)
                {
                    primaryScoutWeaponButtons[1].image.sprite = primaryScoutWeaponImages[7];
                    primaryScoutWeaponButtons[3].image.sprite = primaryScoutWeaponImages[15];
                    primaryWeaponIndex = i;
                }
                else if (primaryWeaponIndex == 4)
                {
                    primaryScoutWeaponButtons[0].image.sprite = primaryScoutWeaponImages[2];
                    primaryScoutWeaponButtons[4].image.sprite = primaryScoutWeaponImages[20];
                    primaryWeaponIndex = i;
                }
            }
            else if (i == 2)
            {
                if (primaryWeaponIndex == -1)
                {
                    primaryScoutWeaponButtons[2].image.sprite = primaryScoutWeaponImages[12];
                    primaryWeaponIndex = i;
                }
                else if (primaryWeaponIndex == 0)
                {
                    primaryScoutWeaponButtons[2].image.sprite = primaryScoutWeaponImages[12];
                    primaryScoutWeaponButtons[0].image.sprite = primaryScoutWeaponImages[0];
                    primaryWeaponIndex = i;
                }
                else if (primaryWeaponIndex == 1)
                {
                    primaryScoutWeaponButtons[2].image.sprite = primaryScoutWeaponImages[12];
                    primaryScoutWeaponButtons[1].image.sprite = primaryScoutWeaponImages[5];
                    primaryWeaponIndex = i;
                }
                else if (primaryWeaponIndex == 2)
                {
                    primaryScoutWeaponButtons[2].image.sprite = primaryScoutWeaponImages[10];
                    primaryWeaponIndex = -1;
                }
                else if (primaryWeaponIndex == 3)
                {
                    primaryScoutWeaponButtons[2].image.sprite = primaryScoutWeaponImages[12];
                    primaryScoutWeaponButtons[3].image.sprite = primaryScoutWeaponImages[15];
                    primaryWeaponIndex = i;
                }
                else if (primaryWeaponIndex == 4)
                {
                    primaryScoutWeaponButtons[2].image.sprite = primaryScoutWeaponImages[12];
                    primaryScoutWeaponButtons[4].image.sprite = primaryScoutWeaponImages[20];
                    primaryWeaponIndex = i;
                }
            }
            else if (i == 3)
            {
                if (primaryWeaponIndex == -1)
                {
                    primaryScoutWeaponButtons[3].image.sprite = primaryScoutWeaponImages[17];
                    primaryWeaponIndex = i;
                }
                else if (primaryWeaponIndex == 0)
                {
                    primaryScoutWeaponButtons[3].image.sprite = primaryScoutWeaponImages[17];
                    primaryScoutWeaponButtons[0].image.sprite = primaryScoutWeaponImages[0];
                    primaryWeaponIndex = i;
                }
                else if (primaryWeaponIndex == 1)
                {
                    primaryScoutWeaponButtons[3].image.sprite = primaryScoutWeaponImages[17];
                    primaryScoutWeaponButtons[1].image.sprite = primaryScoutWeaponImages[5];
                    primaryWeaponIndex = i;
                }
                else if (primaryWeaponIndex == 2)
                {
                    primaryScoutWeaponButtons[3].image.sprite = primaryScoutWeaponImages[17];
                    primaryScoutWeaponButtons[2].image.sprite = primaryScoutWeaponImages[10];
                    primaryWeaponIndex = i;
                }
                else if (primaryWeaponIndex == 3)
                {
                    primaryScoutWeaponButtons[3].image.sprite = primaryScoutWeaponImages[15];
                    primaryWeaponIndex = -1;
                }
                else if (primaryWeaponIndex == 4)
                {
                    primaryScoutWeaponButtons[3].image.sprite = primaryScoutWeaponImages[17];
                    primaryScoutWeaponButtons[4].image.sprite = primaryScoutWeaponImages[20];
                    primaryWeaponIndex = i;
                }
            }
            else if (i == 4)
            {
                if (primaryWeaponIndex == -1)
                {
                    primaryScoutWeaponButtons[4].image.sprite = primaryScoutWeaponImages[22];
                    primaryWeaponIndex = i;
                }
                else if (primaryWeaponIndex == 0)
                {
                    primaryScoutWeaponButtons[4].image.sprite = primaryScoutWeaponImages[22];
                    primaryScoutWeaponButtons[0].image.sprite = primaryScoutWeaponImages[0];
                    primaryWeaponIndex = i;
                }
                else if (primaryWeaponIndex == 1)
                {
                    primaryScoutWeaponButtons[4].image.sprite = primaryScoutWeaponImages[22];
                    primaryScoutWeaponButtons[1].image.sprite = primaryScoutWeaponImages[5];
                    primaryWeaponIndex = i;
                }
                else if (primaryWeaponIndex == 2)
                {
                    primaryScoutWeaponButtons[4].image.sprite = primaryScoutWeaponImages[22];
                    primaryScoutWeaponButtons[2].image.sprite = primaryScoutWeaponImages[10];
                    primaryWeaponIndex = i;
                }
                else if (primaryWeaponIndex == 3)
                {
                    primaryScoutWeaponButtons[4].image.sprite = primaryScoutWeaponImages[22];
                    primaryScoutWeaponButtons[3].image.sprite = primaryScoutWeaponImages[15];
                    primaryWeaponIndex = i;
                }
                else if (primaryWeaponIndex == 4)
                {
                    primaryScoutWeaponButtons[4].image.sprite = primaryScoutWeaponImages[20];
                    primaryWeaponIndex = -1;
                }
            }
        }
        else if (characterIndex == 2)
        {
            if (i == 0)
            {
                if (primaryWeaponIndex == -1)
                {
                    primaryBerserkerWeaponButtons[0].image.sprite = primaryBerserkerWeaponImages[2];
                    primaryWeaponIndex = i;
                }
                else if (primaryWeaponIndex == 0)
                {
                    primaryBerserkerWeaponButtons[0].image.sprite = primaryBerserkerWeaponImages[0];
                    primaryWeaponIndex = -1;
                }
                else if (primaryWeaponIndex == 1)
                {
                    primaryBerserkerWeaponButtons[0].image.sprite = primaryBerserkerWeaponImages[2];
                    primaryBerserkerWeaponButtons[1].image.sprite = primaryBerserkerWeaponImages[5];
                    primaryWeaponIndex = i;
                }
                else if (primaryWeaponIndex == 2)
                {
                    primaryBerserkerWeaponButtons[0].image.sprite = primaryBerserkerWeaponImages[2];
                    primaryBerserkerWeaponButtons[2].image.sprite = primaryBerserkerWeaponImages[10];
                    primaryWeaponIndex = i;
                }
                else if (primaryWeaponIndex == 3)
                {
                    primaryBerserkerWeaponButtons[0].image.sprite = primaryBerserkerWeaponImages[2];
                    primaryBerserkerWeaponButtons[3].image.sprite = primaryBerserkerWeaponImages[15];
                    primaryWeaponIndex = i;
                }
                else if (primaryWeaponIndex == 4)
                {
                    primaryBerserkerWeaponButtons[0].image.sprite = primaryBerserkerWeaponImages[2];
                    primaryBerserkerWeaponButtons[4].image.sprite = primaryBerserkerWeaponImages[20];
                    primaryWeaponIndex = i;
                }
            }
            else if (i == 1)
            {
                if (primaryWeaponIndex == -1)
                {
                    primaryBerserkerWeaponButtons[1].image.sprite = primaryBerserkerWeaponImages[7];
                    primaryWeaponIndex = i;
                }
                else if (primaryWeaponIndex == 0)
                {
                    primaryBerserkerWeaponButtons[1].image.sprite = primaryBerserkerWeaponImages[7];
                    primaryBerserkerWeaponButtons[0].image.sprite = primaryBerserkerWeaponImages[0];
                    primaryWeaponIndex = i;
                }
                else if (primaryWeaponIndex == 1)
                {
                    primaryBerserkerWeaponButtons[1].image.sprite = primaryBerserkerWeaponImages[5];
                    primaryWeaponIndex = -1;
                }
                else if (primaryWeaponIndex == 2)
                {
                    primaryBerserkerWeaponButtons[1].image.sprite = primaryBerserkerWeaponImages[7];
                    primaryBerserkerWeaponButtons[2].image.sprite = primaryBerserkerWeaponImages[10];
                    primaryWeaponIndex = i;
                }
                else if (primaryWeaponIndex == 3)
                {
                    primaryBerserkerWeaponButtons[1].image.sprite = primaryBerserkerWeaponImages[7];
                    primaryBerserkerWeaponButtons[3].image.sprite = primaryBerserkerWeaponImages[15];
                    primaryWeaponIndex = i;
                }
                else if (primaryWeaponIndex == 4)
                {
                    primaryBerserkerWeaponButtons[0].image.sprite = primaryBerserkerWeaponImages[2];
                    primaryBerserkerWeaponButtons[4].image.sprite = primaryBerserkerWeaponImages[20];
                    primaryWeaponIndex = i;
                }
            }
            else if (i == 2)
            {
                if (primaryWeaponIndex == -1)
                {
                    primaryBerserkerWeaponButtons[2].image.sprite = primaryBerserkerWeaponImages[12];
                    primaryWeaponIndex = i;
                }
                else if (primaryWeaponIndex == 0)
                {
                    primaryBerserkerWeaponButtons[2].image.sprite = primaryBerserkerWeaponImages[12];
                    primaryBerserkerWeaponButtons[0].image.sprite = primaryBerserkerWeaponImages[0];
                    primaryWeaponIndex = i;
                }
                else if (primaryWeaponIndex == 1)
                {
                    primaryBerserkerWeaponButtons[2].image.sprite = primaryBerserkerWeaponImages[12];
                    primaryBerserkerWeaponButtons[1].image.sprite = primaryBerserkerWeaponImages[5];
                    primaryWeaponIndex = i;
                }
                else if (primaryWeaponIndex == 2)
                {
                    primaryBerserkerWeaponButtons[2].image.sprite = primaryBerserkerWeaponImages[10];
                    primaryWeaponIndex = -1;
                }
                else if (primaryWeaponIndex == 3)
                {
                    primaryBerserkerWeaponButtons[2].image.sprite = primaryBerserkerWeaponImages[12];
                    primaryBerserkerWeaponButtons[3].image.sprite = primaryBerserkerWeaponImages[15];
                    primaryWeaponIndex = i;
                }
                else if (primaryWeaponIndex == 4)
                {
                    primaryBerserkerWeaponButtons[2].image.sprite = primaryBerserkerWeaponImages[12];
                    primaryBerserkerWeaponButtons[4].image.sprite = primaryBerserkerWeaponImages[20];
                    primaryWeaponIndex = i;
                }
            }
            else if (i == 3)
            {
                if (primaryWeaponIndex == -1)
                {
                    primaryBerserkerWeaponButtons[3].image.sprite = primaryBerserkerWeaponImages[17];
                    primaryWeaponIndex = i;
                }
                else if (primaryWeaponIndex == 0)
                {
                    primaryBerserkerWeaponButtons[3].image.sprite = primaryBerserkerWeaponImages[17];
                    primaryBerserkerWeaponButtons[0].image.sprite = primaryBerserkerWeaponImages[0];
                    primaryWeaponIndex = i;
                }
                else if (primaryWeaponIndex == 1)
                {
                    primaryBerserkerWeaponButtons[3].image.sprite = primaryBerserkerWeaponImages[17];
                    primaryBerserkerWeaponButtons[1].image.sprite = primaryBerserkerWeaponImages[5];
                    primaryWeaponIndex = i;
                }
                else if (primaryWeaponIndex == 2)
                {
                    primaryBerserkerWeaponButtons[3].image.sprite = primaryBerserkerWeaponImages[17];
                    primaryBerserkerWeaponButtons[2].image.sprite = primaryBerserkerWeaponImages[10];
                    primaryWeaponIndex = i;
                }
                else if (primaryWeaponIndex == 3)
                {
                    primaryBerserkerWeaponButtons[3].image.sprite = primaryBerserkerWeaponImages[15];
                    primaryWeaponIndex = -1;
                }
                else if (primaryWeaponIndex == 4)
                {
                    primaryBerserkerWeaponButtons[3].image.sprite = primaryBerserkerWeaponImages[17];
                    primaryBerserkerWeaponButtons[4].image.sprite = primaryBerserkerWeaponImages[20];
                    primaryWeaponIndex = i;
                }
            }
            else if (i == 4)
            {
                if (primaryWeaponIndex == -1)
                {
                    primaryBerserkerWeaponButtons[4].image.sprite = primaryBerserkerWeaponImages[22];
                    primaryWeaponIndex = i;
                }
                else if (primaryWeaponIndex == 0)
                {
                    primaryBerserkerWeaponButtons[4].image.sprite = primaryBerserkerWeaponImages[22];
                    primaryBerserkerWeaponButtons[0].image.sprite = primaryBerserkerWeaponImages[0];
                    primaryWeaponIndex = i;
                }
                else if (primaryWeaponIndex == 1)
                {
                    primaryBerserkerWeaponButtons[4].image.sprite = primaryBerserkerWeaponImages[22];
                    primaryBerserkerWeaponButtons[1].image.sprite = primaryBerserkerWeaponImages[5];
                    primaryWeaponIndex = i;
                }
                else if (primaryWeaponIndex == 2)
                {
                    primaryBerserkerWeaponButtons[4].image.sprite = primaryBerserkerWeaponImages[22];
                    primaryBerserkerWeaponButtons[2].image.sprite = primaryBerserkerWeaponImages[10];
                    primaryWeaponIndex = i;
                }
                else if (primaryWeaponIndex == 3)
                {
                    primaryBerserkerWeaponButtons[4].image.sprite = primaryBerserkerWeaponImages[22];
                    primaryBerserkerWeaponButtons[3].image.sprite = primaryBerserkerWeaponImages[15];
                    primaryWeaponIndex = i;
                }
                else if (primaryWeaponIndex == 4)
                {
                    primaryBerserkerWeaponButtons[4].image.sprite = primaryBerserkerWeaponImages[20];
                    primaryWeaponIndex = -1;
                }
            }
        }
        else if (characterIndex == 3)
        {
            if (i == 0)
            {
                if (primaryWeaponIndex == -1)
                {
                    primaryArcherWeaponButtons[0].image.sprite = primaryArcherWeaponImages[2];
                    primaryWeaponIndex = i;
                }
                else if (primaryWeaponIndex == 0)
                {
                    primaryArcherWeaponButtons[0].image.sprite = primaryArcherWeaponImages[0];
                    primaryWeaponIndex = -1;
                }
                else if (primaryWeaponIndex == 1)
                {
                    primaryArcherWeaponButtons[0].image.sprite = primaryArcherWeaponImages[2];
                    primaryArcherWeaponButtons[1].image.sprite = primaryArcherWeaponImages[5];
                    primaryWeaponIndex = i;
                }
                else if (primaryWeaponIndex == 2)
                {
                    primaryArcherWeaponButtons[0].image.sprite = primaryArcherWeaponImages[2];
                    primaryArcherWeaponButtons[2].image.sprite = primaryArcherWeaponImages[10];
                    primaryWeaponIndex = i;
                }
                else if (primaryWeaponIndex == 3)
                {
                    primaryArcherWeaponButtons[0].image.sprite = primaryArcherWeaponImages[2];
                    primaryArcherWeaponButtons[3].image.sprite = primaryArcherWeaponImages[15];
                    primaryWeaponIndex = i;
                }
                else if (primaryWeaponIndex == 4)
                {
                    primaryArcherWeaponButtons[0].image.sprite = primaryArcherWeaponImages[2];
                    primaryArcherWeaponButtons[4].image.sprite = primaryArcherWeaponImages[20];
                    primaryWeaponIndex = i;
                }
            }
            else if (i == 1)
            {
                if (primaryWeaponIndex == -1)
                {
                    primaryArcherWeaponButtons[1].image.sprite = primaryArcherWeaponImages[7];
                    primaryWeaponIndex = i;
                }
                else if (primaryWeaponIndex == 0)
                {
                    primaryArcherWeaponButtons[1].image.sprite = primaryArcherWeaponImages[7];
                    primaryArcherWeaponButtons[0].image.sprite = primaryArcherWeaponImages[0];
                    primaryWeaponIndex = i;
                }
                else if (primaryWeaponIndex == 1)
                {
                    primaryArcherWeaponButtons[1].image.sprite = primaryArcherWeaponImages[5];
                    primaryWeaponIndex = -1;
                }
                else if (primaryWeaponIndex == 2)
                {
                    primaryArcherWeaponButtons[1].image.sprite = primaryArcherWeaponImages[7];
                    primaryArcherWeaponButtons[2].image.sprite = primaryArcherWeaponImages[10];
                    primaryWeaponIndex = i;
                }
                else if (primaryWeaponIndex == 3)
                {
                    primaryArcherWeaponButtons[1].image.sprite = primaryArcherWeaponImages[7];
                    primaryArcherWeaponButtons[3].image.sprite = primaryArcherWeaponImages[15];
                    primaryWeaponIndex = i;
                }
                else if (primaryWeaponIndex == 4)
                {
                    primaryArcherWeaponButtons[0].image.sprite = primaryArcherWeaponImages[2];
                    primaryArcherWeaponButtons[4].image.sprite = primaryArcherWeaponImages[20];
                    primaryWeaponIndex = i;
                }
            }
            else if (i == 2)
            {
                if (primaryWeaponIndex == -1)
                {
                    primaryArcherWeaponButtons[2].image.sprite = primaryArcherWeaponImages[12];
                    primaryWeaponIndex = i;
                }
                else if (primaryWeaponIndex == 0)
                {
                    primaryArcherWeaponButtons[2].image.sprite = primaryArcherWeaponImages[12];
                    primaryArcherWeaponButtons[0].image.sprite = primaryArcherWeaponImages[0];
                    primaryWeaponIndex = i;
                }
                else if (primaryWeaponIndex == 1)
                {
                    primaryArcherWeaponButtons[2].image.sprite = primaryArcherWeaponImages[12];
                    primaryArcherWeaponButtons[1].image.sprite = primaryArcherWeaponImages[5];
                    primaryWeaponIndex = i;
                }
                else if (primaryWeaponIndex == 2)
                {
                    primaryArcherWeaponButtons[2].image.sprite = primaryArcherWeaponImages[10];
                    primaryWeaponIndex = -1;
                }
                else if (primaryWeaponIndex == 3)
                {
                    primaryArcherWeaponButtons[2].image.sprite = primaryArcherWeaponImages[12];
                    primaryArcherWeaponButtons[3].image.sprite = primaryArcherWeaponImages[15];
                    primaryWeaponIndex = i;
                }
                else if (primaryWeaponIndex == 4)
                {
                    primaryArcherWeaponButtons[2].image.sprite = primaryArcherWeaponImages[12];
                    primaryArcherWeaponButtons[4].image.sprite = primaryArcherWeaponImages[20];
                    primaryWeaponIndex = i;
                }
            }
            else if (i == 3)
            {
                if (primaryWeaponIndex == -1)
                {
                    primaryArcherWeaponButtons[3].image.sprite = primaryArcherWeaponImages[17];
                    primaryWeaponIndex = i;
                }
                else if (primaryWeaponIndex == 0)
                {
                    primaryArcherWeaponButtons[3].image.sprite = primaryArcherWeaponImages[17];
                    primaryArcherWeaponButtons[0].image.sprite = primaryArcherWeaponImages[0];
                    primaryWeaponIndex = i;
                }
                else if (primaryWeaponIndex == 1)
                {
                    primaryArcherWeaponButtons[3].image.sprite = primaryArcherWeaponImages[17];
                    primaryArcherWeaponButtons[1].image.sprite = primaryArcherWeaponImages[5];
                    primaryWeaponIndex = i;
                }
                else if (primaryWeaponIndex == 2)
                {
                    primaryArcherWeaponButtons[3].image.sprite = primaryArcherWeaponImages[17];
                    primaryArcherWeaponButtons[2].image.sprite = primaryArcherWeaponImages[10];
                    primaryWeaponIndex = i;
                }
                else if (primaryWeaponIndex == 3)
                {
                    primaryArcherWeaponButtons[3].image.sprite = primaryArcherWeaponImages[15];
                    primaryWeaponIndex = -1;
                }
                else if (primaryWeaponIndex == 4)
                {
                    primaryArcherWeaponButtons[3].image.sprite = primaryArcherWeaponImages[17];
                    primaryArcherWeaponButtons[4].image.sprite = primaryArcherWeaponImages[20];
                    primaryWeaponIndex = i;
                }
            }
            else if (i == 4)
            {
                if (primaryWeaponIndex == -1)
                {
                    primaryArcherWeaponButtons[4].image.sprite = primaryArcherWeaponImages[22];
                    primaryWeaponIndex = i;
                }
                else if (primaryWeaponIndex == 0)
                {
                    primaryArcherWeaponButtons[4].image.sprite = primaryArcherWeaponImages[22];
                    primaryArcherWeaponButtons[0].image.sprite = primaryArcherWeaponImages[0];
                    primaryWeaponIndex = i;
                }
                else if (primaryWeaponIndex == 1)
                {
                    primaryArcherWeaponButtons[4].image.sprite = primaryArcherWeaponImages[22];
                    primaryArcherWeaponButtons[1].image.sprite = primaryArcherWeaponImages[5];
                    primaryWeaponIndex = i;
                }
                else if (primaryWeaponIndex == 2)
                {
                    primaryArcherWeaponButtons[4].image.sprite = primaryArcherWeaponImages[22];
                    primaryArcherWeaponButtons[2].image.sprite = primaryArcherWeaponImages[10];
                    primaryWeaponIndex = i;
                }
                else if (primaryWeaponIndex == 3)
                {
                    primaryArcherWeaponButtons[4].image.sprite = primaryArcherWeaponImages[22];
                    primaryArcherWeaponButtons[3].image.sprite = primaryArcherWeaponImages[15];
                    primaryWeaponIndex = i;
                }
                else if (primaryWeaponIndex == 4)
                {
                    primaryArcherWeaponButtons[4].image.sprite = primaryArcherWeaponImages[20];
                    primaryWeaponIndex = -1;
                }
            }
        }
        PrimaryWeaponButtonPointerEnter(i);
    }

    public void ClickSecondaryWeaponButton(int i)
    {
        if (characterIndex == 0)
        {
            if (i == 0)
            {
                if (secondaryWeaponIndex == -1)
                {
                    secondaryKnightWeaponButtons[0].image.sprite = secondaryKnightWeaponImages[2];
                    secondaryWeaponIndex = i;
                }
                else if (secondaryWeaponIndex == 0)
                {
                    secondaryKnightWeaponButtons[0].image.sprite = secondaryKnightWeaponImages[0];
                    secondaryWeaponIndex = -1;
                }
                else if (secondaryWeaponIndex == 1)
                {
                    secondaryKnightWeaponButtons[0].image.sprite = secondaryKnightWeaponImages[2];
                    secondaryKnightWeaponButtons[1].image.sprite = secondaryKnightWeaponImages[5];
                    secondaryWeaponIndex = i;
                }
                else if (secondaryWeaponIndex == 2)
                {
                    secondaryKnightWeaponButtons[0].image.sprite = secondaryKnightWeaponImages[2];
                    secondaryKnightWeaponButtons[2].image.sprite = secondaryKnightWeaponImages[10];
                    secondaryWeaponIndex = i;
                }
                else if (secondaryWeaponIndex == 3)
                {
                    secondaryKnightWeaponButtons[0].image.sprite = secondaryKnightWeaponImages[2];
                    secondaryKnightWeaponButtons[3].image.sprite = secondaryKnightWeaponImages[15];
                    secondaryWeaponIndex = i;
                }
                else if (secondaryWeaponIndex == 4)
                {
                    secondaryKnightWeaponButtons[0].image.sprite = secondaryKnightWeaponImages[2];
                    secondaryKnightWeaponButtons[4].image.sprite = secondaryKnightWeaponImages[20];
                    secondaryWeaponIndex = i;
                }
            }
            else if (i == 1)
            {
                if (secondaryWeaponIndex == -1)
                {
                    secondaryKnightWeaponButtons[1].image.sprite = secondaryKnightWeaponImages[7];
                    secondaryWeaponIndex = i;
                }
                else if (secondaryWeaponIndex == 0)
                {
                    secondaryKnightWeaponButtons[1].image.sprite = secondaryKnightWeaponImages[7];
                    secondaryKnightWeaponButtons[0].image.sprite = secondaryKnightWeaponImages[0];
                    secondaryWeaponIndex = i;
                }
                else if (secondaryWeaponIndex == 1)
                {
                    secondaryKnightWeaponButtons[1].image.sprite = secondaryKnightWeaponImages[5];
                    secondaryWeaponIndex = -1;
                }
                else if (secondaryWeaponIndex == 2)
                {
                    secondaryKnightWeaponButtons[1].image.sprite = secondaryKnightWeaponImages[7];
                    secondaryKnightWeaponButtons[2].image.sprite = secondaryKnightWeaponImages[10];
                    secondaryWeaponIndex = i;
                }
                else if (secondaryWeaponIndex == 3)
                {
                    secondaryKnightWeaponButtons[1].image.sprite = secondaryKnightWeaponImages[7];
                    secondaryKnightWeaponButtons[3].image.sprite = secondaryKnightWeaponImages[15];
                    secondaryWeaponIndex = i;
                }
                else if (secondaryWeaponIndex == 4)
                {
                    secondaryKnightWeaponButtons[0].image.sprite = secondaryKnightWeaponImages[2];
                    secondaryKnightWeaponButtons[4].image.sprite = secondaryKnightWeaponImages[20];
                    secondaryWeaponIndex = i;
                }
            }
            else if (i == 2)
            {
                if (secondaryWeaponIndex == -1)
                {
                    secondaryKnightWeaponButtons[2].image.sprite = secondaryKnightWeaponImages[12];
                    secondaryWeaponIndex = i;
                }
                else if (secondaryWeaponIndex == 0)
                {
                    secondaryKnightWeaponButtons[2].image.sprite = secondaryKnightWeaponImages[12];
                    secondaryKnightWeaponButtons[0].image.sprite = secondaryKnightWeaponImages[0];
                    secondaryWeaponIndex = i;
                }
                else if (secondaryWeaponIndex == 1)
                {
                    secondaryKnightWeaponButtons[2].image.sprite = secondaryKnightWeaponImages[12];
                    secondaryKnightWeaponButtons[1].image.sprite = secondaryKnightWeaponImages[5];
                    secondaryWeaponIndex = i;
                }
                else if (secondaryWeaponIndex == 2)
                {
                    secondaryKnightWeaponButtons[2].image.sprite = secondaryKnightWeaponImages[10];
                    secondaryWeaponIndex = -1;
                }
                else if (secondaryWeaponIndex == 3)
                {
                    secondaryKnightWeaponButtons[2].image.sprite = secondaryKnightWeaponImages[12];
                    secondaryKnightWeaponButtons[3].image.sprite = secondaryKnightWeaponImages[15];
                    secondaryWeaponIndex = i;
                }
                else if (secondaryWeaponIndex == 4)
                {
                    secondaryKnightWeaponButtons[2].image.sprite = secondaryKnightWeaponImages[12];
                    secondaryKnightWeaponButtons[4].image.sprite = secondaryKnightWeaponImages[20];
                    secondaryWeaponIndex = i;
                }
            }
            else if (i == 3)
            {
                if (secondaryWeaponIndex == -1)
                {
                    secondaryKnightWeaponButtons[3].image.sprite = secondaryKnightWeaponImages[17];
                    secondaryWeaponIndex = i;
                }
                else if (secondaryWeaponIndex == 0)
                {
                    secondaryKnightWeaponButtons[3].image.sprite = secondaryKnightWeaponImages[17];
                    secondaryKnightWeaponButtons[0].image.sprite = secondaryKnightWeaponImages[0];
                    secondaryWeaponIndex = i;
                }
                else if (secondaryWeaponIndex == 1)
                {
                    secondaryKnightWeaponButtons[3].image.sprite = secondaryKnightWeaponImages[17];
                    secondaryKnightWeaponButtons[1].image.sprite = secondaryKnightWeaponImages[5];
                    secondaryWeaponIndex = i;
                }
                else if (secondaryWeaponIndex == 2)
                {
                    secondaryKnightWeaponButtons[3].image.sprite = secondaryKnightWeaponImages[17];
                    secondaryKnightWeaponButtons[2].image.sprite = secondaryKnightWeaponImages[10];
                    secondaryWeaponIndex = i;
                }
                else if (secondaryWeaponIndex == 3)
                {
                    secondaryKnightWeaponButtons[3].image.sprite = secondaryKnightWeaponImages[15];
                    secondaryWeaponIndex = -1;
                }
                else if (secondaryWeaponIndex == 4)
                {
                    secondaryKnightWeaponButtons[3].image.sprite = secondaryKnightWeaponImages[17];
                    secondaryKnightWeaponButtons[4].image.sprite = secondaryKnightWeaponImages[20];
                    secondaryWeaponIndex = i;
                }
            }
            else if (i == 4)
            {
                if (secondaryWeaponIndex == -1)
                {
                    secondaryKnightWeaponButtons[4].image.sprite = secondaryKnightWeaponImages[22];
                    secondaryWeaponIndex = i;
                }
                else if (secondaryWeaponIndex == 0)
                {
                    secondaryKnightWeaponButtons[4].image.sprite = secondaryKnightWeaponImages[22];
                    secondaryKnightWeaponButtons[0].image.sprite = secondaryKnightWeaponImages[0];
                    secondaryWeaponIndex = i;
                }
                else if (secondaryWeaponIndex == 1)
                {
                    secondaryKnightWeaponButtons[4].image.sprite = secondaryKnightWeaponImages[22];
                    secondaryKnightWeaponButtons[1].image.sprite = secondaryKnightWeaponImages[5];
                    secondaryWeaponIndex = i;
                }
                else if (secondaryWeaponIndex == 2)
                {
                    secondaryKnightWeaponButtons[4].image.sprite = secondaryKnightWeaponImages[22];
                    secondaryKnightWeaponButtons[2].image.sprite = secondaryKnightWeaponImages[10];
                    secondaryWeaponIndex = i;
                }
                else if (secondaryWeaponIndex == 3)
                {
                    secondaryKnightWeaponButtons[4].image.sprite = secondaryKnightWeaponImages[22];
                    secondaryKnightWeaponButtons[3].image.sprite = secondaryKnightWeaponImages[15];
                    secondaryWeaponIndex = i;
                }
                else if (secondaryWeaponIndex == 4)
                {
                    secondaryKnightWeaponButtons[4].image.sprite = secondaryKnightWeaponImages[20];
                    secondaryWeaponIndex = -1;
                }
            }
        }
        else if (characterIndex == 1)
        {
            if (i == 0)
            {
                if (secondaryWeaponIndex == -1)
                {
                    secondaryScoutWeaponButtons[0].image.sprite = secondaryScoutWeaponImages[2];
                    secondaryWeaponIndex = i;
                }
                else if (secondaryWeaponIndex == 0)
                {
                    secondaryScoutWeaponButtons[0].image.sprite = secondaryScoutWeaponImages[0];
                    secondaryWeaponIndex = -1;
                }
                else if (secondaryWeaponIndex == 1)
                {
                    secondaryScoutWeaponButtons[0].image.sprite = secondaryScoutWeaponImages[2];
                    secondaryScoutWeaponButtons[1].image.sprite = secondaryScoutWeaponImages[5];
                    secondaryWeaponIndex = i;
                }
                else if (secondaryWeaponIndex == 2)
                {
                    secondaryScoutWeaponButtons[0].image.sprite = secondaryScoutWeaponImages[2];
                    secondaryScoutWeaponButtons[2].image.sprite = secondaryScoutWeaponImages[10];
                    secondaryWeaponIndex = i;
                }
                else if (secondaryWeaponIndex == 3)
                {
                    secondaryScoutWeaponButtons[0].image.sprite = secondaryScoutWeaponImages[2];
                    secondaryScoutWeaponButtons[3].image.sprite = secondaryScoutWeaponImages[15];
                    secondaryWeaponIndex = i;
                }
                else if (secondaryWeaponIndex == 4)
                {
                    secondaryScoutWeaponButtons[0].image.sprite = secondaryScoutWeaponImages[2];
                    secondaryScoutWeaponButtons[4].image.sprite = secondaryScoutWeaponImages[20];
                    secondaryWeaponIndex = i;
                }
            }
            else if (i == 1)
            {
                if (secondaryWeaponIndex == -1)
                {
                    secondaryScoutWeaponButtons[1].image.sprite = secondaryScoutWeaponImages[7];
                    secondaryWeaponIndex = i;
                }
                else if (secondaryWeaponIndex == 0)
                {
                    secondaryScoutWeaponButtons[1].image.sprite = secondaryScoutWeaponImages[7];
                    secondaryScoutWeaponButtons[0].image.sprite = secondaryScoutWeaponImages[0];
                    secondaryWeaponIndex = i;
                }
                else if (secondaryWeaponIndex == 1)
                {
                    secondaryScoutWeaponButtons[1].image.sprite = secondaryScoutWeaponImages[5];
                    secondaryWeaponIndex = -1;
                }
                else if (secondaryWeaponIndex == 2)
                {
                    secondaryScoutWeaponButtons[1].image.sprite = secondaryScoutWeaponImages[7];
                    secondaryScoutWeaponButtons[2].image.sprite = secondaryScoutWeaponImages[10];
                    secondaryWeaponIndex = i;
                }
                else if (secondaryWeaponIndex == 3)
                {
                    secondaryScoutWeaponButtons[1].image.sprite = secondaryScoutWeaponImages[7];
                    secondaryScoutWeaponButtons[3].image.sprite = secondaryScoutWeaponImages[15];
                    secondaryWeaponIndex = i;
                }
                else if (secondaryWeaponIndex == 4)
                {
                    secondaryScoutWeaponButtons[0].image.sprite = secondaryScoutWeaponImages[2];
                    secondaryScoutWeaponButtons[4].image.sprite = secondaryScoutWeaponImages[20];
                    secondaryWeaponIndex = i;
                }
            }
            else if (i == 2)
            {
                if (secondaryWeaponIndex == -1)
                {
                    secondaryScoutWeaponButtons[2].image.sprite = secondaryScoutWeaponImages[12];
                    secondaryWeaponIndex = i;
                }
                else if (secondaryWeaponIndex == 0)
                {
                    secondaryScoutWeaponButtons[2].image.sprite = secondaryScoutWeaponImages[12];
                    secondaryScoutWeaponButtons[0].image.sprite = secondaryScoutWeaponImages[0];
                    secondaryWeaponIndex = i;
                }
                else if (secondaryWeaponIndex == 1)
                {
                    secondaryScoutWeaponButtons[2].image.sprite = secondaryScoutWeaponImages[12];
                    secondaryScoutWeaponButtons[1].image.sprite = secondaryScoutWeaponImages[5];
                    secondaryWeaponIndex = i;
                }
                else if (secondaryWeaponIndex == 2)
                {
                    secondaryScoutWeaponButtons[2].image.sprite = secondaryScoutWeaponImages[10];
                    secondaryWeaponIndex = -1;
                }
                else if (primaryWeaponIndex == 3)
                {
                    secondaryScoutWeaponButtons[2].image.sprite = secondaryScoutWeaponImages[12];
                    secondaryScoutWeaponButtons[3].image.sprite = secondaryScoutWeaponImages[15];
                    secondaryWeaponIndex = i;
                }
                else if (secondaryWeaponIndex == 4)
                {
                    secondaryScoutWeaponButtons[2].image.sprite = secondaryScoutWeaponImages[12];
                    secondaryScoutWeaponButtons[4].image.sprite = secondaryScoutWeaponImages[20];
                    secondaryWeaponIndex = i;
                }
            }
            else if (i == 3)
            {
                if (secondaryWeaponIndex == -1)
                {
                    secondaryScoutWeaponButtons[3].image.sprite = secondaryScoutWeaponImages[17];
                    secondaryWeaponIndex = i;
                }
                else if (secondaryWeaponIndex == 0)
                {
                    secondaryScoutWeaponButtons[3].image.sprite = secondaryScoutWeaponImages[17];
                    secondaryScoutWeaponButtons[0].image.sprite = secondaryScoutWeaponImages[0];
                    secondaryWeaponIndex = i;
                }
                else if (secondaryWeaponIndex == 1)
                {
                    secondaryScoutWeaponButtons[3].image.sprite = secondaryScoutWeaponImages[17];
                    secondaryScoutWeaponButtons[1].image.sprite = secondaryScoutWeaponImages[5];
                    secondaryWeaponIndex = i;
                }
                else if (secondaryWeaponIndex == 2)
                {
                    secondaryScoutWeaponButtons[3].image.sprite = secondaryScoutWeaponImages[17];
                    secondaryScoutWeaponButtons[2].image.sprite = secondaryScoutWeaponImages[10];
                    secondaryWeaponIndex = i;
                }
                else if (secondaryWeaponIndex == 3)
                {
                    secondaryScoutWeaponButtons[3].image.sprite = secondaryScoutWeaponImages[15];
                    secondaryWeaponIndex = -1;
                }
                else if (secondaryWeaponIndex == 4)
                {
                    secondaryScoutWeaponButtons[3].image.sprite = secondaryScoutWeaponImages[17];
                    secondaryScoutWeaponButtons[4].image.sprite = secondaryScoutWeaponImages[20];
                    secondaryWeaponIndex = i;
                }
            }
            else if (i == 4)
            {
                if (secondaryWeaponIndex == -1)
                {
                    secondaryScoutWeaponButtons[4].image.sprite = secondaryScoutWeaponImages[22];
                    secondaryWeaponIndex = i;
                }
                else if (secondaryWeaponIndex == 0)
                {
                    secondaryScoutWeaponButtons[4].image.sprite = secondaryScoutWeaponImages[22];
                    secondaryScoutWeaponButtons[0].image.sprite = secondaryScoutWeaponImages[0];
                    secondaryWeaponIndex = i;
                }
                else if (secondaryWeaponIndex == 1)
                {
                    secondaryScoutWeaponButtons[4].image.sprite = secondaryScoutWeaponImages[22];
                    secondaryScoutWeaponButtons[1].image.sprite = secondaryScoutWeaponImages[5];
                    secondaryWeaponIndex = i;
                }
                else if (secondaryWeaponIndex == 2)
                {
                    secondaryScoutWeaponButtons[4].image.sprite = secondaryScoutWeaponImages[22];
                    secondaryScoutWeaponButtons[2].image.sprite = secondaryScoutWeaponImages[10];
                    secondaryWeaponIndex = i;
                }
                else if (secondaryWeaponIndex == 3)
                {
                    secondaryScoutWeaponButtons[4].image.sprite = secondaryScoutWeaponImages[22];
                    secondaryScoutWeaponButtons[3].image.sprite = secondaryScoutWeaponImages[15];
                    secondaryWeaponIndex = i;
                }
                else if (secondaryWeaponIndex == 4)
                {
                    secondaryScoutWeaponButtons[4].image.sprite = secondaryScoutWeaponImages[20];
                    secondaryWeaponIndex = -1;
                }
            }
        }
        else if (characterIndex == 2)
        {
            if (i == 0)
            {
                if (secondaryWeaponIndex == -1)
                {
                    secondaryBerserkerWeaponButtons[0].image.sprite = secondaryBerserkerWeaponImages[2];
                    secondaryWeaponIndex = i;
                }
                else if (secondaryWeaponIndex == 0)
                {
                    secondaryBerserkerWeaponButtons[0].image.sprite = secondaryBerserkerWeaponImages[0];
                    secondaryWeaponIndex = -1;
                }
                else if (secondaryWeaponIndex == 1)
                {
                    secondaryBerserkerWeaponButtons[0].image.sprite = secondaryBerserkerWeaponImages[2];
                    secondaryBerserkerWeaponButtons[1].image.sprite = secondaryBerserkerWeaponImages[5];
                    secondaryWeaponIndex = i;
                }
                else if (secondaryWeaponIndex == 2)
                {
                    secondaryBerserkerWeaponButtons[0].image.sprite = secondaryBerserkerWeaponImages[2];
                    secondaryBerserkerWeaponButtons[2].image.sprite = secondaryBerserkerWeaponImages[10];
                    secondaryWeaponIndex = i;
                }
                else if (secondaryWeaponIndex == 3)
                {
                    secondaryBerserkerWeaponButtons[0].image.sprite = secondaryBerserkerWeaponImages[2];
                    secondaryBerserkerWeaponButtons[3].image.sprite = secondaryBerserkerWeaponImages[15];
                    secondaryWeaponIndex = i;
                }
                else if (secondaryWeaponIndex == 4)
                {
                    secondaryBerserkerWeaponButtons[0].image.sprite = secondaryBerserkerWeaponImages[2];
                    secondaryBerserkerWeaponButtons[4].image.sprite = secondaryBerserkerWeaponImages[20];
                    secondaryWeaponIndex = i;
                }
            }
            else if (i == 1)
            {
                if (secondaryWeaponIndex == -1)
                {
                    secondaryBerserkerWeaponButtons[1].image.sprite = secondaryBerserkerWeaponImages[7];
                    secondaryWeaponIndex = i;
                }
                else if (secondaryWeaponIndex == 0)
                {
                    secondaryBerserkerWeaponButtons[1].image.sprite = secondaryBerserkerWeaponImages[7];
                    secondaryBerserkerWeaponButtons[0].image.sprite = secondaryBerserkerWeaponImages[0];
                    secondaryWeaponIndex = i;
                }
                else if (secondaryWeaponIndex == 1)
                {
                    secondaryBerserkerWeaponButtons[1].image.sprite = secondaryBerserkerWeaponImages[5];
                    secondaryWeaponIndex = -1;
                }
                else if (secondaryWeaponIndex == 2)
                {
                    secondaryBerserkerWeaponButtons[1].image.sprite = secondaryBerserkerWeaponImages[7];
                    secondaryBerserkerWeaponButtons[2].image.sprite = secondaryBerserkerWeaponImages[10];
                    secondaryWeaponIndex = i;
                }
                else if (secondaryWeaponIndex == 3)
                {
                    secondaryBerserkerWeaponButtons[1].image.sprite = secondaryBerserkerWeaponImages[7];
                    secondaryBerserkerWeaponButtons[3].image.sprite = secondaryBerserkerWeaponImages[15];
                    secondaryWeaponIndex = i;
                }
                else if (secondaryWeaponIndex == 4)
                {
                    secondaryBerserkerWeaponButtons[0].image.sprite = secondaryBerserkerWeaponImages[2];
                    secondaryBerserkerWeaponButtons[4].image.sprite = secondaryBerserkerWeaponImages[20];
                    secondaryWeaponIndex = i;
                }
            }
            else if (i == 2)
            {
                if (secondaryWeaponIndex == -1)
                {
                    secondaryBerserkerWeaponButtons[2].image.sprite = secondaryBerserkerWeaponImages[12];
                    secondaryWeaponIndex = i;
                }
                else if (secondaryWeaponIndex == 0)
                {
                    secondaryBerserkerWeaponButtons[2].image.sprite = secondaryBerserkerWeaponImages[12];
                    secondaryBerserkerWeaponButtons[0].image.sprite = secondaryBerserkerWeaponImages[0];
                    secondaryWeaponIndex = i;
                }
                else if (secondaryWeaponIndex == 1)
                {
                    secondaryBerserkerWeaponButtons[2].image.sprite = secondaryBerserkerWeaponImages[12];
                    secondaryBerserkerWeaponButtons[1].image.sprite = secondaryBerserkerWeaponImages[5];
                    secondaryWeaponIndex = i;
                }
                else if (secondaryWeaponIndex == 2)
                {
                    secondaryBerserkerWeaponButtons[2].image.sprite = secondaryBerserkerWeaponImages[10];
                    secondaryWeaponIndex = -1;
                }
                else if (secondaryWeaponIndex == 3)
                {
                    secondaryBerserkerWeaponButtons[2].image.sprite = secondaryBerserkerWeaponImages[12];
                    secondaryBerserkerWeaponButtons[3].image.sprite = secondaryBerserkerWeaponImages[15];
                    secondaryWeaponIndex = i;
                }
                else if (secondaryWeaponIndex == 4)
                {
                    secondaryBerserkerWeaponButtons[2].image.sprite = secondaryBerserkerWeaponImages[12];
                    secondaryBerserkerWeaponButtons[4].image.sprite = secondaryBerserkerWeaponImages[20];
                    secondaryWeaponIndex = i;
                }
            }
            else if (i == 3)
            {
                if (secondaryWeaponIndex == -1)
                {
                    secondaryBerserkerWeaponButtons[3].image.sprite = secondaryBerserkerWeaponImages[17];
                    secondaryWeaponIndex = i;
                }
                else if (secondaryWeaponIndex == 0)
                {
                    secondaryBerserkerWeaponButtons[3].image.sprite = secondaryBerserkerWeaponImages[17];
                    secondaryBerserkerWeaponButtons[0].image.sprite = secondaryBerserkerWeaponImages[0];
                    secondaryWeaponIndex = i;
                }
                else if (secondaryWeaponIndex == 1)
                {
                    secondaryBerserkerWeaponButtons[3].image.sprite = secondaryBerserkerWeaponImages[17];
                    secondaryBerserkerWeaponButtons[1].image.sprite = secondaryBerserkerWeaponImages[5];
                    secondaryWeaponIndex = i;
                }
                else if (secondaryWeaponIndex == 2)
                {
                    secondaryBerserkerWeaponButtons[3].image.sprite = secondaryBerserkerWeaponImages[17];
                    secondaryBerserkerWeaponButtons[2].image.sprite = secondaryBerserkerWeaponImages[10];
                    secondaryWeaponIndex = i;
                }
                else if (secondaryWeaponIndex == 3)
                {
                    secondaryBerserkerWeaponButtons[3].image.sprite = secondaryBerserkerWeaponImages[15];
                    secondaryWeaponIndex = -1;
                }
                else if (secondaryWeaponIndex == 4)
                {
                    secondaryBerserkerWeaponButtons[3].image.sprite = secondaryBerserkerWeaponImages[17];
                    secondaryBerserkerWeaponButtons[4].image.sprite = secondaryBerserkerWeaponImages[20];
                    secondaryWeaponIndex = i;
                }
            }            
        }
        else if (characterIndex == 3)
        {
            if (i == 0)
            {
                if (secondaryWeaponIndex == -1)
                {
                    secondaryArcherWeaponButtons[0].image.sprite = secondaryArcherWeaponImages[2];
                    secondaryWeaponIndex = i;
                }
                else if (secondaryWeaponIndex == 0)
                {
                    secondaryArcherWeaponButtons[0].image.sprite = secondaryArcherWeaponImages[0];
                    secondaryWeaponIndex = -1;
                }
                else if (secondaryWeaponIndex == 1)
                {
                    secondaryArcherWeaponButtons[0].image.sprite = secondaryArcherWeaponImages[2];
                    secondaryArcherWeaponButtons[1].image.sprite = secondaryArcherWeaponImages[5];
                    secondaryWeaponIndex = i;
                }
                else if (secondaryWeaponIndex == 2)
                {
                    secondaryArcherWeaponButtons[0].image.sprite = secondaryArcherWeaponImages[2];
                    secondaryArcherWeaponButtons[2].image.sprite = secondaryArcherWeaponImages[10];
                    secondaryWeaponIndex = i;
                }
                else if (secondaryWeaponIndex == 3)
                {
                    secondaryArcherWeaponButtons[0].image.sprite = secondaryArcherWeaponImages[2];
                    secondaryArcherWeaponButtons[3].image.sprite = secondaryArcherWeaponImages[15];
                    secondaryWeaponIndex = i;
                }
                else if (secondaryWeaponIndex == 4)
                {
                    secondaryArcherWeaponButtons[0].image.sprite = secondaryArcherWeaponImages[2];
                    secondaryArcherWeaponButtons[4].image.sprite = secondaryArcherWeaponImages[20];
                    secondaryWeaponIndex = i;
                }
            }
            else if (i == 1)
            {
                if (secondaryWeaponIndex == -1)
                {
                    secondaryArcherWeaponButtons[1].image.sprite = secondaryArcherWeaponImages[7];
                    secondaryWeaponIndex = i;
                }
                else if (secondaryWeaponIndex == 0)
                {
                    secondaryArcherWeaponButtons[1].image.sprite = secondaryArcherWeaponImages[7];
                    secondaryArcherWeaponButtons[0].image.sprite = secondaryArcherWeaponImages[0];
                    secondaryWeaponIndex = i;
                }
                else if (secondaryWeaponIndex == 1)
                {
                    secondaryArcherWeaponButtons[1].image.sprite = secondaryArcherWeaponImages[5];
                    secondaryWeaponIndex = -1;
                }
                else if (secondaryWeaponIndex == 2)
                {
                    secondaryArcherWeaponButtons[1].image.sprite = secondaryArcherWeaponImages[7];
                    secondaryArcherWeaponButtons[2].image.sprite = secondaryArcherWeaponImages[10];
                    secondaryWeaponIndex = i;
                }
                else if (secondaryWeaponIndex == 3)
                {
                    secondaryArcherWeaponButtons[1].image.sprite = secondaryArcherWeaponImages[7];
                    secondaryArcherWeaponButtons[3].image.sprite = secondaryArcherWeaponImages[15];
                    secondaryWeaponIndex = i;
                }
                else if (secondaryWeaponIndex == 4)
                {
                    secondaryArcherWeaponButtons[0].image.sprite = secondaryArcherWeaponImages[2];
                    secondaryArcherWeaponButtons[4].image.sprite = secondaryArcherWeaponImages[20];
                    secondaryWeaponIndex = i;
                }
            }
            else if (i == 2)
            {
                if (secondaryWeaponIndex == -1)
                {
                    secondaryArcherWeaponButtons[2].image.sprite = secondaryArcherWeaponImages[12];
                    secondaryWeaponIndex = i;
                }
                else if (secondaryWeaponIndex == 0)
                {
                    secondaryArcherWeaponButtons[2].image.sprite = secondaryArcherWeaponImages[12];
                    secondaryArcherWeaponButtons[0].image.sprite = secondaryArcherWeaponImages[0];
                    secondaryWeaponIndex = i;
                }
                else if (secondaryWeaponIndex == 1)
                {
                    secondaryArcherWeaponButtons[2].image.sprite = secondaryArcherWeaponImages[12];
                    secondaryArcherWeaponButtons[1].image.sprite = secondaryArcherWeaponImages[5];
                    secondaryWeaponIndex = i;
                }
                else if (secondaryWeaponIndex == 2)
                {
                    secondaryArcherWeaponButtons[2].image.sprite = secondaryArcherWeaponImages[10];
                    secondaryWeaponIndex = -1;
                }
                else if (secondaryWeaponIndex == 3)
                {
                    secondaryArcherWeaponButtons[2].image.sprite = secondaryArcherWeaponImages[12];
                    secondaryArcherWeaponButtons[3].image.sprite = secondaryArcherWeaponImages[15];
                    secondaryWeaponIndex = i;
                }
                else if (secondaryWeaponIndex == 4)
                {
                    secondaryArcherWeaponButtons[2].image.sprite = secondaryArcherWeaponImages[12];
                    secondaryArcherWeaponButtons[4].image.sprite = secondaryArcherWeaponImages[20];
                    secondaryWeaponIndex = i;
                }
            }
            else if (i == 3)
            {
                if (secondaryWeaponIndex == -1)
                {
                    secondaryArcherWeaponButtons[3].image.sprite = secondaryArcherWeaponImages[17];
                    secondaryWeaponIndex = i;
                }
                else if (secondaryWeaponIndex == 0)
                {
                    secondaryArcherWeaponButtons[3].image.sprite = secondaryArcherWeaponImages[17];
                    secondaryArcherWeaponButtons[0].image.sprite = secondaryArcherWeaponImages[0];
                    secondaryWeaponIndex = i;
                }
                else if (secondaryWeaponIndex == 1)
                {
                    secondaryArcherWeaponButtons[3].image.sprite = secondaryArcherWeaponImages[17];
                    secondaryArcherWeaponButtons[1].image.sprite = secondaryArcherWeaponImages[5];
                    secondaryWeaponIndex = i;
                }
                else if (secondaryWeaponIndex == 2)
                {
                    secondaryArcherWeaponButtons[3].image.sprite = secondaryArcherWeaponImages[17];
                    secondaryArcherWeaponButtons[2].image.sprite = secondaryArcherWeaponImages[10];
                    secondaryWeaponIndex = i;
                }
                else if (secondaryWeaponIndex == 3)
                {
                    secondaryArcherWeaponButtons[3].image.sprite = secondaryArcherWeaponImages[15];
                    secondaryWeaponIndex = -1;
                }
                else if (secondaryWeaponIndex == 4)
                {
                    secondaryArcherWeaponButtons[3].image.sprite = secondaryArcherWeaponImages[17];
                    secondaryArcherWeaponButtons[4].image.sprite = secondaryArcherWeaponImages[20];
                    secondaryWeaponIndex = i;
                }
            }
            else if (i == 4)
            {
                if (secondaryWeaponIndex == -1)
                {
                    secondaryArcherWeaponButtons[4].image.sprite = secondaryArcherWeaponImages[22];
                    secondaryWeaponIndex = i;
                }
                else if (secondaryWeaponIndex == 0)
                {
                    secondaryArcherWeaponButtons[4].image.sprite = secondaryArcherWeaponImages[22];
                    secondaryArcherWeaponButtons[0].image.sprite = secondaryArcherWeaponImages[0];
                    secondaryWeaponIndex = i;
                }
                else if (secondaryWeaponIndex == 1)
                {
                    secondaryArcherWeaponButtons[4].image.sprite = secondaryArcherWeaponImages[22];
                    secondaryArcherWeaponButtons[1].image.sprite = secondaryArcherWeaponImages[5];
                    secondaryWeaponIndex = i;
                }
                else if (secondaryWeaponIndex == 2)
                {
                    secondaryArcherWeaponButtons[4].image.sprite = secondaryArcherWeaponImages[22];
                    secondaryArcherWeaponButtons[2].image.sprite = secondaryArcherWeaponImages[10];
                    secondaryWeaponIndex = i;
                }
                else if (secondaryWeaponIndex == 3)
                {
                    secondaryArcherWeaponButtons[4].image.sprite = secondaryArcherWeaponImages[22];
                    secondaryArcherWeaponButtons[3].image.sprite = secondaryArcherWeaponImages[15];
                    secondaryWeaponIndex = i;
                }
                else if (secondaryWeaponIndex == 4)
                {
                    secondaryArcherWeaponButtons[4].image.sprite = secondaryArcherWeaponImages[20];
                    secondaryWeaponIndex = -1;
                }
            }
        }
        SecondaryWeaponButtonPointerEnter(i);
    }

    public void ClickWeaponBackButton(int i)
    {
        if(teamIndex == 0)
        {
            if(characterIndex == 0)
            {
                blueCharacterLayer.gameObject.SetActive(true);
                KnightWeaponsLayer.gameObject.SetActive(false);
            }
            else if (characterIndex == 1)
            {
                blueCharacterLayer.gameObject.SetActive(true);
                ScoutWeaponsLayer.gameObject.SetActive(false);
            }
            else if (characterIndex == 2)
            {
                blueCharacterLayer.gameObject.SetActive(true);
                BerserkerWeaponsLayer.gameObject.SetActive(false);
            }
            else if (characterIndex == 3)
            {
                blueCharacterLayer.gameObject.SetActive(true);
                ArcherWeaponsLayer.gameObject.SetActive(false);
            }           
        }
        else if (teamIndex == 1)
        {
            if (characterIndex == 0)
            {
                redCharacterLayer.gameObject.SetActive(true);
                KnightWeaponsLayer.gameObject.SetActive(false);
            }
            else if (characterIndex == 1)
            {
                redCharacterLayer.gameObject.SetActive(true);
                ScoutWeaponsLayer.gameObject.SetActive(false);
            }
            else if (characterIndex == 2)
            {
                redCharacterLayer.gameObject.SetActive(true);
                BerserkerWeaponsLayer.gameObject.SetActive(false);
            }
            else if (characterIndex == 3)
            {
                redCharacterLayer.gameObject.SetActive(true);
                ArcherWeaponsLayer.gameObject.SetActive(false);
            }
        }
    }
}
