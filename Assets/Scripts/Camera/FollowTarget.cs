using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowTarget : MonoBehaviour
{
    public Transform target;
    private UserData userToFollow;

    private void Start()
    {
        userToFollow = GetComponent<UserData>();
    }
    // Update is called once per frame
    void Update()
    {
        if (target != null)
        {
            transform.position = new Vector3(target.position.x, transform.position.y, target.position.z);
        }
        else
        {
            if(userToFollow.controller)
            {
                target = userToFollow.controller.transform;
            }
        }
    }
}
