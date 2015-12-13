using UnityEngine;
using System.Collections;

public class CameraProperties {
	
	Vector3 viewPort;

	public Vector3 ViewPort {
		get {
			updateCameraProperties();
			return viewPort;
		}
	}

	Vector3 bottomLeft;

	public Vector3 BottomLeft {
		get {
			updateCameraProperties();
			return bottomLeft;
		}
	}

	Vector3 topRight;

	public Vector3 TopRight {
		get {
			updateCameraProperties();
			return topRight;
		}
	}

	float camHeight;

	public float CamHeight {
		get {
			updateCameraProperties();
			return camHeight;
		}
	}

	float camWidth;

	public float CamWidth {
		get {
			updateCameraProperties();
			return camWidth;
		}
	}

	Vector2 camCenter;

	public Vector2 CamCenter {
		get {
			updateCameraProperties();
			return camCenter;
		}
	}

	public void updateCameraProperties () {
		viewPort.Set(0,0,0);
		bottomLeft = Camera.main.ViewportToWorldPoint(viewPort);
		viewPort.Set(1,1,1);
		topRight = Camera.main.ViewportToWorldPoint(viewPort);
//		Debug.Log ("Camera botLeft : " + bottomLeft);
//		Debug.Log ("Camera topRight : " + topRight);
		camHeight = topRight.y - bottomLeft.y;
		camWidth = topRight.x - bottomLeft.x;
		camCenter.Set( (topRight.x - camWidth / 2), (topRight.y - camHeight / 2) );
	}

	Vector3 getNewBackgroundScale (){
		return new Vector3 (camWidth, camHeight, 1);
	}

	public Vector3 GetBackgroundScale {
		get {
			updateCameraProperties();
			return getNewBackgroundScale();
		}
	}

	public CameraProperties(){
		viewPort = new Vector2 ();
		camCenter = new Vector2 ();
		updateCameraProperties();
	}

}