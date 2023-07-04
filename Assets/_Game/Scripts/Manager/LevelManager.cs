using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UIElements;

public class LevelManager : Singleton<LevelManager>         /// Fix lại cái mà đứng trong vòng target của nhau nhưng không ném vũ khí
{
    [SerializeField] private GameObject enemys;
    [SerializeField] private ColorScriptableObject colorScriptableObject;

    [SerializeField] private PlayerController player;

    [SerializeField] private int numberEnemyInMap;
    [SerializeField] private int allEnemy;
    [SerializeField] private float timer;
    [SerializeField] private int numberAliveEnemy;
    private GameObject[] enemysGameObject;
    private int countEnemys;
    private int centroidSpawnPosition;
    private int[,] matrixMap;
    private float rectMap;

    private void Start()
    {
        timer = 0;
        numberAliveEnemy = 0;
        matrixMap = new int[5, 5];
        centroidSpawnPosition = 5;
        rectMap = 200;

        countEnemys = enemys.transform.childCount;
        enemysGameObject = new GameObject[countEnemys];

        for(int i=0;i<countEnemys;i++)
        {
            enemysGameObject[i] = enemys.transform.GetChild(i).gameObject;
        }
        //Debug.Log(countEnemys);
    }

    private void Update()
    {
        timer -= Time.deltaTime;
        if (timer < 0f)
        {
            timer = 4;

            UpdateMatrixMap();
            SpawnFullEnemyInMap();
        }
    }

    //private void InitMatrixMap()
    //{
    //    building = new Transform[mapBuildings.transform.childCount];

    //    for (int i = 0; i < mapBuildings.transform.childCount; i++)
    //    {
    //        building[i] = mapBuildings.transform.GetChild(i);
    //    }

    //    matrixMap = new int[5, 5];
    //    /// maxtrixMap[i, j] = 0 <=> Không có enemy nào ở trong khu vực này
    //    /// maxtrixMap[i, j] = k <=> Có k enemy ở trong khu vực này

    //    for (int i = 0; i < 5; i++)
    //    {
    //        for (int j = 0; j < 5; j++)
    //        {
    //            matrixMap[i, j] = 0;
    //        }
    //    }
    //}

    internal void UpdateMatrixMap()
    {
        int x = new int();
        int z = new int();

        for (int i = 0; i < 5; i++) for (int j = 0; j < 5; j++)
            matrixMap[i, j] = 0;

        GetMatrixMapFromPosition(player.transform.position, ref x, ref z);
        matrixMap[x, z]++;

        for (int i = 0; i < countEnemys; i++)
        {
            if (enemysGameObject[i].active)
            {
                GetMatrixMapFromPosition(enemysGameObject[i].transform.position, ref x, ref z); 
                matrixMap[x, z]++;
                //Debug.Log("haha " + x + " hihi " + z);
            }    
            //Debug.Log(enemysTransform[i].position);
        }
        //Debug.Log("=========================");
    }

    internal void GetAreaFromMatrixMap(ref float x1, ref float x2, ref float z1, ref float z2, int x, int z)
    {
        x1 = x * (rectMap/5) - (rectMap/2);
        x2 = (x + 1) * (rectMap / 5) - (rectMap / 2);
        z1 = z * (rectMap / 5) - (rectMap / 2);
        z2 = (z + 1) * (rectMap / 5) - (rectMap / 2);
    }

    internal void GetMatrixMapFromPosition(Vector3 pos, ref int x, ref int z)
    {
        x = (int)((pos.x + (rectMap / 2)) / (rectMap / 5));
        z = (int)((pos.z + (rectMap / 2)) / (rectMap / 5));
    }

    internal Vector3 GetPlayerPosition()
    {
        return player.transform.position;
    }

    private void OnApplicationQuit()
    {

    }

    #region Enemy
    List<EnemyController> listEnemy = new List<EnemyController>();

    public void SpawnFullEnemyInMap()
    {
        if (allEnemy >= numberEnemyInMap)
        {
            int tmp = numberEnemyInMap - numberAliveEnemy;
            for (int i = 0; i < tmp; i++)
            {
                SpawnEnemy();
            }
        }
    }

    public void SpawnEnemy()
    {
        Vector3 pos = new Vector3();
        float x1 = new float();
        float x2 = new float();
        float z1 = new float();
        float z2 = new float();

        for(int i=0;i<5;i++)
        {
            for(int j=0;j<5;j++)
            {
                if (matrixMap[i,j] == 0)
                {
                    matrixMap[i,j]++;
                    GetAreaFromMatrixMap(ref x1, ref x2, ref z1, ref z2, i, j);
                    pos = RandomPosition(x1 + centroidSpawnPosition, x2 - centroidSpawnPosition, z1 + centroidSpawnPosition, z2 - centroidSpawnPosition);

                    EnemyController enemy = SimplePool.Spawn<EnemyController>(PoolType.Enemy, pos, Quaternion.identity);
                    enemy.OnInit();
                    enemy.AddLevel(UnityEngine.Random.Range(player.characterLevel > 2? player.characterLevel - 2 : player.characterLevel, player.characterLevel + 3));

                    /////Debug
                    //int x = new int();
                    //int z = new int();
                    //Debug.Log(pos);
                    //GetMatrixMapFromPosition(pos, ref x, ref z);
                    //Debug.Log("x la " + x + " z la " + z);
                    //Debug.Log("haha " + i + " hihi " + j);
                    /////End
                    return;
                }
            }    
        }    
    }

    private Vector3 RandomPosition(float x1, float x2, float z1, float z2)
    {
        Vector3 pos = new Vector3(UnityEngine.Random.Range(x1, x2), 0, UnityEngine.Random.Range(z1, z2));

        return pos;
    }

    public void EnemyInit(EnemyController enemy)
    {
        numberAliveEnemy++;
        listEnemy.Add(enemy);
    }

    public void EnemyDespawn(EnemyController enemy)
    {
        numberAliveEnemy--;
        allEnemy--;
        listEnemy.Remove(enemy);
    }

    #endregion
}
