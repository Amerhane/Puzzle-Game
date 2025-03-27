using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class GameController : MonoBehaviour
{
    #region Properties

    [SerializeField]
    private Game game;

    [SerializeField]
    private Tile[] tilePrefabs;

    [SerializeField, Range(0.1f, 1f)]
    private float dragThreshold = 0.5f;

    [SerializeField]
    private TileSwapper tileSwapper;

    [SerializeField]
    private float dropSpeed = 8f;

    [SerializeField, Range(0f, 10f)]
    private float newDropOffset = 2f; //how high off the grid a tile will spawn
                                      //to enhance drop animation.

    private Grid<Tile> tiles;
    float2 tileOffset;

    private bool isPlaying;
    //private bool isBusy;

    private float busyDuration;

    [SerializeField]
    private TMP_Text[] compTexts;
    [SerializeField]
    private Image[] compImages;
    [SerializeField]
    private Sprite[] compSprites;

    #endregion

    #region Unity Methods

    private void Start()
    {
        isPlaying = true;
        //isBusy = false;
    }

    #endregion

    #region Methods

    public void StartNewGame()
    {
        busyDuration = 0f;
        game.StartNewGame();
        SetRecipeUI();
        tileOffset = -0.5f * (float2)(game.GetSize() - 1);

        if (tiles.IsUndefined())
        {
            tiles = new(game.GetSize());
        }
        else
        {
            for (int y = 0; y < tiles.GetSizeY(); y++)
                for (int x = 0; x < tiles.GetSizeX(); x++)
                {
                    int2 coordinate = new int2(x, y);
                    tiles[coordinate].Despawn();
                    tiles[coordinate] = null;
                }
        }

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

    public bool EvaluateDrag (Vector3 start, Vector3 end)
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
            return false;
        }

        return true;
    }

    private void MakeMove(Move move)
    {
        bool success = game.TryMove(move);
        Tile a = tiles[move.GetFrom()];
        Tile b = tiles[move.GetTo()];
        busyDuration = tileSwapper.Swap(a, b, !success);

        if (success)
        {
            tiles[move.GetFrom()] = b;
            tiles[move.GetTo()] = a;
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

    public void DoWork()
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
        //despawn the tile and set the position to null for now.
        for (int i = 0; i < game.GetClearedTileCoordinates().Count; i++)
        {
            int2 coordinate = game.GetClearedTileCoordinates()[i];
            busyDuration = Mathf.Max(tiles[coordinate].Disappear(), busyDuration);
            tiles[coordinate] = null;
        }
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
        Ray ray = Camera.main.ScreenPointToRay(screenPosition);
        Vector3 position = ray.origin - ray.direction *
            (ray.origin.z / ray.direction.z);

        return new float2(position.x - tileOffset.x + 0.5f,
            position.y - tileOffset.y + 0.5f);
    }

    private void SetRecipeUI()
    {
        Dictionary<TileType, int> componentInRecipe = game.GetRecipe().GetComponents();
        int componentIndex = 0;
        foreach (TileType tileComp in componentInRecipe.Keys)
        {
            //compImages[componentIndex] = compSprites[]
        }
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
