using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ship : MonoBehaviour, IPlayerShipControl, IDamagable, IPushable
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

    public bool vulnerable;

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
        //transform.rotation = Quaternion.Euler(0, transform.rotation.eulerAngles.y, 0);

        if (DiscordLobbyService.INSTANCE.IsTheHost())
        {
            TransformDataPackage movementData = new TransformDataPackage(transform.position, transform.rotation, userId);
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
        if(DiscordLobbyService.INSTANCE.IsTheHost())
        {
            TriggerActionPackage package = new TriggerActionPackage(userId, 0);
            DiscordNetworkLayerService.INSTANCE.SendMessegeToAllOthers(NetworkChannel.ACTION_TRIGGER, package.ToBytes());
        }
    }
    public void ActionOneUpgrade()
    {
        if (actionOne == null)
            return;
        actionOne.ApplyUpgrade(this, storedUpgrade);
    }
    #endregion

    #region Action 2
    public void ActionTwo()
    {
        if (actionTwo == null)
            return;

        actionTwo.Execute();
    }
    public void ActionTwoUpgrade()
    {
        if (actionTwo == null)
            return;

        actionTwo.ApplyUpgrade(this, storedUpgrade);
    }
    #endregion

    #region Action 3
    public void ActionThree()
    {
        if (actionThree == null)
            return;

        actionThree.Execute();
    }
    public void ActionThreeUpgrade()
    {
        if (actionThree == null)
            return;

        actionThree.ApplyUpgrade(this, storedUpgrade);
    }
    #endregion
    TransformDataPackage latestTransformUpdate;
    #region Movement
    public void Move(float horizontal, float vertical, long userId)
    {
        this.userId = userId;
        this.horizontal = horizontal;
        this.vertical = vertical;
    }

    
    public void ChangeTransform(TransformDataPackage transformData)
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
        Debug.Log(gameObject.name + " - You diead!");
        SpawnLocationHandler.INSTANCE.Respawn(this);
        hp.ResetHealth();
        gameObject.SetActive(false);
        //Spawn on new position after a set amount of miliseconds/seconds
        //Send Messege to SpawnLocationHandler to relocate object
    }
    #endregion
    public void PushAway(Vector3 directionToPush, float strength)
    {
        rb.AddForce(directionToPush * strength, ForceMode.Impulse);
        vulnerable = true;
        Invoke("ResetVulnerable", 2);
    }

    public void ResetVulnerable()
    {
        vulnerable = false;
    }
    public void OnCollisionEnter(Collision collision)
    {
        if(vulnerable && collision.collider.tag == "Asteriod" && collision.relativeVelocity.sqrMagnitude > 60 )
        {
            Debug.Log("Hit a Asteriod while Vulnerable, Relative Velocity sqr Magnitude: " + collision.relativeVelocity.sqrMagnitude, gameObject);
            hp.TakeDamage(1);
            //Maybe the one that made me vulnerable should get points
        }
    }
}

public struct TransformDataPackage
{
    public Vector3 position;
    public Quaternion rotation;
    public long id;
    public TransformDataPackage(Vector3 position,Quaternion rotation ,long id)
    {
        this.position = position;
        this.rotation = rotation;
        this.id = id;
        
    }
    public TransformDataPackage(byte[] data)
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
        if (!(obj is TransformDataPackage))
        {
            return false;
        }

        var data = (TransformDataPackage)obj;
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

    public static bool operator ==(TransformDataPackage left, TransformDataPackage right)
    {
        return left.id == right.id && left.position == right.position && left.rotation == right.rotation;
    }
    public static bool operator !=(TransformDataPackage left, TransformDataPackage right)
    {
        return left.id != right.id || left.position != right.position || left.rotation != right.rotation;
    }
}