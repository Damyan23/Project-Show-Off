using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BabyBehaviour : MonoBehaviour
{
    private float interactionDisntance;
    private KeyCode interactionKey = KeyCode.E;
    private Transform player;

    void Awake()
    {
        interactionDisntance = PlayerInteraction.Instance.interactionDistance;
    }

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
            Debug.Log("asd");
        }
    }

    private void gameOver()
    {
        SceneManager.LoadScene(0);
    }
}
