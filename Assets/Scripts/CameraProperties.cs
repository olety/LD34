using UnityEngine;
using System.Collections;

public class CameraProperties {
	
	Vector3 viewPort;

	public Vector3 ViewPort {
		get {
			return viewPort;
		}
	}

	Vector3 bottomLeft;

	public Vector3 BottomLeft {
		get {
			return bottomLeft;
		}
	}

	Vector3 topRight;

	public Vector3 TopRight {
		get {
			return topRight;
		}
	}

	float camHeight;

	public float CamHeight {
		get {
			return camHeight;
		}
	}

	float camWidth;

	public float CamWidth {
		get {
			return camWidth;
		}
	}
	
	public void updateCameraProperties () {
		viewPort = new Vector3(0,0,0);
		bottomLeft = Camera.main.ViewportToWorldPoint(viewPort);
		viewPort.Set(1,1,1);
		topRight = Camera.main.ViewportToWorldPoint(viewPort);
		Debug.Log ("Camera botLeft : " + bottomLeft);
		Debug.Log ("Camera topRight : " + topRight);
		camHeight = topRight.y - bottomLeft.y;
		camWidth = topRight.x - bottomLeft.x;
	}

	Vector3 getNewBackgroundScale (){
		return new Vector3 (camHeight, camHeight, 1);
	}

	public Vector3 GetBackgroundScale {
		get {
			return getNewBackgroundScale();
		}
	}

	public CameraProperties(){
		updateCameraProperties();
	}
}