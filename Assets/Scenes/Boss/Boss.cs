using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;

public class Boss : MonoBehaviour
{
    public GameObject bossHp;
    public Trigger helloTrigger;
    public bool victory = false;
    public GameObject skull;
    public bool fight = false;
    public GameObject heart;

    bool didFight = false;

    Animator animator;
    bool didVictory;
    bool startHello = false;
    float acceleration = 0.1f;
    Transform target;
    public Transform position1;
    public Transform position2;
    List<Emitter> emitters;
    List<HideBeforeBattle> hides;

    // Start is called before the first frame update
    void Start()
    {
        Sounds.Play("Cave", null, true, 0.75f);

        animator = GetComponent<Animator>();
        emitters = FindObjectsOfType<Emitter>().ToList();
        emitters.ForEach(x => x.gameObject.SetActive(false));
        hides = FindObjectsOfType<HideBeforeBattle>().ToList();
        hides.ForEach(x => x.gameObject.SetActive(false));
        helloTrigger.OnEnter = (Collider other) =>
        {
            if (other.CompareTag("Player") && !startHello)
            {
                startHello = true;
                _ = StartTalk();
            }
        };
    }

    async Task StartTalk()
    {
        // 0

        var textbox = Instantiate(Prefabs.Get("TextBox"), GameObject.FindGameObjectWithTag("Canvas").transform).GetComponentInChildren<TextMeshProUGUI>();

        // 1

        textbox.text = "Hello.";

        Vector3 oldAngle = Camera.main.transform.eulerAngles;
        Camera.main.GetComponent<Follow>().follow = null;
        target = position1;

        while (true)
        {
            if (Game.ProceedText())
            {
                break;
            }
            await new WaitForUpdate();
        }
        await new WaitForUpdate();

        // 2

        textbox.text = "I see you've fallen for my cool dude trap.";

        target = position2;

        while (true)
        {
            if (Game.ProceedText())
            {
                break;
            }
            await new WaitForUpdate();
        }
        await new WaitForUpdate();

        // 3

        textbox.text = "I'm going to kill you, how else am I supposed to impress my friends?";

        while (true)
        {
            if (Game.ProceedText())
            {
                break;
            }
            await new WaitForUpdate();
        }
        await new WaitForUpdate();

        // 4

        target = null;
        Camera.main.GetComponent<Follow>().follow = FindObjectOfType<Player>().transform;
        Camera.main.transform.eulerAngles = oldAngle;

        Destroy(textbox.transform.parent.gameObject);

        bossHp.SetActive(true);
        heart.SetActive(true);

        Sounds.Play("CoolBattleIGuess", null, true, 1, 1.25f);
    }

    void MakeTimer()
    {
        if (victory) return;
        animator.SetInteger("State", 2);
        Timer.Create(new List<float>() { 15, 30, 45 }.Random(), MakeTimer);
    }

    async Task DieTalk()
    {
        // 0

        var textbox = Instantiate(Prefabs.Get("TextBox"), GameObject.FindGameObjectWithTag("Canvas").transform).GetComponentInChildren<TextMeshProUGUI>();

        skull.GetComponent<Animator>().SetInteger("State", 2);
        skull.GetComponent<Rigidbody>().useGravity = true;
        skull.GetComponent<Rigidbody>().AddForce(new Vector3(5, 100, 10));

        // 1

        textbox.text = "NOOOOOOOOOOOOOOOOOOOOOOOOOOO.";

        Vector3 oldAngle = Camera.main.transform.eulerAngles;
        Camera.main.GetComponent<Follow>().follow = null;
        target = position1;

        while (true)
        {
            if (Game.ProceedText())
            {
                break;
            }
            await new WaitForUpdate();
        }
        await new WaitForUpdate();

        // 2

        textbox.text = "tell    them     i      was        cooooooooo o o o o o ooo o o  l l l l ll ll.";

        while (true)
        {
            if (Game.ProceedText())
            {
                break;
            }
            await new WaitForUpdate();
        }
        await new WaitForUpdate();

        // end

        target = null;
        Camera.main.GetComponent<Follow>().follow = FindObjectOfType<Player>().transform;
        Camera.main.transform.eulerAngles = oldAngle;

        Destroy(textbox.transform.parent.gameObject);

        bossHp.SetActive(false);

        Sounds.Play("Wind", null, true, 1, 1);
        Sounds.Play("Victory");

        _ = Game.LoadAsync("Thanks", Prefabs.Get<SceneTransition>("FadeSceneTransition"));
    }

    void HandAttackEnd()
    {
        animator.SetInteger("State", 1);
    }

    // Update is called once per frame
    void Update()
    {
        if (fight && !didFight)
        {
            emitters.ForEach(x => x.gameObject.SetActive(true));
            hides.ForEach(x => x.gameObject.SetActive(true));
            emitters.First().emitTime = 0;
            emitters.Last().emitTime = 0;
            Timer.Create(new List<float>() { 10, 20 }.Random(), MakeTimer);
            didFight = true;
        }
        

        if (victory && !didVictory)
        {
            didVictory = true;
            _ = DieTalk();
        }

        if (target != null)
        {
            Camera.main.transform.position = new Vector3(
                Camera.main.transform.position.x.MoveOverTime(target.position.x, acceleration),
                Camera.main.transform.position.y.MoveOverTime(target.position.y, acceleration),
                Camera.main.transform.position.z.MoveOverTime(target.position.z, acceleration)
            );

            Camera.main.transform.eulerAngles = new Vector3(
                Camera.main.transform.eulerAngles.x.MoveOverTime(target.eulerAngles.x, acceleration),
                Camera.main.transform.eulerAngles.y.MoveOverTime(target.eulerAngles.y, acceleration),
                Camera.main.transform.eulerAngles.z.MoveOverTime(target.eulerAngles.z, acceleration)
            );
        }
    }
}
