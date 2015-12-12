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
	
	// Update is called once per frame
	void Update () {
		Debug.Log ("Bullet pos : " + this.transform.position);
		if (this.transform.position.x > props.TopRight.x ) {
			Debug.Log("Bullet is out of the screen, killing it");
			Destroy(this.gameObject);
		}
	}
}
