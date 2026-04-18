using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AimatPlayer : MonoBehaviour
{
    private Transform m_Player;
    private void Start()
    {
        m_Player = GameObject.FindGameObjectWithTag("Player").transform;
    }
    public void Aim()
    {
        transform.LookAt(m_Player.position);
    }
}
