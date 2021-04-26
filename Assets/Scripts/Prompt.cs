using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Prompt : MonoBehaviour
{
    public Vector3 offset;

    Transform promptTarget;

    void Start()
    {
    }

    void Update()
    {
        promptTarget = promptTarget ?? GameObject.FindGameObjectWithTag("PromptTarget")?.transform;

        if (promptTarget == null) return;

        Vector3 point = Camera.main.WorldToScreenPoint(promptTarget.position) + offset;

        transform.position = point;
    }
}
