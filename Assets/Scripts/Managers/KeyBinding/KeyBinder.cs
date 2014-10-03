using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.Linq;

[AddComponentMenu("Scripts/manager/KeyBinder")]

public class KeyBinder : MonoBehaviour
{

	public static bool HasStopped {get; private set;}
	public bool ApplyAction { get; set; }
	public Timer KeyChangeTimer {get; set;}
	
	public event EventHandler<EventArgs> BindingsChanged;
	
	public static KeyBinder Instance
	{
		get
		{
			if(_instance == null) 
			{
				GameObject singleton = new GameObject();
				_instance = singleton.AddComponent<KeyBinder>();
				singleton.name = "KeyBinder";
				DontDestroyOnLoad(singleton);
				_instance.RestoreInputsPrefs();
				_instance.ActionBinds = KeyBindRefs.DefaultBinds;
				_instance.ApplyAction = true;
				_instance.KeyChangeTimer = new Timer();
			}
			
			return _instance;
		}
	}
	
	private Dictionary< string, ActionConfig > _actionBinds = new Dictionary< string, ActionConfig >();
	private Dictionary< KeyConfig, List<string> > _keyBinds = new Dictionary< KeyConfig, List<string> >();
	private static KeyBinder _instance;
	
	void Update()
	{

		if(KeyChangeTimer.IsElapsedLoop && KeyBindRefs.ChangingKey != "")
		{
			KeyCode keycode = Keyfunctions.FetchKey();
			string axisName = Keyfunctions.FetchAxis();
			
			if( (KeyBindRefs.LastInput.IsAxis && axisName != null) || (!KeyBindRefs.LastInput.IsAxis && keycode != KeyCode.None) )
			{
				ChangeBind(KeyBindRefs.LastInput, KeyBindRefs.ChangingKey, KeyBindRefs.LastInput.IsAxis ? new KeyConfig(axisName) : new KeyConfig(keycode) );
				SaveInputsPrefs();
				KeyBindRefs.ChangingKey = "";
				KeyBindRefs.LastInput = new KeyConfig(null);
			}
		}
		
		if (_keyBinds.Count <= 0 || !ApplyAction)
			return;
		
		foreach (var input in _keyBinds.Where( kb => kb.Key.IsAxis == false && kb.Key.IsActive))
		{
			foreach (string keyId in input.Value.Where(id => _actionBinds.ContainsKey(id)))
			{
				
				KeyActionConfig kac = (KeyActionConfig)_actionBinds [keyId];
				
				//key down action
				if (kac.KeyDownAction != null && Input.GetKeyDown(input.Key.KeyName))
				{
					kac.KeyDownAction();
				}
				
				// key up action
				if (kac.KeyUpAction != null && Input.GetKeyUp(input.Key.KeyName))
				{
					kac.KeyUpAction();
				}
			}
		}
		
		float axisValue = 0f; 
		
		foreach (var input in _keyBinds.Where( kb => kb.Key.IsAxis == true && kb.Key.IsActive && kb.Key.AxisName != null))
		{
			axisValue = Input.GetAxis(input.Key.AxisName);
			
			AxisActionConfig aac;
			foreach (string keyId in input.Value)
			{
				aac =  (AxisActionConfig)_actionBinds [keyId];
				if(aac.AxisAction != null)
				{
					aac.AxisAction(axisValue);
				}
			}
		}
	}
	
	public void DefineActions(string keyId, ActionConfig config)
	{
		if (keyId == null || config == null)
			Debug.LogError("keyId and config can't be null in DefineActions");
		
		if (_actionBinds.ContainsKey(keyId))
			_actionBinds [keyId] = config;
		else
			_actionBinds.Add(keyId, config);
	}
	
	public void SetKeyBind(KeyCode input, string actionId)
	{
		if (input == KeyCode.None || actionId == null)
			Debug.LogError("input and keyId can't be null in SetKeyBind");
		SetKeyBindImpl(actionId, new KeyConfig(input));
	}
	
	public void SetKeyBind(string input, string actionId)
	{
		if (input == null || actionId == null)
			Debug.LogError("input and keyId can't be null in SetKeyBind");
		SetKeyBindImpl(actionId, new KeyConfig(input));
	}
	
	private void SetKeyBindImpl(string keyId, KeyConfig keyconfig)
	{
		
		if (_keyBinds.ContainsKey(keyconfig))
		{
			if (!_keyBinds[keyconfig].Contains(keyId))
				_keyBinds[keyconfig].Add(keyId);
		}
		else
			_keyBinds.Add(keyconfig, new List<string>() {
				keyId
			});
	}
	
