using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Melee : MonoBehaviour {

    [SerializeField]
    Animator anim;

    [SerializeField]
    AnimationClip attackClip;

    [SerializeField]
    bool attack;

    [SerializeField]
    Vector2 attackDamage;

    [SerializeField]
    float attackDistance;

    [SerializeField]
    LayerMask mask;

    [SerializeField]
    Transform detectionPoint;

    Vector2 mouse;

    Vector3 weaponPosition;
    Vector3 weaponRotation;

    [SerializeField]
    SwayState hipSway = new SwayState(new Vector2(0.001f, 0.001f), new Vector2(0.03f, 0.03f), 0.5f, 10);

    RaycastHit hit;

    bool midWaySprint;

    FirstPersonController fps;

    [SerializeField]
    Text weaponLabel;

    void OnEnable()
    {
        attack = false;
        anim.SetBool("attack", false);
        anim.SetBool("switch", false);

        if (!fps)
            fps = transform.parent.parent.parent.GetComponent<FirstPersonController>();
        fps.currentWeapon = gameObject;

        weaponLabel.text = "-- / --";
    }

    internal void Damage()
    {
        if (!attack)
            return;

        if(Physics.Raycast(detectionPoint.position, detectionPoint.forward, out hit, attackDistance,mask))
        {
            float Damage = Tools.GetRandom(attackDamage);

            if (hit.transform.tag == "prop")
            {
                Prop hitProp = hit.transform.GetComponent<Prop>();

                hitProp.transform.SendMessage("TakeDamage", Tools.GetRandom(attackDamage));
            }

            if (hit.transform.tag == "head")
            {
                BodyPart hitPart = hit.transform.GetComponent<BodyPart>();

                hitPart.Character.SendMessage("TakeDamage", Tools.GetRandom(attackDamage));
            }

            if (hit.transform.tag == "body")
            {
                BodyPart hitPart = hit.transform.GetComponent<BodyPart>();

                hitPart.Character.SendMessage("TakeDamage", Tools.GetRandom(attackDamage));
            }

            if (hit.transform.tag == "limp")
            {
                BodyPart hitPart = hit.transform.GetComponent<BodyPart>();

                hitPart.Character.SendMessage("TakeDamage", Tools.GetRandom(attackDamage));
            }
        }
    }

    void Start () {
	    
	}
	
	void Update () {
        if (Input.GetMouseButtonDown(0) && !attack && !midWaySprint)
            StartCoroutine(Attack());

        midWaySprint = fps.sprinting && fps.speed > (fps.normal.Speed + fps.sprint.Speed) / 2;

        anim.SetBool("sprint", midWaySprint);
        Sway();
    }

    void Sway()
    {
        weaponPosition = transform.localPosition;
        weaponRotation = transform.localEulerAngles;

        weaponRotation.z = Tools.FixAngle(weaponRotation.z);

        mouse.x = Input.GetAxisRaw("Mouse X");
        mouse.y = Input.GetAxisRaw("Mouse Y");

        if (mouse.x != 0)
        {
            weaponRotation.z = Mathf.MoveTowards(weaponRotation.z, 0, hipSway.rotationDelta / 2);
            weaponPosition.x = Mathf.MoveTowards(weaponPosition.x, 0, hipSway.positionDelta.x / 2);
        }
        else
        {
            weaponRotation.z = Mathf.MoveTowards(weaponRotation.z, 0, hipSway.rotationDelta * 2);
            weaponPosition.x = Mathf.MoveTowards(weaponPosition.x, 0, hipSway.positionDelta.x * 2);
        }

        if (mouse.y != 0)
        {
            weaponPosition.y = Mathf.MoveTowards(weaponPosition.y, 0, hipSway.positionDelta.y / 2);
        }
        else
        {
            weaponPosition.y = Mathf.MoveTowards(weaponPosition.y, 0, hipSway.positionDelta.y * 2);
        }

        weaponRotation.z += mouse.x * hipSway.rotationDelta;

        weaponRotation.z = Mathf.Clamp(weaponRotation.z, -hipSway.rotationLimmit, hipSway.rotationLimmit);


        weaponPosition.x += mouse.x * hipSway.positionDelta.x;
        weaponPosition.y += mouse.y * hipSway.positionDelta.y;

        weaponPosition.x = Mathf.Clamp(weaponPosition.x, -hipSway.positionLimmit.x, hipSway.positionLimmit.x);
        weaponPosition.y = Mathf.Clamp(weaponPosition.y, -hipSway.positionLimmit.y, hipSway.positionLimmit.y);

        transform.localEulerAngles = weaponRotation;
        transform.localPosition = weaponPosition;
    }

    IEnumerator Attack()
    {
        attack = true;
        anim.SetBool("attack", true);

        yield return new WaitForSeconds(attackClip.length);

        attack = false;
        anim.SetBool("attack", false);
    }

    public void UnEquip()
    {
        anim.SetBool("switch", true);
    }
}
