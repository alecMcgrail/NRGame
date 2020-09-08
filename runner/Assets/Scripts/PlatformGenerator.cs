using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Ninja Runner Platform Generator script

public class PlatformGenerator : MonoBehaviour {
    public GameObject       platform;
    public GameObject       platformBase;
    public GameObject       perfectZone;

    public GameObject       basicBuilding_S;

    public GameObject       obstacle;
    public GameObject       slideUnder;
    public GameObject       roofHouse;
    public GameObject       goal;
    public GameObject       store;

    public Transform        generationPoint;
    public float            initialDistanceBetween;
    private static float    singleWidth = 6;
    private static float    defaultWidth = 2*singleWidth;
    public int              stageLength;

    private float           gameSpeedMultiplier = 1;
    private float           chanceOfObstacle = 0f;
    private int             platformsThisLevel;
 
    public enum PlatformType
    {
        SS_BASIC_S,
        SS_BASIC_M,
        SS_BASIC_L,
        SS_BASIC_XL,
        TUNNEL
    }
    public enum ObstacleType
    {
        JUMP_OVER,
        SLIDE_UNDER,
        ROOF_HOUSE
    }

    public class Obstacle
    {
        public GameObject       oBody;
        public ObstacleType     oType;
        public Platform         parentPlat;

        public Obstacle(Platform parent)
        {
            parentPlat = parent;
            oType = ObstacleType.JUMP_OVER;
            oBody = Instantiate(instance.obstacle, parent.pos + new Vector2(0,1), Quaternion.identity);
        }
        public Obstacle(Platform parent, int position)
        {
            parentPlat = parent;
            oType = ObstacleType.JUMP_OVER;
            oBody = Instantiate(instance.obstacle, parent.pos + new Vector2((position*singleWidth) - (parent.width/2), 1), Quaternion.identity);


        }
        public Obstacle(Platform parent, int position, ObstacleType ot)
        {
            parentPlat = parent;
            oType = ot;

            switch (oType)
            {
                case ObstacleType.JUMP_OVER:
                    oBody = Instantiate(instance.obstacle, parent.pos + new Vector2((position * singleWidth) - (parent.width / 2), 1), Quaternion.identity);
                    break;

                case ObstacleType.SLIDE_UNDER:
                    oBody = Instantiate(instance.slideUnder, parent.pos + new Vector2((position * singleWidth) - (parent.width / 2), 3), Quaternion.identity);
                    break;

                case ObstacleType.ROOF_HOUSE:
                    oBody = Instantiate(instance.roofHouse, parent.pos + new Vector2((position * singleWidth) + (singleWidth/2) - (parent.width / 2), 2), Quaternion.identity);
                    oBody.transform.localScale = new Vector3(singleWidth-1, 1, 1);

                    break;

                default:
                    oBody = Instantiate(instance.obstacle, parent.pos + new Vector2((position * singleWidth) - (parent.width / 2), 1), Quaternion.identity);
                    break;
            }
        }
    }

    public class Platform
    {
        public PlatformType     pType;
        public Vector2          pos;
        public float            width;

        public GameObject       pGround;
        public GameObject       pBase;
        public GameObject       perfZone;
        public bool             perfected = false; //whether the platform was Perfect Jumped

        public List<Obstacle>   hazards = new List<Obstacle>();

