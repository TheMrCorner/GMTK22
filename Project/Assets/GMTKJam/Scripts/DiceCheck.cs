using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiceCheck : MonoBehaviour
{
    Vector3 diceVel;

    void FixedUpdate(){
        diceVel = DiceScript.diceVelocity; 
    }

    void OnTriggerStay(Collider col){

        if (diceVel == new Vector3 (0f, 0f, 0f)){

        switch (col.gameObject.name){
            case "Side1":
                DiceCanvas.diceNumber = 1;
                break;
            case "Side2":
                DiceCanvas.diceNumber = 2;
                break;
            case "Side3":
                DiceCanvas.diceNumber = 3;
                break;
            case "Side4":
                DiceCanvas.diceNumber = 4;
                break;
            case "Side5":
                DiceCanvas.diceNumber = 5;
                break;
            case "Side6":
                DiceCanvas.diceNumber = 6;
                break;
            }
        }
    }
}