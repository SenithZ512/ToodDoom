using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UpdateBuffCountdown : MonoBehaviour
{
   
    public TextMeshProUGUI Coundown_txt;
    private float count;
    private void Start()
    {
        Coundown_txt.text = "";
    }
    public void Callcoundwotn(float duration)
    {

        StopAllCoroutines();
        StartCoroutine(startCOundown(duration));
    }
    private void OnEnable()
    {
        GameEvent.CriticalActive += Callcoundwotn;
    }
    private void OnDisable()
    {
        GameEvent.CriticalActive -= Callcoundwotn;
    }

    private IEnumerator startCOundown(float Duration)
    {
        count = Duration;
        while (count > 0)
        {
           
            Coundown_txt.text = count.ToString("F1");

          
            count -= Time.deltaTime;

            yield return null;
        }

        Coundown_txt.text = "0.0";
        yield return new WaitForSeconds(2f);
        Coundown_txt.text = "";

    }
}
