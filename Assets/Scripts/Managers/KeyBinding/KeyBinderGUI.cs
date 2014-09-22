using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.Linq;

public class KeyBinderGUI : MonoBehaviour
{    
	public int tabOffset = 20;
	public float buttonWidth = 0.1f;
    public float panelLeft = 0.1f;
    public float panelTop = 0.1f;
    public float panelWidth = 0.8f;
    public float panelHeight = 0.8f;
	
	private Vector2 _scrollPosition = Vector2.zero;
	
	private class KeyBindsGroup
	{
		public string Name{get;set;}
		public List<KeyBindInfos> KeyBindings {get;set;}
		
	}
	private class KeyBindInfos
	{
		public string Name{get;set;}
		public List<KeyConfig> Key{get;set;}
	}
	
	
	void Start()
	{
		panelLeft = 0.1f;
		panelTop = 0.1f;
		panelWidth = 0.8f;
		panelHeight = 0.8f;
		
		KeyBinder.Instance.DefineActions("KeyBindGUI", new KeyActionConfig(KeyType.Menu, 0,
			() => 
			{
				enabled = true;
				KeyBinder.Instance.ApplyAction = false;
			},
			null));
		
		
	}

	private void OnBindingsChanged(object sender, EventArgs args)
	{
		this.ClearGuiInfosCache();
	}
	private List<KeyBindsGroup> _guiInfos;
	
	private List<KeyBindsGroup> GetGuiInfos()
	{
		if(_guiInfos ==null)
		{
			
			
			_guiInfos = (from bind in KeyBinder.Instance.ActionBinds
			             where bind.Value.Category != KeyType.Menu
			             group bind.Key by bind.Value.Category into g
			             select new KeyBindsGroup()
			             { 
				Name=g.Key.ToString(), 
				KeyBindings = (from action in g select new KeyBindInfos
				               { 
					Name = action,
					Key = (from input in KeyBinder.Instance.KeyBinds where input.Value.Contains(action) select input.Key).ToList()
				}).ToList()
			} 
			).ToList();
		}
		return _guiInfos;
	}
	
	private void ClearGuiInfosCache()
	{
		_guiInfos = null;
	}
	
	public void OnGUI()
	{
		var guiInfos = GetGuiInfos();
		
		int vertOffset = 0;
		
		GUILayout.BeginArea(new Rect(Screen.width * panelLeft, Screen.height * panelTop, Screen.width * panelWidth, Screen.height * panelHeight));
		_scrollPosition = GUILayout.BeginScrollView(_scrollPosition);
		GUILayout.BeginVertical();
		
		foreach(var group in guiInfos)
		{
			GUILayout.Label(group.Name);
			vertOffset += 30;
			foreach(var bindingInfos in group.KeyBindings)
			{
				GUILayout.BeginHorizontal();
				GUILayout.Space(tabOffset);
				GUILayout.Label(bindingInfos.Name);
				foreach(var input in bindingInfos.Key)
				{
					GUILayout.Space(tabOffset);
					if(bindingInfos.Name == KeyBindRefs.ChangingKey)
						GUILayout.Button("Press any input...", GUILayout.Width(Screen.width * buttonWidth));
					else
					{
						if(GUILayout.Button(input.Name, GUILayout.Width(Screen.width * buttonWidth) ) )
						{
							KeyBinder.Instance.KeyChangeTimer.Reset(0.2f);
							KeyBindRefs.ChangingKey = bindingInfos.Name;
							KeyBindRefs.LastInput = input;
						}
					}
				}
				GUILayout.EndHorizontal();
				vertOffset += 20;
			}
			vertOffset += 20;
		}
		
		GUILayout.BeginHorizontal();
		if(GUILayout.Button("Default inputs", GUILayout.Width(Screen.width * buttonWidth)))
		{
			PlayerPrefs.SetString("InputsPrefs", "");
			KeyBinder.Instance.RestoreInputsPrefs();
			KeyBinder.Instance.SaveInputsPrefs();
		}
		
		if(GUILayout.Button("Back", GUILayout.Width(Screen.width * buttonWidth)))
		{
			enabled = false;
			KeyBinder.Instance.ApplyAction = true;
		}
		GUILayout.EndHorizontal();
		
		GUILayout.EndVertical();
		GUILayout.EndScrollView();
		GUILayout.EndArea();
		
	}
	
	public void OnShow()
	{
		KeyBinder.Instance.BindingsChanged+=OnBindingsChanged;
	}
	public void OnHide()
	{
		KeyBinder.Instance.BindingsChanged-=OnBindingsChanged;
	}
}
