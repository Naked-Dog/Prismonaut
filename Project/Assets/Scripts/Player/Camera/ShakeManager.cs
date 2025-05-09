using UnityEngine;
using Cinemachine;

public class ShakeManager : MonoBehaviour
{
    public static ShakeManager Instance;
    [SerializeField] private float globalShakeForce = 0.6f;
    private CinemachineImpulseDefinition impulseDefinition;
    private CinemachineImpulseSource impulseSource;
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        impulseSource = GetComponent<CinemachineImpulseSource>();
    }

    public void CameraShake()
    {
        impulseSource.GenerateImpulseWithForce(globalShakeForce);
    }

    public void CameraShakeWithProfile(ShakeScriptable profile)
    {
        impulseDefinition = impulseSource.m_ImpulseDefinition;

        impulseDefinition.m_ImpulseType = profile.impulseType;
        impulseDefinition.m_ImpulseShape = profile.impulseShape;
        impulseDefinition.m_ImpulseDuration = profile.shakeTime;
        impulseSource.m_DefaultVelocity = profile.Velocity;

        impulseSource.GenerateImpulseWithForce(profile.shakeForce);
    }
}
