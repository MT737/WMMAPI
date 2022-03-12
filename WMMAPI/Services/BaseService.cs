using Microsoft.EntityFrameworkCore;
using System;
using WMMAPI.Database;
using WMMAPI.Interfaces;

namespace WMMAPI.Services
{
    public abstract class BaseService<TEntity> where TEntity : class
    {
        // Properties
        protected WMMContext Context { get; set; }

        // Base Constructor
        public BaseService(WMMContext context)
        {
            Context = context;
        }

        //TODO: Add get and get list. Can pass includes as an argument

        // CUD of CRUD
        /// <summary>
        /// Add entity to the DbSet and then save the changes to the database.
        /// </summary>
        /// <param name="entity">Repository type</param>
        public void Add(TEntity entity, bool saveImmediately = true)
        {
            Context.Set<TEntity>().Add(entity);

            if (saveImmediately)
            {
                Context.SaveChanges();
            }
        }

        /// <summary>
        /// Save the changes to the database. Setting modified state is not required since all changes take place within the context.
        /// </summary>
        /// <param name="entity">Repository type</param>
        public void Update(TEntity entity)
        {
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

        /// <summary>
        /// Calls save changes on current context.
        /// </summary>
        public void SaveChanges()
        {
            Context.SaveChanges();
        }
    }
}
