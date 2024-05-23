using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.NetCode;
using UnityEngine;

public struct GoInGameCommand : IRpcCommand { }

[WorldSystemFilter(WorldSystemFilterFlags.ClientSimulation)]
public partial struct ClientGoInGameSystem : ISystem
{
    void OnCreate(ref SystemState state) {
        state.RequireForUpdate<BeginSimulationEntityCommandBufferSystem.Singleton>();
    }

    void OnUpdate(ref SystemState state) {
        var ecb = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>()
            .CreateCommandBuffer(state.WorldUnmanaged);

        foreach (var (networkId, entity) in SystemAPI
            .Query<RefRO<NetworkId>>()
            .WithNone<NetworkStreamInGame>()
            .WithEntityAccess()) {

            ecb.AddComponent<NetworkStreamInGame>(entity);

            var sendEntity = ecb.CreateEntity();
            ecb.AddComponent<SendRpcCommandRequest>(sendEntity);
            ecb.AddComponent<GoInGameCommand>(sendEntity);
        }
    }
}

[WorldSystemFilter(WorldSystemFilterFlags.ServerSimulation)]
public partial struct ServerGoInGameSystem : ISystem {
    void OnCreate(ref SystemState state) {
        state.RequireForUpdate<BeginSimulationEntityCommandBufferSystem.Singleton>();
        state.RequireForUpdate<Prefabs>();
    }

    void OnUpdate(ref SystemState state) {
        var ecb = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>()
            .CreateCommandBuffer(state.WorldUnmanaged);
        var networkIdLookup = SystemAPI.GetComponentLookup<NetworkId>();

        foreach (var (receive, command, receiveEntity) in SystemAPI
            .Query<RefRO<ReceiveRpcCommandRequest>, RefRO<GoInGameCommand>>()
            .WithEntityAccess()) {

            ecb.DestroyEntity(receiveEntity);

            ecb.AddComponent<NetworkStreamInGame>(receive.ValueRO.SourceConnection);

            Debug.Log("Create player");

            // create player
            SystemAPI.TryGetSingleton(out Prefabs prefabs);

            var playerEntity = ecb.Instantiate(prefabs.Player);

            var networkId = networkIdLookup[receive.ValueRO.SourceConnection].Value;
            ecb.AddComponent(playerEntity, new GhostOwner { NetworkId = networkId });
        }
    }
}