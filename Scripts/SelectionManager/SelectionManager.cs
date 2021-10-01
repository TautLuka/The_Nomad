using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectionManager : MonoBehaviour
{
    //Transforms

    //Trees
    [SerializeField] GameObject treeHitOncePrefab;
    [SerializeField] GameObject treeHitTwicePrefab;
    [SerializeField] GameObject treeHitThreeTimesPrefab;
    [SerializeField] GameObject treeHitFourTimesPrefab;
    [SerializeField] GameObject stumpPrefab;
    [SerializeField] GameObject fallingTreePrefab;
    [SerializeField] GameObject Log1;
    [SerializeField] GameObject Log2;
    [SerializeField] GameObject Log3;
    [SerializeField] GameObject ScreenUI;

    //Ui
    private WristScreen wristScLogic;

    //testing
    [SerializeField] GameObject raycastTarget;
    [SerializeField] GameObject playerOnboardRaycast;
    [SerializeField] GameObject Testingshit;
    float currentHitDistance;


    //Ziplines



    void Start()
    {
        //wristScLogic = ScreenUI.GetComponent<WristScreen>();

    }

    float RandomRange()
    {
        int i = Random.Range(1, 3);
        if (i == 1)
        {
            return 1;
        }
        return -1;
    }

    void Update()
    {

        var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        /*
        if (Physics.Raycast(ray, out hit))
        {
            var selection = hit.transform;

            if (Input.GetKeyDown(KeyCode.Mouse0) && selection.CompareTag("Tree"))
            {
                TreeScript tree = hit.collider.GetComponent<TreeScript>();
                TreeScript spawnedTreeScript;

                GameObject treePrefab = selection.transform.parent.gameObject;

                GameObject spawnedTree;

                int health = tree.treeHealth;

                switch (health)
                {
                    case 4:
                        spawnedTree = Instantiate(treeHitOncePrefab, treePrefab.transform.position, treePrefab.transform.rotation);
                        spawnedTree.transform.GetChild(0).gameObject.AddComponent<TreeScript>();
                        spawnedTreeScript = spawnedTree.transform.GetChild(0).gameObject.GetComponent<TreeScript>();
                        spawnedTreeScript.treeHealth = 3;
                        Destroy(treePrefab);
                        return;
                    case 3:
                        spawnedTree = Instantiate(treeHitTwicePrefab, treePrefab.transform.position, treePrefab.transform.rotation);
                        spawnedTree.transform.GetChild(0).gameObject.AddComponent<TreeScript>();
                        spawnedTreeScript = spawnedTree.transform.GetChild(0).gameObject.GetComponent<TreeScript>();
                        spawnedTreeScript.treeHealth = 2;
                        Destroy(treePrefab);
                        return;
                    case 2:
                        spawnedTree = Instantiate(treeHitThreeTimesPrefab, treePrefab.transform.position, treePrefab.transform.rotation);
                        spawnedTree.transform.GetChild(0).gameObject.AddComponent<TreeScript>();
                        spawnedTreeScript = spawnedTree.transform.GetChild(0).gameObject.GetComponent<TreeScript>();
                        spawnedTreeScript.treeHealth = 1;
                        Destroy(treePrefab);
                        return;
                    case 1:
                        Instantiate(stumpPrefab, treePrefab.transform.position, treePrefab.transform.rotation);
                        Destroy(treePrefab);
                        spawnedTree = Instantiate(fallingTreePrefab, treePrefab.transform.position, treePrefab.transform.rotation * Quaternion.Euler(RandomRange(), RandomRange(), 0));
                        spawnedTree.AddComponent<Rigidbody>();
                        Rigidbody rb = spawnedTree.GetComponent<Rigidbody>();
                        rb.mass = 1000f;
                        rb.constraints = RigidbodyConstraints.FreezeRotationZ;
                        rb.interpolation = RigidbodyInterpolation.Interpolate;
                        spawnedTree.AddComponent<FallingTreeScript>();
                        FallingTreeScript script = spawnedTree.GetComponent<FallingTreeScript>();
                        script.Log1 = Log1;
                        script.Log2 = Log2;
                        script.Log3 = Log3;
                        return;
                }

            }

            
            if (Input.GetKeyDown(KeyCode.Mouse0) && selection.CompareTag("Water"))
            {
                wristScLogic.Sip("Water");
            }
            if (Input.GetKeyDown(KeyCode.Mouse0) && selection.CompareTag("Antifreeze"))
            {
                wristScLogic.Sip("Antifreeze");
            }
            if (Input.GetKeyDown(KeyCode.Mouse0) && selection.CompareTag("Station"))
            {
                wristScLogic.Charge();
            }
            
        }
        */


        //RaycastTarget
        if(Physics.SphereCast(playerOnboardRaycast.transform.position, 0.2f, playerOnboardRaycast.transform.forward, out hit, 7f))
        {
            currentHitDistance = hit.distance;
        }
        else
        {
            currentHitDistance = 7f;
        }

        raycastTarget.transform.localPosition = new Vector3(0, 0, currentHitDistance);

        //ßßTestingshit.transform.position = raycastTarget.transform.position;
    }

}
