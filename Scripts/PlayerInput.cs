using UnityEngine;

public class PlayerInput : MonoBehaviour
{
    public ShipMovement controlledShip;
    public CameraFollow cameraSettings;

    void Start()
    {
        
    }


    void Update()
    {
        float thrust = 0f;
        float rotation = 0f;
        float yaw = 0f;
    //----------------------------------------- Camera Controls
        float scroll = (Input.GetAxis("Mouse ScrollWheel"));
        if (scroll != 0f)
        {
            cameraSettings.PanOutIn(scroll);
        }
        

    //----------------------------------------- Ship Controls
        if (Input.GetKey(KeyCode.W))
            thrust = 1f;
        else if (Input.GetKey(KeyCode.S))
            thrust = -1f;


        if (Input.GetKey(KeyCode.A))
            rotation = -1f;
        else if (Input.GetKey(KeyCode.D))
            rotation = 1f;
        
        if (Input.GetKey(KeyCode.Q))
            yaw = 1f;
        else if (Input.GetKey(KeyCode.E))
            yaw = -1f;

        controlledShip.SetMoveInput(thrust, rotation, yaw);

    }
}
