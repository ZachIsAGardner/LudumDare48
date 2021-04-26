using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Emitter : MonoBehaviour
{
    public GameObject target;
    public List<float> times;
    public float emitTime;

    // Start is called before the first frame update
    void Start()
    {
        emitTime = times.Random();   
    }

    // Update is called once per frame
    void Update()
    {
        emitTime -= Time.deltaTime;

        if (emitTime <= 0)
        {
            Instantiate(target, transform.position, target.transform.rotation);
            emitTime = times.Random();
            Sounds.Play("Cough", transform.position);
        }
    }
}
