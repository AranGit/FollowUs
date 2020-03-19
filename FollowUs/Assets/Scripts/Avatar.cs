using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Avatar : MonoBehaviour
{
    public enum AvatarStatus { Hero_leader, Hero_following, Enemy };
    public enum AvatarType { Red, Green, Blue };
    private int _heart;
    private int _sword;
    private int _shield;

    private string _spriteName;
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

    public Avatar(AvatarStatus avtarStatus, AvatarType avatarType, string spriteName)
    {
        this._avtarStatus = avtarStatus;
        this._spriteName = spriteName;
        this._avatarType = avatarType;
    }
}
