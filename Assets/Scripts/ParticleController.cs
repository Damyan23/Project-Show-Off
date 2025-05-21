using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleController : MonoBehaviour
{
    private ParticleSystem particle;
    private ParticleSystem.MainModule main;
    [SerializeField] private float buildUpTime = 2f;

    private float timer = 0f;
    private float startLifetime = 0;
    private float targetLifetime = 3;

    void Start()
    {
        particle = GetComponent<ParticleSystem>();
        main = particle.main;
        main.startLifetime = startLifetime;
    }

    void Update()
    {
        if (!particle.isPlaying) return;
        if (timer < buildUpTime)
        {
            timer += Time.deltaTime;
            float t = Mathf.Clamp01(timer / buildUpTime);
            main.startLifetime = Mathf.Lerp(startLifetime, targetLifetime, t);
        }
    }
}