	//    public void SetKeyDownBind(string keyId, Action act)
	//    {
	//        if (keyId == null)
	//            Debug.LogError("keyId can't be null in SetKeyDownBind");
	//
	//        if(_ActionBinds [keyId].GetType() == typeof(KeyActionConfig))
	//        {
	//            KeyActionConfig kac = (KeyActionConfig)_ActionBinds [keyId];
	//            kac.KeyDownAction = act;
	//        }
	//    }
	//
	//    public void SetKeyUpBind(string keyId, Action act)
	//    {
	//        if (keyId == null)
	//            Debug.LogError("keyId can't be null in SetKeyUpBind");
	//
	//        if(_ActionBinds [keyId].GetType() == typeof(KeyActionConfig))
	//        {
	//            KeyActionConfig kac = (KeyActionConfig)_ActionBinds [keyId];
	//            kac.KeyUpAction = act;
	//        }
	//    }
	
	public void ChangeBind(KeyConfig currentInput, string keyId, KeyConfig newInput)
	{
		if ( keyId == null )
			Debug.LogError("keyId can't be null in ChangeBind");
		
		_keyBinds [currentInput].Remove(keyId);
		
		if (_keyBinds.ContainsKey(newInput))
			_keyBinds [newInput].Add(keyId);
		else
			_keyBinds.Add(newInput, new List<string>(){keyId});
		
		if (_keyBinds [currentInput].Count <= 0)
			_keyBinds.Remove(currentInput);
		
		var d = this.BindingsChanged;
		if(d!=null)
		{
			d(this,EventArgs.Empty);
		}
	}
	
	public void RemoveBind(KeyConfig input, string keyId)
	{
		if (keyId == null || !_keyBinds.ContainsKey(input))
			Debug.LogError("keyId and input can't be null in RemoveBind");
		
		_keyBinds [input].Remove(keyId);
	}
	
	public void RemoveAction(string keyId)
	{
		if (keyId == null)
			Debug.LogError("keyId can't be null in RemoveAction");
		
		_actionBinds.Remove(keyId);
	}

	public void EmptyAction(string actionId)
	{
		if (actionId == null)
			Debug.LogError("actionId can't be null in EmptyAction");
		
		var actionConf = _actionBinds[actionId];
		
		if(actionConf == null)
			return;
		
		_actionBinds.Remove(actionId);
		
		if(actionConf.GetType() == typeof(KeyActionConfig))
		{
			DefineActions(actionId, new KeyActionConfig(actionConf.Category, actionConf.Order, null, null));
		}else{
			DefineActions(actionId, new AxisActionConfig(actionConf.Category, actionConf.Order, null));
		}
	}

	//    public void RemoveDownBind(string keyId)
	//    {
	//        if (keyId == null)
	//            Debug.LogError("keyId can't be null in RemoveDownBind");
	//
	//        _ActionBinds [keyId].KeyDownAction = (Action)null;
	//    }
	//    
	//    public void RemoveUpBind(string keyId)
	//    {
	//        if (keyId == null)
	//            Debug.LogError("keyId can't be null in RemoveUpBind");
	//
	//        _ActionBinds [keyId].KeyUpAction = (Action)null;
	//    }
	
	public void ClearBinds()
	{ 
		_keyBinds.Clear();
		_actionBinds.Clear();
	}
	
	public Dictionary< KeyConfig, List<string> > KeyBinds
	{
		get{ return _keyBinds;}
		set{ _keyBinds = value;}
	}
	
	public Dictionary< string, ActionConfig > ActionBinds
	{
		get{ return _actionBinds;}
		set{ _actionBinds = value;}
	}
	
	public void RestoreInputsPrefs()
	{
		Dictionary<KeyConfig, List<string>> keyBindsRefsSave = new Dictionary<KeyConfig, List<string>>();
		
		if(PlayerPrefs.GetString("InputsPrefs") != "")
		{
			var jsonBindsRefs = JsonConvert.DeserializeObject<Dictionary<string, List<string>>>(PlayerPrefs.GetString("InputsPrefs"));
			
			try{
				foreach(var kb in jsonBindsRefs)
					keyBindsRefsSave.Add(KeyConfig.FromString(kb.Key), kb.Value);
			}catch{
				PlayerPrefs.SetString("InputsPrefs", "");
				RestoreInputsPrefs();
			}
			
		}else
			keyBindsRefsSave = KeyBindRefs.DefaultBindsRefs;
		
		_keyBinds = keyBindsRefsSave;
		
		var d = this.BindingsChanged;
		if(d!=null)
		{
			d(this,EventArgs.Empty);
		}
		
	}
	
	public void SaveInputsPrefs()
	{
		Dictionary<string, List<string>> jsonBinds = new Dictionary<string, List<string>>();
		
		foreach(var kb in _keyBinds)
		{
			jsonBinds.Add(kb.Key.ToString(), kb.Value);
		}
		
		PlayerPrefs.SetString("InputsPrefs", JsonConvert.SerializeObject(jsonBinds, Formatting.Indented));
	}
	
	void OnApplicationQuit()
	{
		KeyBinder.HasStopped = true;
	}
	
}

public enum KeyType
{
	Action,
	Movement,
	Menu,
	InGameMenu,
	Server
}
