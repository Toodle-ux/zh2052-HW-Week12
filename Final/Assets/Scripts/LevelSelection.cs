using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class LevelSelection : MonoBehaviour
{
    // an array of buttons
    public Button[] levelButtons;
    
    // Start is called before the first frame update
    void Start()
    {
        int level4 = PlayerPrefs.GetInt("level4", 0);
        int level5 = PlayerPrefs.GetInt("level5", 0);
        int level6 = PlayerPrefs.GetInt("level6", 0);

        if (level4 == 0)
        {
            levelButtons[3].interactable = false;
        }
        
        if (level5 == 0)
        {
            levelButtons[4].interactable = false;
        }
        
        if (level6 == 0)
        {
            levelButtons[5].interactable = false;
        }
    }
}
