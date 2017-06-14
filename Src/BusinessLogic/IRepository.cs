using System;
using System.Collections.Generic;
using System.Linq;

namespace BusinessLogic
{
    public interface IRepository<T> where T : class
    {
        T Load(int id);

        IEnumerable<T> Load();

        IEnumerable<T> Load(object filter);

        IQueryable<T> Query();

        T Save(T item);

        T SaveOrUpdate(T item);        

        T Update(T item);

        void Delete(T item);   
    }
}
