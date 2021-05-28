namespace Framework.Scripts.Constants
{
    /// <summary>
    /// 事件类型
    /// (根据需要取名称，不得重复)
    /// </summary>
    public enum EventType
    {
        StartGame,
        ClickBlock,
        
        RedDot,
        ItemChangeNum,
        BagpackChange,
        
        //_______________________ Bullet _______________________
        PlayerWeaponFire,
        PlayerBulletCollide,
    }
}