using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelGen : MonoBehaviour
{
    //Set platform bounds
    public int maxX;
    public int maxY;

    //Set players jump hight
    public int maxJump;
    public GameObject finish;
    public GameObject player;
    public GameObject[] enemy;
    private GameObject platform;
//    private GameObject inAirPlatform;
    private PlatformTemplates templates;
    //private PlatformTemplates 
    [HideInInspector] public int color;
    private int randY;
    private GameObject prev;   
    private int x = 0;      
    private int y = 0;
    private int enemies;
    private int numEnemy = 0;
    

    //public GameObject Brick;

    // Start is called before the first frame update
    void Start()
    {
        //Get parent object
        platform = GameObject.FindGameObjectWithTag("Platform");
        //Get platform templates
        templates = GameObject.FindGameObjectWithTag("Platform").GetComponent<PlatformTemplates>();
        //Chose platform color
        color = Random.Range(0, 4);

        enemies = Random.Range(4, 7);

        //Set spawn location and name Start
        GameObject spawn = Instantiate(templates.left[color], new Vector3(0.5f,0.5f,0), Quaternion.identity, platform.transform);
        spawn.name = "Start";
        Instantiate(templates.leftBot[color], new Vector3(0.5f, -.5f, 0), Quaternion.identity, platform.transform);
        //Set for last tile spawned
        prev = spawn;

        SpawnBoarder();
       


    }

    // Update is called once per frame
    void Update()
    {
        //Get next tile spawn location
        while (x < maxX && y < maxY)
        {
            GameObject prev2 = prev;
            if (prev.tag == "Right")       
            {
                int ytemp = y;       
                randY = Random.Range(-maxJump, maxJump);
                
                if (randY == 0) 
                {
                    randY = 2;
                }

                y+= randY; 
                
                if (y < 0) 
                {
                    y = ytemp + 1;
                }
            }

            x++;
            //spawn new tile and set as last spawn
            prev = spawnTile(prev, x, y);
            spawnRight(prev, prev2, x, y);

            //spawns floating platforms
            spawnFloatingPlatform(prev, x, y);
//            if (spawnFloatingPlatform  


        }

        //Makes sure the last tile is a right tile
        if (x >= maxX && x < maxX + 1 && prev.tag != "Right")
        {
            x++;
            Instantiate(templates.right[color], new Vector3(0.5f + x, 0.5f + y, 0), Quaternion.identity, platform.transform);
            Instantiate(finish, new Vector3(0.5f + x, 0.5f + y + 1, 0), Quaternion.identity, platform.transform);
            for (float i = -.5f; i <= y; i++)
            {
                Instantiate(templates.rightBot[color], new Vector3(0.5f + x, i, 0), Quaternion.identity, platform.transform);
            }
        }
        else
        {

            if (x >= maxX && x < maxX + 1 && prev.tag == "Right")
            {
                x++;
                Instantiate(templates.middle[color], new Vector3(0.5f + x - 1, 0.5f + y, 0), Quaternion.identity, platform.transform);
                for (float i = -.5f; i <= y; i++)
                {
                    Instantiate(templates.middleBot[color], new Vector3(0.5f + x - 1, i, 0), Quaternion.identity, platform.transform);
                }

                Instantiate(templates.right[color], new Vector3(0.5f + x, 0.5f + y, 0), Quaternion.identity, platform.transform);
                Instantiate(finish, new Vector3(0.5f + x, 0.5f + y + 1, 0), Quaternion.identity, platform.transform);
                for (float i = -.5f; i <= y; i++)
                {
                    Instantiate(templates.rightBot[color], new Vector3(0.5f + x, i, 0), Quaternion.identity, platform.transform);
                }
            }
        }

        //spawns random enemies
        while (numEnemy < enemies)
        {
            GameObject[] middles = GameObject.FindGameObjectsWithTag("Middle");
            
               
            for (int i = 4; i <middles.Length - 4; i++)               
            {
                if(!Physics.CheckSphere(middles[i].transform.position, .4f) && !Physics.CheckSphere(middles[i].transform.position + new Vector3(1,0,0), .4f) && !Physics.CheckSphere(middles[i].transform.position + new Vector3(2, 0, 0), .4f)
                    && !Physics.CheckSphere(middles[i].transform.position + new Vector3(-2, 0, 0), .4f) && !Physics.CheckSphere(middles[i].transform.position + new Vector3(-1, 0, 0), .4f)) 
                {
                    SpawnEnemy(middles[i]);                       
                    numEnemy++;                   
                    i += Random.Range(2,6);
                }
                                                     
            }
            
        }



    }

    //Creates Floating Platforms
    void spawnFloatingPlatform(GameObject prev, int x, int y)
    {
        //GameObject flyingPlatform = null;

        //chose floating platform color
        int flyColor = Random.Range(0, 3);

        //Give 1 in x chance of spawning
        int a = Random.Range(1, 10);
        if (a == 1)
        {
            //selects length of floating platform
            int b = Random.Range(2, 5);
            //selects height of floating platform
            int c = Random.Range(4, maxJump);

            if (b == 2)
            {
                Instantiate(templates.floatLeft[flyColor], new Vector3(x, y + c, 0), Quaternion.identity, platform.transform);
                Instantiate(templates.floatRight[flyColor], new Vector3(x + 1, y + c, 0), Quaternion.identity, platform.transform);
            }

            else
                if (b == 3)
                {
                    Instantiate(templates.floatLeft[flyColor], new Vector3(x, y + c, 0), Quaternion.identity, platform.transform);
                    Instantiate(templates.floatMid[flyColor], new Vector3(x + 1, y + c, 0), Quaternion.identity, platform.transform);
                    Instantiate(templates.floatRight[flyColor], new Vector3(x + 2, y + c, 0), Quaternion.identity, platform.transform);
                }

            else
                if (b == 4)
                {
                    Instantiate(templates.floatLeft[flyColor], new Vector3(x, y + c, 0), Quaternion.identity, platform.transform);
                    for (int i = 1; i <= 2; i++)
                    {
                        Instantiate(templates.floatMid[flyColor], new Vector3(x + i, y + c, 0), Quaternion.identity, platform.transform);
                    }
                    Instantiate(templates.floatRight[flyColor], new Vector3(x + 3, y + c, 0), Quaternion.identity, platform.transform);
                }

            else
            {
                Instantiate(templates.floatLeft[flyColor], new Vector3(x, y + c, 0), Quaternion.identity, platform.transform);
                for (int i = 1; i <= 3; i++)
                    {
                        Instantiate(templates.floatMid[flyColor], new Vector3(x + i, y + c, 0), Quaternion.identity, platform.transform);
                    }
                Instantiate(templates.floatRight[flyColor], new Vector3(x + 4, y + c, 0), Quaternion.identity, platform.transform);
            }
        }
    }       

    //Select the spawn point and type for next tile
    GameObject spawnTile(GameObject prev, int x, int y)
    {
        GameObject prevtag = prev;
        if (prev.tag == "Left") 
        {
            prevtag =  Instantiate(templates.middle[color], new Vector3(0.5f + x, 0.5f + y, 0), Quaternion.identity, platform.transform);
            for (float i = -.5f; i<= y; i++)
            {
                Instantiate(templates.middleBot[color], new Vector3(0.5f + x, i, 0), Quaternion.identity, platform.transform);
            }
        }
        else 
        {
            if (prev.tag == "Middle") 
            {
                int r = Random.Range(0, 4);
                if (r != 0) 
                {
                    prevtag = Instantiate(templates.middle[color], new Vector3(0.5f + x, 0.5f + y, 0), Quaternion.identity, platform.transform);
                    for (float i = -.5f; i <= y; i++)
                    {
                        Instantiate(templates.middleBot[color], new Vector3(0.5f + x, i, 0), Quaternion.identity, platform.transform);
                    }
                }
                else 
                {
                    prevtag = Instantiate(templates.right[color], new Vector3(0.5f + x, 0.5f + y, 0), Quaternion.identity, platform.transform);
                }
            }
            else 
            {
                if (prev.tag == "Right") 
                {                   
                    prevtag = Instantiate(templates.left[color], new Vector3(0.5f + x, 0.5f + y, 0), Quaternion.identity, platform.transform);
                    for (float i = -.5f; i <= y; i++)
                    {
                        if (i >= prev.transform.position.y)
                        {
                            Instantiate(templates.leftBot[color], new Vector3(0.5f + x, i, 0), Quaternion.identity, platform.transform);
                        }
                        else
                        {
                            Instantiate(templates.middleBot[color], new Vector3(0.5f + x, i, 0), Quaternion.identity, platform.transform);
                        }
                        
                    }
                }
            }
        }
        return prevtag;
    }

    void spawnRight(GameObject curr, GameObject prev, int x, int yNext)
    {
        if (prev.tag == "Right")
        {            
            for (float i = -.5f; i < prev.transform.position.y; i++)
            {
                if (i >= yNext)
                {
                    Instantiate(templates.rightBot[color], new Vector3(0.5f + x - 1, i, 0), Quaternion.identity, platform.transform);
                }
                else
                {
                    Instantiate(templates.middleBot[color], new Vector3(0.5f + x - 1, i, 0), Quaternion.identity, platform.transform);
                }
            }
        }
    }

    public void SpawnBoarder()
    {
        Instantiate(templates.sideBot[color], new Vector3(-0.5f, -1.5f, 0), Quaternion.identity, platform.transform);
        for (int i = 1; i <= maxY + 1; i++)
        {
            Instantiate(templates.sideMid[color], new Vector3(-0.5f, -1.5f + i, 0), Quaternion.identity, platform.transform);
        }
        Instantiate(templates.sideTop[color], new Vector3(-0.5f, -1.5f + maxY + 2, 0), Quaternion.identity, platform.transform);

        Instantiate(templates.sideBot[color], new Vector3(-0.5f + maxX + 3, -1.5f, 0), Quaternion.identity, platform.transform);
        for (int i = 1; i <= maxY + 1; i++)
        {
            Instantiate(templates.sideMid[color], new Vector3(-0.5f + maxX + 3, -1.5f + i, 0), Quaternion.identity, platform.transform);
        }
        Instantiate(templates.sideTop[color], new Vector3(-0.5f + maxX + 3, -1.5f + maxY + 2, 0), Quaternion.identity, platform.transform);

        Instantiate(templates.floatLeft[color], new Vector3(0.5f, -1.5f, 0), Quaternion.identity, platform.transform);
        for (int i = 1; i <= maxX; i++)
        {
            Instantiate(templates.floatMid[color], new Vector3(0.5f + i, -1.5f, 0), Quaternion.identity, platform.transform);
        }
        Instantiate(templates.floatRight[color], new Vector3(0.5f + maxX + 1, -1.5f, 0), Quaternion.identity, platform.transform);

        Instantiate(templates.floatLeft[color], new Vector3(0.5f, -1.5f + maxY + 2, 0), Quaternion.identity, platform.transform);
        for (int i = 1; i <= maxX; i++)
        {
            Instantiate(templates.floatMid[color], new Vector3(0.5f + i, -1.5f + maxY + 2, 0), Quaternion.identity, platform.transform);
        }
        Instantiate(templates.floatRight[color], new Vector3(0.5f + maxX + 1, -1.5f + maxY + 2, 0), Quaternion.identity, platform.transform);
    }

   void SpawnEnemy(GameObject plat)
    {
        int i = Random.Range(0, enemy.Length);
        Instantiate(enemy[i], plat.transform.position + new Vector3(0, 1, 0), Quaternion.identity, platform.transform);        
    }

   
}
