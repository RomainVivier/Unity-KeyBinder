# What does it provide to you ?

it allows you to centralize the functions related to different possible inputs and not to verify them each frame, create automation, controls, debugs, etc.

It also lets you save and load your preferences and change the bind inputs/functions through a GUI sorted by KeyType (Attack, Movement, Dodge, Menus, etc.) free to you to redefine the keytype depending on your need.

# How to use it ?

## Architecture :

The KeyBinder is based on two classes :

1. KeyConfig define by the following members
⋅⋅* bool isAxis (precise if it is a button or an axis like mouse wheel, gamepad joystick, etc.)
⋅⋅* KeyCode KeyName (for buttons)
⋅⋅* string  AxisName (for axes, must be define in Unity inputs)

2. ActionConfig (abstract)
⋅⋅* KeyType Category (use to sort action in GUI)
⋅⋅* int Order (use to sort action in a same category)

* KeyActionConfig (extend ActionConfig)
⋅⋅* Action KeyDownAction (action triggered to on button down event)
⋅⋅* Action KeyUpAction (action triggered to on button up event)

* AxisActionConfig (extend ActionConfig)
⋅⋅* Action<float> AxisAction (action passed with pression float value)


The KeyBinder use those classes in its two dictionaries :

The first one works with pair of KeyConfig and a List of strings.
```
Dictionary< KeyConfig, List<string> >
```
Each string of the list correspond to an unique id representating an ActionConfig 

The second one with string and ActionConfig.
```
Dictionary< string, ActionConfig >
```
The strings here are the unique ids respresenting the ActionConfigs as precised just before.

## Setting the binds :

There is only two functions to set the binds :

#### Keyconfig

Setting KeyConfig for button :
```
public void SetKeyBind(KeyCode input, string keyId)
```
For axis : 
```
public void SetKeyBind(string input, string keyId)
```

#### ActionConfig
```
public void DefineActions(string keyId, ActionConfig config)
```


#### Important fact :

I use function “FetchAxis()” in class KeyFunctions to detect if an Axis is pressed when you try changing a bind with the GUI. This function work with list of string corresponding to the name of the Unity inputs you must define.

KeyBinder is a singleton. That mean you don’t need to create a gameobject with an instance of it. You just need to call this member instance to use it like this :

```
KeyBinder.Instance
```

Here is two example on how to register a key bind :

For a key :

```
KeyBinder.Instance.DefineActions("AButton", new KeyActionConfig(KeyType.Action, 0,
                     () => { Debug.Log("A button clicked down!"); }, 
                     () => { Debug.Log("A button clicked up!"); } ));
KeyBinder.Instance.SetKeyBind(KeyCode.A, "AButton");
```

For an Axis :

```
KeyBinder.Instance.DefineActions("VerticalTest", new AxisActionConfig(KeyType.Action, 0, 
                     value => { Debug.Log("Axis value = " + value); }));
KeyBinder.Instance.SetKeyBind(“Vertical”, "VerticalTest");
```

So you just have to use those functions to register key bind but those keybind will only register when your script Start.

This way your keybind won’t exist at the beginning of your game and your players won’t be able to change the binds.

To avoid this problem i created two members in a static class (KeyBindRefs.cs) corresponding to our two dictionnaries. Those members will be call the first time you call the KeyBinder Instance.

So finally what you have to do is to fill those members like this :

```
public static Dictionary<KeyConfig, List<string>> DefaultBindsRefs
{
  get
  {
    var temp = new Dictionary<KeyConfig, List<string>>();
    temp.Add(new KeyConfig(KeyCode.A), new List<string>{"AButton"});
    temp.Add(new KeyConfig("Vertical"), new List<string>{"VerticalTest"});

    return temp;
  }
}

public static Dictionary<string, ActionConfig> DefaultBinds
{
  get
  {
    var temp = new Dictionary<string, ActionConfig>();
    temp.Add("AButton", new KeyActionConfig(KeyType.Action, 0, null,null));
    temp.Add("VerticalTest", new AxisActionConfig(KeyType.Action, 0, null));

            
    return temp;
  }
}
```

Then you just have to call DefineAction to set the corresponding action :

```
void DefineActions(string keyId, ActionConfig config)
```

This way you will have your binds define from the start with empty actions, but the players will be able to setis own bind.

Last details to use it. Axis can only be change for another axis and same goes for the keies.

I hope it might help you, if you encounter problems don’t hesitate to report them to me.
