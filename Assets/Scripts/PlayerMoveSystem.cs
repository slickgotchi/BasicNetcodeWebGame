using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using Unity.NetCode;
using Unity.Transforms;
using UnityEngine;

[UpdateInGroup(typeof(PredictedSimulationSystemGroup))] 
public partial struct PlayerMoveSystem : ISystem
{
    void OnUpdate(ref SystemState state) {
        var dt_s = SystemAPI.Time.DeltaTime;

        foreach (var (input, transform) in SystemAPI
            .Query<RefRO<PlayerInput>, RefRW<LocalTransform>>()) {

            float3 delta = input.ValueRO.Move;
            float SPEED = 4;

            transform.ValueRW.Position += delta * SPEED * dt_s;
        }
    }
}
