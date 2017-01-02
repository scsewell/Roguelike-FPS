using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;

public static class Utils
{
    public static T PickRandom<T>(T[] array)
    {
        return array[UnityEngine.Random.Range(0, array.Length)];
    }
}