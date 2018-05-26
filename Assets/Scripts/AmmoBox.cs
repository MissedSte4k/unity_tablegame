using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class AmmoBox : NetworkBehaviour {

    [Header("Red and blue ammo box models")]
    public GameObject[] models;

    [Header("Number of times the ammo box can give ammo")]
    [Range(0, 10)]
    public int ammoRefreshes;

    [SyncVar] [HideInInspector] public int team;
    [SyncVar] [HideInInspector] public NetworkInstanceId spawnedBy;

    void Start () {
        if (team == 1)
        {
            models[1].SetActive(true);
        }
        else
        {
            models[0].SetActive(true);
        }
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
            transform.rotation = Quaternion.Euler(0, transform.rotation.eulerAngles.y, 0);
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
        if (other.CompareTag("Player") && other.GetComponent<CharacterControl>().Team() == team)
        {
            if (other.gameObject.GetComponent<ScoutControl>() != null)
            {
                ScoutControl classControl = other.gameObject.GetComponent<ScoutControl>();
                if (!classControl.isAmmoFull())
                {
                    ammoRefreshes--;
                    classControl.ammoRefresh();
                }
            }
            else if (other.gameObject.GetComponent<ArcherControl>() != null)
            {
                ArcherControl classControl = other.gameObject.GetComponent<ArcherControl>();
                if (!classControl.isAmmoFull())
                {
                    ammoRefreshes--;
                    classControl.ammoRefresh();
                }
            }
            else if (other.gameObject.GetComponent<KnightControl>() != null)
            {
                KnightControl classControl = other.gameObject.GetComponent<KnightControl>();
                if (!classControl.isAmmoFull())
                {
                    ammoRefreshes--;
                    classControl.ammoRefresh();
                }
            }
            else if (other.gameObject.GetComponent<BerserkerControl>() != null)
            {
                BerserkerControl classControl = other.gameObject.GetComponent<BerserkerControl>();
                if (!classControl.isAmmoFull())
                {
                    ammoRefreshes--;
                    classControl.ammoRefresh();
                }
            }
            if (ammoRefreshes == 0)
            {
                Destroy(gameObject);
            }
        }
    } 
}
