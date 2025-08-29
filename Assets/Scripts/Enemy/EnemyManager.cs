using FGSTest.Common;
using FGSTest.Enemy;
using FGSTest.Payload;
using SuperMaxim.Messaging;
using Unity.Burst;
using Unity.Collections;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Rendering;
using Random = UnityEngine.Random;

[BurstCompile]
public class EnemyManager : BaseGameSystem
{
    private const int _maxSize = 500;

    [Header("Spawn Config")] 
    [SerializeField] private float _radiusSpawn;
    [SerializeField] private float _rateSpawn;

    [Header("Process Config")]
    //[SerializeField] private int _partitionSize;
    //[SerializeField] private int _cleanupFrame;
    //[SerializeField] private int _sizeBeginCleanUp;

    //native array
    private NativeArray<EnemyEntityData> _enemyEntityData;
    private NativeArray<float4x4> _enemyTransformData;
    private NativeBitArray _entityDataMask;

    [Header("Enemy Entity Config")] 
    [SerializeField] private float _radius;

    [SerializeField] private float _height;
    [SerializeField] private float _chaseSpeed;
    [SerializeField] private float _centerOffsetY;

    [Header("External Ref")] 
    [SerializeField] private PlayerTracker _playerTracker;

    [Header("Render Params")] 
    [SerializeField] private Material _enemyMaterial;
    [SerializeField] private Mesh _enemyMesh;

    private float _countDownSpawn;
    //private int _partitionIndex;
    //private int _cleanupCountDown;
    private int _currentLength;
    private RenderParams _renderParams;
    
    private void Start()
    {
        _entityDataMask = new NativeBitArray(_maxSize, Allocator.Persistent);
        _enemyEntityData = new NativeArray<EnemyEntityData>(_maxSize, Allocator.Persistent);
        _enemyTransformData = new NativeArray<float4x4>(_maxSize, Allocator.Persistent);
        _renderParams = new RenderParams
        {
            material = _enemyMaterial,
            instanceID = _enemyMaterial.GetInstanceID(),
            renderingLayerMask = RenderingLayerMask.defaultRenderingLayerMask,
            worldBounds = new Bounds(Vector3.zero, new Vector3(100f, 100f, 100f)) // Adjust size as needed
        };
    }
    
    protected override void SystemFixedUpdate()
    {
        
        ProcessCountDownSpawn();
        //ProcessCleanupCountDown();

        UpdateEnemyPosition();
        UpdateEnemyRotation();
        UpdateEnemyTransformMats();
        
        //can extract to separate class
        UpdateEnemyChasePlayer();
        var tag = CheckEnemiesTouchPlayer();

        if (tag)
        {
            Debug.Log("Enemy Touch Player");
            Messenger.Default.Publish(new CaughtPlayerPayload());
        }
    }

    protected override void SystemUpdate()
    {
        RenderEnemies();
    }
    
    private void UpdateEnemyPosition()
    {
        EnemyManagerUtils.UpdateEnemyDistance(ref _entityDataMask, ref _enemyEntityData, _playerTracker.PlayerPosition);
    }
    
    private void UpdateEnemyRotation()
    {
        EnemyManagerUtils.UpdateEnemyRotation(ref _entityDataMask, ref _enemyEntityData);
    }
    
    private void UpdateEnemyTransformMats()
    {
        EnemyManagerUtils.UpdateEnemyTransformMats(ref _entityDataMask, ref _enemyTransformData, ref _enemyEntityData);
    }
    
    private void UpdateEnemyChasePlayer()
    {
        EnemyManagerUtils.UpdateEnemyChasePlayer(ref _entityDataMask, ref _enemyEntityData, _chaseSpeed, Time.deltaTime);
    }
    
    private void RenderEnemies()
    {
        Graphics.RenderMeshInstanced(_renderParams, _enemyMesh, 0, _enemyTransformData.Reinterpret<Matrix4x4>());
        // const int batchSize = 20;
        // int remaining = _currentLength;
        // int offset = 0;
        //
        // while (remaining > 0)
        // {
        //     int count = math.min(batchSize, remaining);
        //
        //     // Pass a subarray slice to RenderMeshInstanced
        //     var slice = _enemyTransformData.GetSubArray(offset, count);
        //
        //     Graphics.RenderMeshInstanced(_renderParams, _enemyMesh, 0, slice);
        //     
        //     Debug.Log($"Render Enemy at {_enemyEntityData[offset].PhysicalData.Position}");
        //
        //     remaining -= count;
        //     offset += count;
        // }
        
    }
    
    private bool CheckEnemiesTouchPlayer()
    {
        return EnemyManagerUtils.CheckEnemyTouchPlayer(ref _entityDataMask, ref _enemyEntityData, _playerTracker.PlayerPosition, _playerTracker.PlayerRadius, _radius, _centerOffsetY);
    }
    

    // private void ProcessCleanupCountDown()
    // {
    //     if (_enemyEntityData.Length < _sizeBeginCleanUp)
    //         return;
    //     
    //     _cleanupCountDown--;
    //     
    //     if (_cleanupCountDown <= 0)
    //     {
    //         CleanupEnemy(); //clean up inactive data
    //         _cleanupCountDown = _cleanupFrame;
    //     }
    // }

    // private void CleanupEnemy()
    // {
    // }

    private void ProcessCountDownSpawn()
    {
       
        _countDownSpawn -= Time.deltaTime;
        
        if (_countDownSpawn <= 0)
        {
            SpawnEnemy();
            _countDownSpawn = _rateSpawn;
        }
    }
    
    private void SpawnEnemy()
    {
        if (LimitReach())
            return;
        
        _entityDataMask.Set(_currentLength, true);
        
        var pos = Random.insideUnitSphere * _radiusSpawn + _playerTracker.PlayerPosition;
        pos.y = 1;
        
        _enemyEntityData[_currentLength] = new EnemyEntityData()
        {
            PhysicalData = new EnemyEntityPhysicalData()
            {
                Position = pos,
                DistanceFromPlayer = float3.zero,
                MovementDirection = float3.zero,
            },
            VisualData = new EnemyEntityVisualData()
            {
                YEulerRotation = 0,
            },
        };
        _currentLength++;
        
    }

    private bool LimitReach()
    {
        return _currentLength >= _maxSize - 1;
    }

    protected override void SystemDestroy()
    {
        _entityDataMask.Dispose();
        _enemyEntityData.Dispose();
        _enemyTransformData.Dispose();
    }
}