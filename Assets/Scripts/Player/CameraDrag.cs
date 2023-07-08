using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraDrag : MonoBehaviour
{
    #region Variables

    private Vector3 _origin;
    private Vector3 _difference;

    private Camera _mainCamera;

    private bool _isDragging;

    #endregion

    private void Awake() => _mainCamera = Camera.main;

    public void OnDrag(InputAction.CallbackContext ctx)
    {
        if (ctx.started) _origin = GetMousePosition;
        _isDragging = ctx.started || ctx.performed;
    }

    private void LateUpdate()
    {
        if (!_isDragging) return;

        _difference = GetMousePosition - transform.position; 
        transform.position = _origin - _difference;
        if (transform.position.y< -3)
        {
            float x  = transform.position.x;
            transform.position = new Vector3(x, -3,-10 );
        }
        if (transform.position.y> +3)
        {
            float x  = transform.position.x;
            transform.position = new Vector3(x,  3,-10 );
        }

        if (transform.position.x< -5)
        {
            float y  = transform.position.y;
            transform.position = new Vector3(-5, y,-10 );
        }
        if (transform.position.x> +3)
        {
            float y  = transform.position.y;
            transform.position = new Vector3(5,  y,-10 );
        }
    }

    private Vector3 GetMousePosition => _mainCamera.ScreenToWorldPoint(Mouse.current.position.ReadValue());
}
