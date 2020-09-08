using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

//Ninja Runner UI Manager class
//Keep all the UI references in one place

public class UIManager : MonoBehaviour {

    public UnityEngine.UI.Text      debugText;
    public TextMeshProUGUI          scoreText;
    public TextMeshProUGUI          countdownText;

    public TextMeshProUGUI          copperCount;
    public TextMeshProUGUI          silverCount;
    public TextMeshProUGUI          goldCount;
    public TextMeshProUGUI          platinumCount;

    public UnityEngine.UI.Image     copperFront;
    public UnityEngine.UI.Image     silverFront;
    public UnityEngine.UI.Image     goldFront;
    public UnityEngine.UI.Image     platinumFront;

    public UnityEngine.UI.Image     hookIcon;
    public TextMeshProUGUI          hookCount;

    public UnityEngine.UI.Image     logIcon;
    public TextMeshProUGUI          logCount;

    public Image[]                  hearts;
    public Sprite                   fullHeart;

    public Image[] emptyHearts;
    public Sprite emptyHeart;

    public GameObject gameOverPanel;
    public Image HurtFlash;
    private Color hurtColor;

    public static UIManager         instance;

	void Awake () {
        if (instance != null)
        {
            Debug.LogError("UI Manager: more than one UI Manager in the Scene.");
        }
        else
        {
            instance = this;
        }
    }

    private void Start()
    {
        //Initialize
        scoreText.text = GameController.playerScore.ToString();
        scoreText.enabled = false;

        hurtColor = HurtFlash.color;
        hurtColor.a = 1.0f;
    }

    void Update () {
        //flash red when injured
        HurtFlash.color = Color.Lerp(HurtFlash.color, Color.clear, 2*Time.deltaTime);

        //player health ui
        for (int i = 0; i < hearts.Length; i++)
        {
            if (i < PlayerController.GetCurrentHealth())
            {
                hearts[i].GetComponent<Image>().color = new Color(1, 0, 0);
            }
            else
            {
                hearts[i].GetComponent<Image>().color = new Color(1, 0, 0, 0);
            }

            if (i < PlayerController.GetMaximumHealth())
            {
                hearts[i].enabled = true;
                emptyHearts[i].enabled = true;
            }
            else
            {
                hearts[i].enabled = false;
                emptyHearts[i].enabled = false;
            }
        }

        //respawn countdown timer
        if (GameController.respawnTimerValue > 0.0f)
        {
            countdownText.enabled = true;
            countdownText.text = GameController.respawnTimerValue.ToString("F2");
        }
        else
        {
            countdownText.enabled = false;
        }

        //score UI
        scoreText.text = GameController.playerScore.ToString();
        UpdateCoinCountUI();

        //consumables/resources
        hookCount.text = FormatInteger(PlayerController.GetHookCount());
        logCount.text = FormatInteger(PlayerController.GetLogCount());

    }

    private void UpdateCoinCountUI()
    {
        copperFront.fillAmount = GameController.coinProgress.x / GameController.coinThresholds.x;
        silverFront.fillAmount = GameController.coinProgress.y / GameController.coinThresholds.y;
        goldFront.fillAmount = GameController.coinProgress.z / GameController.coinThresholds.z;
        platinumFront.fillAmount = GameController.coinProgress.w / GameController.coinThresholds.w;

        copperCount.text = FormatInteger((int)GameController.playerCoins.x);
        silverCount.text = FormatInteger((int)GameController.playerCoins.y);
        goldCount.text = FormatInteger((int)GameController.playerCoins.z);
        platinumCount.text = FormatInteger((int)GameController.playerCoins.w);
        
    }

    public void ToggleGameOverScreen(bool isActive)
    {
        gameOverPanel.gameObject.SetActive(isActive);
    }

    //Utility function, turns 5 into 05, 2 into 02, etc.
    private string FormatInteger(int n)
    {
        if (n < 10)
        {
            return "0" + n.ToString();
        }
        else
        {
            return n.ToString();
        }
    }

    public void GotHurt()
    {
        HurtFlash.color = hurtColor;
    }
}
