using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TorchToggle : MonoBehaviour
{
    Light currentLight;

    public bool forceStayOn;

    public ParticleSystem particle;

    private void Start()
    {
        currentLight = GetComponent<Light>();
        currentLight.enabled = false;
        
    }

    private void Update()
    {
        if (forceStayOn)
        {
            currentLight.enabled = true;
        }

        //Lets update the fire
        if (currentLight.enabled == true)
        {
            currentLight.intensity = Random.Range(0.8f, 1.1f);
        }
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            currentLight.enabled = true;
            if (particle != null)
            {
                particle.Play();
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            currentLight.enabled = false;
            if (particle != null)
            {
                particle.Stop();
            }
        }
    }
}
