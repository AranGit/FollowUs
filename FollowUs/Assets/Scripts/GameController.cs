using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class GameController : MonoBehaviour
{
    private List<GameObject> heros = new List<GameObject>();
    private List<Enemy> Enemies = new List<Enemy>();

    public enum GameMode { ReadyToPlay, Playing, Combat, Lose };
    private GameMode currentGameMode = GameMode.ReadyToPlay;

    void Start()
    {
        GameObject newGameObj = new GameObject();
        newGameObj.name = "hero_0";
        Hero firstHero = new Hero(Avatar.AvatarStatus.Hero_leader, Avatar.AvatarType.Blue, newGameObj.name);
        newGameObj.AddComponent<Hero>();
        Sprite[] sprite2dMulti = Resources.LoadAll<Sprite>("Sprites/Avatars");
        Sprite sprite2d = sprite2dMulti.Single(s => s.name == newGameObj.name);
        Debug.Log(sprite2d);
        newGameObj.AddComponent<SpriteRenderer>();
        newGameObj.GetComponent<SpriteRenderer>().sprite = sprite2d;
        newGameObj.transform.position = new Vector3(0, 0, 0);
        heros.Add(newGameObj);
    }

    void Update()
    {
        
    }

}
