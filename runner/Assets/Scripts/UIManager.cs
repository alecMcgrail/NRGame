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

    public Image[]                  hearts;
    public Sprite                   fullHeart;

    public Image[] emptyHearts;
    public Sprite emptyHeart;

    public GameObject gameOverPanel;

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
    }

    void Update () {
        //player health ui
        for (int i = 0; i < hearts.Length; i++)
        {
            if (i < PlayerController.CurrentHealth())
            {
                hearts[i].GetComponent<Image>().color = new Color(1, 0, 0);
            }
            else
            {
                hearts[i].GetComponent<Image>().color = new Color(1, 0, 0, 0);
            }

            if (i < PlayerController.MaximumHealth())
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
    }

    private void UpdateCoinCountUI()
    {
        copperFront.fillAmount = GameController.coinProgress.x / GameController.coinThresholds.x;
        silverFront.fillAmount = GameController.coinProgress.y / GameController.coinThresholds.y;
        goldFront.fillAmount = GameController.coinProgress.z / GameController.coinThresholds.z;
        platinumFront.fillAmount = GameController.coinProgress.w / GameController.coinThresholds.w;

        copperCount.text = GameController.playerCoins.x.ToString();
        silverCount.text = GameController.playerCoins.y.ToString();
        goldCount.text = GameController.playerCoins.z.ToString();
        platinumCount.text = GameController.playerCoins.w.ToString();
    }

    public void ToggleGameOverScreen(bool isActive)
    {
        gameOverPanel.gameObject.SetActive(isActive);
    }

}
