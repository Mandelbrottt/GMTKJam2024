using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameMode : MonoBehaviour
{
    public PlayerController P1;
    public PlayerController P2;

    public void onDeath()
    {
        if (P1.GetHealth() > P2.GetHealth())
        {
            Debug.Log("Player 1 is the ACE");
        }
        else
        {
            Debug.Log("Player 2 is the ACE");
        }
    }
}
