using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Ninja Runner Platform Generator script

public class PlatformGenerator : MonoBehaviour {
    public GameObject       platform;
    public GameObject       platformBase;
    public GameObject       perfectZone;

    public GameObject       obstacle;
    public GameObject       goal;

    public Transform        generationPoint;
    public float            initialDistanceBetween;
    public static float     minWidth = 5; 
    public static float     maxWidth = 16;
    public int              stageLength;

    private float           gameSpeedMultiplier = 1;
    private float           chanceOfObstacle = 0f;
    private int             platformsThisLevel;
 
    public enum PlatformType
    {
        BASIC,
        MINI,
        TUNNEL
    }
    public enum ObstacleType
    {
        JUMP_OVER,
        DUCK_UNDER
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
        public Obstacle(Platform parent, ObstacleType ot)
        {
            parentPlat = parent;
            oType = ot;
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

        public Platform(Vector2 inP, float inWid, PlatformType inTyp)
        {
            pType = inTyp;
            pos = inP;
            width = inWid;

            switch (pType)
            {

                case PlatformType.BASIC:
                    pGround = Instantiate(instance.platform, pos, Quaternion.identity);
                    pGround.transform.localScale = new Vector3(width, 1, 1);
                    pGround.GetComponent<PlayerLand>().SetParent(this);

                    pBase = Instantiate(instance.platformBase, pos + new Vector2(0, -25), Quaternion.identity);
                    pBase.transform.localScale = new Vector3(width, 50, 1);

                    perfZone = Instantiate(instance.perfectZone, pos + new Vector2(-width / 2 + 0.4f, 0.5f), Quaternion.identity);
                    perfZone.GetComponent<PerfectZone>().SetParent(this);
                    break;

                case PlatformType.MINI:
                    width = minWidth;

                    pGround = Instantiate(instance.platform, pos, Quaternion.identity);
                    pGround.transform.localScale = new Vector3(width, 1, 1);
                    pGround.GetComponent<PlayerLand>().SetParent(this);

                    pBase = Instantiate(instance.platformBase, pos + new Vector2(0, -25), Quaternion.identity);
                    pBase.transform.localScale = new Vector3(width, 50, 1);

                    perfZone = Instantiate(instance.perfectZone, pos + new Vector2(-width / 2 + 0.4f, 0.5f), Quaternion.identity);
                    perfZone.GetComponent<PerfectZone>().SetParent(this);
                    break;

                default:
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
        public Platform(Vector2 inP, float inWid)
        {
            pos = inP;
            width = inWid;
            pType = PlatformType.BASIC;

            pGround = Instantiate(instance.platform, pos, Quaternion.identity);
            pGround.transform.localScale = new Vector3(inWid, 1, 1);
            pGround.GetComponent<PlayerLand>().SetParent(this);

            pBase = Instantiate(instance.platformBase, pos + new Vector2(0, -25), Quaternion.identity);
            pBase.transform.localScale = new Vector3(inWid, 50, 1);

            perfZone = Instantiate(instance.perfectZone, pos + new Vector2(-width / 2 + 0.4f, 0.5f), Quaternion.identity);
            perfZone.GetComponent<PerfectZone>().SetParent(this);

        }
    }

    private List<Platform>              platforms = new List<Platform>();
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
        platforms.Add(new Platform(new Vector2(6, -1), 20));
        transform.position = new Vector2(platforms[0].pos.x, transform.position.y);
    }

    void Update () {
        if(transform.position.x < generationPoint.position.x)
        {
            Platform lastPlat = platforms[platforms.Count - 1];

            PlatformType t = (PlatformType)Random.Range(0, System.Enum.GetValues(typeof(PlatformType)).Length);

            float newWidth = Mathf.Floor(Random.Range(minWidth + 1, maxWidth)); 
            if(t == PlatformType.MINI)
            {
                newWidth = minWidth;
            }
            float newHeight = Mathf.Floor(Random.Range(lastPlat.pos.y - 4, lastPlat.pos.y + 4f));
            float newDistBetween = gameSpeedMultiplier * Mathf.Floor(Random.Range(initialDistanceBetween, initialDistanceBetween * 2.1f));

            //New platform will be reachable from last platform
            transform.position = new Vector2(transform.position.x + (lastPlat.width/2) + newWidth/2 + newDistBetween, newHeight);

            platforms.Add(new Platform(transform.position, newWidth, t));
            platformsThisLevel += 1;

            if (platformsThisLevel >= stageLength)
            {
                //spawn the exit
                Instantiate(instance.goal, platforms[platforms.Count - 1].pos + new Vector2(0, 1), Quaternion.identity);
            }
            else
            {
                int rn = Random.Range(1, 100);
                if (rn <= chanceOfObstacle)
                {
                    //add obstacle
                    if (platforms[platforms.Count - 1].pType != PlatformType.MINI)
                    {
                        platforms[platforms.Count - 1].hazards.Add(new Obstacle(platforms[platforms.Count - 1]));
                        chanceOfObstacle -= 5f;
                    }
                }
                else
                {
                    chanceOfObstacle += 1;
                }
            }
        }

        //Clean up old platforms
        if (platforms.Count > 15)
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
