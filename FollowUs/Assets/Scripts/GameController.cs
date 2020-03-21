using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class GameController : MonoBehaviour
{
    [SerializeField]
    private List<GameObject> heros = new List<GameObject>();
    private GameObject _newHero;
    private GameObject _currentEnemies;

    [SerializeField]
    private float _speed = 3f;
    private float _timeForReSpawn = 0f;
    private float _maxTimeForReSpawn = 5f;

    private string heroName = "hero_";
    private string enemyName = "enemy_";

    public enum GameMode { ReadyToPlay, Playing, Combat, Lose };
    private GameMode _currentGameMode = GameMode.ReadyToPlay;

    void Start()
    {
        heros.Add(
            spawnNewHero(heroName + Random.Range(1, 15), Avatar.AvatarStatus.Hero_leader, Avatar.AvatarType.Blue, new Vector3(0,0,0))
        );
    }

    void Update()
    {
        if (heros.Count > 0)
        {
            ChangeDirection();
            Heros_Run();
            if (heros[0].GetComponent<Hero>().IsDead())
            {
                Destroy(heros[0]);
                heros.RemoveAt(0);
            }
        }
        if (heros.Count == 0)
        {
            _currentGameMode = GameMode.Lose;
        }

        //spawn new avatar
        if (_currentGameMode == GameMode.Playing)
        {
            _timeForReSpawn += Time.deltaTime;
            if ((_timeForReSpawn >= _maxTimeForReSpawn) &&
                (_newHero == null && _currentEnemies == null))
            {
                _newHero = spawnNewHero(
                    heroName + Random.Range(1, 16),
                    Avatar.AvatarStatus.Hero_Unknow,
                    Avatar.AvatarType.Blue,
                    new Vector3(Random.Range(-4, 4), Random.Range(-3.5f, 3.5f), 0)
                );
                _timeForReSpawn = 0f;
            }
        }
    }

    void Heros_Run()
    {
        try
        {
            for (int index = 0; index < heros.Count; index++)
            {
                if (heros[index].GetComponent<Hero>().GetAvatarStatus() == Avatar.AvatarStatus.Hero_leader)
                {
                    var direction = heros[index].GetComponent<Hero>().GetDirection();
                    if (direction == Hero.Direction.Right)
                        heros[index].GetComponent<SpriteRenderer>().flipX = false;
                    else if (direction == Hero.Direction.Left)
                        heros[index].GetComponent<SpriteRenderer>().flipX = true;
                    var x = (direction == Hero.Direction.Right ? 0.5f : (direction == Hero.Direction.Left ? -0.5f : 0));
                    var y = (direction == Hero.Direction.Top ? 0.5f : (direction == Hero.Direction.Bottom ? -0.5f : 0));
                    heros[index].transform.position += new Vector3(x, y, 0) * Time.deltaTime * _speed;
                    heros[index].GetComponent<Hero>().SetNextPosition(heros[index].transform.position);
                }
                else
                {
                    /* follow front */
                    //heros[index].transform.position += new Vector3(0.5f, 0.0f, 0) * Time.deltaTime * _speed;
                }
            }
        } catch (System.Exception e)
        {
            Debug.Log(e);
        }
    }

    public GameObject spawnNewHero(string name, Avatar.AvatarStatus avatarStatus, Avatar.AvatarType avatarType, Vector3 position)
    {
        GameObject newGameObj = new GameObject();
        newGameObj.name = name;
        newGameObj.transform.position = position;
        newGameObj.transform.localScale = new Vector3(2, 2, 1);

        Hero avatarComponent = newGameObj.AddComponent<Hero>();
        avatarComponent.initialHero(avatarStatus, avatarType, newGameObj.name, Hero.Direction.Right);
        Sprite[] spriteMulti = Resources.LoadAll<Sprite>("Sprites/Avatars");
        Sprite sprite = spriteMulti.Single(s => s.name == newGameObj.name);

        newGameObj.AddComponent<SpriteRenderer>();
        newGameObj.GetComponent<SpriteRenderer>().sprite = sprite;
        newGameObj.AddComponent<BoxCollider2D>();
        newGameObj.AddComponent<Rigidbody2D>();
        newGameObj.GetComponent<Rigidbody2D>().gravityScale = 0;
        newGameObj.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeRotation;

        return newGameObj;
    }

    void ChangeDirection()
    {
        try
        {
            if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                GameObject leader = heros.Single(hero => hero.GetComponent<Hero>().GetAvatarStatus() == Avatar.AvatarStatus.Hero_leader);
                leader.GetComponent<Hero>().SetDirection(Hero.Direction.Top);
            }
            if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                GameObject leader = heros.Single(hero => hero.GetComponent<Hero>().GetAvatarStatus() == Avatar.AvatarStatus.Hero_leader);
                leader.GetComponent<Hero>().SetDirection(Hero.Direction.Bottom);
            }
            if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                GameObject leader = heros.Single(hero => hero.GetComponent<Hero>().GetAvatarStatus() == Avatar.AvatarStatus.Hero_leader);
                leader.GetComponent<Hero>().SetDirection(Hero.Direction.Left);
            }
            if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                GameObject leader = heros.Single(hero => hero.GetComponent<Hero>().GetAvatarStatus() == Avatar.AvatarStatus.Hero_leader);
                leader.GetComponent<Hero>().SetDirection(Hero.Direction.Right);
            }
        } catch (System.Exception e)
        {
            Debug.Log(e);
        }
    }

    void JoinParty()
    {

    }
}
