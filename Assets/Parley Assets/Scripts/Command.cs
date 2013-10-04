using UnityEngine;
using System.Collections.Generic;

public class Command : MonoBehaviour{
	public string objectName;
	public string method;
	public string[] paramaters;
	public bool assignment;
	
	public Command(){
	}
	
	public string GetFullParam(){
		string s="";
		if (paramaters!=null){
			foreach (string p in paramaters){
				s=s+p;
			}
		}
		return s;
	}
	
	public object CalculateParam(){
		if (paramaters==null || paramaters.Length==0){
			return null;
		}
		
		if (paramaters.Length==1 && paramaters[0].StartsWith("\"")){
			return paramaters[0].Substring(1,paramaters.Length-2);
		}
		
		Stack<float> values=new Stack<float>();
		Stack<Operator> operators=new Stack<Operator>();
		
		bool lastWasTerm=false;
		bool nextTermFlip=false;

		foreach (string p in paramaters){
			bool term="+-*/".IndexOf(p)==-1;
			if (term){
				float v=0;
				if ("0123456789".IndexOf(p.Substring(0,1))!=-1){
					v=float.Parse(p);
				}else{
					object o=Parley.GetInstance().GetParleyEnviromentInfo().GetEnviromentInfo(p);
					if (o==null){
						Debug.LogWarning("Parley could not find the term <"+term+"> from the Enviroment");
					}else{
						v=float.Parse(o.ToString());
					}
				}
				if (nextTermFlip){
					values.Push(-v);
				}else{
					values.Push(v);
				}
				nextTermFlip=false;
				lastWasTerm=true;
			}else{
				if (lastWasTerm){
					Operator o=null;
					if ("+".Equals(p)){
						o=new Add();
					} else if ("-".Equals(p)){
						o=new Subtract();
					} else if ("*".Equals(p)){
						o=new Multiply();
					} else if ("/".Equals(p)){
						o=new Divide();
					}
					
					while (operators.Count>0 && o.weight>operators.Peek().weight){
						operators.Pop().Execute(values);
					}
					operators.Push(o);	
				}else{
					// Change sign on next term
					if ("-".Equals(p)){
						nextTermFlip=true;
					}
				}
				lastWasTerm=false;
			}
		}
		while (operators.Count>0){
			operators.Pop().Execute(values);
		}
		return values.Pop();
	}
	
	public abstract class Operator {
		public int weight=0;
		
		public abstract void Execute(Stack<float> values);
	}
	
	public class Add : Operator{
		public Add(){
			weight=50;
		}
		
		override public void Execute(Stack<float> values){
			float v1=values.Pop(),v2=values.Pop();
			values.Push(v1+v2);
		}
	}
	
	public class Subtract : Operator{
		public Subtract(){
			weight=50;
		}
		
		override public void Execute(Stack<float> values){
			float v1=values.Pop(),v2=values.Pop();
			values.Push(v2-v1);
		}
	}
	
	public class Multiply : Operator{
		public Multiply(){
			weight=10;
		}
		
		override public void Execute(Stack<float> values){
			float v1=values.Pop(),v2=values.Pop();
			values.Push(v1*v2);
		}
	}
	
	public class Divide : Operator{
		public Divide(){
			weight=10;
		}
		
		override public void Execute(Stack<float> values){
			float v1=values.Pop(),v2=values.Pop();
			values.Push(v2/v1);
		}
	}
}
