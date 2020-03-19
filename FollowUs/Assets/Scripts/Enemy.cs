using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : Avatar
{
    public Enemy(AvatarType avatarType, string spriteName) : base(AvatarStatus.Enemy, avatarType, spriteName)
    {

    }

    private void Awake()
    {
        Heart = Random.Range(1, 11);
        Sword = Random.Range(1, 11);
        Shield = Random.Range(1, 11);
    }
}