        public Platform(Vector2 inP, PlatformType inTyp)
        {
            pType = inTyp;
            pos = inP;

            switch (pType)
            {

                case PlatformType.SS_BASIC_S:
                    width = singleWidth;

                    pGround = Instantiate(instance.platform, pos, Quaternion.identity);
                    pGround.transform.localScale = new Vector3(width, 1, 1);
                    pGround.GetComponent<PlayerLand>().SetParent(this);

                    pBase = Instantiate(instance.platformBase, pos + new Vector2(0, -25), Quaternion.identity);
                    pBase.transform.localScale = new Vector3(width, 50, 1);

                    perfZone = Instantiate(instance.perfectZone, pos + new Vector2(-width / 2 + 0.4f, 0.5f), Quaternion.identity);
                    perfZone.GetComponent<PerfectZone>().SetParent(this);
                    break;

                case PlatformType.SS_BASIC_M:
                    width = defaultWidth;

                    pGround = Instantiate(instance.platform, pos, Quaternion.identity);
                    pGround.transform.localScale = new Vector3(width, 1, 1);
                    pGround.GetComponent<PlayerLand>().SetParent(this);

                    pBase = Instantiate(instance.platformBase, pos + new Vector2(0, -25), Quaternion.identity);
                    pBase.transform.localScale = new Vector3(width, 50, 1);

                    perfZone = Instantiate(instance.perfectZone, pos + new Vector2(-width / 2 + 0.4f, 0.5f), Quaternion.identity);
                    perfZone.GetComponent<PerfectZone>().SetParent(this);
                    break;

                case PlatformType.SS_BASIC_L:
                    width = defaultWidth + singleWidth;

                    pGround = Instantiate(instance.platform, pos, Quaternion.identity);
                    pGround.transform.localScale = new Vector3(width, 1, 1);
                    pGround.GetComponent<PlayerLand>().SetParent(this);

                    pBase = Instantiate(instance.platformBase, pos + new Vector2(0, -25), Quaternion.identity);
                    pBase.transform.localScale = new Vector3(width, 50, 1);

                    perfZone = Instantiate(instance.perfectZone, pos + new Vector2(-width / 2 + 0.4f, 0.5f), Quaternion.identity);
                    perfZone.GetComponent<PerfectZone>().SetParent(this);
                    break;

                case PlatformType.SS_BASIC_XL:
                    width = defaultWidth * 2;

                    pGround = Instantiate(instance.platform, pos, Quaternion.identity);
                    pGround.transform.localScale = new Vector3(width, 1, 1);
                    pGround.GetComponent<PlayerLand>().SetParent(this);

                    pBase = Instantiate(instance.platformBase, pos + new Vector2(0, -25), Quaternion.identity);
                    pBase.transform.localScale = new Vector3(width, 50, 1);

                    perfZone = Instantiate(instance.perfectZone, pos + new Vector2(-width / 2 + 0.4f, 0.5f), Quaternion.identity);
                    perfZone.GetComponent<PerfectZone>().SetParent(this);
                    break;

                default:
                    width = defaultWidth;

                    pGround = Instantiate(instance.platform, pos, Quaternion.identity);
                    pGround.transform.localScale = new Vector3(width, 1, 1);
                    pGround.GetComponent<PlayerLand>().SetParent(this);

                    pBase = Instantiate(instance.platformBase, pos + new Vector2(0, -25), Quaternion.identity);
                    pBase.transform.localScale = new Vector3(width, 50, 1);

                    perfZone = Instantiate(instance.perfectZone, pos + new Vector2(-width / 2 + 0.4f, 0.5f), Quaternion.identity);
                    perfZone.GetComponent<PerfectZone>().SetParent(this);
                    break;
            }
        
        }
    }

    private List<Platform>              platforms = new List<Platform>();
    public static GameObject            currentPlatform;
    public static Platform              nextPlatform;
    public static PlatformGenerator     instance;

    private void Awake()
    {
        if (instance != null)
        {
            Debug.LogError("Platform Generator: more than one Platform Generator in the Scene.");
        }
        else
        {
            instance = this;
        }
    }

    void Start () {
        //Initialize, add starting platform
        platforms.Add(new Platform(new Vector2(6, -1), PlatformType.SS_BASIC_XL));
        currentPlatform = platforms[0].pGround.gameObject;
        transform.position = new Vector2(platforms[0].pos.x, transform.position.y);
    }

