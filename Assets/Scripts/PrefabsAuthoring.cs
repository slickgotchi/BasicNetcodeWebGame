using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class PrefabsAuthoring : MonoBehaviour
{
    public GameObject PlayerPrefab;

    public class Baker : Baker<PrefabsAuthoring> {
        public override void Bake(PrefabsAuthoring authoring) {
            var entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new Prefabs {
                Player = GetEntity(authoring.PlayerPrefab, TransformUsageFlags.Dynamic),
            });
        }
    }
}

public struct Prefabs : IComponentData {
    public Entity Player;
}