﻿using Globals.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Globals.Abstractions
{
    public interface IRepoServiceBase<T> where T : EntityBase
    {
        public Boolean AddEntity(T entity);

        public T GetEntity(Guid guid);

        public List<T> GetEntities();

        public Boolean UpdateEntity(T entity);

        public Boolean DelEntity(Guid guid);
    }
}
