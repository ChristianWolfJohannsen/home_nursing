using UnityEngine;
using System.Collections;

public class CollectItems : PackUnpackableBehaviour {
	public string message="SomthingCollected";
	public int qty=1;

	public bool collectActive=true;
	public string activeevent=null;
	public GameObject activeNotificationObject=null;
	
	public void Start(){
		if (activeNotificationObject!=null){
			activeNotificationObject.SetActive(collectActive);
		}
	}
	
	public void Update(){
		if (activeevent==null || activeevent.Length==0){
			return;
		}
		
		if (collectActive!=Parley.GetInstance().IsEventActive(activeevent)){
			collectActive=Parley.GetInstance().IsEventActive(activeevent);
			if (!collectActive){
				BroadcastMessage("EndDialog",SendMessageOptions.DontRequireReceiver);
			}
			if (activeNotificationObject!=null){
				if (!collectActive){
					BroadcastMessage("EndDialog",SendMessageOptions.DontRequireReceiver);
					activeNotificationObject.BroadcastMessage("EndDialog",SendMessageOptions.DontRequireReceiver);
				}
				activeNotificationObject.SetActive(collectActive);
			}
		}
	}
	
	public void OnTriggerEnter(Collider collider) {
		if (!collectActive){
			return;
		}
		if ("Player".Equals(collider.gameObject.tag)){
			collider.gameObject.SendMessage(message,qty);
			gameObject.SetActive(false);
		}
	}
	
	override public void Pack(StreamPacker sp){
		sp.WriteBool(gameObject.activeSelf);
	}
	
	override public void Unpack(StreamUnpacker sp){
		gameObject.SetActive(sp.ReadBool());
	}

}
