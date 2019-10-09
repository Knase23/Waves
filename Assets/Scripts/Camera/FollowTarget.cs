using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowTarget : MonoBehaviour
{
    public Transform target;
    private Vector3 offset;
    private UserData userToFollow;

    private void Start()
    {
        offset = transform.position;
        userToFollow = GetComponent<UserData>();
    }
    // Update is called once per frame
    void Update()
    {
        if (target != null)
        {
            transform.position = new Vector3(target.position.x + offset.x, transform.position.y, target.position.z + offset.z);
        }
        else
        {
            if(userToFollow && userToFollow.controller)
            {
                target = userToFollow.controller.transform;
            }
        }
    }
}
