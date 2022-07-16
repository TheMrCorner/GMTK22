using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Card : MonoBehaviour
{
    [Tooltip("Amount of uses")]
    public int _amountOfUses = 2;

    [Tooltip("Cool down, in seconds")]
    public float _cooldownBetweenUses = 0.5f;

    public abstract void UseCard();
}
