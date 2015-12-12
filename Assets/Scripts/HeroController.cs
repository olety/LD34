using UnityEngine;
using System.Collections;

public class HeroController : MonoBehaviour {
	public CameraProperties camProps;
	public GameObject hpBar;
	public GameObject apBar;
	public float horSpeed;
	public float spriteLen = 1f;
	public float bulletLen = 0.5f;
	public float maxHealth = 200f;
	public float healthResfeshRate = 5f;
	public float maxAbilityPoints = 100f;
	public float APRefreshRate = 5f;
	public float bulletAPConsumption = 20f;
	public float slamAPConsumption = 50f;
	
	Rigidbody2D rigidbody;
	Object Bullet;
	Vector3 hpScale, apScale;
	float abilityPoints;

	public float AbilityPoints {
		get {
			return abilityPoints;
		}
		set {
			abilityPoints = value;
		}
	}

	public void incAP ( float amount ){
		this.abilityPoints = Mathf.Clamp (this.abilityPoints + amount, 0, this.maxAbilityPoints);
	}
	
	public void decAP ( float amount ){
		this.abilityPoints = Mathf.Clamp (this.abilityPoints - amount, 0, this.maxAbilityPoints);
	}

	float health;

	public float Health {
		get {
			return health;
		}
		set {
			health = value;
		}
	}

	public void incHealth ( float amount ){
		this.health = Mathf.Clamp (this.health + amount, 0, this.maxHealth);
	}
	
	public void decHealth ( float amount ){
		this.health = Mathf.Clamp (this.health - amount, 0, this.maxHealth);
	}

	void hpBarUpdate(){
		this.hpBar.transform.localScale = new Vector3(hpScale.x*this.health / this.maxHealth, 
		                                              hpScale.y, 
		                                              hpScale.z); 
		Vector3 newPos = new Vector3 (this.hpBar.transform.localPosition.x,
		                              -1f+(this.health / this.maxHealth),
		                              this.hpBar.transform.localPosition.z);
		Debug.Log ("Setting new position of hpbar to " + newPos);
		this.hpBar.transform.localPosition = newPos;
	}
	
	void apBarUpdate(){
		this.apBar.transform.localScale = new Vector3(apScale.x*this.abilityPoints / this.maxAbilityPoints, 
		                                              apScale.y, 
		                                              apScale.z); 
		Vector3 newPos = new Vector3 (this.apBar.transform.localPosition.x,
		                              -1f+(this.abilityPoints / this.maxAbilityPoints),
		                              this.apBar.transform.localPosition.z);
		Debug.Log ("Setting new position of apbar to " + newPos);
		this.apBar.transform.localPosition = newPos;
	}

	void Awake () { 
		this.rigidbody = this.GetComponent<Rigidbody2D> ();
	}

	// Use this for initialization
	void Start () {
		hpScale = hpBar.transform.localScale;
		apScale = apBar.transform.localScale;
		abilityPoints = maxAbilityPoints;
		health = maxHealth;
		if (!(Bullet = Resources.Load ("Bullet", typeof(GameObject)))) {
			Debug.LogError("Couldn't load a bullet object");
		}
	
	}
	
	// Update is called once per frame
	void Update () {
		if ( Input.GetButtonDown("FireBullet") ){
			if ( abilityPoints >= bulletAPConsumption ){
				abilityPoints -= bulletAPConsumption;
				Vector3 bulletPos = new Vector3(this.transform.position.x+this.spriteLen+this.bulletLen/2, 
				                                this.transform.position.y, 
				                                0);
				Debug.Log ("Creating a bullet at " + bulletPos);
				Instantiate(Bullet, bulletPos, Quaternion.identity);

			} 
		}
		if ( Input.GetKeyDown(KeyCode.Space) ){
			decHealth(50.0f);
			decAP(50.0f);
		}
		
		abilityPoints = Mathf.Clamp (abilityPoints + Time.deltaTime * APRefreshRate, 0, maxAbilityPoints);
		health = Mathf.Clamp (health + Time.deltaTime * healthResfeshRate, 0, maxHealth);
		apBarUpdate ();
		hpBarUpdate ();
	}

	void FixedUpdate () {
		Vector2 velocity = new Vector2 ();
		velocity.x += horSpeed;
		this.rigidbody.velocity = velocity;

	}



}
