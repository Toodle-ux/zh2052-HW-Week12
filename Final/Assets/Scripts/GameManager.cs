using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
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
    
    public int currentLevel = 0;
    
    // Start is called before the first frame update
    void Start()
    {
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
        
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {

            // get the position of the cursor when click on the mouse
            Vector3 worldPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition, Camera.MonoOrStereoscopicEye.Mono);

            // round the position of the cursor into integers
            int gridX = Convert.ToInt32(Math.Round(worldPoint.x)) + Convert.ToInt32(offsetX);
            int gridY = Convert.ToInt32(Math.Round(worldPoint.y)) + Convert.ToInt32(offsetY);

            // if the position is on the chess board and there is no chess piece in the grid
            if (gridX >= 0 && gridX < 7 && gridY >= 0 && gridY < 7 && grid[gridX, gridY] != 0 && !Win())
            {
                Debug.Log(gridX + ", " + gridY);

                grid[gridX, gridY] = -grid[gridX, gridY];
                
                // if there is any grid on the left, change that grid
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
            }
        }
        
        UpdateDisplay();
    }

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
        foreach (var star in spawnedStars)
        {
            Destroy(star);
        }

        spawnedStars.Clear();

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (ContainsDark(x, y))
                {
                    var darkStar = Instantiate(darkPrefab);
                    darkStar.transform.position = new Vector3(x - offsetX, y - offsetY);
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
            display.text = "The sky becomes bright again.";
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

        

        string current_file_path = Application.dataPath +
                                   "/Levels/" +
                                   levelFileName.Replace("Num",
                                       currentLevel + "");

        string[] fileLines = File.ReadAllLines(current_file_path);

        for (int y = 0; y < height; y++)
        {
            string lineText = fileLines[y];

            char[] characters = lineText.ToCharArray();
            
            for (int x = 0; x < width; x++)
            {
                char c = characters[x];

                switch (c)
                {
                    case'b':
                        grid[x, y] = 1;
                        break;
                    case'd':
                        grid[x, y] = -1;
                        break;
                    default:
                        grid[x, y] = 0;
                        break;
                }
                
                
            }
        }

    }
}
