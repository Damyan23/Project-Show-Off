using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TaskManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI taskText;
    private string currentTask;

    public void SetTask(string currentTask)
    {
        this.currentTask = currentTask;
        taskText.text = "Current task:" + "" + currentTask;
    }
}
