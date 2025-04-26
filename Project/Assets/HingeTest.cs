using UnityEngine;

public class HingeTest : MonoBehaviour
{
    public Rigidbody2D hinge;

    // Update is called once per frame
    void FixedUpdate()
    {
        hinge.AddTorque(50);
    }
}
