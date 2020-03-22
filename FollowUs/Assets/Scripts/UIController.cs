using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject panel_Enemy;
    public Text enemy_text_hp;
    public Text enemy_text_sword;
    public Text enemy_text_shield;
    public Text enemy_text_type;

    public Text hero_text_hp;
    public Text hero_text_sword;
    public Text hero_text_shield;
    public Text hero_text_type;

    public Text points;

    public Text start_text;

    string AvatarTypeToString(Text txt, Avatar.AvatarType type)
    {
        if (type == Avatar.AvatarType.Blue)
        {
            txt.color = Color.blue;
            return "Blue";
        }
        else if (type == Avatar.AvatarType.Red)
        {
            txt.color = Color.red;
            return "Red";
        }
        else
        {
            txt.color = Color.green;
            return "Green";
        }
    }

    public void SetHeroState(float hp, float sword, float shield, Avatar.AvatarType type)
    {
        hero_text_hp.text = hp.ToString();
        hero_text_sword.text = sword.ToString();
        hero_text_shield.text = shield.ToString();
        var typeString = AvatarTypeToString(hero_text_type, type);
        hero_text_type.text = typeString;
    }

    public void SetEnemyState(float hp, float sword, float shield, Avatar.AvatarType type)
    {
        enemy_text_hp.text = hp.ToString();
        enemy_text_sword.text = sword.ToString();
        enemy_text_shield.text = shield.ToString();
        var typeString = AvatarTypeToString(enemy_text_type, type);
        enemy_text_type.text = typeString;
    }

    public void SetActivePanelEnemy(bool isActive)
    {
        panel_Enemy.SetActive(isActive);
    }

    public void SetPoints(float point)
    {
        points.text = point.ToString();
    }
}
