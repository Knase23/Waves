using UnityEngine;
public interface IPlayerShipControl
{
    /// <summary>
    /// Activets Action 1
    /// </summary>
    void ActionOne();
    
    /// <summary>
    /// Activets Action 2
    /// </summary>
    void ActionTwo();
    
    /// <summary>
    /// Activets Action 3
    /// </summary>
    void ActionThree();

    /// <summary>
    /// Used by the client side of;
    /// </summary>
    /// <param name="horizontal"> The input between -1 and 1</param>
    /// <param name="vertical">The input between -1 and 1</param>
    void Move(float horizontal, float vertical);

    /// <summary>
    /// Used by Networking
    /// </summary>
    /// <param name="position"></param>
    void Move(Vector3 position);


}
