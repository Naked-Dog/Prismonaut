using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraActivationManager : MonoBehaviour
{
    [SerializeField] private float activationPadding = 2f;
    [SerializeField] private float checkInterval = 0.2f;

    private List<GameObject> registeredObjects = new List<GameObject>();
    private Camera mainCam;

    private void Awake()
    {
        mainCam = Camera.main;

        MonoBehaviour[] behaviours = Object.FindObjectsByType<MonoBehaviour>(FindObjectsSortMode.None);
        foreach (var b in behaviours)
        {
            if (b is ICullable)
            {
                RegisterObject(b.gameObject);
            }
        }

        StartCoroutine(CheckLoop());
    }

    public void RegisterObject(GameObject obj)
    {
        if (!registeredObjects.Contains(obj))
            registeredObjects.Add(obj);
    }

    public void UnregisterObject(GameObject obj)
    {
        registeredObjects.Remove(obj);
    }

    private IEnumerator CheckLoop()
    {
        while (true)
        {
            if (mainCam != null)
            {
                Vector3 bottomLeft = mainCam.ViewportToWorldPoint(new Vector3(0, 0, mainCam.nearClipPlane)) - Vector3.one * activationPadding;
                Vector3 topRight = mainCam.ViewportToWorldPoint(new Vector3(1, 1, mainCam.nearClipPlane)) + Vector3.one * activationPadding;

                foreach (var obj in registeredObjects)
                {
                    if (obj == null) continue;

                    var activatable = obj.GetComponent<ICullable>();
                    if (activatable != null && !activatable.ShouldBeCameraCulled) continue;

                    Vector3 pos = obj.transform.position;
                    bool inView = pos.x >= bottomLeft.x && pos.x <= topRight.x &&
                                  pos.y >= bottomLeft.y && pos.y <= topRight.y;

                    obj.SetActive(inView);
                }
            }
            yield return new WaitForSeconds(checkInterval);
        }
    }
}
