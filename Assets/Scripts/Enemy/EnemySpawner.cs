using System;
using FGSTest.Enemy;
using Unity.Burst;
using Unity.Collections;
using UnityEngine;
using Unity.Mathematics;

namespace FGSTest.Enemy
{

    public struct EnemyEntityPhysicalData
    {
        public float3 Position;
        public float3 DistanceFromPlayer;
        public float3 MovementDirection;
    }

    public struct EnemyEntityVisualData
    {
        public float YEulerRotation;
        //todo add more visual data: spawning, despawning
    }
    
    [Serializable]
    public struct EnemyEntityData
    {
        //public byte IsActiveData; //bool 0 for not active, 1 for active
        public EnemyEntityPhysicalData PhysicalData;
        public EnemyEntityVisualData VisualData;
    }
}

public static class EnemyManagerUtils
{
    [BurstCompile]
    public static void UpdateEnemyDistance(NativeBitArray mask, NativeArray<EnemyEntityData> enemyEntityData, float3 playerPosition)
    {
        for (int i = 0; i < enemyEntityData.Length; i++)
        {
            if (!mask.IsSet(i))
                continue;
            
            var entityData = enemyEntityData[i];
            entityData.PhysicalData.DistanceFromPlayer = playerPosition - enemyEntityData[i].PhysicalData.Position;
            var dir = entityData.PhysicalData.DistanceFromPlayer;
            dir.y = 0;
            entityData.PhysicalData.MovementDirection = math.normalize(dir);
            enemyEntityData[i] = entityData;
        }
    }

    [BurstCompile]
    public static void UpdateEnemyChasePlayer(NativeBitArray mask, NativeArray<EnemyEntityData> enemyEntityData, float constantSpeed,
        float deltaTime)
    {
        for (int i = 0; i < enemyEntityData.Length; i++)
        {
            if (!mask.IsSet(i))
                continue;
            var entityData = enemyEntityData[i];
            var moveDir = entityData.PhysicalData.MovementDirection;
            entityData.PhysicalData.Position += moveDir * constantSpeed * deltaTime;
            enemyEntityData[i] = entityData;
        }
    }
        
    [BurstCompile]
    public static void UpdateEnemyRotation(NativeBitArray mask, NativeArray<EnemyEntityData> enemyEntityData)
    {
        for (int i = 0; i < enemyEntityData.Length; i++)
        {
            if (!mask.IsSet(i))
                continue;
            var entityData = enemyEntityData[i];
            entityData.VisualData.YEulerRotation = math.degrees(math.atan2(entityData.PhysicalData.MovementDirection.x, entityData.PhysicalData.MovementDirection.z));
            enemyEntityData[i] = entityData;
        }
    }
    
    
    [BurstCompile]
    public static void UpdateEnemyTransformMats(NativeBitArray mask, NativeArray<Matrix4x4> transformData, NativeArray<EnemyEntityData> enemyEntityData)
    {
        for (int i = 0; i < enemyEntityData.Length; i++)
        {
            if (!mask.IsSet(i))
                continue;
            
            var entityData = enemyEntityData[i];
            transformData[i] = float4x4.TRS(entityData.PhysicalData.Position, Quaternion.identity, Vector3.one);
        }
    }
    
    static float3 up = new float3(0, 1, 0);

    [BurstCompile]
    public static bool CheckEnemyTouchPlayer(NativeBitArray mask, NativeArray<EnemyEntityData> enemyEntityData, float3 playPos, float playerRadius, float enemyRadius, float centerOffsetY)
    {
        for (int i = 0; i < enemyEntityData.Length; i++)
        {
            if (!mask.IsSet(i))
                continue;
            
            var distance = math.distance(playPos, enemyEntityData[i].PhysicalData.Position + up * centerOffsetY);

            if (distance <= playerRadius + enemyRadius)
                return true;
        }
        
        return false;
    }
        
    
}
