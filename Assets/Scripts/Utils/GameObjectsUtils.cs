using UnityEngine;

static class GameObjectsUtils
{
    public static void ClearChildren(Transform parentObj)
    {
        GameObject[] children = new GameObject[parentObj.childCount];
        for (int i = 0; i < parentObj.childCount; i++)
        {
            children[i] = parentObj.GetChild(i).gameObject;
        }
        foreach (GameObject child in children)
        {
            Object.Destroy(child);
        }
    }
}
