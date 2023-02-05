using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UpgradeManager : MonoBehaviour
{
    private static UpgradeManager _instance;

    public static UpgradeManager Instance
    {
        get
        {
            if (!_instance)
            {
                _instance = FindObjectOfType<UpgradeManager>();
            }

            return _instance;
        }
    }

    public Transform upgradeUI;
    public TextMeshProUGUI priceText;
    public Transform canOpenUpgradeText;
    public ParticleSystem canUpgradePS;
    
    public float extraWalkSpeed;
    public float extraAttackRange;
    public int extraAttackDamage;
    public int upgradedAmount;

    public int CurrUpgradePrice =>
        GameConstants.InitUpgradePrice + GameConstants.UpgradePriceIncreaseRate * upgradedAmount;
    
    public void Init()
    {
        extraWalkSpeed = 0;
        extraAttackRange = 0;
        extraAttackDamage = 0;
        upgradedAmount = 0;
        upgradeUI.gameObject.SetActive(false);
        canUpgradePS.gameObject.SetActive(false);
        UpdatePriceText();
    }

    
    
    public void UpdateManager()
    {
        bool canUpgrade = GameplayManager.Instance.RootAmount >= CurrUpgradePrice;
        canUpgradePS.gameObject.SetActive(canUpgrade);
        bool canOpenUpgrade = canUpgrade && PlayerRuntime.Instance.transform.position.magnitude < 15;
        canOpenUpgradeText.gameObject.SetActive(canOpenUpgrade);
        if (canOpenUpgrade && Input.GetKeyDown(KeyCode.Space))
        {
            OpenUpgradeUI();
        }
    }

    public void ClickWalkSpeedUpgrade()
    {
        extraWalkSpeed += GameConstants.WalkSpeedUpgradeAmount;
        AfterClickUpgrade();
    }

    public void ClickAttackRangeUpgrade()
    {
        extraAttackRange += GameConstants.AttackRangeUpgradeAmount;
        AfterClickUpgrade();
    }

    public void ClickAttackDamageUpgrade()
    {
        extraAttackDamage += GameConstants.AttackDamageUpgradeAmount;
        AfterClickUpgrade();
    }

    public void OpenUpgradeUI()
    {
        upgradeUI.gameObject.SetActive(true);
        Time.timeScale = 0;
    }

    public AudioClip upgradeSFX;

    public void AfterClickUpgrade()
    {
        upgradeUI.gameObject.SetActive(false);
        Time.timeScale = 1;
        GameplayManager.Instance.RootAmount -= CurrUpgradePrice;
        upgradedAmount++;
        PlayerRuntime.Instance.audioSource.PlayOneShot(upgradeSFX);
        UpdatePriceText();
    }

    public void UpdatePriceText()
    {
        priceText.text = CurrUpgradePrice.ToString();
    }
}
