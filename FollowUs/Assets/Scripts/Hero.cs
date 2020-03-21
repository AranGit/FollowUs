using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hero : Avatar
{
    public enum Direction { Top, Bottom, Left, Right };
    private Vector3 _nextPosition;
    private Direction _currentDirection;
    private bool _isDead = false;

    public bool IsDead()
    {
        return _isDead;
    }
    public void initialHero(AvatarStatus avtarStatus, AvatarType avatarType, string name, Direction direction)
    {
        Heart = Random.Range(1, 11);
        Sword = Random.Range(1, 11);
        Shield = Random.Range(1, 11);
        Name = name;
        SetAvatarStatus(avtarStatus);
        SetAvatarType(avatarType);
        _currentDirection = direction;
    }
    public void SetDirection(Direction value)
    {
        _currentDirection = value;
    }

    public Direction GetDirection()
    {
        return _currentDirection;
    }
    public void SetNextPosition(Vector3 value)
    {
        _nextPosition = value;
    }

    public Vector3 GetNextTranform()
    {
        return _nextPosition;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("OnTriggerEnter2D");
        if (collision.gameObject.name == "border" && GetAvatarStatus() == AvatarStatus.Hero_leader)
        {
            _isDead = true;
        }
    }
}
