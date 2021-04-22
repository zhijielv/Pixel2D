using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Framework.Scripts.Constants
{
    public static class RandomHelper
    {
        public static List<T> GetRandomValueFromList<T>(List<T> objects, int num)
        {
            T[] srcObjects = objects.ToArray();
            T[] tmpList = new T[num];
            if (num > objects.Count)
            {
                Debug.LogError($"{num} is greater than {objects}");
                return null;
            }
            
            for (int i = 0; i < num; )
            {
                int tmpIndex = Random.Range(0, objects.Count);
                if(tmpList.Contains(objects[tmpIndex])) continue;
                tmpList[i] = srcObjects[tmpIndex];
                i++;
            }

            return tmpList.ToList();
        }

        public static T GetWidgetRandom<T>(Dictionary<T, int> dictionary)
        {
            int widgetSum = 0;
            foreach (KeyValuePair<T,int> keyValuePair in dictionary)
            {
                widgetSum += keyValuePair.Value;
            }
            T[] tmpDictionary = new T[widgetSum];
            int index = 0;

            foreach (KeyValuePair<T,int> keyValuePair in dictionary)
            {
                for (int i = 0; i < keyValuePair.Value; i++)
                {
                    tmpDictionary[i + index] = keyValuePair.Key;
                }
                index += keyValuePair.Value;
            }

            int randomIndex = Random.Range(0, widgetSum);
            return tmpDictionary[randomIndex];
        }
    }
}