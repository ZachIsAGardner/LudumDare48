using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cave : MonoBehaviour
{
    public Trigger NextTrigger;

    public PromptedTrigger attackTip;
    public GameObject attackNote;
    public PromptedTrigger healTip;
    public GameObject healNote;
    public PromptedTrigger dodgeTip;
    public GameObject dodgeNote;
    float noteCooldown = 0;
    GameObject note;

    // Start is called before the first frame update
    void Start()
    {
        Sounds.Play("Cave", null, true, 0.75f);

        NextTrigger.OnEnter = (Collider other) =>
        {
            if (other.CompareTag("Player"))
            {
                _ = Game.LoadAsync("Boss", Prefabs.Get<SceneTransition>("FadeSceneTransition"));
            }
        };

        attackTip.Execute = (_) => 
        {
            if (note != null)
                note.SetActive(false);

            note = attackNote;

            note.SetActive(true);
            noteCooldown = 0.25f;
        };
        attackTip.OnExit = (_) => 
        {
            if (note != null)
                note.SetActive(false);
        };

        healTip.Execute = (_) => 
        {
            if (note != null)
                note.SetActive(false);

            note = healNote;

            note.SetActive(true);
            noteCooldown = 0.25f;
        };
        healTip.OnExit = (_) => 
        {
            if (note != null)
                note.SetActive(false);
        };

        dodgeTip.Execute = (_) => 
        {
            if (note != null)
                note.SetActive(false);

            note = dodgeNote;

            note.SetActive(true);
            noteCooldown = 0.25f;
        };
        dodgeTip.OnExit = (_) => 
        {
            if (note != null)
                note.SetActive(false);
        };
    }

    // Update is called once per frame
    void Update()
    {
        noteCooldown -= Time.deltaTime;

        if (note != null && note.activeSelf && Game.ProceedText() && noteCooldown <= 0)
        {
            note.SetActive(false);
        }
    }
}
