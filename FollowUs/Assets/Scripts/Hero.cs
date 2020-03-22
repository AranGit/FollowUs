using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hero : Avatar
{
    public enum Direction { Top, Bottom, Left, Right };
    private Vector3 _nextPosition;
    private Direction _currentDirection;
    private bool _isDead = false;
    private bool _fighting = false;
    public bool Fighting
    {
        get { return _fighting; }
        set { _fighting = value; }
    }

    public bool IsDead()
    {
        return _isDead;
    }
    public void HeroDead()
    {
        _isDead = true;
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

    public Vector3 GetNextPosition()
    {
        return _nextPosition;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //Trigger by wall
        if (collision.gameObject.name == "border" && GetAvatarStatus() == AvatarStatus.Hero_leader)
        {
            if (GetDirection() == Direction.Top || GetDirection() == Direction.Bottom)
            {
                SetDirection((Direction)Random.Range(2, 3));
            } else if (GetDirection() == Direction.Left || GetDirection() == Direction.Right)
            {
                SetDirection((Direction)Random.Range(0, 1));
            }
            _isDead = true;
        }

        if (collision.gameObject.tag == "Fire" && GetAvatarStatus() == AvatarStatus.Hero_leader)
        {
            _isDead = true;
            Destroy(collision.gameObject);
        }
    }
    void OnCollisionEnter2D(Collision2D collision)
    {
        //Trigger by leader
        if (collision.gameObject.GetComponent<Hero>())
        {
            if (collision.gameObject.GetComponent<Hero>().GetAvatarStatus() == AvatarStatus.Hero_leader &&
                GetAvatarStatus() == AvatarStatus.Hero_Unknow)
            {
                SetAvatarStatus(AvatarStatus.Hero_following);
            }
        } else if (collision.gameObject.tag == "Enemy")
        {
            _fighting = true;
        }
    }
}
