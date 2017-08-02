using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Framework
{
    public static class Utils
    {
        public static T PickRandom<T>(T[] array)
        {
            return array[Random.Range(0, array.Length)];
        }

        public static T PickRandom<T>(List<T> list)
        {
            return list[Random.Range(0, list.Count)];
        }

        public static T PickRandom<T>(IEnumerable<T> enumerable)
        {
            return enumerable.ElementAtOrDefault(Random.Range(0, enumerable.Count()));
        }
    }
}