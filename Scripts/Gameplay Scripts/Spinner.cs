using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spinner : MonoBehaviour
{
    public float spinSpeed = 50f; // How many degrees to rotate per second
    private float temp = 0f;

    void Update()
    {
        // Get the rotation of the object
        Quaternion rotation = transform.rotation;

        // spins then clamps to 360
        temp += spinSpeed * Time.deltaTime;
        temp %= 360;

        // Rotate the object around its Z axis by the spin speed
        rotation = Quaternion.Euler(0f, 0f, temp);

        // Set the rotation of the object
        transform.rotation = rotation;
    }
}
