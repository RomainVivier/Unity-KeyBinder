using UnityEngine;
using System;
using System.Collections;

public abstract class ActionConfig
{
    public KeyType Category{ get; set; }
    public int Order{ get; set; }
}

public class KeyActionConfig:ActionConfig
{
	
    public Action KeyDownAction{ get; set; }
	public Action KeyUpAction{ get; set; }

	public KeyActionConfig(KeyType type, int order, Action down, Action up)
	{
		Category = type;
        Order = order;
		KeyDownAction = down;
		KeyUpAction = up;
	}

}

public class AxisActionConfig:ActionConfig
{
    public Action<float> AxisAction { get; set; }

    public AxisActionConfig(KeyType type, int order, Action<float> action)
    {
        Category = type;
        Order = order;
        AxisAction = action;
    }
}
