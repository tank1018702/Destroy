using System;
using System.Collections.Generic;
using System.Linq;

namespace Destroy.ECS
{
    public class EntityManager
    {
        private struct EntityComponent
        {
            public Entity Entity;
            public List<IComponent> Components;
        }

        private int id;

        private Dictionary<int, EntityComponent> entities = new Dictionary<int, EntityComponent>();


        public int EntityCount => entities.Count;

        public Entity CreatEntity()
        {
            Entity entity = new Entity(id);
            EntityComponent entityComponent = new EntityComponent { Entity = entity };

            entities.Add(id, entityComponent);
            id++;
            return entity;
        }

        public Entity CreatEntity(params IComponent[] components)
        {
            Entity entity = new Entity(id);
            EntityComponent entityComponent = new EntityComponent { Entity = entity };
            entityComponent.Components = components.ToList();

            entities.Add(id, entityComponent);
            id++;
            return entity;
        }

        public void DestroyEntity(Entity entity) => entities.Remove(entity.ID);


        public void AddComponent<T>(Entity entity) where T : struct, IComponent
        {
            if (!entities.ContainsKey(entity.ID))
                return;
            EntityComponent entityComponent = entities[entity.ID];

            foreach (var each in entityComponent.Components)
            {
                if (each.GetType() == typeof(T))
                    return;
            }

            T instance = new T();
            entityComponent.Components.Add(instance);
            entities[entity.ID] = entityComponent;
        }

        public void RemoveComponent<T>(Entity entity) where T : struct, IComponent
        {
            if (!entities.ContainsKey(entity.ID))
                return;

            EntityComponent entityComponent = entities[entity.ID];
            IComponent component = null;

            foreach (var each in entityComponent.Components)
            {
                if (each.GetType() == typeof(T))
                    component = each;
            }

            entityComponent.Components.Remove(component);
            entities[entity.ID] = entityComponent;
        }

        public void SetComponent<T>(Entity entity, T component) where T : struct, IComponent
        {
            if (!entities.ContainsKey(entity.ID))
                return;

            EntityComponent entityComponent = entities[entity.ID];

            for (int i = 0; i < entityComponent.Components.Count; i++)
            {
                if (entityComponent.Components[i].GetType() == typeof(T))
                    entityComponent.Components[i] = component;
            }

            entities[entity.ID] = entityComponent;
        }

        public IComponent GetComponent<T>(Entity entity) where T : struct, IComponent
        {
            if (!entities.ContainsKey(entity.ID))
                return null;

            EntityComponent entityComponent = entities[entity.ID];

            foreach (var each in entityComponent.Components)
            {
                if (each.GetType() == typeof(T))
                    return each;
            }
            return null;
        }
    }
}