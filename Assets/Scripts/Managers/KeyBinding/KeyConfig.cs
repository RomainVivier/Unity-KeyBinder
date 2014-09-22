using UnityEngine;
using System.Collections;

public struct KeyConfig
{
	private readonly bool _isAxis;
	public bool IsAxis
	{ 
		get
		{
			return _isAxis;
		}
	}
	
	private readonly KeyCode _keyName;
	public KeyCode KeyName { 
		get
		{
			return _keyName;
		}
	}
	
	private readonly string _axisName;
	
	public string  AxisName 
	{
		get
		{
			return _axisName;
		}
	}
	
	public KeyConfig( string input)
	{
		_isAxis = true;
		_axisName = input;
		_keyName = KeyCode.None;
	}
	
	public KeyConfig( KeyCode keycode) : this()
	{
		_isAxis = false;
		_keyName = keycode;
		// _axisName = null;
	}
	
	
	public bool IsActive
	{
		get
		{
			return (!IsAxis && ( Input.GetKeyDown(KeyName) || Input.GetKeyUp(KeyName) ) ) || (IsAxis && Input.GetAxis(AxisName) != 0f );
		}
	}
	
	public string Name
	{
		get
		{
			return IsAxis ? AxisName : KeyName.ToString();
		}
	}
	
	private const string AXIS = "Axis:";
	private const string KEY="Key:";
	
	public override bool Equals(object obj)
	{
		KeyConfig kc = (KeyConfig)obj;
		
		if(kc.AxisName == null && kc.KeyName == KeyCode.None)
			return false;
		
		return (IsAxis == kc.IsAxis && KeyName == kc.KeyName && AxisName == kc.AxisName);
	}
	
	public override int GetHashCode()
	{
		var hash = IsAxis.GetHashCode();
		hash = (hash * 7) + KeyName.GetHashCode();
		hash = AxisName != null ? (hash * 7) + AxisName.GetHashCode() : (hash * 7);
		
		return hash;
	}
	
	public override string ToString()
	{
		return (IsAxis? AXIS : KEY)+this.Name;
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
