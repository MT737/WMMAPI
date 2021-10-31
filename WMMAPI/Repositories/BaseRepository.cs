using Microsoft.EntityFrameworkCore;
using System;
using WMMAPI.Database;
using WMMAPI.Interfaces;

namespace WMMAPI.Repositories
{
    public abstract class BaseRepository<TEntity> where TEntity : class
    {
        // Properties
        protected WMMContext Context { get; set; }

        // Base Constructor
        public BaseRepository(WMMContext context)
        {
            Context = context;
        }

        // CUD of CRUD
        /// <summary>
        /// Add entity to the DbSet and then save the changes to the database.
        /// </summary>
        /// <param name="entity">Repository type</param>
        public void Add(TEntity entity)
        {
            Context.Set<TEntity>().Add(entity);
            Context.SaveChanges();
        }

        /// <summary>
        /// Set entity's state to modified and save the changes to the database.
        /// </summary>
        /// <param name="entity">Repository type</param>
        public void Update(TEntity entity)
        {
            Context.Entry(entity).State = EntityState.Modified;
            Context.SaveChanges();
        }

        /// <summary>
        /// Find the specified entity in the DbSet, remove it, and save the changes to the database.
        /// </summary>
        /// <param name="id">Int: Primary key of the entity to be removed.</param>
        public void Delete(Guid id)
        {
            var set = Context.Set<TEntity>();
            var entity = set.Find(id);
            set.Remove(entity);
            Context.SaveChanges();
        }
    }
}
