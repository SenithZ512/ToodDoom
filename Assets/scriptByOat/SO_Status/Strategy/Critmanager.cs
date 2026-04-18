using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Critmanager : MonoBehaviour
{
    public static bool PlayerCritActive { get; private set; }

    private static Coroutine _cancelRoutine;
    private static MonoBehaviour _runner;

    public static void Init(MonoBehaviour runner) => _runner = runner;

    public static void ActivatePlayerCrit(float duration)
    {
        if (_runner == null)
        {
          
            PlayerCritActive = true;
            return;
        }

        if (_cancelRoutine != null)
            _runner.StopCoroutine(_cancelRoutine);

        PlayerCritActive = true;
        _cancelRoutine = _runner.StartCoroutine(DeactivateAfter(duration));
    }

    private static IEnumerator DeactivateAfter(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        PlayerCritActive = false;
        _cancelRoutine = null;
    }
}
