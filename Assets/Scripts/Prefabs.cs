using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Prefabs : SingleInstance<Prefabs>
{
    public List<GameObject> Items;

    public static GameObject Get(string name)
    {
        GameObject result = null;

        try
        {
            result = Instance.Items.FirstOrDefault(i => i.name == name);
        }
        catch (Exception err)
        {
            var a = 1;
        }

        if (result == null)
        {
            throw new System.Exception($"Couldn't find prefab with name {name}");
        }

        return result;
    }

    public static T Get<T>(string name)
    {
        var result = Instance.Items.FirstOrDefault(i => i.name == name);

        if (result == null)
        {
            throw new System.Exception($"Couldn't find prefab with name {name}");
        }

        var result2 = result.GetComponent<T>();

        if (result2 == null)
        {
            throw new System.Exception($"Couldn't find prefab with name {name} and with that component.");
        }

        return result2;
    }
}