using UnityEngine;

public class SaveLoadTransform : PackUnpackableBehaviour {
	
	override public void Pack(StreamPacker sp){
		PackTransform(gameObject.transform,sp);
		if (rigidbody!=null){
			PackRigidbody(rigidbody,sp);
		}
	}
	
	override public void Unpack(StreamUnpacker su){
		UnpackTransform(gameObject.transform,su);
		if (rigidbody!=null){
			UnpackRigidbody(rigidbody,su);
		}
	}
	
	public static void PackTransform(Transform packTransform,StreamPacker sp){
		sp.WriteFloat(packTransform.localPosition.x);
		sp.WriteFloat(packTransform.localPosition.y);
		sp.WriteFloat(packTransform.localPosition.z);
		sp.WriteFloat(packTransform.localScale.x);
		sp.WriteFloat(packTransform.localScale.y);
		sp.WriteFloat(packTransform.localScale.z);
		sp.WriteFloat(packTransform.localRotation.x);
		sp.WriteFloat(packTransform.localRotation.y);
		sp.WriteFloat(packTransform.localRotation.z);
		sp.WriteFloat(packTransform.localRotation.w);
		
	}
	
	public static void UnpackTransform(Transform packTransform,StreamUnpacker su){
		packTransform.localPosition=new Vector3(su.ReadFloat(),su.ReadFloat(),su.ReadFloat());
		packTransform.localScale=new Vector3(su.ReadFloat(),su.ReadFloat(),su.ReadFloat());
		packTransform.localRotation=new Quaternion(su.ReadFloat(),su.ReadFloat(),su.ReadFloat(),su.ReadFloat());
	}
	
	public static void PackRigidbody(Rigidbody rb,StreamPacker sp){
		sp.WriteFloat(rb.velocity.x);
		sp.WriteFloat(rb.velocity.y);
		sp.WriteFloat(rb.velocity.z);
		sp.WriteFloat(rb.angularVelocity.x);
		sp.WriteFloat(rb.angularVelocity.y);
		sp.WriteFloat(rb.angularVelocity.z);
		sp.WriteFloat(rb.drag);
		sp.WriteFloat(rb.angularDrag);
	}
	
	public static void UnpackRigidbody(Rigidbody rb,StreamUnpacker su){
		rb.velocity=new Vector3(su.ReadFloat(),su.ReadFloat(),su.ReadFloat());
		rb.angularVelocity=new Vector3(su.ReadFloat(),su.ReadFloat(),su.ReadFloat());
		rb.drag=su.ReadFloat();
		rb.angularDrag=su.ReadFloat();
	}

}

