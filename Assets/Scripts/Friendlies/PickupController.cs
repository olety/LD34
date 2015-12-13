using UnityEngine;
using UnityEngine.Sprites;
using System.Collections;


public class PickupController : MonoBehaviour {
	public enum pickupType { Null, Simple, Transformation };
	pickupType type;
	SpriteRenderer sprite;
	Color color;
	CameraProperties props;

	public static T GetRandomEnum<T>()
	{
		System.Array A = System.Enum.GetValues(typeof(T));
		T V;
		if (A.Length >= 2) {
			V = (T)A.GetValue (UnityEngine.Random.Range (1, A.Length));
		} else {
			V = (T)A.GetValue (UnityEngine.Random.Range (0, A.Length));
		}
		Debug.Log ("Getting random enums for " + A + " size = " + A.Length + " Returning random enum " + V );
		return V;
	}

	// Use this for initialization
	void Start () {
		props = new CameraProperties();
		setSizeToScaleRatio ();
		type = GetRandomEnum<pickupType> ();
		sprite = this.GetComponent<SpriteRenderer> ();
		setRandomSize ();
		InvokeRepeating ("changeColor", 0f, 0.5f);
	}
	
	// Update is called once per frame
	void Update () {
		if (this.transform.position.y < props.BottomLeft.y || this.transform.position.y > props.TopRight.y ) {
			Debug.LogError("Pickup fell through the level. Killing it");
			Destroy(this.gameObject);
		}
	}

	void changeColor(){
		color = new Color ((Random.Range (0f, 250f))/255, //unity takes RGB colors in floats
		                   (Random.Range (0f, 250f))/255, // in the form (0..1,0..1,0..1)
		                   (Random.Range (0f, 250f))/255,  // so we take a random from 0.250 (255 would be all white) and normalize it
		                   255); // opacity. for this to be seen, it has to be 255
//		Debug.Log("Changing pickup color to " + color);
		this.sprite.color = color;
	}
	
	//SIZE PACKAGE BEGINS HERE
	float size; 
	float sizeToScaleRatio;
	public float minSize,maxSize;
	void setSizeToScaleRatio(){
		// Since pickup block is 0.5x0.5 world units in size, (area = 0.25)
		// we'll have to multiply the scale of hero (2x2=4 area) by 4/0.25=16
		sizeToScaleRatio = HeroController.SizeToScaleRatio*16; 
	}
	public void setSize ( float amount ){
//		Debug.Log ("Pickup size set requested, amount = " + amount);
		size = Mathf.Clamp (amount, minSize, maxSize);
//		Debug.Log ("Set pickup size to : " + this.size);
		this.updateSize ();
	}

	void updateSize(){
//		Debug.Log ("Size : " + size + "ratio : " + sizeToScaleRatio);
		Vector3 newScale = new Vector3 (size * sizeToScaleRatio, size * sizeToScaleRatio, 0);
//		Debug.Log ("Pickup size update requested. NewScale = " + newScale);
		this.transform.localScale = newScale;
	}

	void setRandomSize(){
		setSize (Random.Range (minSize, maxSize));
	}
	//SIZE PACKAGE ENDS HERE

	public struct pickupMessage {
		public float size;
		public pickupType type;
	}

	void OnCollisionEnter2D(Collision2D coll) {
		if (coll.gameObject.tag == "Hero") {
			pickupMessage msg;
			msg.size = this.size;
			msg.type = this.type;
//			Debug.Log("Pickup size : " + this.size);
			Debug.Log("Sending a message : " + msg);
			coll.gameObject.SendMessage("processPickup", msg);
			Destroy (this.gameObject);
		}
	}

	void OnCollisionStay2D(Collision2D coll){
		OnCollisionEnter2D (coll);
	}
}
