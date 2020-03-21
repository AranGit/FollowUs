
using UnityEngine;

public class Avatar : MonoBehaviour
{
    public enum AvatarStatus { Hero_leader, Hero_following, Hero_Unknow, Enemy };
    public enum AvatarType { Red, Green, Blue };
    private int _heart;
    private int _sword;
    private int _shield;

    private string _name;
    private AvatarStatus _avtarStatus;
    private AvatarType _avatarType;

    public int Heart
    {
        get { return _heart; }
        set { _heart = value; }
    }
    public int Sword
    {
        get { return _sword; }
        set { _sword = (value < 1 ? 1 : value); }
    }

    public int Shield
    {
        get { return _shield; }
        set { _shield = value; }
    }

    public string Name
    {
        get { return _name; }
        set { _name = value; }
    }

    public void SetAvatarStatus(AvatarStatus value)
    {
        _avtarStatus = value;
    }

    public AvatarStatus GetAvatarStatus()
    {
        return _avtarStatus;
    }

    public AvatarType GetAvatarType()
    {
        return _avatarType;
    }

    public void SetAvatarType(AvatarType value)
    {
        _avatarType = value;
    }
}
