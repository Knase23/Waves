using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ship : MonoBehaviour, IPlayerShipControl, IDamagable
{
    public float movmentSpeed = 10;
    public float rotationSpeed = 10;

    public Color shipColor = Color.green;

    /// <summary>
    /// Health of the Ship
    /// </summary>
    public Health hp = new Health();

    /// <summary>
    /// Storing a picked up action waiting for which action to replace
    /// </summary>
    [SerializeField]
    private Upgrade storedUpgrade;

    /// <summary>
    /// First action that the ship can use!
    /// </summary>
    public Action actionOne;

    /// <summary>
    /// Second action that the ship can use!
    /// </summary>
    public Action actionTwo;

    /// <summary>
    /// Third action that the ship can use!
    /// </summary>
    public Action actionThree;


    private Rigidbody rb;

    private long userId;
    private float vertical;
    private float horizontal;


    private void Start()
    {
        hp.OnDeath += OnDeath;
        rb = GetComponent<Rigidbody>();
    }
    private void Update()
    {
        #region Movement
        Vector3 increment = vertical * transform.forward * Time.deltaTime * movmentSpeed;
        rb.velocity += increment;
        transform.Rotate(horizontal * Vector3.up * rotationSpeed * Time.deltaTime);
        transform.rotation = Quaternion.Euler(0, transform.rotation.eulerAngles.y, 0);

        if (DiscordLobbyService.INSTANCE.IsTheHost())
        {
            TransformData movementData = new TransformData(transform.position, transform.rotation, userId);
            if (latestTransformUpdate != movementData)
            {
                latestTransformUpdate = movementData;
                DiscordNetworkLayerService.INSTANCE.SendMessegeToAllOthers(NetworkChannel.SHIP_TRANSFORM, movementData.ToBytes());
            }

        }
        #endregion
    }
    public void StoreUpgrade(Upgrade upgrade)
    {
        if (upgrade == null)
        {
            storedUpgrade = null;
            return;
        }

        if (upgrade is MovmentSpeedUpgrade)
        {
            movmentSpeed = upgrade.amount;
            return;
        }

        storedUpgrade = upgrade;

    }

    #region IPlayerShipControl Functions

    #region Action 1
    public void ActionOne(long userId)
    {
        if (actionOne == null)
            return;

        actionOne.Execute();
    }
    public void ActionOneUpgrade(long userId)
    {
        if (actionOne == null)
            return;
        actionOne.ApplyUpgrade(this, storedUpgrade);
    }
    #endregion

    #region Action 2
    public void ActionTwo(long userId)
    {
        if (actionTwo == null)
            return;

        actionTwo.Execute();
    }
    public void ActionTwoUpgrade(long userId)
    {
        if (actionTwo == null)
            return;

        actionTwo.ApplyUpgrade(this, storedUpgrade);
    }
    #endregion

    #region Action 3
    public void ActionThree(long userId)
    {
        if (actionThree == null)
            return;

        actionThree.Execute();
    }
    public void ActionThreeUpgrade(long userId)
    {
        if (actionThree == null)
            return;

        actionThree.ApplyUpgrade(this, storedUpgrade);
    }
    #endregion
    TransformData latestTransformUpdate;
    #region Movement
    public void Move(float horizontal, float vertical, long userId)
    {
        this.userId = userId;
        this.horizontal = horizontal;
        this.vertical = vertical;
    }

    
    public void ChangeTransform(TransformData transformData)
    {
        transform.position = transformData.position;
        transform.rotation = transformData.rotation;
    }
    #endregion

    #endregion

    #region IDamageble Functions
    public void TakeDamage(float damage)
    {
        hp.TakeDamage(damage);
    }
    #endregion;

    #region Health Delegate Functions
    public void OnDeath()
    {
        Debug.Log(gameObject.name + " - You Dead!");
        gameObject.SetActive(false);
    }
    #endregion
}

public struct TransformData
{
    public Vector3 position;
    public Quaternion rotation;
    public long id;
    public TransformData(Vector3 position,Quaternion rotation ,long id)
    {
        this.position = position;
        this.rotation = rotation;
        this.id = id;
        
    }
    public TransformData(byte[] data)
    {
        position.x = BitConverter.ToSingle(data, 0);
        position.y = BitConverter.ToSingle(data, 4);
        position.z = BitConverter.ToSingle(data, 8);
        rotation.x = BitConverter.ToSingle(data, 12);
        rotation.y = BitConverter.ToSingle(data, 16);
        rotation.z = BitConverter.ToSingle(data, 20);
        rotation.w = BitConverter.ToSingle(data, 24);
        id = BitConverter.ToInt64(data, 28);
    }
    public byte[] ToBytes()
    {
        List<byte> vs = new List<byte>();

        vs.AddRange(BitConverter.GetBytes(position.x));
        vs.AddRange(BitConverter.GetBytes(position.y));
        vs.AddRange(BitConverter.GetBytes(position.z));

        vs.AddRange(BitConverter.GetBytes(rotation.x));
        vs.AddRange(BitConverter.GetBytes(rotation.y));
        vs.AddRange(BitConverter.GetBytes(rotation.z));
        vs.AddRange(BitConverter.GetBytes(rotation.w));

        vs.AddRange(BitConverter.GetBytes(id));
        return vs.ToArray();
    }

    public override bool Equals(object obj)
    {
        if (!(obj is TransformData))
        {
            return false;
        }

        var data = (TransformData)obj;
        return data == this;
    }

    public override int GetHashCode()
    {
        var hashCode = 1745363393;
        hashCode = hashCode * -1521134295 + EqualityComparer<Vector3>.Default.GetHashCode(position);
        hashCode = hashCode * -1521134295 + EqualityComparer<Quaternion>.Default.GetHashCode(rotation);
        hashCode = hashCode * -1521134295 + id.GetHashCode();
        return hashCode;
    }

    public static bool operator ==(TransformData left, TransformData right)
    {
        return left.id == right.id && left.position == right.position && left.rotation == right.rotation;
    }
    public static bool operator !=(TransformData left, TransformData right)
    {
        return left.id != right.id || left.position != right.position || left.rotation != right.rotation;
    }
}