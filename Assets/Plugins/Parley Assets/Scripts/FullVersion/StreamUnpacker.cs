using UnityEngine;
using System;
using System.Collections.Generic;
using System.IO;

public class StreamUnpacker {

	private Stream stream;
	
	public StreamUnpacker(Stream stream){
		this.stream=stream;
	}
	
	private byte[] Read(int length){
		byte[] v=new byte[length];
		stream.Read(v,0,v.Length);
		return v;
	}
	
	public bool ReadBool(){
		return BitConverter.ToBoolean(Read(1),0);
	}
	
	public byte ReadByte(){
		return (byte)stream.ReadByte();
	}
		
	public short ReadShort(){
		return BitConverter.ToInt16(Read(2),0);
	}
	
	public char ReadChar(){
		return BitConverter.ToChar(Read(2),0);
	}
	
	public int ReadInt(){
		return BitConverter.ToInt32(Read(4),0);
	}
	
	public long ReadLong(){
		return BitConverter.ToInt64(Read(8),0);
	}
	
	public float ReadFloat(){
		return BitConverter.ToSingle(Read(4),0);
	}
	
	public double ReadDouble(){
		return BitConverter.ToDouble(Read(8),0);
	}
	
	public string ReadString(){
		char[] chars=ReadChars();
		if (chars==null){
			return null;
		}
		return new string(chars);
	}

	public bool[] ReadBools(){
		int l=ReadInt();
		if (l==-1){
			return null;
		}
		bool[] v=new bool[l];
		for (int t=0;t<l;t++){
			v[t]=ReadBool();
		}
		return v;
	}
	
	public byte[] ReadBytes(){
		int l=ReadInt();
		if (l==-1){
			return null;
		}
		byte[] v=new byte[l];
		for (int t=0;t<l;t++){
			v[t]=ReadByte();
		}
		return v;
	}
		
	public short[] ReadShorts(){
		int l=ReadInt();
		if (l==-1){
			return null;
		}
		short[] v=new short[l];
		for (int t=0;t<l;t++){
			v[t]=ReadShort();
		}
		return v;
	}
	
	public char[] ReadChars(){
		int l=ReadInt();
		if (l==-1){
			return null;
		}
		char[] v=new char[l];
		for (int t=0;t<l;t++){
			v[t]=ReadChar();
		}
		return v;
	}
	
	public int[] ReadInts(){
		int l=ReadInt();
		if (l==-1){
			return null;
		}
		int[] v=new int[l];
		for (int t=0;t<l;t++){
			v[t]=ReadInt();
		}
		return v;
	}
	
	public long[] ReadLongs(){
		int l=ReadInt();
		if (l==-1){
			return null;
		}
		long[] v=new long[l];
		for (long t=0;t<l;t++){
			v[t]=ReadLong();
		}
		return v;
	}
	
	public float[] ReadFloats(){
		int l=ReadInt();
		if (l==-1){
			return null;
		}
		float[] v=new float[l];
		for (int t=0;t<l;t++){
			v[t]=ReadFloat();
		}
		return v;
	}
	
	public double[] ReadDoubles(){
		int l=ReadInt();
		if (l==-1){
			return null;
		}
		double[] v=new double[l];
		for (int t=0;t<l;t++){
			v[t]=ReadDouble();
		}
		return v;
	}
	
	public string[] ReadStrings(){
		int l=ReadInt();
		if (l==-1){
			return null;
		}
		string[] v=new string[l];
		for (int t=0;t<l;t++){
			v[t]=ReadString();
		}
		return v;
	}
	
	public PackUnpackable ReadObject(){
		PackUnpackable o=null;
		if (ReadBool()){
			string c=ReadString();
			o=(PackUnpackable)System.Reflection.Assembly.GetExecutingAssembly().CreateInstance(c);
			o.Unpack(this);
		}
		return o;

	}
	
	public PackUnpackable[] ReadObjects(Type type){
		int l=ReadInt();
		if (l==-1){
			return null;
		}
		PackUnpackable[] v=(PackUnpackable[])Array.CreateInstance(type,l);
		for (int t=0;t<l;t++){
			v[t]=ReadObject();
		}
		return v;
	}
	

}
