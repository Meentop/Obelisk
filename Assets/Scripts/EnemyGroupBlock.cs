using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyGroupBlock : MonoBehaviour
{
    [SerializeField] Sprite[] enemyTypeSprites;
    [SerializeField] EnemyDescriptionButton enemyDescriptionButton;
    [Space]
    [SerializeField] Sprite lowDensity;
    [SerializeField] Sprite highDensity;
    [SerializeField] Image density;
    [Space]
    [SerializeField] Text healthCount, enemyCount;

    public void Initialization(EnemyType enemyType, bool lowDensity, int healthCount, int enemyCount)
    {
        enemyDescriptionButton.GetComponent<Image>().sprite = enemyTypeSprites[(int)enemyType];
        enemyDescriptionButton.type = enemyType;
        if (lowDensity)
            density.sprite = this.lowDensity;
        else
            density.sprite = highDensity;
        this.healthCount.text = healthCount.ToString();
        this.enemyCount.text = enemyCount.ToString();
    }
}
