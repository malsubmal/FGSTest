using UnityEngine;

namespace FGSTest.Environment
{
    public class MapConstraint : MonoBehaviour
    {
        [SerializeField] private MeshRenderer _planeMesh;
        [SerializeField] private float _border = 5;
        
        [SerializeField] private Vector3 _maxPos;
        [SerializeField] private Vector3 _minPos;
        
        private void Awake()
        {
            BakeBound();
        }

        private void BakeBound()
        {
            var bounds = _planeMesh.bounds;
            _maxPos = bounds.max;
            _minPos = bounds.min;
            _maxPos -= Vector3.one * _border;
            _minPos += Vector3.one * _border;
        }

        public Vector3 SampleRandomPosInsideMesh()
        {
            Vector3 randomPos = new Vector3(
                Random.Range(_minPos.x, _maxPos.x),
                Random.Range(_minPos.y, _maxPos.y),
                Random.Range(_minPos.z, _maxPos.z)
            );
            
            return randomPos;
        }

        public Vector3 ClampXZPosToMap(Vector3 pos)
        {
            float x = Mathf.Clamp(pos.x, _minPos.x, _maxPos.x);
            float y = Mathf.Clamp(pos.y, _minPos.y, _maxPos.y);
            float z = Mathf.Clamp(pos.z, _minPos.z, _maxPos.z);

            return new Vector3(x, y, z);
        }
        
    }
}