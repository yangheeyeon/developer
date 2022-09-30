using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    //delta==πÊ«‚∫§≈Õ
    [SerializeField]
    Vector3 _delta = new Vector3(0.0f, 6.0f, -5.0f);
    [SerializeField]
    GameObject _player = null;
    [SerializeField]
     Define.CameraMode _mode = Define.CameraMode.QuarterView;
    public GameObject SetPlayer { get { return _player; } set { _player = value; } }

    void LateUpdate()
    {
       if(_mode == Define.CameraMode.QuarterView)
        {
            if (_player.IsValid() == false)
                return;
            RaycastHit hit;
            int mask = 1 << 7;
            if( Physics.Raycast(_player.transform.position, _delta, out hit, 30.0f, mask))
            {
                float length = (hit.point - _player.transform.position).magnitude * 0.8f;

                transform.position = _player.transform.position + _delta.normalized * length;
            }
            else
            {
                transform.position =_player.transform.position+  _delta;
                transform.LookAt(_player.transform);
            }
        }
    }
}
