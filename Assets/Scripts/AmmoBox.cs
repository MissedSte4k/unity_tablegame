using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class AmmoBox : NetworkBehaviour {

    public GameObject[] models;
    public NetworkInstanceId spawnedBy;
    public int ammoRefreshes;

    // Use this for initialization
    void Start () {
        GameObject obj = ClientScene.FindLocalObject(spawnedBy);
        Physics.IgnoreCollision(GetComponent<Collider>(), obj.GetComponent<Collider>());
    }

    void OnCollisionEnter(Collision collision)
    {
        GameObject hit = collision.gameObject;
        RaycastHit hit2;
        Collider collider = GetComponent<Collider>();
        Rigidbody rb = GetComponent<Rigidbody>();

        if (hit.CompareTag("Ground"))
        {
            transform.rotation = Quaternion.Euler(0, transform.rotation.y, transform.rotation.z);
            rb.isKinematic = true;
            collider.isTrigger = true;
            if (Physics.Raycast(transform.position, Vector3.down, out hit2))
            {
                transform.position = new Vector3(transform.position.x, transform.position.y - hit2.distance, transform.position.z);
            }
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            float ammoRef = ammoRefreshes;
            if (other.gameObject.GetComponent<ScoutControl>() == null)
            {
                ScoutControl classControl = other.gameObject.GetComponent<ScoutControl>();
                if (classControl.primaryWeaponAmmo < classControl.maxAmmo[classControl.primaryWeapon])
                {
                    ammoRef -= 0.5f;
                    classControl.primaryWeaponAmmo = classControl.maxAmmo[classControl.primaryWeapon];
                }
                if (classControl.secondaryWeaponAmmo < classControl.maxAmmo[classControl.secondaryWeapon])
                {
                    ammoRef -= 0.5f;
                    classControl.secondaryWeaponAmmo = classControl.maxAmmo[classControl.secondaryWeapon];
                }
            }
            else if (other.gameObject.GetComponent<ArcherControl>() == null)
            {
                ArcherControl classControl = other.gameObject.GetComponent<ArcherControl>();
                if (classControl.primaryWeaponAmmo < classControl.maxAmmo[classControl.primaryWeapon])
                {
                    ammoRef -= 0.5f;
                    classControl.primaryWeaponAmmo = classControl.maxAmmo[classControl.primaryWeapon];
                }
                if (classControl.secondaryWeaponAmmo < classControl.maxAmmo[classControl.secondaryWeapon])
                {
                    ammoRef -= 0.5f;
                    classControl.secondaryWeaponAmmo = classControl.maxAmmo[classControl.secondaryWeapon];
                }
            }
            else if (other.gameObject.GetComponent<KnightControl>() == null)
            {
                KnightControl classControl = other.gameObject.GetComponent<KnightControl>();
                if (classControl.primaryWeaponAmmo < classControl.maxAmmo[classControl.primaryWeapon])
                {
                    ammoRef -= 0.5f;
                    classControl.primaryWeaponAmmo = classControl.maxAmmo[classControl.primaryWeapon];
                }
                if (classControl.secondaryWeaponAmmo < classControl.maxAmmo[classControl.secondaryWeapon])
                {
                    ammoRef -= 0.5f;
                    classControl.secondaryWeaponAmmo = classControl.maxAmmo[classControl.secondaryWeapon];
                }
            }
            else if (other.gameObject.GetComponent<BerserkerControl>() == null)
            {
                BerserkerControl classControl = other.gameObject.GetComponent<BerserkerControl>();
                if (classControl.primaryWeaponAmmo < classControl.maxAmmo[classControl.primaryWeapon])
                {
                    ammoRef -= 0.5f;
                    classControl.primaryWeaponAmmo = classControl.maxAmmo[classControl.primaryWeapon];
                }
                if (classControl.secondaryWeaponAmmo < classControl.maxAmmo[classControl.secondaryWeapon])
                {
                    ammoRef -= 0.5f;
                    classControl.secondaryWeaponAmmo = classControl.maxAmmo[classControl.secondaryWeapon];
                }
            }
            ammoRefreshes = (int)ammoRef;
            if (ammoRefreshes == 0)
            {
                Destroy(gameObject);
            }
        }
    }
}
