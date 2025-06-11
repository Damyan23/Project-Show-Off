using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class DeathScreen : MonoBehaviour
{
    [SerializeField] private Image blackFade;

    [SerializeField] private Transform enemy;
    [SerializeField] private float enemyMoveSpeed;
    [SerializeField, Tooltip("Lower Value = More Fog")] private float fogIntensity = 5f;

    private bool transitionDone = false;
    private Volume globalVolume;
    private Transform camT;
    private Vector3 enemyMoveDir;
   
    private void Start()
    {
        Fog fog;
        globalVolume = GameObject.Find("Global Volume").GetComponent<Volume>();
        globalVolume.profile.TryGet(out fog);
        fog.meanFreePath.value = fogIntensity;

        camT = Camera.main.transform;

        enemyMoveDir = Vector3.Normalize(camT.position - enemy.position);
        enemy.rotation = Quaternion.LookRotation(-enemyMoveDir);

        StartCoroutine(TransitionIntoScene());
    }

    private void Update()
    {
        if (!transitionDone) return;

        //Make enemy move faster over time
        //enemyMoveSpeed = Mathf.Min(enemyMoveSpeed * 1.05f, 15f);
        float frameDst = Time.deltaTime * enemyMoveSpeed;   

        if(Vector3.Distance(enemy.position, camT.position) > frameDst)
        {
            enemy.Translate(enemyMoveDir * frameDst);
        }
        else
        {
            StartCoroutine(EndGame());
        }
    }

    private IEnumerator TransitionIntoScene()
    {
        float alpha = 1f;

        while (alpha > 0f)
        {
            alpha -= 0.05f;
            blackFade.color = new Color(0f, 0f, 0f, alpha);
            yield return new WaitForSeconds(0.05f);
        }

        yield return new WaitForSeconds(1f);

        transitionDone = true;
    }

    private IEnumerator EndGame()
    {
        blackFade.color = new Color(0f, 0f, 0f, 1f);

        yield return new WaitForSeconds(2f);

        SceneManager.LoadScene(0);
    }
}
