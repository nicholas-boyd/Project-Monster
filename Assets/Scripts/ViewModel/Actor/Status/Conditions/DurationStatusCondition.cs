using UnityEngine;
using System.Collections;
public class DurationStatusCondition : StatusCondition {
    public float duration = 10;

    void Update ()
    {
        duration -= Time.deltaTime;
        if (duration < 0)
            Remove();
    }
}