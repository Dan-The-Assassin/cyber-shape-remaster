using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreakableScript : MonoBehaviour
{
    [SerializeField] private GameObject _intactVersion;
    [SerializeField] private GameObject _brokenVersion;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void OnTriggerEnter(Collider collision)
    {
        if ( collision.CompareTag("Player") || collision.CompareTag("Enemy"))
        {
            Break();
        }
        
    }
    void Break()
    {
        _intactVersion.SetActive(false);
        _brokenVersion.SetActive(true);
    }
}
