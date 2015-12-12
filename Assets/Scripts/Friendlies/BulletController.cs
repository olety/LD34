using UnityEngine;
using System.Collections;

public class BulletController : MonoBehaviour {
	public float goingRight = 1.0f;
	public float damage = 50f;
	public float speed = 8f;
	CameraProperties props;
	Rigidbody2D rigidbody;
	Vector2 velocity;
	// Use this for initialization
	void Start () {
		props = new CameraProperties();
		this.rigidbody = this.GetComponent<Rigidbody2D> ();
		this.velocity = new Vector2 ();
		velocity.x = speed;
		this.rigidbody.velocity = velocity;
	}

	public void changeColor(){

	}

	public void chngeColorRandom(float deltaTime){
	
	}

	// Update is called once per frame
	void Update () {
		props.updateCameraProperties ();
//		Debug.Log ("Bullet pos : " + this.transform.position);
		if (this.transform.position.x > props.TopRight.x ) {
			Debug.Log("Bullet is out of the screen, killing it");
			Destroy(this.gameObject);
		}
	}
	void OnCollisionEnter2D(Collision2D coll) {
		if (coll.gameObject.tag == "Enemy") {
			coll.gameObject.SendMessage("decSize", damage);
			Destroy (this.gameObject);
		}
		if (coll.gameObject.tag == "Pickup") {
			Destroy (coll.gameObject);
			Destroy (this.gameObject);
		}
	}
}