    void Update () {
        if (currentPlatform != null)
        {
            currentPlatform.GetComponent<Renderer>().material.color = new Color(0, 1, 0);
        }

        if (transform.position.x < generationPoint.position.x && platformsThisLevel < stageLength)
        {
            Platform lastPlat = platforms[platforms.Count - 1];
            PlatformType t = (PlatformType)Random.Range(0, System.Enum.GetValues(typeof(PlatformType)).Length);

            float newWidth = GetWidthOfType(t);
            float newHeight = Mathf.Floor(Random.Range(lastPlat.pos.y - 4, lastPlat.pos.y + 4f));
            float newDistBetween = gameSpeedMultiplier * Mathf.Floor(Random.Range(initialDistanceBetween, initialDistanceBetween * 2.1f));

            //New platform will be reachable from last platform
            transform.position = new Vector2(transform.position.x + (lastPlat.width/2) + newWidth/2 + newDistBetween, newHeight);

            platforms.Add(new Platform(transform.position, t));
            nextPlatform = platforms[platforms.Count - 2];
            platformsThisLevel += 1;

            if (platformsThisLevel >= stageLength)
            {
                //spawn the exit
                Instantiate(instance.goal, platforms[platforms.Count - 1].pos + new Vector2(0, 1), Quaternion.identity);
            }
            else if (platformsThisLevel == Mathf.Floor(stageLength / 2))
            {
                //spawn the store
                Instantiate(instance.store, platforms[platforms.Count - 1].pos + new Vector2(0, 2.5f), Quaternion.identity);
            }
            else
            {
                //for each obstacle slot on top of the new platform
                for(int i=1; i < platforms[platforms.Count - 1].width / singleWidth; i++)
                {
                    int rn = Random.Range(1, 100);
                    if (rn <= chanceOfObstacle)
                    {
                        //add obstacle
                        ObstacleType ot = (ObstacleType)Random.Range(0, System.Enum.GetValues(typeof(ObstacleType)).Length);
                        platforms[platforms.Count - 1].hazards.Add(new Obstacle(platforms[platforms.Count - 1], i, ot));
                        chanceOfObstacle -= 5f;
                        if(ot == ObstacleType.ROOF_HOUSE)
                        {
                            i++;
                        }
                    }
                    else
                    {
                        chanceOfObstacle += 2f;
                    }
                }
                
            }
        }

        //Clean up old platforms
        if (platforms.Count > 10)
        {
            Platform toDestroy = platforms[0];
            foreach (Obstacle h in toDestroy.hazards)
            {
                Destroy(h.oBody);
            }
            toDestroy.hazards.Clear();
            platforms.Remove(toDestroy);
            Destroy(toDestroy.pBase);
            Destroy(toDestroy.pGround);
            Destroy(toDestroy.perfZone);
        }
    }

    //Utility function; calculates average height of platforms in play
    public float AvgPlatformHeight()
    {
        float result = 0.0f;

        foreach(Platform plat in platforms)
        {
            result += plat.pos.y;
        }

        return result / platforms.Count;
    }

    private float GetWidthOfType(PlatformType typ)
    {
        float result = 0.0f;

        switch (typ)
        {
            case PlatformType.SS_BASIC_S:
                result = singleWidth;
                break;
            case PlatformType.SS_BASIC_M:
                result = defaultWidth;
                break;
            case PlatformType.SS_BASIC_L:
                result = defaultWidth + singleWidth;
                break;
            case PlatformType.SS_BASIC_XL:
                result = defaultWidth * 2;
                break;
            default:
                result = defaultWidth;
                break;
        }

        return result;
    }

    //Used to respawn the Player
    public Vector2 PosOfLeftmostPlatform()
    {
        Vector2 result = platforms[0].pos;
        return result;
    }
    public Vector2 PosOfPlatformAt(int p)
    {
        Vector2 result = Vector2.zero;

        if (p <= platforms.Count - 1)
        {
            result = platforms[p].pos;
        }
        return result;
    }

    public void UpdateMultiplier(float inMult)
    {
        gameSpeedMultiplier = inMult;
    }
}
