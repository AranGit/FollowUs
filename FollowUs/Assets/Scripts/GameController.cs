using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class GameController : MonoBehaviour
{

    private UIController ui_controller;
    [SerializeField]
    private List<GameObject> heros = new List<GameObject>();
    private List<GameObject> enemies = new List<GameObject>();
    private GameObject _newHero;
    private GameObject _currentEnemies;

    private float _speed = 5f;
    private float _timeForReSpawn = 0f;
    private float _maxTimeForReSpawn = 5f;

    private float point = 0;

    private string heroName = "hero_";
    private string enemyName = "enemy_";

    public enum SpawnObject { Hero, Enemy };
    public enum CombatResualt { Win, Lose, Tie };

    public enum GameMode { ReadyToPlay, Playing, Combat, Fighting };
    private GameMode _currentGameMode = GameMode.ReadyToPlay;

    private void Awake()
    {
        ui_controller = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<UIController>();
    }

    void Start()
    {
        heros.Add(
            spawnNewHero(heroName + Random.Range(1, 16), 
                        Avatar.AvatarStatus.Hero_leader, 
                        (Avatar.AvatarType)Random.Range(0, 3),
                        new Vector3(0,0,0)
            )
        );
        ui_controller.SetHeroState(
            heros[0].GetComponent<Hero>().Heart, 
            heros[0].GetComponent<Hero>().Sword,
            heros[0].GetComponent<Hero>().Shield,
            heros[0].GetComponent<Hero>().GetAvatarType()
        );

    }

    void Update()
    {
        if (_currentGameMode == GameMode.ReadyToPlay)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                Debug.Log("Start Game");
                point = 0;
                ui_controller.SetPoints(point);
                ui_controller.start_text.text = "";
                ui_controller.SetActivePanelEnemy(false);

                StartCoroutine(StartGame());
            }
        }
        if (_currentGameMode == GameMode.Playing)
        {
            if (heros.Count > 0)
            {
                ChangeDirection();
                SwitchLeader(true);
                Heros_Run();
                CheckLeaderAlive();
            }

            JoinParty();

            _timeForReSpawn += Time.deltaTime;
            if ((_timeForReSpawn >= _maxTimeForReSpawn) &&
                (_newHero == null && _currentEnemies == null))
            {
                SpawnAvartar();
                _timeForReSpawn = 0f;
            }

            if (heros.Count > 0)
            {
                if (heros[0].GetComponent<Hero>().Fighting && _currentEnemies)
                {
                    _timeForReSpawn = 0;
                    _currentEnemies.GetComponent<SpriteRenderer>().flipX = !heros[0].GetComponent<SpriteRenderer>().flipX;
                    _currentGameMode = GameMode.Combat;
                }
            }
        }
        if (_currentGameMode == GameMode.Combat)
        {
            _currentGameMode = GameMode.Fighting;
            StartCoroutine(Combat());
        }
    }

    void Heros_Run()
    {
        try
        {
            for (int index = 0; index < heros.Count; index++)
            {
                if (index == 0)
                {
                    if (heros[index].GetComponent<Hero>().GetAvatarStatus() != Avatar.AvatarStatus.Hero_leader)
                        heros[index].GetComponent<Hero>().SetAvatarStatus(Avatar.AvatarStatus.Hero_leader);

                    heros[index].GetComponent<Hero>().SetNextPosition(heros[index].transform.position);
                    var direction = heros[index].GetComponent<Hero>().GetDirection();
                    if (direction == Hero.Direction.Right)
                        heros[index].GetComponent<SpriteRenderer>().flipX = false;
                    else if (direction == Hero.Direction.Left)
                        heros[index].GetComponent<SpriteRenderer>().flipX = true;
                    var x = (direction == Hero.Direction.Right ? 0.5f : (direction == Hero.Direction.Left ? -0.5f : 0));
                    var y = (direction == Hero.Direction.Top ? 0.5f : (direction == Hero.Direction.Bottom ? -0.5f : 0));
                    heros[index].transform.position += new Vector3(x, y, 0) * Time.deltaTime * _speed;
                }
                else
                {
                    /* follow front */
                    if (heros[index].GetComponent<Hero>().GetAvatarStatus() != Avatar.AvatarStatus.Hero_following)
                        heros[index].GetComponent<Hero>().SetAvatarStatus(Avatar.AvatarStatus.Hero_following);
                    heros[index].GetComponent<Hero>().SetNextPosition(heros[index].transform.position);
                    var frontHero = heros[index - 1];
                    var nextPosition = frontHero.GetComponent<Hero>().GetNextPosition();
                    if (Vector2.Distance(nextPosition, heros[index].transform.position) > 0.5f)
                    {
                        heros[index].transform.position = Vector2.MoveTowards(heros[index].transform.position, nextPosition, Time.deltaTime * _speed);
                    }
                    heros[index].GetComponent<SpriteRenderer>().flipX = frontHero.GetComponent<SpriteRenderer>().flipX;
                }
            }
        } catch (System.Exception e)
        {
            Debug.Log(e);
        }
    }

    void AddStar(GameObject avatarObject, Avatar.AvatarStatus avatarStatus)
    {
        GameObject starObject = new GameObject();
        starObject.name = "Star";
        Sprite starSprite = Resources.Load<Sprite>("Sprites/Yellow_Star");
        starObject.AddComponent<SpriteRenderer>();
        starObject.GetComponent<SpriteRenderer>().sprite = starSprite;
        starObject.transform.SetParent(avatarObject.transform);
        starObject.transform.localScale = new Vector3(0.05f, 0.05f, 1);
        starObject.transform.localPosition = new Vector3(0, 0.5f, 0);
        if (avatarStatus != Avatar.AvatarStatus.Hero_leader)
        {
            starObject.SetActive(false);
        }
    }
    public GameObject spawnNewHero(string name, Avatar.AvatarStatus avatarStatus, Avatar.AvatarType avatarType, Vector3 position)
    {
        GameObject avatarObject = new GameObject();
        avatarObject.name = name;
        avatarObject.tag = "Hero";
        avatarObject.transform.position = position;
        avatarObject.transform.localScale = new Vector3(2, 2, 1);

        AddStar(avatarObject, avatarStatus);

        Hero avatarComponent = avatarObject.AddComponent<Hero>();
        avatarComponent.initialHero(avatarStatus, avatarType, avatarObject.name, Hero.Direction.Right);
        Sprite[] spriteMulti = Resources.LoadAll<Sprite>("Sprites/Avatars");
        Sprite sprite = spriteMulti.Single(s => s.name == avatarObject.name);

        avatarObject.AddComponent<SpriteRenderer>();
        avatarObject.GetComponent<SpriteRenderer>().sprite = sprite;
        avatarObject.AddComponent<BoxCollider2D>();
        avatarObject.AddComponent<Rigidbody2D>();
        avatarObject.GetComponent<Rigidbody2D>().gravityScale = 0;
        avatarObject.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeRotation;

        return avatarObject;
    }

    public GameObject spawnEnemy(string name, Avatar.AvatarType avatarType, Vector3 position)
    {
        GameObject newGameObj = new GameObject();
        newGameObj.name = name;
        newGameObj.tag = "Enemy";
        newGameObj.transform.position = position;
        newGameObj.transform.localScale = new Vector3(2, 2, 1);

        Enemy avatarComponent = newGameObj.AddComponent<Enemy>();
        avatarComponent.initialEnemy(avatarType, newGameObj.name);
        Sprite[] spriteMulti = Resources.LoadAll<Sprite>("Sprites/Avatars");
        Sprite sprite = spriteMulti.Single(s => s.name == newGameObj.name);

        newGameObj.AddComponent<SpriteRenderer>();
        newGameObj.GetComponent<SpriteRenderer>().sprite = sprite;
        newGameObj.AddComponent<BoxCollider2D>();
        newGameObj.AddComponent<Rigidbody2D>();
        newGameObj.GetComponent<Rigidbody2D>().gravityScale = 0;
        newGameObj.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeAll;

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

    IEnumerator StartGame()
    {
        yield return new WaitForSeconds(1);
        _currentGameMode = GameMode.Playing;
    }

    void ClearRigibodyAndBoxCollider(GameObject hero)
    {
        var rigidbody2D = hero.GetComponent<Rigidbody2D>();
        Destroy(rigidbody2D);
        var boxCollider2D = hero.GetComponent<BoxCollider2D>();
        boxCollider2D.enabled = false;
    }

    void SwitchSecondHeroToBeLeader()
    {
        var oldLeader = heros[0];
        heros.RemoveAt(0);
        heros.Add(oldLeader);
        heros[0].GetComponent<Hero>().SetAvatarStatus(Avatar.AvatarStatus.Hero_leader);
        heros[0].GetComponent<BoxCollider2D>().enabled = true;
        heros[0].AddComponent<Rigidbody2D>();
        heros[0].GetComponent<Rigidbody2D>().gravityScale = 0;
        heros[0].GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeRotation;
        heros[0].GetComponent<Hero>().SetDirection(oldLeader.GetComponent<Hero>().GetDirection());
        ui_controller.SetHeroState(
            heros[0].GetComponent<Hero>().Heart,
            heros[0].GetComponent<Hero>().Sword,
            heros[0].GetComponent<Hero>().Shield,
            heros[0].GetComponent<Hero>().GetAvatarType()
        );
        if (heros[0].transform.GetChild(0).transform.name == "Star")
        {
            heros[0].transform.GetChild(0).gameObject.SetActive(true);
        }
        heros[heros.Count - 1].GetComponent<Hero>().SetAvatarStatus(Avatar.AvatarStatus.Hero_following);
        if (heros[heros.Count - 1].transform.GetChild(0).transform.name == "Star")
        {
            heros[heros.Count - 1].transform.GetChild(0).gameObject.SetActive(false);
        }
        ClearRigibodyAndBoxCollider(heros[heros.Count - 1]);
    }
    void SwitchLastHeroToBeLeader()
    {
        var newLeader = heros[heros.Count - 1];
        heros.RemoveAt(heros.Count - 1);
        heros.Insert(0, newLeader);

        heros[0].GetComponent<Hero>().SetAvatarStatus(Avatar.AvatarStatus.Hero_leader);
        heros[0].GetComponent<BoxCollider2D>().enabled = true;
        heros[0].AddComponent<Rigidbody2D>();
        heros[0].GetComponent<Rigidbody2D>().gravityScale = 0;
        heros[0].GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeRotation;
        heros[0].GetComponent<Hero>().SetDirection(heros[1].GetComponent<Hero>().GetDirection());
        ui_controller.SetHeroState(
            heros[0].GetComponent<Hero>().Heart,
            heros[0].GetComponent<Hero>().Sword,
            heros[0].GetComponent<Hero>().Shield,
            heros[0].GetComponent<Hero>().GetAvatarType()
        );
        if (heros[0].transform.GetChild(0).transform.name == "Star")
        {
            heros[0].transform.GetChild(0).gameObject.SetActive(true);
        }
        heros[1].GetComponent<Hero>().SetAvatarStatus(Avatar.AvatarStatus.Hero_following);
        if (heros[1].transform.GetChild(0).transform.name == "Star")
        {
            heros[1].transform.GetChild(0).gameObject.SetActive(false);
        }
        ClearRigibodyAndBoxCollider(heros[1]);
    }

    void SwitchLeader(bool isKeyCode)
    {
        if (heros.Count > 1)
        {
            if (isKeyCode)
            {
                if (Input.GetKeyDown(KeyCode.Z))
                {
                    SwitchSecondHeroToBeLeader();
                }
                if (Input.GetKeyDown(KeyCode.X))
                {
                    SwitchLastHeroToBeLeader();
                }
            } else
            {
                SwitchSecondHeroToBeLeader();
            }
        }
    }
    
    void SpawnAvartar()
    {
        try
        {
            // 0 = hero, 1 = enemy
            var spawnObject = (SpawnObject)Random.Range(0, 2);
            if (spawnObject == SpawnObject.Hero)
            {
                _newHero = spawnNewHero(
                    heroName + Random.Range(1, 16),
                    Avatar.AvatarStatus.Hero_Unknow,
                    (Avatar.AvatarType)Random.Range(0, 3),
                    new Vector3(Random.Range(-4, 4), Random.Range(-3.5f, 3.5f), 0)
                );
            }
            else
            {
                _currentEnemies = spawnEnemy(
                    enemyName + Random.Range(1, 16),
                    (Avatar.AvatarType)Random.Range(0, 3),
                    new Vector3(Random.Range(-4, 4), Random.Range(-3.5f, 3.5f), 0));
                ui_controller.SetActivePanelEnemy(true);
                ui_controller.SetEnemyState(
                    _currentEnemies.GetComponent<Enemy>().Heart,
                    _currentEnemies.GetComponent<Enemy>().Sword,
                    _currentEnemies.GetComponent<Enemy>().Shield,
                    _currentEnemies.GetComponent<Enemy>().GetAvatarType()
                );
            }
        } catch (System.Exception e)
        {
            Debug.Log(e);
        }
    }

    void CheckLeaderAlive()
    {
        try
        {
            if (heros[0].GetComponent<Hero>().IsDead())
            {
                if (heros.Count > 1)
                {
                    // Change leader and direction
                    heros[1].GetComponent<Hero>().SetDirection(heros[0].GetComponent<Hero>().GetDirection());
                    heros[1].GetComponent<BoxCollider2D>().enabled = true;
                    heros[1].AddComponent<Rigidbody2D>();
                    heros[1].GetComponent<Rigidbody2D>().gravityScale = 0;
                    heros[1].GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeRotation;
                    ui_controller.SetHeroState(
                        heros[1].GetComponent<Hero>().Heart,
                        heros[1].GetComponent<Hero>().Sword,
                        heros[1].GetComponent<Hero>().Shield,
                        heros[1].GetComponent<Hero>().GetAvatarType()
                    );
                    if (heros[1].transform.GetChild(0).transform.name == "Star")
                    {
                        heros[1].transform.GetChild(0).gameObject.SetActive(true);
                    }
                }
                Destroy(heros[0]);
                heros.RemoveAt(0);
                if (heros.Count <= 0)
                {
                    LoseState();
                }
            }
        } catch (System.Exception e)
        {
            Debug.Log(e);
        }
    }

    void JoinParty()
    {
        if (_newHero)
        {
            if (_newHero.GetComponent<Hero>().GetAvatarStatus() == Avatar.AvatarStatus.Hero_following)
            {
                ClearRigibodyAndBoxCollider(_newHero);
                heros.Add(_newHero);
                _newHero = null;
            }
        }
    }

    CombatResualt InBattle()
    {
        var leader = heros.Single(hero => hero.GetComponent<Hero>().GetAvatarStatus() == Avatar.AvatarStatus.Hero_leader);
        bool heroTurn = true;
        CombatResualt resualt = CombatResualt.Win;

        float heroDamage = 999999f;
        float enemyDamage = 999999f;

        if (_currentEnemies)
        {
            while (_currentEnemies.GetComponent<Enemy>().Heart > 0 || heros.Count > 0)
            {
                if (heroTurn)
                {
                    heroDamage = _currentEnemies.GetComponent<Enemy>().GotDamage(
                        leader.GetComponent<Hero>().Sword,
                        leader.GetComponent<Hero>().GetAvatarType());
                    Debug.Log("Enemies left HP: " + _currentEnemies.GetComponent<Enemy>().Heart);
                    heroTurn = false;
                } else
                {
                    enemyDamage = leader.GetComponent<Hero>().GotDamage(
                        _currentEnemies.GetComponent<Enemy>().Sword,
                        _currentEnemies.GetComponent<Enemy>().GetAvatarType());
                    Debug.Log("Leader left HP: " + leader.GetComponent<Hero>().Heart);
                    heroTurn = true;
                }
                if (heroDamage == 0 && enemyDamage == 0)
                {
                    SwitchLeader(false);
                    Destroy(heros[heros.Count - 1]);
                    heros.RemoveAt(heros.Count - 1);
                    if (heros.Count == 0)
                    {
                        resualt = CombatResualt.Lose;
                        break;
                    }
                    resualt = CombatResualt.Tie;
                    break;
                }
                if (leader.GetComponent<Hero>().Heart < 1)
                {
                    SwitchLeader(false);
                    Destroy(heros[heros.Count - 1]);
                    heros.RemoveAt(heros.Count - 1);
                    if (heros.Count == 0)
                    {
                        resualt = CombatResualt.Lose;
                        break;
                    }
                    leader = heros.Single(hero => hero.GetComponent<Hero>().GetAvatarStatus() == Avatar.AvatarStatus.Hero_leader);
                    leader.GetComponent<Hero>().Fighting = true;
                    heroTurn = true;
                }
                if (_currentEnemies.GetComponent<Enemy>().Heart < 1)
                {
                    resualt = CombatResualt.Win;
                    break;
                }
                if (heros.Count < 1)
                {
                    resualt = CombatResualt.Lose;
                    break;
                }

            };
            return resualt;
        } else
        {
            return resualt;
        }
    }

    IEnumerator Combat()
    {
        CombatResualt resualt = InBattle();
        Debug.Log("Resualt battle: " + resualt);

        yield return new WaitForSeconds(0.5f);

        if (resualt == CombatResualt.Win)
        {
            var leader = heros.Single(hero => hero.GetComponent<Hero>().GetAvatarStatus() == Avatar.AvatarStatus.Hero_leader);
            leader.GetComponent<Hero>().Fighting = false;
            ui_controller.SetHeroState(
                        leader.GetComponent<Hero>().Heart,
                        leader.GetComponent<Hero>().Sword,
                        leader.GetComponent<Hero>().Shield,
                        leader.GetComponent<Hero>().GetAvatarType()
                    );
            _speed += 1;
            point += leader.GetComponent<Hero>().Heart;
            ui_controller.SetPoints(point);
            Destroy(_currentEnemies);
            _currentEnemies = null;
            ui_controller.SetActivePanelEnemy(false);
        } else if (resualt == CombatResualt.Tie)
        {
            var leader = heros.Single(hero => hero.GetComponent<Hero>().GetAvatarStatus() == Avatar.AvatarStatus.Hero_leader);
            leader.GetComponent<Hero>().Fighting = false;
            ui_controller.SetHeroState(
                        leader.GetComponent<Hero>().Heart,
                        leader.GetComponent<Hero>().Sword,
                        leader.GetComponent<Hero>().Shield,
                        leader.GetComponent<Hero>().GetAvatarType()
                    );
            Destroy(_currentEnemies);
            _currentEnemies = null;
            ui_controller.SetActivePanelEnemy(false);
        }

        if (heros.Count < 1)
        {
            LoseState();
        } 
        else if (_currentEnemies == null)
        {
            _currentGameMode = GameMode.Playing;
        }
    }

    void LoseState()
    {
        _currentGameMode = GameMode.ReadyToPlay;
        ui_controller.start_text.text = "You lose. Please spacebar to play again";
        heros = new List<GameObject>();
        enemies = new List<GameObject>();
        _newHero = null;
        _currentEnemies = null;
        _speed = 5f;

        heros.Add(
            spawnNewHero(heroName + Random.Range(1, 16),
                        Avatar.AvatarStatus.Hero_leader,
                        (Avatar.AvatarType)Random.Range(0, 3),
                        new Vector3(0, 0, 0)
            )
        );
        ui_controller.SetHeroState(
            heros[0].GetComponent<Hero>().Heart,
            heros[0].GetComponent<Hero>().Sword,
            heros[0].GetComponent<Hero>().Shield,
            heros[0].GetComponent<Hero>().GetAvatarType()
        );
    }
}
