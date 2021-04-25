using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour, IHurter
{
    public void LandedHit(GameObject other)
    {
        Destroy(gameObject);
    }
}
