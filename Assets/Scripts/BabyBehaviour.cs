using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BabyBehaviour : MonoBehaviour
{
    private float interactionDisntance = 5;
    private KeyCode interactionKey = KeyCode.E;
    private Transform player;
    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    // Update is called once per frame
    void Update()
    {
        if (Vector3.Distance(player.position, this.transform.position) < interactionDisntance && Input.GetKeyDown(interactionKey))
        {
            gameOver();
        }
    }

    private void gameOver()
    {
        SceneManager.LoadScene(0);
    }
}
