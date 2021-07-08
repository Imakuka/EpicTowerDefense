using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameDevHQ.TowerDefense
{
    public class CameraController : MonoSingleton<CameraController>
    {
        [Header("Camera Movement")]
        [SerializeField]
        private Camera _mainCamera;
        [SerializeField]
        private float _cameraSpeed = 6.0f;

        [Header("Camera Input Axis")]
        [SerializeField]
        private float _horizontalInput;
        [SerializeField]
        private float _verticalInput;
        [SerializeField]
        private Vector3 _direction;
        [SerializeField]
        private int _min;
        [SerializeField]
        private int _max;

        [Header("Axis Clamping")]
        [SerializeField]
        private Vector3 _clampedPos;

        [Header("Mouse Look")]
        [SerializeField]
        private Vector3 _mousePos;
        [SerializeField]
        private float _edgeBuffer = .05f;
        [SerializeField]
        private float _leftEdge, _rightEdge, _topEdge, _bottomEdge;
        [SerializeField]
        private bool _inBounds;
      
        private float _scrollWheel = 0.0f;
        [Header("Zoom In/Out")]
        [SerializeField]
        private float _zoomSpeed = 8.0f;
        [SerializeField]
        private float _defaultZoom = 35.0f;
        [SerializeField]
        private float _maxZoomIn = 10.0f;
        [SerializeField]
        private float _maxZoomOut = 20.0f;
        private float _zoomClamp;

        

        // Start is called before the first frame update
        void Start()
        {
            CalculateEdges();
            _mainCamera.fieldOfView = _defaultZoom;
        }

        // Update is called once per frame
        void Update()
        {
            BoundaryCheck();
            CameraMovement();
            CameraZoom();
        }
        void CameraZoom()
        {
            _scrollWheel = Input.GetAxis("Mouse ScrollWheel");
 
            _mainCamera.fieldOfView += _scrollWheel * _zoomSpeed;

            _zoomClamp = _mainCamera.fieldOfView;
            _zoomClamp = Mathf.Clamp(_mainCamera.fieldOfView, _maxZoomIn, _maxZoomOut);
            _mainCamera.fieldOfView = _zoomClamp;

        }
        void CameraMovement()
        {
            
            //WSAD Key Movement
            _horizontalInput = Input.GetAxis("Horizontal");//AD Keys
            _verticalInput = Input.GetAxis("Vertical");//WS Keys


            _mousePos = Input.mousePosition;

            if (_inBounds == true)
            {
                if (_mousePos.x < _leftEdge)
                {
                    _horizontalInput--;
                }
                if (_mousePos.x > _rightEdge)
                {
                    _horizontalInput++;
                }
                if (_mousePos.y < _bottomEdge)
                {
                    _verticalInput--;
                }
                if (_mousePos.y > _topEdge)
                {
                    _verticalInput++;
                }
            }


            _direction = new Vector3(_horizontalInput, _verticalInput, 0);
            transform.Translate(_direction * _cameraSpeed * Time.deltaTime);

            //Clamping the movemet
            _clampedPos = transform.position;
            _clampedPos.x = Mathf.Clamp(transform.position.x, _min, _max);
            _clampedPos.z = Mathf.Clamp(transform.position.z, _min, _max);
            _clampedPos.y = Mathf.Clamp(transform.position.y, 0, 20);//Need to cache
            transform.position = _clampedPos;
            
        }

        void CalculateEdges()
        {
            _leftEdge = Screen.width * _edgeBuffer;
            _rightEdge = Screen.width * (1 - _edgeBuffer);
            _bottomEdge = Screen.height * _edgeBuffer;
            _topEdge = Screen.height * (1 - _edgeBuffer);
        }

        void BoundaryCheck()
        {
            if(_mousePos.x > Screen.width || _mousePos.x < 0 || _mousePos.y < 0 || _mousePos.y > Screen.height)
            {
                _inBounds = false;
            }
            else
            {
                _inBounds = true;
            }
        }


    }
}


