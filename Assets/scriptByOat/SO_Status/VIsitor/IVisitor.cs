using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IVisitor 
{
    
    void Visit(HeldStatus heldstatus);
}
