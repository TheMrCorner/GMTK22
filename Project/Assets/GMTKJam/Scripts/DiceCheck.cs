using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiceCheck : MonoBehaviour
{
    Vector3 Velocity;
    public RigidBody Dice;
    public int diceNumber;

    void FixedUpdate(){
        Velocity = Dice.Velocity; 
    }

    void OnTriggerStay(Collider col){

        if(Velocity == Vector3(0,0,0)){

        switch (col.gameObject.name){
            case "Side1"
                diceNumber = 1;
                break;
            case "Side2"
                diceNumber = 2;
                break;
            case "Side3"
                diceNumber = 3;
                break;
            case "Side4"
                diceNumber = 4;
                break;
            case "Side5"
                diceNumber = 5;
                break;
            case "Side6"
                diceNumber = 6;
                break;
            }
        }
    }
}