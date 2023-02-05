using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GameConstants
{
    public const int MapWidth = 120;
    public const int MapHeight = 120;

    public const int SineIncrement = 15;
    public const float CenterDragForce = 0.12f;
    public const int ScaleAmount = 5;

    public const int NexusMaxHealth = 50;

    public const float StartSpawnRootCooldown = 10;
    public const float ThreeMinuteSpawnRootCooldown = 1f;

    public const float WalkSpeedUpgradeAmount = 10;
    public const float AttackRangeUpgradeAmount = 1.5f;
    public const int AttackDamageUpgradeAmount = 1;

    public const int InitUpgradePrice = 200;
    public const int UpgradePriceIncreaseRate = 100;
}
