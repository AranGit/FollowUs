using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hero : Avatar
{
    private Transform _nextTranform;

    public Hero(AvatarStatus avtarStatus, AvatarType avatarType, string spriteName) : base(avtarStatus, avatarType, spriteName)
    {

    }

    public void SetNextTranform(Transform value)
    {
        _nextTranform = value;
    }

    public Transform GetNextTranform()
    {
        return _nextTranform;
    }
    
    public void spawnAvatar(AvatarStatus avtarStatus, Transform currentTransform)
    {
        SetAvatarStatus(avtarStatus);
        SetNextTranform(currentTransform);
    }

    private void Awake()
    {
        Heart = Random.Range(1, 11);
        Sword = Random.Range(1, 11);
        Shield = Random.Range(1, 11);
    }
}
