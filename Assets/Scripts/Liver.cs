using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Liver : MonoBehaviour
{
    public int MaxHealth = 2;
    public int Health = 1;
    public bool IsInvincible;
    public float InvincibilityDuration = 0.25f;
    public RectTransform Ui;

    private float startUiWidth;
    private float invincibilityTime = 0;

    public void SetUi(RectTransform ui)
    {
        this.Ui = ui;
        startUiWidth = ui.sizeDelta.x;
    }

    void Start()
    {
        if (Ui != null)
        {
            startUiWidth = Ui.sizeDelta.x;
        }
    }

    void Update()
    {
        if (IsInvincible)
        {
            invincibilityTime -= Time.deltaTime;
            if (invincibilityTime <= 0)
            {
                IsInvincible = false;
            }
        }
        if (Ui != null)
        {
            float percentage = (float)Health / (float)MaxHealth;
            Ui.sizeDelta = new Vector2(startUiWidth * percentage, Ui.sizeDelta.y);
        }
    }

    public void TakeDamage(int damage = 1, GameObject other = null)
    {
        if (IsInvincible) return;
        Health -= damage;
        IsInvincible = true;
        invincibilityTime = InvincibilityDuration;
        Sounds.Play("Hit");

        if (other)
        {
            IHurter hurter = other.GetComponent<IHurter>();

            if (hurter != null)
            {
                hurter.LandedHit(gameObject);
            }
        }
    }
}
