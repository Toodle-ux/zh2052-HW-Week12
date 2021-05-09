using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    // indicate the effect of each click, set default to 0
    private int clickEffect = -1;

    // the boxes of button UI
    public Image crossSelected;
    public Image singleSelected;
    public Image lineSelected;
    public Image bombSelected;

    // buttons
    public Button crossButton;
    public Button singleButton;
    public Button lineButton;
    public Button bombButton;

    // the remained steps for each click effect
    public int crossRemain;
    public int singleRemain;
    public int lineRemain;
    public int bombRemain;
    
    // the display text of the remained clicks
    public Text crossText;
    public Text singleText;
    public Text lineText;
    public Text bombText;
    
    // the size of the board
    public int width = 7;
    public int height = 7;

    // to center the board in the middle of the screen
    private float offsetX = 3;
    private float offsetY = 3;

    // a 2D array that stores the stars
    private int[,] grid;

    public Text display;

    // the list of all the instantiated stars
    private List<GameObject> spawnedStars = new List<GameObject>();

    public GameObject darkPrefab, brightPrefab;

    // the level loader file
    public string levelFileName;

    //public int currentLevel = 1;

    // Start is called before the first frame update
    void Start()
    {
        // disable all the button box UI by default
        crossSelected.enabled = false;
        singleSelected.enabled = false;
        lineSelected.enabled = false;
        bombSelected.enabled = false;
        
        LoadLevel();

        display.text = "Light up all the stars.";
    }

    // Update is called once per frame
    void Update()
    {
        // If you press space, it reloads the scene.
        if (Input.GetKeyDown(KeyCode.Space))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
        
        // disable the button when the remain step is 0
        if (crossRemain == 0)
        {
            crossButton.interactable = false;
        }

        if (singleRemain == 0)
        {
            singleButton.interactable = false;
        }

        if (lineRemain == 0)
        {
            lineButton.interactable = false;
        }

        if (bombRemain == 0)
        {
            bombButton.interactable = false;
        }
        
        // update the remained steps display
        crossText.text = "Remained:\n" + crossRemain;
        singleText.text = "Remained:\n" + singleRemain;
        lineText.text = "Remained:\n" + lineRemain;
        bombText.text = "Remained:\n" + bombRemain;

        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            // get the position of the cursor when clicking on the mouse
            Vector3 worldPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition, Camera.MonoOrStereoscopicEye.Mono);

            // round the position of the cursor into integers
            int gridX = Convert.ToInt32(Math.Round(worldPoint.x)) + Convert.ToInt32(offsetX);
            int gridY = Convert.ToInt32(Math.Round(worldPoint.y)) + Convert.ToInt32(offsetY);

            //Debug.Log(clickEffect);
            
            // if the position is on the chess board and there is no chess piece in the grid
            if (gridX >= 0 && gridX < 7 && gridY >= 0 && gridY < 7 && grid[gridX, gridY] != 0 && !Win())
            {
                //Debug.Log(gridX + ", " + gridY);

                switch (clickEffect)
                {
                    case 0:
                        Cross(gridX, gridY);
                        break;
                    case 1:
                        Single(gridX, gridY);
                        break;
                    case 2:
                        Line(gridX, gridY);
                        break;
                    case 3:
                        Bomb(gridX,gridY);
                        break;
                    default:
                        break;
                }
                
                
            }
        }

        UpdateDisplay();
    }

    // check if there is a dark star
    public bool ContainsDark(int x, int y)
    {
        return grid[x, y] == -1;
    }

    public bool ContainsBright(int x, int y)
    {
        return grid[x, y] == 1;
    }

    private void UpdateDisplay()
    {
        //destroy all the existing stars
        foreach (var star in spawnedStars)
        {
            Destroy(star);
        }

        // clear the array of the stars
        spawnedStars.Clear();

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                //if there is a dark star, display a dark star
                if (ContainsDark(x, y))
                {
                    var darkStar = Instantiate(darkPrefab);
                    darkStar.transform.position = new Vector3(x - offsetX, y - offsetY);
                    //add it to the array
                    spawnedStars.Add(darkStar);
                }

                if (ContainsBright(x, y))
                {
                    var brightStar = Instantiate(brightPrefab);
                    brightStar.transform.position = new Vector3(x - offsetX, y - offsetY);
                    spawnedStars.Add(brightStar);
                }
            }
        }

        if (Win())
        {
            int sceneIndex = SceneManager.GetActiveScene().buildIndex;

            //Move to main menu
            SceneManager.LoadScene(0);

            // unlock the difficult levels by saving data in playerprefs
            switch (sceneIndex)
            {
                case 1:
                    PlayerPrefs.SetInt("level4", 1);
                    break;
                case 2:
                    PlayerPrefs.SetInt("level5", 1);
                    break;
                case 3:
                    PlayerPrefs.SetInt("level6", 1);
                    break;
                default:
                    break;
            }
        }
    }

    // check if all the stars have been lit up
    private bool Win()
    {
        bool light = true;

        // if any star on the board is dark, you don't win
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (grid[x, y] < 0)
                {
                    light = false;
                }
            }
        }

        return light;
    }

    void LoadLevel()
    {
        //initialize the board
        grid = new int[width, height];

        int sceneIndex = SceneManager.GetActiveScene().buildIndex;

        // since the difficult and easy levels use the same map, load the simple level map
        if (sceneIndex > 3)
        {
            sceneIndex = sceneIndex - 3;
        }
        
        // find file path of the level document
        string current_file_path = Application.dataPath +
                                   "/Levels/" +
                                   levelFileName.Replace("Num",
                                       sceneIndex + "");

        string[] fileLines = File.ReadAllLines(current_file_path);

        for (int y = 0; y < height; y++)
        {
            // read the file line by line, then character by character
            string lineText = fileLines[y];

            char[] characters = lineText.ToCharArray();

            // turn each character into the star
            for (int x = 0; x < width; x++)
            {
                char c = characters[x];

                switch (c)
                {
                    case 'b':
                        grid[x, y] = 1;
                        break;
                    case 'd':
                        grid[x, y] = -1;
                        break;
                    default:
                        grid[x, y] = 0;
                        break;
                }
            }
        }
    }

    // when the cross effect is selected, light up stars accordingly
    void Cross(int gridX, int gridY)
    {
        // execute the click only when there are remained steps
        if (crossRemain > 0)
        {
            grid[gridX, gridY] = -grid[gridX, gridY];

            // if there is any grid on the surrounding, change that grid to the opposite color
            if (gridX > 0)
            {
                grid[gridX - 1, gridY] = -grid[gridX - 1, gridY];
            }

            if (gridX < 6)
            {
                grid[gridX + 1, gridY] = -grid[gridX + 1, gridY];
            }

            if (gridY > 0)
            {
                grid[gridX, gridY - 1] = -grid[gridX, gridY - 1];
            }

            if (gridY < 6)
            {
                grid[gridX, gridY + 1] = -grid[gridX, gridY + 1];
            }

            // remained steps -1
            crossRemain--;
        }
    }

    // when single effect is selected
    void Single(int gridX, int gridY)
    {
        if (singleRemain > 0)
        {
            grid[gridX, gridY] = -grid[gridX, gridY];

            singleRemain--;
        }
    }

    // when line effect is selected
    void Line(int gridX, int gridY)
    {
        if (lineRemain > 0)
        {
            grid[gridX, gridY] = -grid[gridX, gridY];
        
            if (gridX > 0)
            {
                grid[gridX - 1, gridY] = -grid[gridX - 1, gridY];
            }

            if (gridX < 6)
            {
                grid[gridX + 1, gridY] = -grid[gridX + 1, gridY];
            }

            lineRemain--;
        }
    }
    
    // when bomb effect is selected
    void Bomb(int gridX, int gridY)
    {
        if (bombRemain > 0)
        {
            // that star disappears
            grid[gridX, gridY] = 0;

            // remained steps for bombs -1
            bombRemain--;
        }
    }

    public void ClickButton(int clickNum)
    {
        clickEffect = clickNum;
        
        // display the selected UI accordingly
        ClickDisplay(clickNum);
    }

    void ClickDisplay(int clickNum)
    {
        switch (clickNum)
        {
            case 0:
                crossSelected.enabled = true;
                singleSelected.enabled = false;
                lineSelected.enabled = false;
                bombSelected.enabled = false;
                break;
            case 1:
                crossSelected.enabled = false;
                singleSelected.enabled = true;
                lineSelected.enabled = false;
                bombSelected.enabled = false;
                break;
            case 2:
                crossSelected.enabled = false;
                singleSelected.enabled = false;
                lineSelected.enabled = true;
                bombSelected.enabled = false;
                break;
            case 3:
                crossSelected.enabled = false;
                singleSelected.enabled = false;
                lineSelected.enabled = false;
                bombSelected.enabled = true;
                break;
            default:
                break;
        }
    }
}