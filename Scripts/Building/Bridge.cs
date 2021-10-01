using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bridge : MonoBehaviour
{
    [SerializeField] private Material BlueprintMaterial;

    public GameObject PlankPrefab;
    public GameObject PlanksParent;

    GameObject Parent;

    public GameObject p1;
    public GameObject p2;
    public GameObject midPoint;

    private Vector3 a;
    private Vector3 b;
    private Vector3 c;

    [Range(0.0f, 1.0f)]
    public float t;

    float Distance;
    float SagCalc;

    Vector3 LastPosition;
    Vector3 ZeroPos = new Vector3(0, 0, 0);
    Quaternion ZeroRot = Quaternion.Euler(0, 0, 0);

    float Placement = 0.6f;

    float DistanceTraveled = 0f;

    public static bool StartBridgeBuilding = false;
    public static bool StartedBuildingBridge = false;
    public static bool BluePrint = false;

    public bool CheckForBridge(Vector3 MiddleMiddlePoint)
    {
        if (Physics.CheckSphere(MiddleMiddlePoint, 0.5f))
        {
            return true;
        }

        return false;
    }

    private void Update()
    {

        if(Input.GetKeyDown(KeyCode.G))
        {
            StartBridgeBuilding = true;
        }
    }
    void FixedUpdate()
    {
        if (StartBridgeBuilding)
        {

            Distance = Vector3.Distance(p1.transform.position, p2.transform.position);

            Vector3 MiddlePoint = Vector3.Lerp(p1.transform.position, p2.transform.position, 0.5f);



            midPoint.transform.position = MiddlePoint;

            c = p1.transform.position;

            SagCalc = Distance * 4f;



            Vector3 direction = p2.transform.position - midPoint.transform.position;

            Vector3 RestrictedRotation = new Vector3(direction.x, 0, direction.z);

            p1.transform.rotation = Quaternion.LookRotation(RestrictedRotation);
            p2.transform.rotation = Quaternion.LookRotation(-RestrictedRotation);
            midPoint.transform.rotation = Quaternion.LookRotation(direction);

            p1.transform.position += p1.transform.forward * 125 * Time.fixedDeltaTime;
            p2.transform.position += p2.transform.forward * 125 * Time.fixedDeltaTime;
            midPoint.transform.position += -midPoint.transform.up * SagCalc * Time.fixedDeltaTime;

            Vector3 MiddleMiddlePoint = Vector3.Lerp(MiddlePoint, midPoint.transform.position, 0.5f);

            if (CheckForBridge(MiddleMiddlePoint) == false)
            {
                Parent = GameObject.Instantiate(PlanksParent, ZeroPos, ZeroRot);
                //Parent.transform.gameObject.tag = "BridgeNotBuilt";

                StartedBuildingBridge = true;
            }

            StartBridgeBuilding = false;

        }

        while (t < 1 && StartedBuildingBridge)
        {

            LastPosition = c;

            a = Vector3.Lerp(p1.transform.position, midPoint.transform.position, t);
            b = Vector3.Lerp(midPoint.transform.position, p2.transform.position, t);
            c = Vector3.Lerp(a, b, t);

            DistanceTraveled += Vector3.Distance(LastPosition, c);

            Vector3 direction = b - c;
            Quaternion rotation = Quaternion.LookRotation(direction);

            if (DistanceTraveled > Placement)
            {
                GameObject Plank = GameObject.Instantiate(PlankPrefab, c, rotation);

                var Material = Plank.GetComponent<Renderer>();
                //var Collider = Plank.GetComponent<MeshCollider>();

                //Material.material = BlueprintMaterial;
                //Collider.convex = true;
                //Collider.isTrigger = true;

                Plank.transform.parent = Parent.transform;

                DistanceTraveled = 0f;
                break;
            }

            t += 0.0001f;
        }

        if (t > 1 && StartedBuildingBridge)
        {
            StartedBuildingBridge = false;

            t = 0;
        }
    }
}
