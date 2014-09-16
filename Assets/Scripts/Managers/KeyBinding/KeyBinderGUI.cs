using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.Linq;

public class KeyBinderGUI : MonoBehaviour
{    
    public int tabOffset = 20;
    public float buttonWidth = 0.2f;
	public float panelLeft = 0.1f;
	public float panelTop = 0.1f;
	public float panelWidth = 0.8f;
	public float panelHeight = 0.8f;

    private bool _displayGUI = false;

    private string _changingKey = "";
    private KeyConfig _lastInput;
    private Vector2 _scrollPosition = Vector2.zero;


	
    void Start()
	{
		KeyBinder.Instance.DefineActions("Display", new KeyActionConfig(
            KeyType.Menu, 0,
            () => 
            {
		        Debug.Log(JsonConvert.SerializeObject(KeyBinder.Instance.KeyBinds, Formatting.Indented));
		    }
            , null));

		KeyBinder.Instance.SetKeyBind(KeyCode.E, "Display");
        KeyBinder.Instance.SetKeyBind(KeyCode.R, "Save");

	}



    void Update()
    {
        KeyCode keycode = Keyfunctions.FetchKey();
        string axisName = Keyfunctions.FetchAxis();


        if(_changingKey != "" && ( (_lastInput.isAxis && axisName != null) || (!_lastInput.isAxis && keycode != KeyCode.None) ) )
        {
            KeyBinder.Instance.ChangeBind(_lastInput, _changingKey, _lastInput.isAxis ? new KeyConfig(axisName) : new KeyConfig(keycode) );
            KeyBinder.Instance.SaveInputsPrefs();
            _changingKey = "";
            _lastInput = new KeyConfig(null);

        }

        if(keycode == KeyCode.Tab)
            DisplayGUI();
    }

    void OnGUI()
    {
        if(!_displayGUI)
            return;

        int vertOffset = 0;

        GUILayout.BeginArea(new Rect(Screen.width * panelLeft, Screen.height * panelTop, Screen.width * panelWidth, Screen.height * panelHeight));
        _scrollPosition = GUILayout.BeginScrollView(_scrollPosition);
        GUILayout.BeginVertical();

        foreach(KeyType value in Enum.GetValues(typeof(KeyType)) )
        {
			var tempBinds = (from bind in KeyBinder.Instance.ActionBinds
			                 where bind.Value.Category == value
			                 orderby bind.Value.Order
			                 select bind.Key).ToList();

            if(value == KeyType.Server || value == KeyType.Menu || tempBinds.Count == 0)
                continue;

            GUILayout.Label(Enum.GetName(typeof(KeyType), value));
            vertOffset += 30;

            foreach(string keybind in tempBinds)
            {
                var tempInputs = (from input in KeyBinder.Instance.KeyBinds
                                  where input.Value.Contains(keybind)
                                  select input.Key).ToList();

                GUILayout.BeginHorizontal();
                GUILayout.Space(tabOffset);
                GUILayout.Label(keybind);
                foreach(KeyConfig input in tempInputs)
                {
                    GUILayout.Space(tabOffset);
                    if(keybind == _changingKey)
                        GUILayout.Button("Press any input...", GUILayout.Width(Screen.width * buttonWidth));
                    else
                    {
                        if(GUILayout.Button(input.Name, GUILayout.Width(Screen.width * buttonWidth) ) )
                        {
                            _changingKey = keybind;
                            _lastInput = input;
                            KeyBinder.Instance.SaveInputsPrefs();
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
        GUILayout.EndHorizontal();

        GUILayout.EndVertical();
        GUILayout.EndScrollView();
        GUILayout.EndArea();

    }

    public void DisplayGUI()
    {
        _displayGUI = !_displayGUI;
        KeyBinder.Instance.ApplyAction = !KeyBinder.Instance.ApplyAction;
    }

}
 