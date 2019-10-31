using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticalFollow : MonoBehaviour
{
    public float lifeTime;
    public Transform targetToFollow;
    private new ParticleSystem particleSystem;
    // Start is called before the first frame update

    // Update is called once per frame
    void Update()
    {
        if(targetToFollow)
        {
            transform.position = targetToFollow.position;
            transform.rotation = targetToFollow.rotation;
        }
    }
    public void ActivateDestroyWithSetLifeTime()
    {
         Destroy(gameObject, lifeTime);
    }
    public void SetColorOfParticleEffect(Color color)
    {
        particleSystem = GetComponent<ParticleSystem>();
        var main = particleSystem.main;
        main.startColor = color;
    }
}
