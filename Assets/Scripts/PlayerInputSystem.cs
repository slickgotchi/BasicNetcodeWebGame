using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using Unity.NetCode;
using UnityEngine;

[UpdateInGroup(typeof(GhostInputSystemGroup))]
[WorldSystemFilter(WorldSystemFilterFlags.ClientSimulation)]
public partial struct PlayerInputSystem : ISystem
{
    void OnUpdate(ref SystemState state) {
        float3 move = float3.zero;

        if (Input.GetKey(KeyCode.W)) move.y += 1;
        if (Input.GetKey(KeyCode.S)) move.y -= 1;
        if (Input.GetKey(KeyCode.A)) move.x -= 1;
        if (Input.GetKey(KeyCode.D)) move.x += 1;

        if (!move.Equals(float3.zero)) {
            move = math.normalize(move);
        }

        // update player input data
        foreach (var input in SystemAPI
            .Query<RefRW<PlayerInput>>()
            .WithAll<GhostOwnerIsLocal>()) {

            input.ValueRW.Move = move;
        }
    }
}

[GhostComponent(PrefabType = GhostPrefabType.AllPredicted)]
public struct PlayerInput : IInputComponentData {
    [GhostField] public float3 Move;
}