using UnityEngine;

public class CameraInspector : MonoBehaviour
{
    private Vector3 target;
    private float speed = 1f;
    private float mouseSpeed = 0.3f;
    void Start()
    {
        target = transform.position;
    }

    void Update()
    {
        Vector3 v = new Vector3();
        if (Input.GetKey(KeyCode.W))
        {
            target = Vector3.SmoothDamp(target, target + transform.forward * speed * Time.deltaTime, ref v, Time.deltaTime);
        }
        if (Input.GetKey(KeyCode.S))
        {
            target = Vector3.SmoothDamp(target, target - transform.forward * speed * Time.deltaTime, ref v, Time.deltaTime);
        }
        if (Input.GetKey(KeyCode.D))
        {
            target = Vector3.SmoothDamp(target, target + transform.right * speed * Time.deltaTime, ref v, Time.deltaTime);
        }

        if (Input.GetKey(KeyCode.A))
        {
            target = Vector3.SmoothDamp(target, target + -transform.right * speed * Time.deltaTime, ref v, Time.deltaTime);
        }
        if (Input.GetKey(KeyCode.Space))
        {
            target = Vector3.SmoothDamp(target, target + transform.up * speed * Time.deltaTime, ref v, Time.deltaTime);
        }
        if (Input.GetKey(KeyCode.LeftShift))
        {
            target = Vector3.SmoothDamp(target, target - transform.up * speed * Time.deltaTime, ref v, Time.deltaTime);
        }
        if (Input.GetKeyDown(KeyCode.Q))
        {
            if (InteractableRaycaster.Grabed) InteractableRaycaster.Grabed.Release();
            else if (InteractableRaycaster.LookingAt) InteractableRaycaster.LookingAt.Pick();
        }
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (InteractableRaycaster.Grabed) InteractableRaycaster.Grabed.Interact();
            else if (InteractableRaycaster.LookingAt) if (InteractableRaycaster.LookingAt.Static) InteractableRaycaster.LookingAt.Interact();
        }
        if (Input.GetKeyDown(KeyCode.F))
        {
            NDTStates.ChangeState.Invoke(NDTAction.Skip);
        }
        float dX = Input.GetAxis("Mouse X") * mouseSpeed;
        float dY = Input.GetAxis("Mouse Y") * mouseSpeed;
        var targetRotation = transform.rotation.eulerAngles;

        targetRotation.y += dX;
        targetRotation.x -= dY;
        targetRotation.z = 0;

        transform.rotation = Quaternion.Euler(targetRotation);
        //transform.Rotate(-Vector3.right * Input.GetAxis("Mouse Y") *mouseSpeed);
        transform.position = target;
    }
}
