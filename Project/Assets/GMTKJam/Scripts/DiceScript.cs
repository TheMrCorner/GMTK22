using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiceScript : MonoBehaviour
{
    static Rigidbody rb;
    public static Vector3 diceVelocity;
    
    void Start()
    {
        DiceCanvas.diceNumber = 0;
        rb = GetComponent<Rigidbody> ();
    }

    void Update()
    {
        diceVelocity = rb.velocity;

        if(Input.GetKeyDown (KeyCode.Space)){

            float dirX = Random.Range (-500, 500);
            float dirY = Random.Range (-500, 500);
            float dirZ = Random.Range (-500, 500);
            rb.AddTorque(dirX,dirY,dirZ);

            rb.AddForce(transform.up * 500);
        }

    }
}
