using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Currency : MonoBehaviour
{
    public static int currency = 0;

    public void IncreastCurrency(int amount)
    {
        currency += amount;
    }
}
