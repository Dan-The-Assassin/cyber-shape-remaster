using Constants;
using UnityEngine;
using LayerMask = Constants.LayerMask;

public class CameraController : MonoBehaviour
{
    [SerializeField] private float smoothSpeed = 3.5f;

    private Camera _camera;
    private GameObject _playerTransform;
    private Vector3 _offset;

    private void Awake()
    {
        _camera = GetComponent<Camera>();

        _playerTransform = GameObject.FindWithTag(Tags.Player);
        _offset = transform.position - _playerTransform.transform.position;
    }

    private void LateUpdate()
    {
        if (_playerTransform != null)
        {
            var newCameraPos = _playerTransform.transform.position + _offset;

            // Check if the edges of the camera are seeing outside of the floor plane.
            // Left
            var ray = _camera.ViewportPointToRay(new Vector3(0.0f, 0.5f, 0.0f));
            if (!Physics.Raycast(ray, Mathf.Infinity, (int) LayerMask.Floor))
            {
                newCameraPos.x = Mathf.Max(_camera.transform.position.x, newCameraPos.x);
            }

            // Right
            ray = _camera.ViewportPointToRay(new Vector3(1.0f, 0.5f, 0.0f));
            if (!Physics.Raycast(ray, Mathf.Infinity, (int) LayerMask.Floor))
            {
                newCameraPos.x = Mathf.Min(_camera.transform.position.x, newCameraPos.x);
            }

            // Bottom
            ray = _camera.ViewportPointToRay(new Vector3(0.5f, 0.0f, 0.0f));
            if (!Physics.Raycast(ray, Mathf.Infinity, (int) LayerMask.Floor))
            {
                newCameraPos.z = Mathf.Max(_camera.transform.position.z, newCameraPos.z);
            }

            // Top
            ray = _camera.ViewportPointToRay(new Vector3(0.5f, 1.0f, 0.0f));
            if (!Physics.Raycast(ray, Mathf.Infinity, (int) LayerMask.Floor))
            {
                newCameraPos.z = Mathf.Min(_camera.transform.position.z, newCameraPos.z);
            }

            transform.position = Vector3.Lerp(_camera.transform.position, newCameraPos, smoothSpeed * Time.deltaTime);
        }
    }
}
