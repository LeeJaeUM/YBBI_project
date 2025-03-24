using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Utility
{
    static WaitForEndOfFrame _waitForEndofFrame = new WaitForEndOfFrame();
    public static IEnumerator CoSetActive(GameObject obj, bool active)
    {
        yield return _waitForEndofFrame;
        obj.SetActive(active);
    }

    public static int GetProbability(float[] table)
    {
        float total = 0;

        for (int i = 0; i < table.Length; i++)
        {
            total += table[i];
        }

        float randomPoint = Random.value * total;

        for (int i = 0; i < table.Length; i++)
        {
            if (randomPoint < table[i])
            {
                return i;
            }
            else
            {
                randomPoint -= table[i];
            }
        }

        return table.Length - 1;
    }
}