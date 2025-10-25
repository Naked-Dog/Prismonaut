using UnityEngine;
using Cinemachine;
using System.Collections;

public class ShakeManager : MonoBehaviour
{
    public static ShakeManager Instance;
    [SerializeField] private float globalShakeForce = 0.6f;

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
        if (impulseSource == null) return;

        if (profile != null)
        {
            var def = impulseSource.m_ImpulseDefinition;

            def.m_ImpulseType = profile.impulseType;
            def.m_ImpulseShape = profile.impulseShape;
            def.m_ImpulseDuration = profile.shakeTime;
            impulseSource.m_DefaultVelocity = profile.Velocity;

            impulseSource.GenerateImpulseWithForce(profile.shakeForce);
        }
        else
        {
            impulseSource.GenerateImpulseWithForce(globalShakeForce);
        }
    }

    public void CustomShake(
            float force,
            float duration,
            CinemachineImpulseDefinition.ImpulseTypes? type = null,
            CinemachineImpulseDefinition.ImpulseShapes? shape = null,
            Vector3? velocity = null
        )
    {
        if (impulseSource == null) return;

        var def = impulseSource.m_ImpulseDefinition;

        if (type.HasValue) def.m_ImpulseType = type.Value;
        if (shape.HasValue) def.m_ImpulseShape = shape.Value;

        def.m_ImpulseDuration = duration;

        impulseSource.m_ImpulseDefinition = def;

        if (velocity.HasValue)
            impulseSource.m_DefaultVelocity = velocity.Value;

        impulseSource.GenerateImpulseWithForce(force);
    }

    public void StartShakeSequence(int shakeCount, float interval)
    {
        StartCoroutine(ShakeSequence(shakeCount, interval));
    }

    private IEnumerator ShakeSequence(int shakeCount, float interval)
    {
        for (int i = 0; i < shakeCount; i++)
        {
            CustomShake(
                force: 0.5f,
                duration: 0.5f,
                shape: CinemachineImpulseDefinition.ImpulseShapes.Rumble,
                type: CinemachineImpulseDefinition.ImpulseTypes.Uniform,
                velocity: new Vector3(0f, 1f)
            );

            yield return new WaitForSeconds(interval);
        }
    }
}
