
using UnityEngine;

public class Avatar : MonoBehaviour
{
    public enum AvatarStatus { Hero_leader, Hero_following, Hero_Unknow, Enemy };
    public enum AvatarType { Red, Green, Blue };
    private float _heart;
    private float _sword;
    private float _shield;

    private string _name;
    private AvatarStatus _avtarStatus;
    private AvatarType _avatarType;

    public float Heart
    {
        get { return _heart; }
        set { _heart = value; }
    }
    public float Sword
    {
        get { return _sword; }
        set { _sword = (value < 1 ? 1 : value); }
    }

    public float Shield
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

    // Red > Green > Blue > Red
    float DamageFromType(AvatarType avatarTypeAttacker)
    {
        float extraDamage = 1;
        if (avatarTypeAttacker == AvatarType.Blue)
        {
            if (_avatarType == AvatarType.Red)
            {
                extraDamage = 2;
            } else if (_avatarType == AvatarType.Green)
            {
                extraDamage = 0.7f;
            }
        }
        if (avatarTypeAttacker == AvatarType.Green)
        {
            if (_avatarType == AvatarType.Blue)
            {
                extraDamage = 2;
            }
            else if (_avatarType == AvatarType.Red)
            {
                extraDamage = 0.7f;
            }
        }
        if (avatarTypeAttacker == AvatarType.Red)
        {
            if (_avatarType == AvatarType.Green)
            {
                extraDamage = 2;
            }
            else if (_avatarType == AvatarType.Blue)
            {
                extraDamage = 0.7f;
            }
        }
        return extraDamage;
    }

    public float GotDamage(float AvatarSword, AvatarType avatarType)
    {
        var damage = (AvatarSword * DamageFromType(avatarType)) - Shield;
        if (damage < 0 )
        {
            damage = 0;
        }
        Debug.Log("Damage !! : " + damage);
        _heart -= damage;

        return damage;
    }
}
