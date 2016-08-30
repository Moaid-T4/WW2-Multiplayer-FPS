using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Shoot : ControlableBehaviour {
    [SerializeField]
    bool canShoot;

    [SerializeField]
    FireType fireType;

    [SerializeField]
    int maxAmmo;
    [SerializeField]
    int ammo;
    [SerializeField]
    int clip;

    [SerializeField]
    LayerMask mask;

    [SerializeField]
    float distance;

    [SerializeField]
    float rateOfFire;

    [SerializeField]
    bool shot;

    [SerializeField]
    bool ads;

    [SerializeField]
    bool reloading;

    [SerializeField]
    Vector2 headDamage;
    [SerializeField]
    Vector2 bodyDamage;
    [SerializeField]
    Vector2 limpDamage;

    [SerializeField]
    float maxPenetration = 300;
    float penetration;

    [SerializeField]
    AudioClip shotSound;
    [SerializeField]
    AudioClip noAmmoSound;

    [SerializeField]
    AnimationClip aimClip;
    [SerializeField]
    AnimationClip unAimClip;
    [SerializeField]
    AnimationClip reloadClip;
    [SerializeField]
    AnimationClip switchStartClip;

    [SerializeField]
    KeyCode shootKey = KeyCode.Mouse0;

    [SerializeField]
    KeyCode aimKey = KeyCode.Mouse1;

    [SerializeField]
    KeyCode reloadKey = KeyCode.R;

    [SerializeField]
    Animator anim;

    [SerializeField]
    Transform shotPos;
    [SerializeField]
    Transform end;

    RaycastHit[] hits;
    RaycastHit[] hitsB;

    AudioSource aud;
    FirstPersonController fps;
    GameObject lastHitPart;

    bool midWaySprint;

    [SerializeField]
    float[] depth;

    Vector2 mouse;

    Vector3 weaponPosition;
    Vector3 weaponRotation;

    [SerializeField]
    SwayState hipSway = new SwayState(new Vector2(0.002f, 0.002f),new Vector2(0.03f, 0.03f), 0.5f,10);

    [SerializeField]
    SwayState aimSway = new SwayState(new Vector2(0.0004f, 0.0004f), new Vector2(0.011f, 0.011f), 0.5f, 10);

    SwayState desiredState;

    [SerializeField]
    Text weaponLabel;

    void OnEnable()
    {
        shot = false;
        reloading = false;
        ads = false;
        anim.SetBool("ads", false);
        anim.SetBool("switch", false);
        if (!fps)
            fps = transform.parent.parent.parent.GetComponent<FirstPersonController>();
        fps.currentWeapon = gameObject;

        weaponLabel.text = ammo + " / " + clip;

        StartCoroutine(WaitTillSwitch());
    }

    void Start () {
        desiredState = hipSway;

        aud = shotPos.GetComponent<AudioSource>();
        if(aud == null)
        {
            aud = shotPos.gameObject.AddComponent<AudioSource>();
        }

        Controls.AddListener(this);
        RefreshBinds();
    }

    IEnumerator WaitTillSwitch()
    {
        canShoot = false;

        yield return new WaitForSeconds(switchStartClip.length);

        canShoot = true;
    }

    public override void RefreshBinds()
    {
        shootKey = Controls.controls["On Foot.Shoot"];
        aimKey = Controls.controls["On Foot.Aim"];
        reloadKey = Controls.controls["On Foot.Reload"];
    }

    void Update()
    {
        if (Game.paused)
            return;
        if (fireType == FireType.SemiAutomatic && canShoot && Input.GetKeyDown(shootKey) && !shot && !reloading && ammo > 0 && !midWaySprint)
        {
            StartCoroutine(ShootBullet());
        }

        if (fireType == FireType.Automatic && canShoot && Input.GetKey(shootKey) && !shot && !reloading && ammo > 0 && !midWaySprint)
        {
            StartCoroutine(ShootBullet());
        }

        if (Input.GetKeyDown(aimKey) && !reloading && !midWaySprint)
        {
            ToggleAim();
        }

        if (Input.GetKeyDown(shootKey) && canShoot && ammo == 0 && clip == 0)
        {
            aud.PlayOneShot(noAmmoSound);
        }

        if (Input.GetKeyDown(reloadKey) && !reloading && !shot && ammo < maxAmmo && !midWaySprint)
        {
            StartCoroutine(Reload());
        }

        Sway();

        midWaySprint = fps.sprinting && fps.speed > (fps.normal.Speed + fps.sprint.Speed)/2;

        if(midWaySprint && ads)
        {
            ToggleAim();
        }

        anim.SetBool("sprint", midWaySprint);
    }

    void Sway() {
        weaponPosition = transform.localPosition;
        weaponRotation = transform.localEulerAngles;

        weaponRotation.z = Tools.FixAngle(weaponRotation.z);

        mouse.x = Input.GetAxisRaw("Mouse X");
        mouse.y = Input.GetAxisRaw("Mouse Y");

        if(mouse.x != 0)
        {
            weaponRotation.z = Mathf.MoveTowards(weaponRotation.z, 0, desiredState.rotationDelta / 2);
            weaponPosition.x = Mathf.MoveTowards(weaponPosition.x, 0, desiredState.positionDelta.x / 2);
        }
        else
        {
            weaponRotation.z = Mathf.MoveTowards(weaponRotation.z, 0, desiredState.rotationDelta);
            weaponPosition.x = Mathf.MoveTowards(weaponPosition.x, 0, desiredState.positionDelta.x);
        }

        if (mouse.y != 0)
        {
            weaponPosition.y = Mathf.MoveTowards(weaponPosition.y, 0, desiredState.positionDelta.y / 2);
        }
        else
        {
            weaponPosition.y = Mathf.MoveTowards(weaponPosition.y, 0, desiredState.positionDelta.y);
        }

        weaponRotation.z += mouse.x * desiredState.rotationDelta;

        weaponRotation.z = Mathf.Clamp(weaponRotation.z, -desiredState.rotationLimmit, desiredState.rotationLimmit);


        weaponPosition.x += mouse.x * desiredState.positionDelta.x;
        weaponPosition.y += mouse.y * desiredState.positionDelta.y;

        weaponPosition.x = Mathf.Clamp(weaponPosition.x, -desiredState.positionLimmit.x, desiredState.positionLimmit.x);
        weaponPosition.y = Mathf.Clamp(weaponPosition.y, -desiredState.positionLimmit.y, desiredState.positionLimmit.y);

        transform.localEulerAngles = weaponRotation;
        transform.localPosition = weaponPosition;
    }

    void FixAngle(ref float angle)
    {
        if (angle > 180)
            angle = angle - 360;
    }

    IEnumerator ShootBullet()
    {
        shot = true;
        ammo--;
        aud.PlayOneShot(shotSound);

        weaponLabel.text = ammo + " / " + clip;

        hits = Physics.RaycastAll(shotPos.position, shotPos.forward, Vector3.Distance(shotPos.position,end.position), mask);

        Tools.ReArrangeHits(ref hits, true);

        Debug.DrawLine(shotPos.position, end.position,Color.blue,15);

        //if we hit sumthing
        if (hits != null && hits.Length != 0)
        {

            hitsB = Physics.RaycastAll(end.position, -end.forward, Vector3.Distance(shotPos.position, end.position), mask);

            Tools.ReArrangeHits(ref hitsB, false);

            string allHits = "";
            string allHitsB = "";

            depth = new float[hits.Length];

            penetration = maxPenetration;

            float headD = Tools.GetRandom(headDamage);
            float bodyD = Tools.GetRandom(bodyDamage);
            float limpD = Tools.GetRandom(limpDamage);

            for (int i = 0; i < hits.Length; i++)
            {
                allHits += hits[i].transform.name + '\n';
                allHitsB += hitsB[i].transform.name + '\n';

                depth[i] = distance - (hits[i].distance + hitsB[i].distance);

                //shot procedures
                if (penetration <= 0)
                    break;

                //print("penetration : " + penetration);

                if (hits[i].transform.tag == "prop")
                {
                    Prop hitProp = hits[i].transform.GetComponent<Prop>();

                    hitProp.transform.SendMessage("TakeDamage", limpD * (penetration/maxPenetration));

                    penetration -= hitProp.Hardness * depth[i];
                }

                if (hits[i].transform.tag == "head")
                {
                    if (lastHitPart == hits[i].collider.gameObject)
                        continue;
                    BodyPart hitPart = hits[i].transform.GetComponent<BodyPart>();

                    hitPart.Character.SendMessage("TakeDamage", headD * (penetration / maxPenetration));

                    penetration -= hitPart.Hardness * depth[i];

                    lastHitPart = hits[i].collider.gameObject;
                }

                if (hits[i].transform.tag == "body")
                {
                    if (lastHitPart == hits[i].collider.gameObject)
                        continue;
                        
                    BodyPart hitPart = hits[i].transform.GetComponent<BodyPart>();

                    hitPart.Character.SendMessage("TakeDamage", bodyD * (penetration / maxPenetration));

                    penetration -= hitPart.Hardness * depth[i];

                    lastHitPart = hits[i].collider.gameObject;
                }

                if (hits[i].transform.tag == "limp")
                {
                    if (lastHitPart == hits[i].collider.gameObject)
                        continue;
                    BodyPart hitPart = hits[i].transform.GetComponent<BodyPart>();

                    hitPart.Character.SendMessage("TakeDamage", limpD * (penetration / maxPenetration));

                    penetration -= hitPart.Hardness * depth[i];

                    lastHitPart = hits[i].collider.gameObject;
                }

                if (hits[i].transform.tag == "obstacle")
                {
                    penetration = 0;
                }
            }

            lastHitPart = null;

            //print("hit " + hits.Length + " Objects While Going : \n" + allHits);
            //print("hit " + hitsB.Length + " Objects While Returning : \n" + allHitsB);
            if (ammo < 1 && clip > 0)
                StartCoroutine(Reload());
        }

        yield return new WaitForSeconds(rateOfFire);
        shot = false;
    }

    void ToggleAim()
    {
        ads = !ads;
        if(ads)
        {
            desiredState = aimSway;
        }
        else
        {
            desiredState = hipSway;
        }
        anim.SetBool("ads", ads);
    }

    IEnumerator Reload()
    {
        if (ads)
        {
            ToggleAim();
            yield return new WaitForSeconds(unAimClip.length);
        }

        reloading = true;
        anim.SetBool("reload", true);

        yield return new WaitForSeconds(reloadClip.length);

        if (!reloading)
            yield break;

        int req;
        req = maxAmmo - ammo;
        //if we dont have all the req then just fill it with all that we have
        if (req > clip)
            req = clip;
        ammo += req;
        clip -= req;

        reloading = false;
        anim.SetBool("reload", false);
        weaponLabel.text = ammo + " / " + clip;
    }

    public void UnEquip()
    {
        reloading = true;
        anim.SetBool("switch", true);
    }
}

public enum FireType
{
    SemiAutomatic,
    Automatic,
    BoltAction,
}

[System.Serializable]
public class SwayState
{
    [SerializeField]
    internal Vector2 positionDelta;

    [SerializeField]
    internal Vector2 positionLimmit;

    [SerializeField]
    internal float rotationDelta;

    [SerializeField]
    internal float rotationLimmit;

    public SwayState(Vector2 newPS,Vector2 newPL,float newRD, float newRL)
    {
        positionDelta = newPS;
        positionLimmit = newPL;

        rotationLimmit = newRL;
        rotationDelta = newRD;
    }
}