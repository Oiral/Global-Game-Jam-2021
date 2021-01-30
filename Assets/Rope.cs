using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Rope : MonoBehaviour
{

    public List<Vector3> pointsInRope = new List<Vector3>();

    public bool ropeActived;

    LineRenderer lineRender;

    GameObject playerBase;

    GameObject ropeAttachPoint;

    float ropeLength;

    float ropeUpdateDistance = 0.1f;
    private float nonAttachedRopeLength;

    public Vector3 startingPoint = Vector3.zero;

    public LayerMask layerMask;

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(startingPoint, 0.2f);
    }

    private void Start()
    {
        lineRender = GetComponent<LineRenderer>();
        playerBase = gameObject;

        //set up the rope attach point
        ropeAttachPoint = new GameObject();
        ropeAttachPoint.transform.name = "Rope Attach Point";
        //ropeAttachPoint.AddComponent<Rigidbody>();
        //ropeAttachPoint.GetComponent<Rigidbody>().isKinematic = true;

        EnableRope();
    }


    // Update is called once per frame
    void Update()
    {
        if (ropeActived)
        {

            CheckPoints();
            DrawRope();
            CalculateRopeLength();
            MoveRopeAttachPoint();


        }
    }

    void ToggleRope()
    {
        if (ropeActived)//If the rope is already active and then the player toggles it turn it off
        {
            DisableRope();
        }
        else
        {
            //check if the rope can be placed and the toggle it on
            EnableRope();
        }

    }

    void EnableRope()
    {
        ropeActived = true;
        pointsInRope.Add(startingPoint);
        lineRender.enabled = true;

        //RaycastHit hit;
        //if (Physics.Raycast(transform.position + transform.up, transform.forward, out hit))
        //{
        //    if (hit.distance > maxLength)
        //    {
        //        return;
        //    }
            

        //    ////Add a configurable joint
        //    //ConfigurableJoint joint = playerBase.AddComponent<ConfigurableJoint>();
        //    ////Set up the configurable joint
        //    //joint.xMotion = ConfigurableJointMotion.Limited;
        //    //joint.yMotion = ConfigurableJointMotion.Limited;
        //    //joint.zMotion = ConfigurableJointMotion.Limited;

        //    //SoftJointLimit limits = new SoftJointLimit();
        //    //limits.limit = ropeLength;
        //    //limits.contactDistance = 2f;
        //    //joint.linearLimit = limits;

        //    //joint.connectedBody = ropeAttachPoint.GetComponent<Rigidbody>();
        //    //joint.autoConfigureConnectedAnchor = false;
        //    //joint.connectedAnchor = new Vector3(0, 0, 0);
        //}
    }

    void DisableRope()
    {
        ropeActived = false;
        pointsInRope.Clear();
        lineRender.enabled = false;
        //Check if the configurable joint is attatched to the player
        //if (playerBase.GetComponent<ConfigurableJoint>())
        //{
        //    //Remove the configurable joint
        //    Destroy(playerBase.GetComponent<ConfigurableJoint>());
        //}
    }

    void DrawRope()
    {
        lineRender.positionCount = pointsInRope.Count + 1;
        lineRender.SetPosition(pointsInRope.Count, transform.position);
        for (int i = 0; i < pointsInRope.Count; i++)
        {
            lineRender.SetPosition(i, pointsInRope[i]);
        }
    }

    void CheckPoints()
    {
        //Check the current point in the rope and see if we can see it
        RaycastHit lastPointHit;
        Vector3 lastPoint = pointsInRope[pointsInRope.Count - 1];
        Vector3 dir = lastPoint - transform.position;

        if (Physics.Raycast(transform.position, dir, out lastPointHit, Mathf.Infinity, layerMask))
        {
            if (Vector3.Distance(lastPointHit.point, lastPoint) > ropeUpdateDistance)
            {
                pointsInRope.Add(lastPointHit.point);
            }
        }

        //check if we can see the previous node and if so remove the current node
        if (pointsInRope.Count > 1)
        {
            RaycastHit hit;
            Vector3 prevPoint = pointsInRope[pointsInRope.Count - 2];
            Vector3 dirToPrevPoint = prevPoint - transform.position;

            if (Physics.Raycast(transform.position, dirToPrevPoint, out hit, Mathf.Infinity, layerMask)) 
            {
                Debug.DrawRay(transform.position, hit.point - transform.position, Color.red);
                if (Vector3.Distance(prevPoint, hit.point) < ropeUpdateDistance)
                {
                    pointsInRope.RemoveAt(pointsInRope.Count - 1);
                }
            }
        }

    }

    void CalculateRopeLength()
    {
        //reset rope length
        nonAttachedRopeLength = 0;
        //Calcuate distance between player and last point the the rope
        //ropeLength += Vector3.Distance(transform.position, pointsInRope[pointsInRope.Count - 1]);


        for (int i = 0; i < pointsInRope.Count - 1; i++)
        {
            nonAttachedRopeLength += Vector3.Distance(pointsInRope[i], pointsInRope[i + 1]);
        }
    }

    void MoveRopeAttachPoint()
    {
        ropeAttachPoint.transform.position = pointsInRope[pointsInRope.Count - 1];


        //Reset the joint limit to the current attach point
        //ConfigurableJoint joint = playerBase.GetComponent<ConfigurableJoint>();

        //SoftJointLimit limits = joint.linearLimit;
        //limits.limit = ropeLength - nonAttachedRopeLength;
        //joint.linearLimit = limits;
    }
}