using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputHandler : MonoBehaviour
{


    [Tooltip("Uses Input.GetAxis")]
    public string VerticalAxis = "Vertical";

    [Tooltip("Uses Input.GetAxis")]
    public string HorizontalAxis = "Horizontal";

    [Tooltip("Uses Input.GetButton")]
    public string ActionOneButton = "FirePulse";
    public NetworkLocation networkLocation = NetworkLocation.Local;
    public IPlayerShipControl shipControl;


    public enum NetworkLocation
    {
        Local,
        Network
    }

    // Start is called before the first frame update
    void Start()
    {
        shipControl = GetComponent<IPlayerShipControl>();
    }
    // Update is called once per frame
    void Update()
    {
        if(networkLocation != NetworkLocation.Local)
        {
            return;
        }

        if (Input.GetButtonDown(ActionOneButton))
        {
            shipControl.ActionOne();
        }
        shipControl.Move(Input.GetAxis(HorizontalAxis), Input.GetAxis(VerticalAxis));
    }
}
