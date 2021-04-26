using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyIfVictory : MonoBehaviour
{

    Boss scene;
    // Start is called before the first frame update
    void Start()
    {
        scene = FindObjectOfType<Boss>();
    }

    // Update is called once per frame
    void Update()
    {
        if (scene.victory) Destroy(gameObject);
    }
}
