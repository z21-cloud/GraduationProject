using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class IdGenerator
{
    private static int idCounter = 1;
    private static HashSet<int> usedIds = new HashSet<int>();

    public static int GetNextId()
    {
        int newId = idCounter;
        while (usedIds.Contains(newId))
        {
            newId = idCounter++;
        }
        usedIds.Add(newId);
        return newId;
    }

    public static void ReleaseId(int id)
    {
        usedIds.Remove(id);
    }
}
