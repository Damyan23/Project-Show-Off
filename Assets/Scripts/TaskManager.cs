using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TaskManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI taskText;
    [HideInInspector] public string currentTask;

    private delegate void TaskUpdateHandler(string task);
    private event TaskUpdateHandler OnTaskUpdate;

    void Awake()
    {
        OnTaskUpdate += SetTask;
    }

    void OnDestroy()
    {
        OnTaskUpdate -= SetTask;
    }

    /// <summary>
    /// This delegate is used to update the current task in the UI. Just pass in the task itself without passing in "Current task: " as it is already handled in the method.
    /// </summary>
    /// <param name="task">
    /// The description of the new task to display in the UI (e.g., "Put the <i>Book</i> on one of the altars.").
    /// </param>
    public void UpdateTask(string task)
    {
        if (string.IsNullOrEmpty(task))
        {
            Debug.LogWarning("TaskManager: Attempted to update task with an empty string.");
            return;
        }

        OnTaskUpdate?.Invoke(task);
    }

    private void SetTask(string currentTask)
    {
        this.currentTask = currentTask;
        taskText.text = "Current task:" + "" + currentTask;
    }
}
