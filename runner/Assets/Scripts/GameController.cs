using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using UnityEngine.SceneManagement;

//Ninja Runner Game Controller script

public class GameController : MonoBehaviour {

    public float            speedMultiplier;
    public float            maxSpeedMultiplier;
    public float            speedMultiplierTick; //How much it ticks up every frame.

    public static int       perfectPoints = 1000; //Amount of points awarded for a perfect jump.
    public static int       perfectJumpMultiplier = 0;

    public float            respawnTimer;
    public static float     respawnTimerValue = 0f;
    private static bool     frozen = false;

    public static int       playerScore = 0;
    public static Vector4   playerCoins = new Vector4(0, 0, 0, 0);
    public static Vector4   coinThresholds = new Vector4(1000, 10000, 100000, 1000000);
    public static Vector4   coinProgress = new Vector4(0, 0, 0, 0);
    private static Vector2  currentLevel = new Vector2(1, 1);

    private PlayerController            playerCon;
    private PlatformGenerator           platformGen;
    private UIManager                   UIMan;

    public CinemachineVirtualCamera     vCam;
    public static GameController        instance;

    private void Awake()
    {
        if (instance != null)
        {
            Debug.LogError("Game Controller: more than one Game Controller in the Scene.");
            return;
        }
        else
        {
            instance = this;
        }
    }
    void Start () {
        //Initialization
        playerCon = PlayerController.instance;
        platformGen = PlatformGenerator.instance;
        UIMan = UIManager.instance;

        SetSpeedMultiplier(speedMultiplier);
        SpawnPlayer();
    }

    private void FixedUpdate()
    {
        if(PlayerController.currHealth <= 0)
        {
            ToggleGameOver();
        }

        //Did the Player fall or hit a wall?
        if (playerCon.transform.position.y < platformGen.AvgPlatformHeight() - 25 ||
            playerCon.hitWall)
        {
            if (speedMultiplier - 0.03f > 1.0f)
            {
                SetSpeedMultiplier(speedMultiplier - 0.03f);
            }
            else
            {
                SetSpeedMultiplier(1.0f);
            }

            playerCon.TakeDamage(1);
            SpawnPlayer();
            
        }
        else if (playerCon.hitObstacle)
        {
            playerCon.TakeDamage(1);
            playerCon.hitObstacle = false;
            SetSpeedMultiplier(speedMultiplier - 0.08f);
            
        }
        else if (playerCon.hitGoal)
        {
            //Go to next level
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex );
        }
    }
    void Update () {

        //Freeze in place while respawn timer counts down
        if(respawnTimerValue > 0.0f || PlayerController.CurrentHealth() <= 0)
        { 
            respawnTimerValue -= Time.deltaTime;
        }
        else
        {
            frozen = false;
            playerCon.ToggleFreeze(false);
        }

        //Don't update score if frozen in place
        if (!frozen)
        {
            TickUpMultiplier();
            //tick up score, also sets coins
            AdjustScore((int)speedMultiplier);
        }

        //print debug stuff
        p("");
        p("Perfect jump multiplier: " + perfectJumpMultiplier);
        p("Speed multiplier: " + speedMultiplier.ToString("F2"));

	}

    private void TickUpMultiplier()
    {
        SetSpeedMultiplier(speedMultiplier += speedMultiplierTick * Time.deltaTime);
    }
    private void SetSpeedMultiplier(float inVal)
    {
        speedMultiplier = inVal;
        if(inVal > maxSpeedMultiplier)
        {
            speedMultiplier = maxSpeedMultiplier;
        }
        playerCon.UpdateMultiplier(speedMultiplier);
        platformGen.UpdateMultiplier(speedMultiplier);
    }

    private void SpawnPlayer()
    {
        perfectJumpMultiplier = 0;
        respawnTimerValue = respawnTimer;
        playerCon.ToggleFreeze(true);
        frozen = true;

        playerCon.Respawn(platformGen.PosOfPlatformAt(1));
    }
    private void ToggleGameOver()
    {
        playerCon.ToggleFreeze(true);
        frozen = true;
        ResetScore();
        SceneManager.LoadScene(0);
    }

    public void p(string inS)
    {
        if (inS == "")
        {
            UIMan.debugText.text = "";
            return;
        }
        UIMan.debugText.text += "\n" + inS;
    }

    public static void AdjustScore(int amt)
    {
        //Increment the score by amt

        //actual score
        playerScore += amt;
        if(playerScore < 0)
        {
            playerScore = 0;
        }

        //coins
        coinProgress += new Vector4(amt, amt, amt, amt);
        if(coinProgress.w >= coinThresholds.w)
        {
            playerCoins += new Vector4(1, 1, 1, 1);
            coinProgress -= coinThresholds;
        }
        else if(coinProgress.z >= coinThresholds.z)
        {
            playerCoins += new Vector4(1, 1, 1, 0);
            coinProgress -= new Vector4(coinThresholds.x, coinThresholds.y, coinThresholds.z, 0);
        }
        else if(coinProgress.y >= coinThresholds.y)
        {
            playerCoins += new Vector4(1, 1, 0, 0);
            coinProgress -= new Vector4(coinThresholds.x, coinThresholds.y, 0, 0);
        }
        else if (coinProgress.x >= coinThresholds.x)
        {
            playerCoins += new Vector4(1, 0, 0, 0);
            coinProgress -= new Vector4(coinThresholds.x, 0, 0, 0);
        }
    }
    public static void ResetScore()
    {
        playerScore = 0;
        coinProgress = Vector4.zero;
        playerCoins = Vector4.zero;
    }

    //Static method, callable from other classes
    public static void PerfectJump()
    {
        perfectJumpMultiplier += 1;
        AdjustScore(perfectPoints * perfectJumpMultiplier);
    }
    
}
