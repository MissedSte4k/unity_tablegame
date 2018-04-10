using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class KeybindManager : MonoBehaviour {

    private static KeybindManager instance;

    public static KeybindManager MyInstance
    {
        get
        {
            if(instance == null)
            {
                instance = FindObjectOfType<KeybindManager>();
            }
            return instance;
        }
    }

    public Dictionary<string, KeyCode> Keybinds { get; private set; }

    private string bindName;

	// Use this for initialization
	void Start ()
    {
        Keybinds = new Dictionary<string, KeyCode>();

        BindKey("Button(MoveForward)", (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("Button(MoveForward)", "W")));
        BindKey("Button(MoveBackward)", (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("Button(MoveBackward)", "S")));
        BindKey("Button(MoveLeft)", (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("Button(MoveLeft)", "A")));
        BindKey("Button(MoveRight)", (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("Button(MoveRight)", "D")));
    }
	

	void BindKey (string key, KeyCode keyBind)
    {
        Dictionary<string, KeyCode> currentDictionary = Keybinds;

        if(!currentDictionary.ContainsKey(key))
        {
            currentDictionary.Add(key, keyBind);
            MenuSettings.instance.UpdateKeyText(key, keyBind);
        }
        else if(currentDictionary.ContainsValue(keyBind))
        {
            string myKey = currentDictionary.FirstOrDefault(x => x.Value == keyBind).Key;
            currentDictionary[myKey] = KeyCode.None;
            MenuSettings.instance.UpdateKeyText(myKey, KeyCode.None);
        }

        currentDictionary[key] = keyBind;
        MenuSettings.instance.UpdateKeyText(key, keyBind);
        bindName = string.Empty;
	}

    public void KeyBindOnClick(string bindName)
    {
        this.bindName = bindName;
    }

    private void OnGUI()
    {
        if(bindName != string.Empty)
        {
            Event e = Event.current;

            if(e.isKey)
            {
                BindKey(bindName, e.keyCode);
                SaveKeys();
            }
        }
    }

    public void SaveKeys()
    {
        foreach(var key in Keybinds)
        {
            PlayerPrefs.SetString(key.Key, key.Value.ToString());
        }
        PlayerPrefs.Save();
    }
}
