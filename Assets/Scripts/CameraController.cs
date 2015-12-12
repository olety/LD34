using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour {
	public Transform target; // the object that we'll follow
	private Vector3 pos; // position, that we'll feed into the cameras transform
	// Use this for initialization
	void Start () {
		pos = new Vector3 (0, this.transform.position.y, this.transform.position.z);
	}
	
	// Update is called once per frame
	void Update () {
		pos.x = target.position.x;
		this.transform.position = pos;
	}


}
