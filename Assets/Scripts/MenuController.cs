using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuController : MonoBehaviour
{
    [SerializeField] private Image blackFade;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.None;
    }

    public void StartGame()
    {
        SceneManager.LoadScene(1);
    }
}
