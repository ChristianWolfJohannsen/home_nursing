using UnityEngine;
using System.Collections.Generic;

public class Command : PackUnpackable{
	private static char[] OPERATORS=new char[]{'-','+','/','*'};

	public string objectName;
	public string method;
	public string[] paramaters;
	public bool assignment;
	
	public Command(){
	}
	
	public Command(string commandString){
		// Build functions from string

		// Command or assignment
		if (commandString.IndexOf("(")!=-1){
			string c=commandString.Substring(0,commandString.IndexOf("("));
			string p=commandString.Substring(commandString.IndexOf("(")+1);
			if (p.LastIndexOf(")")!=-1){
				p=p.Substring(0,p.LastIndexOf(")"));
			}
			// Split command into parts
			if (c.IndexOf(".")!=-1){
				objectName=c.Substring(0,c.IndexOf("."));
				method=c.Substring(c.IndexOf(".")+1);
			}else{
				objectName="player";
				method=c;
			}
			
			// Split params
			paramaters=SplitParams(p);
		}else if (commandString.IndexOf("=")!=-1){
			string c=commandString.Substring(0,commandString.IndexOf("="));
			string p=commandString.Substring(commandString.IndexOf("=")+1);
			// Split command into parts
			if (c.IndexOf(".")!=-1){
				objectName=c.Substring(0,c.IndexOf("."));
				method=c.Substring(c.IndexOf(".")+1);
			}else{
				objectName="player";
				method=c;
			}

			// Split params
			paramaters=SplitParams(p);
		}else{
			Debug.LogError("Can not parse command '"+commandString+"'");
		}
	}
		
	private string[] SplitParams(string p){
		List<string> list=new List<string>();
		
		p=p.Trim();
		while (p.Length>0){
			// String?
			if (p.StartsWith("\"")){
				int pos=p.IndexOf("\"");
				
				if (pos!=-1){
					list.Add(p.Substring(1,pos-1));
					p=p.Substring(pos);
				}else{
					Debug.LogError("Error passing params for command/assignment ("+p+") string open found but not closed");
					break;
				}
			}else{
				int pos=p.IndexOfAny(OPERATORS);
				if (pos==-1){
					list.Add(p);
					p="";
				}else{
					list.Add(p.Substring(0,pos));
					list.Add(p.Substring(pos,pos));
					p=p.Substring(pos+1);
				}
			}
		}
		return list.ToArray();
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
		return CalculateParam(Parley.GetInstance().GetParleyEnviromentInfo());
	}
	
	public object CalculateParam(ParleyEnviromentInfo info){
		if (paramaters==null || paramaters.Length==0){
			return null;
		}
		
		if (paramaters.Length==1 && paramaters[0].StartsWith("\"")){
			return paramaters[0].Substring(1,paramaters[0].Length-2);
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
					object o=info.GetEnviromentInfo(p);
					if (o==null){
						Debug.LogWarning("Parley could not find the term <"+term+"> from the Enviroment. Substituting 0");
						v=0;
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
	
	public void Pack(StreamPacker sp){
		sp.WriteString(objectName);
		sp.WriteString(method);
		sp.WriteStrings(paramaters);
		sp.WriteBool(assignment);
	}
	
	public void Unpack(StreamUnpacker sp){
		objectName=sp.ReadString();
		method=sp.ReadString();
		paramaters=sp.ReadStrings();
		assignment=sp.ReadBool();
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
