using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameMode : MonoBehaviour
{
    public PlayerController P1;
    public PlayerController P2;

    public Transform spawn1;
    public Transform spawn2;

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

    public int Join(PlayerController pc)
    {
        pc.PlayerDeath.AddListener(onDeath);
        if(P1 == null)
        {
            Debug.Log("Spawn 1");
            P1 = pc;
            pc.gameObject.transform.position = spawn1.transform.position;
            pc.gameObject.transform.rotation = spawn1.transform.rotation;
            pc.gameObject.transform.parent.GetComponentInChildren<Camera>().cullingMask = ~(1 << 8);
            pc.gameObject.transform.parent.GetComponentInChildren<CinemachineVirtualCamera>().gameObject.layer = 7;
            return 1;
        }
        Debug.Log("Spawn 2");
        P2 = pc;
        pc.gameObject.transform.position = spawn2.transform.position;
        pc.gameObject.transform.rotation = spawn2.transform.rotation;
        pc.gameObject.transform.parent.GetComponentInChildren<Camera>().cullingMask = ~(1 << 7);
        pc.gameObject.transform.parent.GetComponentInChildren<CinemachineVirtualCamera>().gameObject.layer = 8;
        return 2;
    }
}
