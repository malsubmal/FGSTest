using UnityEngine;
using System.Collections.Generic;

namespace FGSTest.Common
{
    [RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
    public class MeshBaker : MonoBehaviour
    {
        [SerializeField] private MeshFilter _combinedMeshFilter;
        [SerializeField] private MeshCollider _meshCollider;

        void Start()
        {
            if (_combinedMeshFilter == null)
                _combinedMeshFilter = GetComponent<MeshFilter>();

            // Collect source MeshFilters (children only)
            var meshFilters = GetComponentsInChildren<MeshFilter>();
            var instances = new List<CombineInstance>();

            foreach (var mf in meshFilters)
            {
                if (mf == _combinedMeshFilter || mf.sharedMesh == null)
                    continue;

                instances.Add(new CombineInstance
                {
                    mesh = mf.sharedMesh,
                    transform = _combinedMeshFilter.transform.worldToLocalMatrix * mf.transform.localToWorldMatrix
                });
            }

            // Create the combined mesh
            var combinedMesh = new Mesh
            {
                indexFormat = UnityEngine.Rendering.IndexFormat.UInt32 // supports >65k verts
            };
            combinedMesh.CombineMeshes(instances.ToArray(), true, true);

            // Assign to MeshFilter
            _combinedMeshFilter.sharedMesh = combinedMesh;

            // Disable sources
            foreach (var mf in meshFilters)
            {
                if (mf != _combinedMeshFilter)
                    mf.gameObject.SetActive(false);
            }

            // Assign collider
            if (_meshCollider)
                _meshCollider.sharedMesh = combinedMesh;
        }
    }
}