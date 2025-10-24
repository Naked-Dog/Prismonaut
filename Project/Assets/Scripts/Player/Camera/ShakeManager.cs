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

    public void CameraShake(ShakeScriptable profile = null)
    {
        if (profile != null)
        {
            impulseDefinition = impulseSource.m_ImpulseDefinition;

            impulseDefinition.m_ImpulseType = profile.impulseType;
            impulseDefinition.m_ImpulseShape = profile.impulseShape;
            impulseDefinition.m_ImpulseDuration = profile.shakeTime;
            impulseSource.m_DefaultVelocity = profile.Velocity;

            impulseSource.GenerateImpulseWithForce(profile.shakeForce);
        }
        else impulseSource.GenerateImpulseWithForce(globalShakeForce);
    }

    public void CustomShake(float force = 1f, float duration = 0.3f)
    {
        if (impulseSource == null) return;

        var def = impulseSource.m_ImpulseDefinition;

        def.m_ImpulseDuration = duration;
        impulseSource.m_ImpulseDefinition = def;

        impulseSource.GenerateImpulseWithForce(force);
    }
}
