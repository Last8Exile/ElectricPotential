using System;
using UnityEngine;

public class KeyFrame : MonoBehaviour, IComparable
{

    public float Time;

    public int CompareTo(object obj)
    {
        var other = (float)obj;

        if (other == Time)
            return 0;
        if (other > Time)
            return -1;
        else
            return 1;
    }
}
