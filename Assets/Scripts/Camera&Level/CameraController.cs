using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour {
	public Transform target; // the object that we'll follow
	public float offsetXRatio = 0f;
	public float offsetYRatio = 0f;
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
//		Debug.Log("Setting camera position to " + pos);
		this.transform.position = pos;
	}


}
