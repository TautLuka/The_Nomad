using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallingTreeScript : MonoBehaviour
{
    public GameObject Log1;
    public GameObject Log2;
    public GameObject Log3;

    private GameObject thisTree;
    private GameObject spawnedLog;
    private bool destructionStarted = false;
    Rigidbody singleLogRb;
    Rigidbody rb;

    void Start()
    {
        thisTree = this.gameObject;
        singleLogRb = GetComponent<Rigidbody>();
    }


    private void OnCollisionEnter(Collision collision)
    {
        if (!collision.collider.CompareTag("Stump") && !destructionStarted)
        {
            StartCoroutine(TreeDestruction());
            destructionStarted = true;
        }
    }

    IEnumerator TreeDestruction()
    {
        singleLogRb.constraints = RigidbodyConstraints.None;
        yield return new WaitForSeconds(0.1f);
        spawnedLog = Instantiate(Log1, thisTree.transform.position + Vector3.up * 0.2f, this.transform.rotation); ;
        rb = spawnedLog.AddComponent<Rigidbody>();
        spawnedLog = Instantiate(Log2, thisTree.transform.position + Vector3.up * 0.2f, this.transform.rotation);
        rb = spawnedLog.AddComponent<Rigidbody>();
        spawnedLog = Instantiate(Log3, thisTree.transform.position + Vector3.up * 0.2f, this.transform.rotation);
        rb = spawnedLog.AddComponent<Rigidbody>();
        Destroy(thisTree);
    }
}
