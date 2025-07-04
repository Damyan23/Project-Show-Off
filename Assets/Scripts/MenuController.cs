using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuController : MonoBehaviour
{
    [SerializeField] private float transitionTime = 2f;
    [SerializeField] private Animator transition;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.None;
    }

    public void StartGame()
    {
        StartCoroutine(StartGameCoroutine());
    }
    private IEnumerator StartGameCoroutine()
    {
        transition.SetTrigger ("Start");

        yield return new WaitForSeconds(transitionTime);
        SceneManager.LoadScene(1);
    }
}
