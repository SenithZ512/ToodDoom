using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static  class GameEvent
{
    public static Action UpdatePLayerStatus;
    public static Action UpdateAmmo;
    public static Action<float> CriticalActive;
}

