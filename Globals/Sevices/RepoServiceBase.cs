using Globals.Abstractions;
using Globals.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Globals.Sevices
{
    public class RepoServiceBase<T, V> : IRepoServiceBase<T> where T : EntityBase where V : ContextBase<T>
    {
        private readonly ILogger<RepoServiceBase<T, V>> _logger;

        public RepoServiceBase(ILogger<RepoServiceBase<T, V>> logger) { _logger = logger; }

        public virtual bool AddEntity(T entity)
        {
            var result = false;
            using (var db = (V)Activator.CreateInstance(typeof(V)))
            {
                db.Values.Add(entity);
                db.SaveChanges();
                result = true;
            }
            return result;
        }

        public virtual bool DelEntity(Guid guid)
        {
            using (var db = (V)Activator.CreateInstance(typeof(V)))
            {
                var findEntity = db.Values.FirstOrDefault(x => x.Guid == guid);
                if (findEntity == null) return false;

                db.Values.Remove(findEntity);
                db.SaveChanges();
            }
            return true;
        }

        public virtual List<T> GetEntities()
        {
            using (var db = (V)Activator.CreateInstance(typeof(V)))
                return db.Values.ToList();
        }

        public virtual T GetEntity(Guid guid)
        {
            using (var db = (V)Activator.CreateInstance(typeof(V)))
                return db.Values.FirstOrDefault(x => x.Guid == guid);
        }

        public virtual bool UpdateEntity(T entity)
        {
            throw new NotImplementedException();
        }
    }
}
