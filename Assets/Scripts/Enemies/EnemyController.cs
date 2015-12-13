using UnityEngine;
using System.Collections;

public class EnemyController : MonoBehaviour {
	public float minPickups, maxPickups;
	public float minPickupOffset, maxPickupOffset;
	public string pickupName = "Pickup";
	static Object pickup; 

	float numPickups;
	float damage;

	// Use this for initialization
	void Start () {
		setSizeToScaleRatio ();
		setRandomSize ();
		damage = size;
		numPickups = Random.Range (minPickups, maxPickups);
		if (!(pickup = Resources.Load (pickupName, typeof(GameObject)))) {
			Debug.LogError("Couldn't load a pickup object");
		}
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	//SIZE PACKAGE BEGINS HERE
	//TODO: transform this to a class maybe?
	float size; 
	float sizeToScaleRatio;
	public float minSize,maxSize;
	void setSizeToScaleRatio(){
		// Since pickup block is 1x1 world units in size, (area = 1)
		// we'll have to multiply the scale of hero (2x2=4 area) by 4/1=4
		sizeToScaleRatio = HeroController.SizeToScaleRatio*4; 
	}
	public void setSize ( float amount ){
		if (amount <= 0f) {
			Debug.Log ("Enemy has received lethal amount of damage, killing it");
			killThis();
			return;
		}
		Debug.Log ("Enemy size set requested, amount = " + amount);
		size = Mathf.Clamp (amount, minSize, maxSize);
		Debug.Log ("Set Enemy size to : " + this.size);
		damage = size;
		this.updateSize ();
	}
	
	public void incSize ( float amount ){
		Debug.Log ("Enemy size inc requested, amount = " + amount);
		this.setSize (this.size + amount);
	}
	
	public void decSize ( float amount ){
		Debug.Log ("Enemy size dec requested, amount = " + amount);
		this.setSize (this.size - amount);
	}

	void updateSize(){
		//		Debug.Log ("Size : " + size + "ratio : " + sizeToScaleRatio);
		Vector3 newScale = new Vector3 (size * sizeToScaleRatio, size * sizeToScaleRatio, 0);
		Debug.Log ("Enemy size update requested. NewScale = " + newScale);
		this.transform.localScale = newScale;
	}
	
	void setRandomSize(){
		setSize (Random.Range (minSize, maxSize));
	}
	//SIZE PACKAGE ENDS HERE

	void spawnPickups(){
		Vector3 pickupPos = new Vector3 (0, this.transform.position.y, 0);
		for (int i = 0; i < numPickups; i++) {
			pickupPos.x = Random.Range(this.transform.position.x+minPickupOffset,
			                           this.transform.position.x+maxPickupOffset);
			Instantiate(pickup,pickupPos,Quaternion.identity);
		}
	}

	void OnCollisionEnter2D(Collision2D coll) {
		if (coll.gameObject.tag == "Hero") {
			coll.gameObject.SendMessage("decSize", damage);
			killThis();
			Destroy (this.gameObject);
		}
		if (coll.gameObject.tag == "Bullet") {
			Destroy (coll.gameObject);
		} else {
		
		}
	}
	void killThis(){
		spawnPickups ();
		Destroy (this.gameObject);
	}
}
