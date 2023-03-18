using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class PickupScript : MonoBehaviour
{

	[SerializeField] private AudioClip collectSound;
	[SerializeField] private GameObject collectEffect;
    [SerializeField] private CollectibleTypes collectibleType;
    private Player _player;
	private GameManager _manager;
	private float _rotationSpeed;
	private AudioSource _source;
	private bool delete;
	private enum CollectibleTypes { NoType, Health, Damage, Shield };
    private void Awake () {

        _manager = GameObject.Find("Game Manager").GetComponent<GameManager>();
		_rotationSpeed = 150f;
		_source = GetComponent<AudioSource>();
		delete = false;
    }

    private void Update()
    {
        transform.Rotate(Vector3.up * _rotationSpeed * Time.deltaTime, Space.World);

		if(!_source.isPlaying && delete)
		{
			Destroy(gameObject);
		}	
    }

    private void OnTriggerEnter(Collider other)
	{
		if (other.tag == "Player") 
		{
			_player = other.GetComponent<Player>();
			Collect();
			_manager.PickupTotal--;
		}
	}

	

	private void Collect()
	{
		if (collectSound)
		{
			_source.Play();
			delete = true;
		}

		if (collectEffect)
		{
			Instantiate(collectEffect, transform.position, Quaternion.identity);
		}

		//Below is space to add in your code for what happens based on the collectible type

		if (collectibleType == CollectibleTypes.NoType) 
		{
			//Add in code here;
		}

		if (collectibleType == CollectibleTypes.Health) 
		{
			_player.Heal(1);
		}

		if (collectibleType == CollectibleTypes.Damage) 
		{
			_player.DamageBuffEffect();
		}

		if (collectibleType == CollectibleTypes.Shield) 
		{
			_player.ShieldBuffEffect();
        }

		gameObject.GetComponent<MeshRenderer>().enabled = false;
        gameObject.GetComponent<SphereCollider>().enabled = false;
    }

}
