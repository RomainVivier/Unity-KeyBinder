using UnityEngine;
using System;
using System.Collections;

public class Keyfunctions
{
    
    public static KeyCode FetchKey()
    {
        foreach(KeyCode codeValue in Enum.GetValues(typeof(KeyCode)))
        {
            if(Input.GetKeyDown(codeValue))
                return codeValue;
        }
        
        return KeyCode.None;
    }

    public static string FetchAxis()
    {
        foreach(string axis in KeyBindRefs.Axes)
        {
            if(Input.GetAxis(axis) != 0)
                return axis;
        }
        
        return null;
    }
}
