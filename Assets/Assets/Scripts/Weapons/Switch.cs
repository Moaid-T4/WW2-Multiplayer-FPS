using UnityEngine;
using System.Collections;
using System;

public class Switch : ControlableBehaviour {

    [SerializeField]
    WeaponSwitch[] weapons;

    [SerializeField]
    bool switching;

    [SerializeField]
    int weapon;

    int weaponExt;

    public KeyCode switchUp = KeyCode.X;
    public KeyCode switchDown = KeyCode.Z;

    public KeyCode shootKey;
    public KeyCode aimKey;

    bool firstSwitch = true;

    void Start () {
        weaponExt = weapon;
        Controls.AddListener(this);
        RefreshBinds();
    }
	

	void Update () {
        if(firstSwitch)
        {
            if (Input.GetKeyDown(shootKey) || Input.GetKeyDown(aimKey))
                StartCoroutine(EquipWeapon(0));
        }

        if (!switching)
        {
            if(Game.MW_Switch)
                weapon += Mathf.RoundToInt(Input.GetAxis("Mouse ScrollWheel"));
            if (Input.GetKeyDown(switchUp))
                weapon++;
            if (Input.GetKeyDown(switchDown))
                weapon--;
        }

        if (weapon < 0)
            weapon = 2;
        if (weapon > 2)
            weapon = 0;

        if (weaponExt != weapon && !switching)
            StartCoroutine(EquipWeapon(weapon));
    }

    IEnumerator EquipWeapon(int index)
    {
        if (firstSwitch)
        {
            weapon = 0;
            weapons[0].Weapon.SetActive(true);
            firstSwitch = false;

            switching = true;

            yield return new WaitForSeconds(weapons[0].SwitchClip.length);
        }

        else
        {

            switching = true;
            weapons[weaponExt].Weapon.SendMessage("UnEquip");

            yield return new WaitForSeconds(weapons[weaponExt].SwitchClip.length);

            for (int i = 0; i < weapons.Length; i++)
            {
                if (i == index)
                {
                    weapons[i].Weapon.SetActive(true);
                }
                else
                    weapons[i].Weapon.SetActive(false);
            }
            weaponExt = weapon;
        }

        switching = false;
    }

    public void UnHolster()
    {
        StartCoroutine(EquipWeapon(0));
    }

    public override void RefreshBinds()
    {
        shootKey = Controls.controls["On Foot.Shoot"];
        aimKey = Controls.controls["On Foot.Aim"];

        switchUp = Controls.controls["On Foot.Switch To Next"];
        switchDown = Controls.controls["On Foot.Switch To Previous"]; 
    }
}

[System.Serializable]
class WeaponSwitch
{
    [SerializeField]
    GameObject weapon;
    [SerializeField]
    AnimationClip switchClip;

    public GameObject Weapon { get { return weapon; } }
    public AnimationClip SwitchClip { get { return switchClip; } }
}
