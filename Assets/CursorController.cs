using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CursorController : MonoBehaviour
{
    public ShipController shipController;
    public PhysicsShipController physicsShipController;
    public ShipControllerMultiRB shipControllerMultiRB;
    public Camera mainCam;
    
    public GameObject lineRendererPrefab;
    public GameObject yPlanePrefab;
    public LayerMask systemPlaneLayerMask;

    private Vector3 systemPlaneHitPoint;
    private Vector3 xzCoordinates;
    private float xCoordinate;
    private float zCoordinate;
    

    //y coordinate related stuff
    private float mouseYPosition;
    public float scrollSpeed = 0.1f;
    private float yCoordinate;

    private LineRenderer xAxisLine;
    private LineRenderer zAxisLine;
    private GameObject yPlane;

    private bool isLeftClicked = false;

    void Start()
    {
        xAxisLine = Instantiate(lineRendererPrefab).GetComponent<LineRenderer>();
        zAxisLine = Instantiate(lineRendererPrefab).GetComponent<LineRenderer>();
        xAxisLine.positionCount = 2;
        zAxisLine.positionCount = 2;
        xAxisLine.enabled = false;
        zAxisLine.enabled = false;

        yPlane = Instantiate(yPlanePrefab);
        yPlane.SetActive(false);
    }

    void Update()
    {
        mouseYPosition = mainCam.ScreenToWorldPoint(Input.mousePosition).y;
        if (!isLeftClicked)
        {
            // Raycast from the camera to the system plane
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, systemPlaneLayerMask))
            {
                // Get the hit point on the system plane and set the cursor position
                systemPlaneHitPoint = hit.point;
                transform.position = systemPlaneHitPoint;

                // Draw the x and z axis lines
                //xAxisLine.enabled = true;
                //zAxisLine.enabled = true;
                //xAxisLine.SetPosition(0, new Vector3(systemPlaneHitPoint.x, 0, 0));
                //xAxisLine.SetPosition(1, new Vector3(systemPlaneHitPoint.x, 0, systemPlaneHitPoint.z));
                //zAxisLine.SetPosition(0, new Vector3(0, 0, systemPlaneHitPoint.z));
                //zAxisLine.SetPosition(1, new Vector3(systemPlaneHitPoint.x, 0, systemPlaneHitPoint.z));
            }
        }
        else
        {
            // Lock the cursor to the y-plane and keep the xz-coordinates the same
            /*This section "locks" the cursor's x and z positions by not allowing them to change. Then it simulates getting a Y position by multiplying 
             * an arbitrary number (scrollSpeed) by the Y axis movement of the mouse. Then the physical cursor's Y coordinate is changed to that value.*/
            float mouseY = Input.GetAxis("Mouse Y") * scrollSpeed;
            yCoordinate += mouseY;

            Vector3 cursorPosition = transform.position;
            xCoordinate = cursorPosition.x;
            zCoordinate = cursorPosition.z;
            cursorPosition.y = yCoordinate;
            transform.position = cursorPosition;
        }

        if (Input.GetMouseButtonDown(0))
        {
            isLeftClicked = true;
            xzCoordinates = new Vector3(systemPlaneHitPoint.x, 0, systemPlaneHitPoint.z);
            xAxisLine.enabled = false;
            zAxisLine.enabled = false;
            yCoordinate = 0f;
            
            //yPlane.SetActive(true);
        }
        else if (Input.GetMouseButtonDown(1))
        {
            yCoordinate = transform.position.y;
            //yPlane.SetActive(false);
            isLeftClicked = false;
            MoveShip(xCoordinate, yCoordinate, zCoordinate);
        }
    }

    void MoveShip(float xCoordinate, float yCoordinate, float zCoordinate)
    {
        // Pass the xz and y coordinates to the ship controller's MoveShip method
        //Debug.Log("Moving ship to x=" + xzCoordinates.x + " z=" + xzCoordinates.z + " y=" + yCoordinate);
        //shipController.MoveShip(new Vector3(xCoordinate, yCoordinate, zCoordinate));
        Vector3 target = new Vector3(xCoordinate, yCoordinate, zCoordinate);
        //physicsShipController.MoveShip(target, 1f, 0f, 0f);
        shipControllerMultiRB.MoveShip(target, 1f, 0f, 0f);
    }
}
