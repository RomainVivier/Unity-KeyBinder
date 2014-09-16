using UnityEngine;
using System.Collections;

public class Test : MonoBehaviour
{

	void Start()
	{
		KeyBinder.Instance.DefineActions("AButton", new KeyActionConfig(KeyType.Action, 0,
        	() => {
				Debug.Log("A button click down");
			},
			() => {
				Debug.Log("A button click up");
			}));

		KeyBinder.Instance.DefineActions("VerticalTest", new AxisActionConfig(KeyType.Action, 0, 
			value =>{
				Debug.Log("Vertical value = " + value);
			} ));
	}
}
