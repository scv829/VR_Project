using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

// 텔레포트 관리
public class GameManager : MonoBehaviour
{
    [SerializeField] Tower tower;
    [SerializeField] Enemy enemy;

    public void TowerTeleport()
    {
        tower.StartShooting();
    }

    public void ExitTowerGround()
    {
        tower.StopShooting();
    }

    public void MonsterTeleport()
    {
        enemy.StartMove();
    }

    public void ExitMonsterGround()
    {
        enemy.Reset();
    }


}
