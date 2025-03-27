using UnityEngine;
using Unity.Mathematics;
using Random = UnityEngine.Random;
using System.Collections.Generic;

public class Game : MonoBehaviour
{
    #region Properties

    [SerializeField]
    private int2 size = GameDefaults.GridSize;

    Grid<TileType> grid;

    private List<Match> matches;

    private List<int2> clearedTileCoordinates;
    private bool tilesNeedFilling;

    private List<TileDrop> droppedTiles;

    private Recipe recipe;

    #endregion

    #region Methods

    public void StartNewGame()
    {
        recipe = new Recipe("Steamed Carrots", "carrot", 5);
        if (grid.IsUndefined())
        {
            grid = new(size);
            matches = new List<Match>();
            clearedTileCoordinates = new List<int2>();
            droppedTiles = new List<TileDrop>();
        }
        FillGrid();
    }

    private void FillGrid()
    {
        for (int y = 0; y < size.y; y++)
            for (int x = 0; x < size.x; x++)
            {
                int2 coordinate = new int2(x, y);

                //Check tiles to the left and downward
                //to see if there is a potential match at start
                TileType left = TileType.none;
                TileType down = TileType.none;
                int potentialMatchCount = 0;
                
                //check tiles left
                if (x > 1)
                {
                    int2 checkCoordinateLeft = new int2(x - 1, y); //check one to left
                    int2 checkCoordinateTwoLeft = new int2(x - 2, y); //check two to left

                    left = grid[checkCoordinateLeft];

                    if (left == grid[checkCoordinateTwoLeft])
                    {
                        potentialMatchCount = 1; //there is a match
                    }
                }
                
                //Check tiles downward
                if (y > 1)
                {
                    int2 checkCoordinateDownOne = new int2(x, y - 1); //check one down
                    int2 checkCoordinateDownTwo = new int2(x, y - 2); //checkk two down

                    down = grid[checkCoordinateDownOne];

                    if (down == grid[checkCoordinateDownTwo])
                    {
                        potentialMatchCount += 1;

                        if (potentialMatchCount == 1)
                        {
                            left = down;
                        }
                        else if (down < left) //order the matches lowest to highest
                        {
                            TileType temp = left;
                            left = down;
                            down = temp;
                        }
                    }
                }

                //Avoid match by decreasing random by the match count,
                //and skip left and down, if needed.
                TileType tile = (TileType)Random.Range(1,
                        GameDefaults.NumberOfDifferentTiles - potentialMatchCount);
                if (potentialMatchCount > 0 && tile >= left)
                {
                    tile += 1;
                }
                
                if (potentialMatchCount == 2 && tile >= down)
                {
                    tile += 1;
                }

                //Assign tile to this coordinate after alterations.
                grid[coordinate] = tile;
            }
    }

    public bool TryMove(Move move)
    {
        grid.Swap(move.GetFrom(), move.GetTo());
        //see if the swap resulted in a match being made
        if (FindMatches())
        {
            return true; //there was a match, make swap
        }
        //if there was no match, undo the swap
        grid.Swap(move.GetFrom(), move.GetTo());
        return false; //invalid swap.
    }

