using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.Linq;

[AddComponentMenu("Scripts/manager/KeyBinder")]

public class KeyBinder : MonoBehaviour
{

    public bool ApplyAction { get; set; }

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
                _instance.ActionBinds = KeyBindRef.DefaultBinds;
                _instance.ApplyAction = true;
            }
            
            return _instance;
        }
    }

    private Dictionary< string, ActionConfig > _actionBinds = new Dictionary< string, ActionConfig >();
    private Dictionary< KeyConfig, List<string> > _keyBinds = new Dictionary< KeyConfig, List<string> >();
    private static KeyBinder _instance;
    
//    private List<IObserver<string>> subs = new List<IObserver<string>>();
//    private IObservable<string> _inputsObservable;
//    public IObservable<string> InputsObservable
//    {
//        get
//        {
//            return _inputsObservable;
//        }
//    }
//
//    void Awake()
//    {
//        _inputsObservable = Observable.Create<string>(
//            subscriber => 
//            {
//            subs.Add(subscriber);
//            return () => { subs.Remove(subscriber);};  
//        });
//    }


//    void Start()
//    {
//        DefineActions("Shoot", new ActionConfig(KeyType.Action, 
//                                                  () => { Debug.Log("Mouse left clicked !"); }, 
//                                              () => { Debug.Log("Mouse left not clicked anymore !"); } ));
//        SetKeyBind(KeyCode.Mouse0, "Shoot");
//        DefineActions("AdditiveShot", new ActionConfig(KeyType.Movement, 
//                                                     () => { Debug.Log("Additive Mouse left clicked !"); }, 
//                                                     () => { Debug.Log("Additive Mouse left not clicked anymore !"); } ));
//        SetKeyBind(KeyCode.Mouse0, "AdditiveShot");
//
//      SetKeyDownBind("Shoot", message => {Debug.Log("Mouse left cliked 2 !");});
//      SetKeyUpBind("AdditiveShot", message => {Debug.Log("Additive Mouse left not clicked anymore 2 !");});
//
//      RemoveBind("Fire1", "AdditiveShot");
//      RemoveDownBind("Shoot");
//      RemoveUpBind("AdditiveShot");
//      ClearBinds();
//
//      ChangeBind("Fire1", "Shoot", "Fire2");
//      ChangeBind("Fire1", "AdditiveShot", "Fire3");
//    }

    void Update()
    {
        if (_keyBinds.Count <= 0 || !ApplyAction)
            return;

        var KeyList = _keyBinds.Where( kb => kb.Key.isAxis == false).ToList();

        foreach (var input in KeyList)
        {
            if (!input.Key.IsPressed)
                continue;

            foreach (string keyId in input.Value)
            {
                if (!_actionBinds.ContainsKey(keyId))
                    continue;

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


        var AxisList = _keyBinds.Where( kb => kb.Key.isAxis == true).ToList();
        float axisValue = 0f; 

        foreach (var input in AxisList)
        {

            if (!input.Key.IsPressed || input.Key.AxisName == null)
                continue;

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

    public void SetKeyBind(KeyCode input, string keyId)
    {
        if (input == KeyCode.None || keyId == null)
            Debug.LogError("input and keyId can't be null in SetKeyBind");

        var keyconfig = new KeyConfig(input);

        if (_keyBinds.ContainsKey(keyconfig))
        {
            if (!_keyBinds [keyconfig].Contains(keyId))
                _keyBinds [keyconfig].Add(keyId);

        } else
            _keyBinds.Add( keyconfig, new List<string>(){keyId});

    }

    public void SetKeyBind(string input, string keyId)
    {
        if (input == null || keyId == null)
            Debug.LogError("input and keyId can't be null in SetKeyBind");

        var keyconfig = new KeyConfig(input);

        if (_keyBinds.ContainsKey(keyconfig))
        {
            if (!_keyBinds [keyconfig].Contains(keyId))
                _keyBinds [keyconfig].Add(keyId);
            
        } else
            _keyBinds.Add( keyconfig, new List<string>(){keyId});
        
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
                Instance.RestoreInputsPrefs();
            }

        }else
            keyBindsRefsSave = KeyBindRef.DefaultBindsRefs;
        
        Instance.KeyBinds = keyBindsRefsSave;
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

}

public enum KeyType
{
    Action,
    Movement,
    Menu,
    InGameMenu,
    Server
}
