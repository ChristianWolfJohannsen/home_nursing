using UnityEngine;

public interface ParleyEnviromentInfo{
	object GetEnviromentInfo(string key);
	void SetEnviromentInfo(string key,object value);
}