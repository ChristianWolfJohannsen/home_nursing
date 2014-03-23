using UnityEngine;
using System.Collections;

[System.Serializable]

public class Items 
{
	public string name;
	public float amount;
	public enum Type
	{
		Pills, Injection, None
	}
	public Type type;

	/*
	 _type int values: 1 = Injection, 2 = Pills, any other number = None
	*/
	public Items(string _name, float _amount, int _type)
	{
		name = _name;
		amount = _amount;

		switch (_type) 
		{
		case 1: type = Type.Injection; break;
		case 2: type = Type.Pills; break;
		default: type = Type.None; break;
		}
	}
}
