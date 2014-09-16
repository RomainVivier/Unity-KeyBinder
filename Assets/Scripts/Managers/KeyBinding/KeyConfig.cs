using UnityEngine;
using System.Collections;

public struct KeyConfig
{
    public bool isAxis { get; set; }
    public KeyCode KeyName { get; set; }
    public string  AxisName { get; set; }
    
    public KeyConfig( string input)
    {

        isAxis = true;
        AxisName = input;
        this.KeyName = KeyCode.None;
    }

    public KeyConfig( KeyCode keycode)
    {
        isAxis = false;
        KeyName = keycode;
        AxisName = null;

    }


    public bool IsPressed
    {
        get
        {
            return (!isAxis && ( Input.GetKeyDown(KeyName) || Input.GetKeyUp(KeyName) ) ) || (isAxis && Input.GetAxis(AxisName) != 0f );
        }
    }

    public string Name
    {
        get
        {
            return isAxis ? AxisName : KeyName.ToString();
        }
    }

    private const string AXIS = "Axis:";
    private const string KEY="Key:";

    public override bool Equals(object obj)
    {
        KeyConfig kc = (KeyConfig)obj;

        if(kc.AxisName == null && kc.KeyName == KeyCode.None)
            return false;

        return (isAxis == kc.isAxis && KeyName == kc.KeyName && AxisName == kc.AxisName);
    }

    public override int GetHashCode()
    {
        int hash = 13;
        hash = (hash * 7) + isAxis.GetHashCode();
        hash = (hash * 7) + KeyName.GetHashCode();
        hash = AxisName != null ? (hash * 7) + AxisName.GetHashCode() : (hash * 7);

        return hash;
    }

    public override string ToString()
    {
        return (isAxis? AXIS : KEY)+this.Name;
    }

    public static KeyConfig FromString(string config)
    {
        var isAxis = config.StartsWith(AXIS);
        if(isAxis)
        {
            string axis = config.Substring(AXIS.Length);
            return new KeyConfig(axis);
        }
        else
        {
            KeyCode key = (KeyCode)System.Enum.Parse(typeof(KeyCode),config.Substring(KEY.Length));
            return new KeyConfig(key);
        }
    }
}
