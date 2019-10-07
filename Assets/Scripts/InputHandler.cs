using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputHandler : MonoBehaviour
{
    public long userID;

    [Tooltip("Uses Input.GetAxis")]
    public string VerticalAxis = "Vertical";

    [Tooltip("Uses Input.GetAxis")]
    public string HorizontalAxis = "Horizontal";

    [Tooltip("Uses Input.GetButton")]
    public string ActionOneButton = "FirePulse";


    public NetworkLocation networkLocation = NetworkLocation.Local;
    public IPlayerShipControl shipControl;
    InputData latestInputUpdate;

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
        if (!DiscordLobbyService.INSTANCE.IsTheHost())
        {
            InputData data = new InputData(Input.GetAxisRaw(HorizontalAxis), Input.GetAxisRaw(VerticalAxis),Input.GetButton(ActionOneButton) ,userID);
            if (latestInputUpdate != data)
            {
                latestInputUpdate = data;
                DiscordNetworkLayerService.INSTANCE.SendMessegeToOwnerOfLobby(NetworkChannel.INPUT_DATA, latestInputUpdate.ToBytes());
            }
            return;
        }

        if (Input.GetButtonDown(ActionOneButton))
        {
            shipControl.ActionOne();
        }

        shipControl.Move(Input.GetAxis(HorizontalAxis), Input.GetAxis(VerticalAxis));
    }
    public void SetOwnerOfThisInputHandler(long userID)
    {
        this.userID = userID;
    }
    public void ReciveInputData(InputData data)
    {
        if(data.id != userID)
        {
            return;
        }

        if (data.action1)
        {
            shipControl.ActionOne();
        }
        shipControl.Move(data.x, data.y);
    }

}
public struct InputData
{
    public float x, y;
    public bool action1;

    public long id;
    public InputData(float x, float y,bool action1,long id)
    {
        this.x = x;
        this.y = y;
        this.action1 = action1;
        this.id = id;

    }
    public InputData(byte[] data)
    {
        x = BitConverter.ToSingle(data, 0);
        y = BitConverter.ToSingle(data, 4);
        action1 = BitConverter.ToBoolean(data, 8);
        id = BitConverter.ToInt64(data, 16);
    }
    public byte[] ToBytes()
    {
        List<byte> vs = new List<byte>();

        vs.AddRange(BitConverter.GetBytes(x));
        vs.AddRange(BitConverter.GetBytes(y));
        vs.AddRange(BitConverter.GetBytes(action1));
        vs.AddRange(BitConverter.GetBytes(id));
        return vs.ToArray();
    }

    public override bool Equals(object obj)
    {
        if (!(obj is InputData))
        {
            return false;
        }

        var data = (InputData)obj;
        return data == this;
    }

    public override int GetHashCode()
    {
        var hashCode = 848949501;
        hashCode = hashCode * -1521134295 + x.GetHashCode();
        hashCode = hashCode * -1521134295 + y.GetHashCode();
        hashCode = hashCode * -1521134295 + action1.GetHashCode();
        hashCode = hashCode * -1521134295 + id.GetHashCode();
        return hashCode;
    }

    public static bool operator ==(InputData left,InputData right)
    {
        return left.id == right.id && left.x == right.x && left.y == right.y && left.action1 == right.action1;
    }
    public static bool operator !=(InputData left, InputData right)
    {
        return left.id != right.id || left.x != right.x || left.y != right.y || left.action1 != right.action1;
    }
}