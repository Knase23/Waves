internal interface IPlayerShipControl
{
    void ActionOne();

    void ActionTwo();

    void ActionThree();

    /// <summary>
    /// </summary>
    /// <param name="horizontal"> The input between -1 and 1</param>
    /// <param name="vertical">The input between -1 and 1</param>
    void Move(float horizontal, float vertical);


}
