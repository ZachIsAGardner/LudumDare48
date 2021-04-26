using UnityEngine;

public class Rotator : MonoBehaviour
{
    public Vector3 Rotation = new Vector3(0,0,0);

    void Update()
    {
        transform.eulerAngles = new Vector3(
            transform.eulerAngles.x + (Rotation.x * Time.deltaTime),
            transform.eulerAngles.y + (Rotation.y * Time.deltaTime),
            transform.eulerAngles.z + (Rotation.z * Time.deltaTime)
        );
    }
}