using System.Collections.Generic;
using System.Collections.ObjectModel;

public static class ListExtensions
{
    public static int FindMaxCount<T>(this List<List<T>> nestList)
    {
        if (nestList == null || nestList.Count == 0) return 0;

        int maxCount = 0;

        for (int i = 0; i < nestList.Count; i++)
        {
            if (nestList[i] == null) continue;

            if (nestList[i].Count > maxCount)
            {
                maxCount = nestList[i].Count;
            }
        }

        return maxCount;
    }

    public static int FindMinCount<T>(this List<List<T>> nestList)
    {
        if (nestList == null || nestList.Count == 0) return 0;

        int minCount = int.MaxValue;
        bool isFound = false;

        for (int i = 0; i < nestList.Count; i++)
        {
            if (nestList[i] == null) continue;

            if (nestList[i].Count < minCount)
            {
                minCount = nestList[i].Count;
                isFound = true;
            }
        }

        return isFound ? minCount : 0;
    }

    public static int FindMaxCount<T>(this ReadOnlyCollection<ReadOnlyCollection<T>> nestList)
    {
        if (nestList == null || nestList.Count == 0) return 0;

        int maxCount = 0;

        for (int i = 0; i < nestList.Count; i++)
        {
            if (nestList[i] == null) continue;

            if (nestList[i].Count > maxCount)
            {
                maxCount = nestList[i].Count;
            }
        }

        return maxCount;
    }

    public static int FindMinCount<T>(this ReadOnlyCollection<ReadOnlyCollection<T>> nestList)
    {
        if (nestList == null || nestList.Count == 0) return 0;

        int minCount = int.MaxValue;
        bool isFound = false;

        for (int i = 0; i < nestList.Count; i++)
        {
            if (nestList[i] == null) continue;

            if (nestList[i].Count < minCount)
            {
                minCount = nestList[i].Count;
                isFound = true;
            }
        }

        return isFound ? minCount : 0;
    }
}
