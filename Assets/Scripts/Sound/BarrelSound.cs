using UnityEngine;
using FMODUnity;

public class BarrelSound : MonoBehaviour
{
    [Header("FMOD Audio")]
    public EventReference rollSoundEvent;
    private FMOD.Studio.EventInstance rollSoundInstance;
    
    private Rigidbody rb;
    private bool isPlaying = false;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        
        if (!rollSoundEvent.IsNull)
        {
            rollSoundInstance = RuntimeManager.CreateInstance(rollSoundEvent);
            RuntimeManager.AttachInstanceToGameObject(rollSoundInstance, transform, rb);
        }
    }

    void Update()
    {
        if (rb == null || rollSoundEvent.IsNull) return;

        // Verifica se o barril está se movendo (velocidade maior que um valor bem pequeno)
        // e garante que ele não parou por causa do TravaBarril (isKinematic)
        bool isMoving = rb.linearVelocity.magnitude > 0.1f && !rb.isKinematic;

        if (isMoving && !isPlaying)
        {
            rollSoundInstance.start();
            isPlaying = true;
        }
        else if (!isMoving && isPlaying)
        {
            rollSoundInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
            isPlaying = false;
        }
    }

    private void OnDestroy()
    {
        if (isPlaying)
        {
            rollSoundInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
        }
        rollSoundInstance.release();
    }
}
