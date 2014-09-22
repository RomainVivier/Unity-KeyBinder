using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public static class KeyBindRefs
{

    public static Dictionary<KeyConfig, List<string>> DefaultBindsRefs
    {
        get
        {
            var temp = new Dictionary<KeyConfig, List<string>>();
            temp.Add(new KeyConfig(KeyCode.A), new List<string>{"AButton"});
			temp.Add(new KeyConfig("Vertical"), new List<string>{"VerticalTest"});
			temp.Add(new KeyConfig(KeyCode.Tab), new List<string>{"KeyBindGUI"});

            return temp;
        }
    }

    public static Dictionary<string, ActionConfig> DefaultBinds
    {
        get
        {
            var temp = new Dictionary<string, ActionConfig>();
            temp.Add("AButton", new KeyActionConfig(KeyType.Action, 0, null, null));
			temp.Add("VerticalTest", new AxisActionConfig(KeyType.Action, 0, null));
			temp.Add("KeyBindGUI", new KeyActionConfig(KeyType.Menu, 0, null, null));

            return temp;
        }
    }

    public static List<string> Axes
    {
        get
        {
            return new List<string>
            {   
                "Horizontal",
                "Vertical",
                "Horizontal_1",
                "Vertical_1",
                "Horizontal_2",
                "Vertical_2",
                "TriggerLeft",
                "TriggerRight"
            };
        }
    }

	public static string ChangingKey = "";
	public static KeyConfig LastInput;
	public static bool isStop = false;

}
