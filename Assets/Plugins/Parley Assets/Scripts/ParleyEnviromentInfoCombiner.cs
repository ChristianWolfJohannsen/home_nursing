using UnityEngine;

public class ParleyEnviromentInfoCombiner : ParleyEnviromentInfo {
	
	private ParleyEnviromentInfo[] infoSets=null;
	
	public ParleyEnviromentInfoCombiner(ParleyEnviromentInfo[] infoSets){
		this.infoSets=infoSets;
	}
	
	public ParleyEnviromentInfoCombiner(ParleyEnviromentInfo i1,ParleyEnviromentInfo i2,ParleyEnviromentInfo i3=null,ParleyEnviromentInfo i4=null,ParleyEnviromentInfo i5=null){
		int count=2;
		if (i3!=null){
			count++;
		}
		if (i4!=null){
			count++;
		}
		if (i5!=null){
			count++;
		}
		infoSets=new ParleyEnviromentInfo[count];
		infoSets[0]=i1;
		infoSets[1]=i1;
		int p=2;
		if (i3!=null){
			infoSets[p++]=i3;
		}
		if (i4!=null){
			infoSets[p++]=i4;
		}
		if (i5!=null){
			infoSets[p++]=i5;
		}
	}
	
	public object GetEnviromentInfo(string key){
		foreach (ParleyEnviromentInfo pe in infoSets){
			object o=pe.GetEnviromentInfo(key);
			if (o!=null){
				return o;
			}
		}
		return null;
	}
	
	public void SetEnviromentInfo(string key,object v){
		foreach (ParleyEnviromentInfo pe in infoSets){
			pe.SetEnviromentInfo(key,v);
		}
	}
}
