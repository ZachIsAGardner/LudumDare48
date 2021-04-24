using UnityEngine;

public class Prefabs : MonoBehaviour
{
    public GameObject HitEffect;
    public static Prefabs Instance;

    void Start()
    {
        Instance = this;
    }
}