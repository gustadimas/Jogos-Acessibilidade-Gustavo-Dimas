using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Audio;
using DG.Tweening;

public class HUDController : MonoBehaviour
{
    public void StartGame()
    {
        SceneManager.LoadScene("CreativeMural");
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}