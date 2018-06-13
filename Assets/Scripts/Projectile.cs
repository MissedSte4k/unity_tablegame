using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Projectile : NetworkBehaviour
{

    [Header("The weapon's fire gameObject (if it has one)")]
    public GameObject fire;

    [Header("Select whether the projectile spins and by how much on each axis")]
    public bool likeARecordBaby;
    public Vector3 spin;

    private Rigidbody rb;
    [HideInInspector] public int damage;
    [SyncVar] [HideInInspector] public NetworkInstanceId spawnedBy;
    [SyncVar] [HideInInspector] public int team;
    [SyncVar] [HideInInspector] public bool isOnFire = false;

    [Header("Audio sources and sounds")]
    public AudioSource audioSourceDamage;

    // Use this for initialization
    void Start()
    {
        if (team == 1){
            GetComponent<TrailRenderer>().startColor = new Color(0, 0, 255, 100);
            GetComponent<TrailRenderer>().endColor = new Color(0, 0, 255, 0);
        }
        rb = GetComponent<Rigidbody>();
        if (isOnFire)
        {
            fire.SetActive(true);
        }
        GameObject obj = ClientScene.FindLocalObject(spawnedBy);
        Physics.IgnoreCollision(GetComponent<Collider>(), obj.GetComponent<Collider>());

    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (!likeARecordBaby)
        {
            Quaternion rotation = transform.rotation;
            rotation.SetLookRotation(rb.velocity);
            transform.rotation = rotation;
        }
        else
        {
            Quaternion angleRotation = Quaternion.Euler(spin * Time.deltaTime);
            rb.MoveRotation(rb.rotation * angleRotation);
        }
    }

    void OnCollisionEnter(Collision collision)
    {

        if (collision.gameObject.CompareTag("Player"))
        {
            var health = collision.gameObject.GetComponent<Health>();
            if (health != null)
            {
                if (isServer)
                    health.RpcTakeDamage(damage, health.IsFatal(damage), collision.gameObject.GetComponent<CharacterControl>().Team());
            }
        }
        else if (collision.gameObject.CompareTag("Block"))
        {
            collision.gameObject.GetComponentInParent<CharacterControl>().RpcBlockHurt();
        }

        audioSourceDamage.Play();
        GetComponent<Collider>().enabled = false;
        GetComponent<TrailRenderer>().enabled = false;

        if (GetComponent<MeshRenderer>() != null)
            GetComponent<MeshRenderer>().enabled = false;
        if (transform.childCount > 0)
            transform.GetChild(0).gameObject.SetActive(false);

        Destroy(gameObject, audioSourceDamage.clip.length);
    }
}
