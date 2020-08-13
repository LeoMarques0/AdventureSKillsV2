using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenerateID
{
    public static string NumberOnly(int digits)
    {
        string id = "";
        for(int x = 0; x < digits; x++)
        {
            id = id + Random.Range(0, 10);
        }
        Debug.Log(id);
        return id;
    }
}
