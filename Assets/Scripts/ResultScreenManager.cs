using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class ResultScreenManager : MonoBehaviour
{
    [Header("Configuração da Tela")]
    public TMP_Text resultText;

    [Header("Configuração das Estrelas")]
    public GameObject[] starParents;
    public GameObject[] starChildren;

    [Header("Configuração dos Botões")]
    public Button continueButton;
    public Button retryButton;
    public Button menuButton;

    public string creativeMural = "CreativeMural";
    public string ballSort = "BallSort";
    public string tangram = "Tangram1";
    public string menuScene = "Menu";
    public string resultScene = "ResultScene";

    void Start()
    {
        bool levelWon = PlayerPrefs.GetInt("LevelWon", 0) == 1;

        if (levelWon)
        {
            resultText.text = "Vitória!";
            SetStars(3);
            continueButton.gameObject.SetActive(true);
            retryButton.gameObject.SetActive(false);
            menuButton.gameObject.SetActive(true);
        }
        else
        {
            resultText.text = "Derrota!";
            SetStars(0);
            continueButton.gameObject.SetActive(false);
            retryButton.gameObject.SetActive(true);
            menuButton.gameObject.SetActive(true);
        }

        continueButton.onClick.AddListener(ContinueToNextLevel);
        retryButton.onClick.AddListener(RetryLevel);
        menuButton.onClick.AddListener(() => SceneManager.LoadScene(menuScene));
    }

    void SetStars(int starsEarned)
    {
        for (int i = 0; i < starParents.Length; i++)
        {
            starChildren[i].SetActive(i < starsEarned);
            if (i < starsEarned)
            {
                starChildren[i].transform.localScale = Vector3.zero;
                starChildren[i].transform.DOScale(1, 0.5f).SetEase(Ease.OutBack);
            }
        }
    }

    void ContinueToNextLevel()
    {
        string lastScene = PlayerPrefs.GetString("LastScene", creativeMural);
        string nextScene = "";
        if (lastScene == creativeMural)
            nextScene = ballSort;
        else if (lastScene == ballSort)
            nextScene = tangram;
        else
            nextScene = menuScene;
        SceneManager.LoadScene(nextScene);
    }

    void RetryLevel()
    {
        string lastScene = PlayerPrefs.GetString("LastScene", creativeMural);
        SceneManager.LoadScene(lastScene);
    }
}