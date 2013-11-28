using UnityEngine;
using System;
using System.IO;

public class StreamPacker {

	private Stream stream;
	
	public StreamPacker(Stream stream){
		this.stream=stream;
	}
	
	private void Write(byte[] v){
		stream.Write(v,0,v.Length);
	}
	
	public void WriteBool(bool v){
		Write(BitConverter.GetBytes(v));
	}
	
	public void WriteByte(byte v){
		stream.WriteByte(v);
	}
		
	public void WriteShort(short v){
		Write(BitConverter.GetBytes(v));
	}
	
	public void WriteChar(char v){
		Write(BitConverter.GetBytes(v));
	}
	
	public void WriteInt(int v){
		Write(BitConverter.GetBytes(v));
	}
	
	public void WriteLong(long v){
		Write(BitConverter.GetBytes(v));
	}
	
	public void WriteFloat(float v){
		Write(BitConverter.GetBytes(v));
	}
	
	public void WriteDouble(double v){
		Write(BitConverter.GetBytes(v));
	}
	
	public void WriteString(string v){
		if (v==null){
			WriteInt(-1);
		}else{
			WriteChars(v.ToCharArray());
		}
	}

	public void WriteBools(bool[] v){
		if (v==null){
			WriteInt(-1);
		}else{
			WriteInt(v.Length);
			foreach (bool b in v){
				Write(BitConverter.GetBytes(b));
			}
		}
	}
	
	public void WriteBytes(byte[] v){
		if (v==null){
			WriteInt(-1);
		}else{
			WriteInt(v.Length);
			foreach (byte b in v){
				Write(BitConverter.GetBytes(b));
			}
		}
	}
		
	public void WriteShorts(short[] v){
		if (v==null){
			WriteInt(-1);
		}else{
			WriteInt(v.Length);
			foreach (short b in v){
				Write(BitConverter.GetBytes(b));
			}
		}
	}
	
	public void WriteChars(char[] v){
		if (v==null){
			WriteInt(-1);
		}else{
			WriteInt(v.Length);
			foreach (char b in v){
				Write(BitConverter.GetBytes(b));
			}
		}
	}
	
	public void WriteInts(int[] v){
		if (v==null){
			WriteInt(-1);
		}else{
			WriteInt(v.Length);
			foreach (int b in v){
				Write(BitConverter.GetBytes(b));
			}
		}
	}
	
	public void WriteLongs(long[] v){
		if (v==null){
			WriteInt(-1);
		}else{
			WriteInt(v.Length);
			foreach (long b in v){
				Write(BitConverter.GetBytes(b));
			}
		}
	}
	
	public void WriteFloats(float[] v){
		if (v==null){
			WriteInt(-1);
		}else{
			WriteInt(v.Length);
			foreach (float b in v){
				Write(BitConverter.GetBytes(b));
			}
		}
	}
	
	public void WriteDoubles(double[] v){
		if (v==null){
			WriteInt(-1);
		}else{
			WriteInt(v.Length);
			foreach (double b in v){
				Write(BitConverter.GetBytes(b));
			}
		}
	}
	
	public void WriteStrings(string[] v){
		if (v==null){
			WriteInt(-1);
		}else{
			WriteInt(v.Length);
			foreach (string b in v){
				WriteString(b);
			}
		}
	}
	
	public void WriteObject(PackUnpackable o){
		if (o==null){
			WriteBool(false);
			return;
		}
		
		WriteBool(true);
		WriteString(o.GetType().FullName);
		o.Pack(this);
	}
	
	public void WriteObjects(PackUnpackable[] v){
		if (v==null){
			WriteInt(-1);
		}else{
			WriteInt(v.Length);
			foreach (PackUnpackable b in v){
				WriteObject(b);
			}
		}
	}
	
}
