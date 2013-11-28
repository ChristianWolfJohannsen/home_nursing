using UnityEngine;

abstract public class PackUnpackableBehaviour : MonoBehaviour {
	public abstract void Pack(StreamPacker sp);
	public abstract void Unpack(StreamUnpacker sp);
}
