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
        StartCoroutine(FadeIn());   
    }

    public void StartGame()
    {
        SceneManager.LoadScene(1);
    }


    private IEnumerator FadeIn()
    {
        float alpha = 1f;

        while (alpha > 0f)
        {
            alpha -= 0.05f;
            blackFade.color = new Color(0f, 0f, 0f, alpha);
            yield return new WaitForSeconds(0.05f);
        }
    }
}