    public bool FindMatches()
    {
        //Search for horizontal matches by row
        for (int y = 0; y < size.y; y++)
        {
            int2 coordinateStart = new int2(0, y);
            TileType start = grid[coordinateStart];
            int length = 1;
            for (int x = 1; x < size.x; x++)
            {
                int2 coordinateTile = new int2(x, y);
                TileType tile = grid[coordinateTile];
                if (tile == start)
                {
                    //match found
                    length++;
                }
                else
                {
                    //end of matching tiles, check if 3 in a row or more.
                    if (length >= 3)
                    {
                        //there is 3 or more, add to list of matches.
                        int2 matchCoordinate = new int2(x - length, y);
                        matches.Add(new Match(matchCoordinate, length, true));
                    }
                    //reset search params
                    start = tile;
                    length = 1;
                }
            }
            //check for match 3 at end of the row.
            if (length >= 3)
            {
                int2 matchCoordinateAtEnd = new int2(size.x - length, y);
                matches.Add(new Match(matchCoordinateAtEnd, length, true));
            }
        }

        //Search for vertial matches by column
        for (int x = 0; x < size.x; x++)
        {
            int2 startCoordinate = new int2(x, 0);
            TileType start = grid[startCoordinate];
            int length = 1;
            for (int y = 1; y < size.y; y++)
            {
                int2 tileCoordinate = new int2(x, y);
                TileType tile = grid[tileCoordinate];
                if (tile == start)
                {
                    //match found
                    length++;
                }
                else
                {
                    //end of matching tiles vertically,
                    //check if 3 in a row or more.
                    if (length >= 3)
                    {
                        //3 or more, add to list of matches.
                        int2 matchCoordinate = new int2(x, y - length);
                        matches.Add(new Match(matchCoordinate, length, false));
                    }
                    //reset search params
                    start = tile;
                    length = 1;
                }
            }
            //check for match 3 at end of the column.
            if (length >= 3)
            {
                int2 matchCoordinateAtEnd = new int2(x, size.y - length);
                matches.Add(new Match(matchCoordinateAtEnd, length, false));
            }
        }

        return matches.Count > 0;
    }

    public void ProcessMatches()
    {
        clearedTileCoordinates.Clear(); //clear match coordinates from last match.
        for (int m = 0; m < matches.Count; m++)
        {
            Match match = matches[m];
            int2 step;
            //see if match is vertical or horizontal
            if (match.IsHorizontal())
            {
                step = new int2(1, 0);
            }
            else
            {
                step = new int2(0, 1);
            }

            int2 matchCoordinate = match.GetCoordinate();
            //remove the matches and add to coordinates that need to 
            //be refilled.

            //Increment coordinates by the step.
            //took me 40 mins to find I forgot this....
            for (int i = 0; i < match.Length(); matchCoordinate += step, i++)
            {
                if (grid[matchCoordinate] != TileType.none)
                {
                    grid[matchCoordinate] = TileType.none;
                    clearedTileCoordinates.Add(matchCoordinate);
                }
            }
        }
        //processed all matches
        matches.Clear();
        tilesNeedFilling = true;
    }

    public void DropTiles()
    {
        droppedTiles.Clear(); //clear dropped tiles from last drop.
        
        //loop through all columns.
        //go to bottom to top
        for (int x = 0; x < size.x; x++)
        {
            int holeCount = 0; //keep track of hole count.
            for (int y = 0; y < size.y; y++)
            {
                int2 checkCoordinate = new int2(x, y);
                if (grid[checkCoordinate] == TileType.none)
                {
                    holeCount++;
                }
                else if (holeCount > 0)
                {
                    int2 coordinateBelowHole = new int2(x, y - holeCount);
                    //if there is a hole below the tile, drop this tile into it
                    //this pushes all holes to the top of the colummn.
                    grid[coordinateBelowHole] = grid[checkCoordinate];
                    droppedTiles.Add(new TileDrop(coordinateBelowHole, holeCount));
                }
            }

            //fill holes at top of column with a new random tile
            //drop these tiles down if needed.
            for (int hole = 1; hole <= holeCount; hole++)
            { 
                int2 tileCoordinate = new int2(x, size.y - hole);
                grid[tileCoordinate] = 
                    (TileType)Random.Range(1, GameDefaults.NumberOfDifferentTiles);
                droppedTiles.Add(new TileDrop(tileCoordinate, holeCount));
            }
        }
        //indicate filling is no longer needed.
        tilesNeedFilling = false;
        //check for matches again with new tiles.
        FindMatches();
    }

    #endregion

    #region Getters

    public TileType GetTileAtCoordinate(int2 coodinate)
    {
        return grid[coodinate];
    }

    public int2 GetSize()
    {
        return size;
    }

    public List<int2> GetClearedTileCoordinates()
    {
        return clearedTileCoordinates;
    }

    public bool TilesNeedFilling()
    {
        return tilesNeedFilling;
    }

    public bool HasMatches()
    {
        return matches.Count > 0;
    }

    public List<TileDrop> GetDroppedTiles()
    {
        return droppedTiles;
    }

    public Recipe GetRecipe()
    {
        return recipe;
    }

    #endregion
}