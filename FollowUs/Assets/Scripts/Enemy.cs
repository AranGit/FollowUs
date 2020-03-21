using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : Avatar
{
    public void initialEnemy(AvatarType avatarType, string name)
    {
        Heart = Random.Range(1, 11);
        Sword = Random.Range(1, 11);
        Shield = Random.Range(1, 11);
        Name = name;
        SetAvatarStatus(AvatarStatus.Enemy);
        SetAvatarType(avatarType);
    }
}
