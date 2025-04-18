using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

/// <summary>
/// Controls the flow of the game's visuals and communicates between the
/// player input and the underlying Game object.
/// </summary>
public class GameController : MonoBehaviour
{
    #region Properties

    [Header("Game Object")]
    [SerializeField]
    private Game game;

    [Header("Tile Prefabs")]
    [SerializeField]
    private Tile[] tilePrefabs;

    [Header("Animation Values")]
    [SerializeField, Range(0.1f, 1f)]
    private float dragThreshold = 0.5f;
    [SerializeField]
    private TileSwapper tileSwapper;
    [SerializeField]
    private float dropSpeed = 8f;
    [SerializeField, Range(0f, 10f)]
    private float newDropOffset = 2f; //how high off the grid a tile will spawn
                                      //to enhance drop animation.

    [Header("UI")]
    [SerializeField]
    private TMP_Text[] compTexts;
    [SerializeField]
    private Image[] compImages;
    [SerializeField]
    private GameObject winPanel;
    [SerializeField]
    private GameObject losePanel;
    [SerializeField]
    private TMP_Text turnTimerText;
    [SerializeField]
    private TMP_Text difficultyText;

    [Header("Audio")]
    [SerializeField]
    private AudioClip victorySound;
    [SerializeField]
    private AudioClip loseSound;
    [SerializeField]
    private AudioClip swapSound;
    private AudioSource audioSource;

    private Grid<Tile> tiles;
    private float2 tileOffset;
    private bool isPlaying;
    private float busyDuration; 
    private int numberOfTurns;

    #endregion

    #region Unity Methods

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    private void Start()
    {
        isPlaying = true;
        winPanel.SetActive(false);
        losePanel.SetActive(false);
    }

    #endregion

    #region Methods

    public void StartNewGame()
    {
        isPlaying = true;

        winPanel.SetActive(false);
        winPanel.transform.localScale = Vector3.zero;

        losePanel.SetActive(false);
        losePanel.transform.localScale = Vector3.zero;

        busyDuration = 0f;
        game.StartNewGame();

        numberOfTurns = game.GetRecipe().GetNumberOfTurns();
        turnTimerText.text = numberOfTurns.ToString();

        SetRecipeUI();

        tileOffset = -0.5f * (float2)(game.GetSize() - 1); //set offset of the tile from eachother.

        if (tiles.IsUndefined()) //If game is being first initalized.
        {
            tiles = new(game.GetSize());
        }
        else //Link visual objects to corresponding under the hood objects.
        {
            for (int y = 0; y < tiles.GetSizeY(); y++)
                for (int x = 0; x < tiles.GetSizeX(); x++)
                {
                    int2 coordinate = new int2(x, y);
                    if (tiles[coordinate] != null) //fix null ref on game restart
                    {
                        tiles[coordinate].Despawn();
                    }
                    tiles[coordinate] = null;
                }
        }

        //Spawn the visual representation of the tiles.
        for (int y = 0; y < tiles.GetSizeY(); y++)
            for (int x = 0; x < tiles.GetSizeX(); x++)
            {
                int2 coordinate = new int2(x, y);
                tiles[coordinate] = SpawnTile(
                    game.GetTileAtCoordinate(coordinate), x, y);
            }
    }

    private Tile SpawnTile(TileType type, float x, float y)
    {
        return 
            tilePrefabs[(int)type - 1].Spawn(
                new Vector3(x + tileOffset.x, y + tileOffset.y));
    }

    public void EvaluateDrag(Vector3 start, Vector3 end)
    {
        //Get the tile coordinates that were swiped on
        float2 tileCoordStart = ScreenToTileSpace(start);
        float2 tileCoordEnd = ScreenToTileSpace(end);

        //Find the direction the player swiped.
        MoveDirection dragDirection = MoveDirection.none;
        float2 difference = tileCoordEnd - tileCoordStart;
        
        if (difference.x > dragThreshold)
        {
            dragDirection = MoveDirection.right;
        }
        else if (difference.x < -dragThreshold)
        {
            dragDirection = MoveDirection.left;
        }
        else if (difference.y > dragThreshold)
        {
            dragDirection = MoveDirection.up;
        }
        else if (difference.y < -dragThreshold)
        {
            dragDirection = MoveDirection.down;
        }
        else
        {
            dragDirection = MoveDirection.none;
        }

        //Create move out of this information.
        Move move = new Move((int2)math.floor(tileCoordStart),
            dragDirection);

        if (move.IsValid() &&
            tiles.AreValidCoordinates(move.GetFrom()) &&
            tiles.AreValidCoordinates(move.GetTo()))
        {
            MakeMove(move);
        }
        else
        {
            Debug.Log("This is not a valid move:\n" + move.ToString());
        }
    }

    private void MakeMove(Move move)
    {
        audioSource.PlayOneShot(swapSound);

        bool success = game.TryMove(move);
        Tile a = tiles[move.GetFrom()];
        Tile b = tiles[move.GetTo()];
        busyDuration = tileSwapper.Swap(a, b, !success);

        if (success)
        {
            tiles[move.GetFrom()] = b;
            tiles[move.GetTo()] = a;

            numberOfTurns--;
            turnTimerText.text = numberOfTurns.ToString();
        }

        //if (game.TryMove(move))
        //{
        //    //swap these two tiles
        //    (tiles[move.GetFrom()].transform.localPosition,
        //      tiles[move.GetTo()].transform.localPosition) 
              
        //      =

        //      (tiles[move.GetTo()].transform.localPosition,
        //      tiles[move.GetFrom()].transform.localPosition);

        //    tiles.Swap(move.GetFrom(), move.GetTo());
        //}
    }

