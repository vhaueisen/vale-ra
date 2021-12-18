// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;
// using UnityEngine.XR.ARFoundation;
// using UnityEngine.XR.ARSubsystems;

// public class Follower : MonoBehaviour
// {
//     private Vector3 initialPos;
//     private bool follow = false;
//     public bool Follow
//     {
//         get => follow;
//         set        {
//             follow = value;
//             if(!value)
//                 transform.position = initialPos;
//         }
//     }

//     void Awake()
//     {
//         initialPos = transform.position;
//     }

//     void Update()
//     {
// if(follow){
// if(_underInertia && _time <= SmoothTime)
//      {
//          transform.position += _velocity;
//          _velocity = Vector3.Lerp(_velocity, Vector3.zero, _time);
//          _time += Time.smoothDeltaTime;
//      }
//      else
//      {
//          _underInertia = false;
//          _time = 0.0f;
//      }
// }
//     }

//     private Vector3 _screenPoint;
//     private Vector3 _offset;
//     private Vector3 _curScreenPoint;
//     private Vector3 _curPosition;
//     private Vector3 _velocity;
//     private bool _underInertia;
//     private float _time = 0.0f;
//     public float SmoothTime;

//  void OnMouseDown()
//  {
//      _screenPoint = Camera.main.WorldToScreenPoint(gameObject.transform.position);
//      _offset = gameObject.transform.position - Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, _screenPoint.z));
//      Screen.showCursor = false;
//      _underInertia = false;
//  }
//  void OnMouseDrag()
//  {
//      Vector3 _prevPosition = _curPosition;
//      _curScreenPoint = new Vector3(Input.mousePosition.x, Input.mousePosition.y, _screenPoint.z);
//      _curPosition = Camera.main.ScreenToWorldPoint(_curScreenPoint) + _offset;
//      _velocity = _curPosition - _prevPosition;
//      transform.position = _curPosition;
//  }
//  void OnMouseUp()
//  {
//      _underInertia = true;
//      Screen.showCursor = true;
//  }
// }

using UnityEngine;

public class Follower : MonoBehaviour
{
    public Transform target;
    float damping = 0.075f;

    void LateUpdate()
    {
        // Early out if we don't have a target
        if (!target) return;

        Vector3 current = transform.position;
        Vector3 vel = Vector3.zero;
        current = Vector3.SmoothDamp(current, target.position, ref vel, damping);
        transform.position = current;
    }
}