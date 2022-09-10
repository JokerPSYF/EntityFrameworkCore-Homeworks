using System;
using System.Linq;
using System.Collections.Generic;
using System.Reflection;
using System.ComponentModel.DataAnnotations;

namespace MiniORM
{
	internal class ChangeTracker<T> where T : class, new()
    {
        public IReadOnlyCollection<T> AllEntities => this.allEntities.AsReadOnly();
        public IReadOnlyCollection<T> Added => this.added.AsReadOnly(); 
        public IReadOnlyCollection<T> Removed => this.removed.AsReadOnly();

        public ChangeTracker(IEnumerable<T> entities)
        {
            this.added = new List<T>;
            this.removed = new List<T>();

            this.allEntities = CloneEntities(entities);
        }


        private readonly List<T> allEntities;

        private readonly List<T> added;

        private readonly List<T> removed;

        private static List<T>  CloneEntities(IEnumerable<T> entities)
        {
            var clonedEntities = new List<T>();

            var propertiesToClone = typeof(T).GetProperties()
                .Where(pi => DbContext.AllowedSqlTypes.Contains(pi.ProperyType))
                .ToArray();

            foreach(var entity in entities)
            {
                var cloneEntity = Activator.CreateInstance<T>();

                foreach(var property in propertiesToClone)
                {
                    var value = property.GetValue(entity);
                    property.SetValue(cloneEntity, value);
                }

                clonedEntities.Add(cloneEntity);
            }


            return clonedEntities;
        }

        public void  Add(T value) => this.added.Add(value);

        public void Remove(T value) => this.removed.Remove(value);

        public IEnumerable<T> GetModified(DbSet<T> dbSet)
        {
            var modifiedEntities = new List<T>();

            var primaryKeys = typeof(T).GetProperties()
                .Where(pi => pi.HasAttribute<KeyAttribute>())
                .ToArray();

            foreach (var proxyEntity in this.AllEntities)
            {
                var primaryKeysValues = GetPrimaryKeyValues(primaryKeys, proxyEntity).ToArray();

                var entity = dbSet.Entities
                    .Single(e => GetPrimaryKeyValues(primaryKeys, e).SequenceEqual(primaryKeysValues));

                var isModified = IsModified(proxyEntity, entity);
                if (isModified)
                {
                    modifiedEntities.Add(entity);
                }
            }



            return modifiedEntities;
        }

        private static IEnumerable<object> GetPrimaryKeyValues(IEnumerable<PropertyInfo> primaryKeys, T entity)
        {
            return primaryKeys.Select(pk => pk.GetValue(entity));
        }

        private static bool IsModified(T entity, T proxyEntity)
        {
            var monitoredProperties = typeof(T).GetProperties()
                .Where(pi => DbContext.AllowedSqlTypes.Contains(pi.PropertyType));

            var modifiedProperties = monitoredProperties
                .Where(pi => !Equals(pi.GetValue(entity), pi.GetValue(proxyEntity)))
                .ToArray();

            var isModified = modifiedProperties.Any();

            return isModified;
        }

        internal object GetModifiedEntities<TEntity>(DbSet<TEntity> dbSet) where TEntity : class, new()
        {
            throw new NotImplementedException();
        }
    }
}