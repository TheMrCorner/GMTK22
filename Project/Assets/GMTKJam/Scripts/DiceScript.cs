using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiceScript : MonoBehaviour
{
    static Rigidbody rb;
    public static Vector3 diceVelocity;
    
    void Start()
    {
        rb = GetComponent<Rigidbody> ();
    }

    void Update()
    {
        diceVelocity = rb.velocity;

        if(Input.GetKeyDown (KeyCode.Space)){

            DiceCanvas.diceNumber = 0;
            float dirX = Random.Range (0, 500);
            float dirY = Random.Range (0, 500);
            float dirZ = Random.Range (0, 500);
            rb.AddTorque(dirX,dirY,dirZ);

            rb.AddForce(transform.up * 500);
        }

    }
}
