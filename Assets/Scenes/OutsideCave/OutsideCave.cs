using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class OutsideCave : MonoBehaviour
{
    public Transform target1;
    public Transform target2;
    public GameObject startUi;
    public PromptedTrigger signTrigger;
    public GameObject Note;
    public Trigger EntranceTrigger;

    float noteCooldown = 0;

    List<TextMeshProUGUI> texts;
    float acceleration = 0.1f;
    bool moving = false;
    Player player;

    void Start()
    {
        Sounds.Play("Wind", null, true);

        Camera.main.transform.position = target1.transform.position;
        Camera.main.transform.eulerAngles = target1.transform.eulerAngles;
        texts = startUi.GetComponentsInChildren<TextMeshProUGUI>().ToList();
        player = FindObjectOfType<Player>();
        player.gameObject.SetActive(false);
        signTrigger.Execute = (_) => 
        {
            Note.SetActive(true);
            noteCooldown = 0.25f;
        };
        signTrigger.OnExit = (_) => 
        {
            Note.SetActive(false);
        };
        EntranceTrigger.OnEnter = (Collider other) => 
        {
            if (other.CompareTag("Player"))
            {
                _ = Game.LoadAsync("Cave", Prefabs.Get<SceneTransition>("FadeSceneTransition"));
            }
        };
    }

    void Update()
    {
        noteCooldown -= Time.deltaTime;

        if (Note.activeSelf && Game.ProceedText() && noteCooldown <= 0)
        {
            Note.SetActive(false);
        }

        if (Game.ProceedText())
        {
            moving = true;
            player.gameObject.SetActive(true);
        }

        if (moving)
        {
            foreach (var text in texts)
            {
                text.color = new Color(
                    text.color.r,
                    text.color.g,
                    text.color.b,
                    text.color.a - Time.deltaTime
                );
            }

            Camera.main.transform.position = new Vector3(
                Camera.main.transform.position.x.MoveOverTime(target2.transform.position.x, acceleration),
                Camera.main.transform.position.y.MoveOverTime(target2.transform.position.y, acceleration),
                Camera.main.transform.position.z.MoveOverTime(target2.transform.position.z, acceleration)
            );

            Camera.main.transform.eulerAngles = new Vector3(
                Camera.main.transform.eulerAngles.x.MoveOverTime(target2.transform.eulerAngles.x, acceleration),
                Camera.main.transform.eulerAngles.y.MoveOverTime(target2.transform.eulerAngles.y, acceleration),
                Camera.main.transform.eulerAngles.z.MoveOverTime(target2.transform.eulerAngles.z, acceleration)
            );
        }
    }
}
