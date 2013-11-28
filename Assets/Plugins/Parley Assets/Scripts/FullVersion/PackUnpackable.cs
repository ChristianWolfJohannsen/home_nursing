using UnityEngine;

public interface PackUnpackable {
	void Pack(StreamPacker sp);
	void Unpack(StreamUnpacker sp);
}
