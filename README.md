# What does this do ?

This is a simple and easy to use keybinding system for Unity3D. It allows you to centralize the functions related to different possible inputs so that you can avoid verifying them each frame and hardcode everything. Furthermore, as it decouple the actual keybinding from the game action, it makes it easy to do automation, Keybinding GUIs, debugs, etc.

It also lets you save and load player preferences from the Unity player prefs and change the bind inputs/functions through a GUI sorted by KeyType (Attack, Movement, Dodge, Menus, etc.). Feel free to redefine the keytype depending on your need.

# How do I use it ?

## Architecture :

The keybinder use to main concepts: Actions and Bindings. Actions are defined by the developer, Bindings are filled by the player (But the developer would be well inspired to provide default bindings.)

  * Actions, as you may suppose, are actions in the game (ie firing, turning left, strafing right, etc...). There are two kind of actions: *KeyActionConfig* that can be binded to keys and *AxisActionConfig* that can be binded to axis controls (joystick axis, triggers, etc..)
  * Bindings are defined through the *KeyConfig* class, and map a key (or an axis) to a developer defined action.

1. KeyConfig

   * bool isAxis (precise if it is a button or an axis like mouse wheel, gamepad joystick, etc.)
   * KeyCode KeyName (for buttons)
   * string  AxisName (for axes, must be define in Unity inputs)


* KeyActionConfig (extend ActionConfig)
   * KeyType Category (use to sort action in GUI)
   * int Order (use to sort action in a same category)
   * Action KeyDownAction (action triggered to on button down event)
   * Action KeyUpAction (action triggered to on button up event)

* AxisActionConfig (extend ActionConfig)
   * KeyType Category (use to sort action in GUI)
   * int Order (use to sort action in a same category)
   * Action<float> AxisAction (action passed with pression float value)


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

## Getting a Keybinder instance
It' (almost) magic. You don't need to create a game object for that, or anything else. Just add the source and get the Keybinder through the Instance property.

```
KeyBinder.Instance
```

## Defining actions
```
public void DefineActions(string keyId, ActionConfig config)
```
Use the define action method with the required ActionConfig to add an action to the keybinder.

## Defining Bindings

Use the following 

Setting KeyConfig for a key :
```
public void SetKeyBind(KeyCode input, string keyId)
```
For an axis : 
```
public void SetKeyBind(string input, string keyId)
```
##Stopping the keybinder from calling actions on keypress.
```
ApplyAction = false
```
To restart the keybinder, just set the ApplyAction property to true.

###Important!
Unity does not provide ways to get a list of available axis controls for your game. The keybinder use the method “FetchAxis()” in class "KeyFunctions" (KeyBindRefs.cs) to know the list of Axis to check when you try changing a bind with the GUI. Change the list of string to match the axis you defined in the Unity input system (Edit>Project Settings>Input ) 

#Examples
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
