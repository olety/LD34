using UnityEngine;
using System.Collections;

public class HeroController : MonoBehaviour {
	public float horSpeed;
	private Rigidbody2D rigidbody;

	void Awake () {
		this.rigidbody = this.GetComponent<Rigidbody2D> ();
	}

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void FixedUpdate () {
		Vector2 velocity = new Vector2 ();
		velocity.x += horSpeed;
		this.rigidbody.velocity = velocity;

	}

}
