using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class EnemyBaseState 
{
    public abstract void OnEnterState(EnemyStateManager state);
    public abstract void OnUpdateState(EnemyStateManager state);
    public abstract void OnExitState(EnemyStateManager state);
}
