using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour {
	public Transform target; // the object that we'll follow
	public float offsetXRatio = 0f;
	public float offsetYRatio = 0f;
	public float maxOffsetY = 10f;
	Vector3 pos; // position, that we'll feed into the cameras transform
	CameraProperties props;
	// Use this for initialization
	void Start () {
		props = new CameraProperties ();
		pos = new Vector3 (0, this.transform.position.y+offsetYRatio*props.CamHeight, this.transform.position.z);
	}
	
	// Update is called once per frame
	void Update () {
		pos.x = target.position.x+offsetXRatio*props.CamWidth;
		pos.y = Mathf.Clamp(target.position.y+offsetYRatio*props.CamHeight, -maxOffsetY, maxOffsetY);
//		Debug.Log("Setting camera position to " + pos);
//		Debug.Log (target.position);
		this.transform.position = pos;
	}


}
