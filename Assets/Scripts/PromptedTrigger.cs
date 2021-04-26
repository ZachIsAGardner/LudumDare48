using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PromptedTrigger : MonoBehaviour
{
    public Action<PromptedTrigger> Execute;

    GameObject prompt;
    GameObject canvas;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            prompt = Instantiate(Prefabs.Get("Prompt"), canvas.transform);
        }

        if (other.CompareTag("Player") && OnEnter != null)
            OnEnter(other);
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player") && OnStay != null)
            OnStay(other);
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") && prompt != null)
        {
            Destroy(prompt);
        }

        if (other.CompareTag("Player") && OnExit != null)
            OnExit(other);
    }

    public Action<Collider> OnEnter;
    public Action<Collider> OnStay;
    public Action<Collider> OnExit;

    void Start()
    {
        canvas = GameObject.FindGameObjectWithTag("Canvas");
    }

    void Update()
    {
        if (Game.ProceedText() && prompt != null)
        {
            Execute(this);
            Sounds.Play("Swing", null, false, 0.5f, 2);
            Destroy(prompt);
        }
    }
}
