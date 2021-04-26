using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Checkpoint : MonoBehaviour
{
    Trigger trigger;
    Animator animator;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        trigger = GetComponentInChildren<Trigger>();
        trigger.OnEnter = (Collider other) =>
        {
            if (other.CompareTag("Player"))
            {
                if (animator.GetInteger("State") != 2)
                    Sounds.Play("Victory");
                animator.SetInteger("State", 2);
                Player player = FindObjectOfType<Player>();
                player.potionCount = 3;
                Game.SavePoint = new SavePoint(SceneManager.GetActiveScene().name, transform.position + new Vector3(1, 0, 1));
            }
        };
    }

    // Update is called once per frame
    void Update()
    {

    }
}
