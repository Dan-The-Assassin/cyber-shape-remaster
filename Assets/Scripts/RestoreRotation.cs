using UnityEngine;

public class RestoreRotation : MonoBehaviour
{
    private Quaternion _lastParentRotation;

    private void Start()
    {
        _lastParentRotation = transform.parent.localRotation;
    }

    private void Update()
    {
        transform.localRotation = Quaternion.Inverse(transform.parent.localRotation)
            * _lastParentRotation * transform.localRotation;

        _lastParentRotation = transform.parent.localRotation;
    }
}