    public void Process()
    {
        //give time to allow the animation to play before processing matches.
        if (busyDuration > 0f)
        {
            tileSwapper.Update();
            busyDuration -= Time.deltaTime;
            if (busyDuration > 0f)
            {
                return;
            }
        }

        if (game.HasMatches())
        {
            ProcessMatches();
        }
        else if (game.TilesNeedFilling())
        {
            DropTiles();
        }
    }

    private void ProcessMatches()
    {
        //Call game to process matches
        game.ProcessMatches();

        //go through all the matches the game processed and
        //despawn the visual tiles and set the position to null for now.
        for (int i = 0; i < game.GetClearedTileCoordinates().Count; i++)
        {
            int2 coordinate = game.GetClearedTileCoordinates()[i];
            busyDuration = Mathf.Max(tiles[coordinate].Disappear(), busyDuration);
            tiles[coordinate] = null;
        }

        if (game.IsGameOver())
        {
            isPlaying = false;
            SpawnWinPanel();
            audioSource.PlayOneShot(victorySound);
            game.MakeGameHarder();
        }
        else if (numberOfTurns <= 0)
        {
            isPlaying = false;
            SpawnLosePanel();
            audioSource.PlayOneShot(loseSound);
        }

        SetRecipeUI();
    }

    private void DropTiles()
    {
        game.DropTiles(); //have game drop tiles.

        //check all drops
        for (int i = 0; i < game.GetDroppedTiles().Count; i++)
        {
            TileDrop drop = game.GetDroppedTiles()[i];
            Tile tile;

            //if tile fell from within grid, adjust postiion
            if (drop.GetFromY() < tiles.GetSizeY())
            {
                int2 dropCoordinates = 
                    new int2(drop.GetCoordinates().x, drop.GetFromY());
                tile = tiles[dropCoordinates];
                //tile.transform.localPosition = new Vector3(
                //    drop.GetCoordinates().x + tileOffset.x,
                //    drop.GetCoordinates().y + tileOffset.y);
            }
            else
            {
                //spawn new tile at position if there is nothing above it.
                tile = 
                    SpawnTile(game.GetTileAtCoordinate(drop.GetCoordinates()), 
                        drop.GetCoordinates().x, drop.GetFromY() + newDropOffset);
            }

            //update tile grid.
            tiles[drop.GetCoordinates()] = tile;
            busyDuration = Mathf.Max(
                tile.Fall(drop.GetCoordinates().y + tileOffset.y, dropSpeed),
                    busyDuration);
        }
    }

    private float2 ScreenToTileSpace(Vector3 screenPosition)
    {
        //TODO: Look this up
        //needed tutorial on this.
        Ray ray = Camera.main.ScreenPointToRay(screenPosition);
        Vector3 position = ray.origin - ray.direction *
            (ray.origin.z / ray.direction.z);

        return new float2(position.x - tileOffset.x + 0.5f,
            position.y - tileOffset.y + 0.5f);
    }

    #endregion

    #region UI Methods

    private void SetRecipeUI()
    {
        ResetRecipeUI();

        Difficulty gameDifficulty = game.GetRecipe().GetDifficulty();

        if (gameDifficulty == Difficulty.easy)
        {
            difficultyText.text = GameDefaults.easyText;
            difficultyText.color = Color.green;
        }
        else if (gameDifficulty == Difficulty.med)
        {
            difficultyText.text = GameDefaults.mediumText;
            difficultyText.color = Color.yellow;
        }
        else if (gameDifficulty == Difficulty.hard)
        {
            difficultyText.text = GameDefaults.hardText;
            difficultyText.color = Color.red;
        }

        Dictionary<Ingredient, int> componentInRecipe = game.GetRecipe().GetComponents();
        int index = 0;
        foreach (KeyValuePair<Ingredient, int> ingredient in componentInRecipe)
        {
            compImages[index].sprite = ingredient.Key.GetSprite();
            compTexts[index].text = ingredient.Value.ToString();

            if (ingredient.Value <= 0)
            {
                compTexts[index].text = "Done!";
                compTexts[index].color = Color.green;
            }

            compImages[index].enabled = true;
            compTexts[index].enabled = true;

            index++;
        }
    }

    private void ResetRecipeUI()
    {
        for (int i = 0; i < compImages.Length; i++)
        {
            compImages[i].enabled = false;
            compTexts[i].enabled = false;
            compTexts[i].color = Color.white;
        }
    }

    private void SpawnWinPanel()
    {
        winPanel.SetActive(true);
    }

    private void SpawnLosePanel()
    {
        losePanel.SetActive(true);
    }

    public void OnRetryButtonPress()
    {
        StartNewGame();
    }

    public void OnQuitButtonPress()
    {
        SceneManager.LoadScene(0);
    }

    #endregion

    #region Getters

    public bool IsPlaying()
    {
        return isPlaying;
    }

    public bool IsBusy()
    {
        return busyDuration > 0f;
    }

    #endregion
}
