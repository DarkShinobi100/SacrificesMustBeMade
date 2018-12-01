using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Random = UnityEngine.Random;

public class BoardManager : MonoBehaviour {

    [Serializable]
    public class Count
    {

        public int Minimum;
        public int Maximum;

        public Count(int Min, int Max)
        {
        Minimum= Min;
        Maximum = Max;
        }

    }

    public int Coloumns = 8;
    public int Rows = 8;
    public Count WallCount = new Count(5, 9);
    public Count FoodCount = new Count(1, 5);
    public GameObject Exit;
    public GameObject[] FloorTiles;
    public GameObject[] WallTiles;
    public GameObject[] PowerUpTiles;
    public GameObject[] enemyTiles;
    public GameObject[] OuterWallTiles;


    private Transform BoardHolder;
    private List<Vector3> GridPositions = new List<Vector3>();

    void InitialiseList()
    {
        GridPositions.Clear();

        for(int x=1; x<Coloumns -1; x++)
        {

            for(int y=1; y<Rows-1;y++)
            {
                GridPositions.Add(new Vector3(x, y, 0f));
            }

        }
    }


    void BoardSetUp()
    {
        BoardHolder = new GameObject("Board").transform;

        for(int x = -1; x < Coloumns + 1;x++)
        {
            for(int y = -1; y < Rows + 1;y++)
            {
                GameObject toInstantiate = FloorTiles[Random.Range(0, FloorTiles.Length)];
                if(x == -1 || x== Coloumns || y== -1 || y== Rows )
                {
                    toInstantiate = OuterWallTiles[Random.Range(0, OuterWallTiles.Length)];
                }

                GameObject instantiate = Instantiate(toInstantiate, new Vector3(x, y, 0f), Quaternion.identity) as GameObject;
                instantiate.transform.SetParent(BoardHolder);
            }
        }
    }


    Vector3 RandomPosition()
    {
        int RandomIndex = Random.Range(0, GridPositions.Count);
        Vector3 RandomPosition = GridPositions[RandomIndex];

        GridPositions.RemoveAt(RandomIndex);
        return RandomPosition;
    }


    void LayoutObjectAtRandom(GameObject[] TileArray,int Minimum, int Maximum)
    {
        int ObjectCount = Random.Range(Minimum, Maximum + 1);

        for (int i=0; i < ObjectCount; i++ )
        {
            Vector3 randomPosition = RandomPosition();
            GameObject TileChoice = TileArray[Random.Range(0, TileArray.Length)];
            Instantiate(TileChoice, randomPosition,Quaternion.identity);

        }

    }


    public void SetUpScene(int level)
    {
        BoardSetUp();
        InitialiseList();
        LayoutObjectAtRandom(WallTiles, WallCount.Minimum, WallCount.Maximum);
        LayoutObjectAtRandom(PowerUpTiles, FoodCount.Minimum, FoodCount.Maximum);
        //set amount of enemies to spawn
        int EnemyCount = (int)Mathf.Log(level*5, 2f);

        LayoutObjectAtRandom(enemyTiles, EnemyCount, EnemyCount);

        Instantiate(Exit, new Vector3(Coloumns - 1, Rows - 1, 0f), Quaternion.identity);
    }
}
