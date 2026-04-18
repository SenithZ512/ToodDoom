using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "New StatusAttachment Type", menuName = "StatusConfig/StatusAttachment Type")]

public class SO_statusAttachment : ScriptableObject, IStatus
{
    public float _Health;
    public float _Armour;
    public float _Speed;

    public float Health => _Health;
        
    public float Armour => _Armour;

    public float Speed => _Speed;
}
