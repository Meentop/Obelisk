using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WaveBlock : MonoBehaviour
{
    [SerializeField] GameObject blockPrefab;
    [SerializeField] Transform verticalGroup;
    [Space]
    [SerializeField] Text order;

    public void InitWaveBlock(int order)
    {
        this.order.text = order.ToString();
    }

    public void InitNewEnemyGroup(EnemyType enemyType, bool lowDensity, int healthCount, int magicCount, int enemyCount)
    {
        EnemyGroupBlock block = Instantiate(blockPrefab, verticalGroup).GetComponent<EnemyGroupBlock>();
        block.Initialization(enemyType, lowDensity, healthCount, magicCount, enemyCount);
        GetComponent<RectTransform>().sizeDelta = new Vector3(GetComponent<RectTransform>().sizeDelta.x, 10 + (40 * verticalGroup.childCount));
    }
}
