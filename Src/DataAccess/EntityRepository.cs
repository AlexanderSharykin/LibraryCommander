using System;
using System.Collections.Generic;
using System.Linq;
using BusinessLogic;
using NHibernate.Criterion;
using NHibernate.Linq;

namespace DataAccess
{
    public class EntityRepository<T> : IRepository<T> where T : class
    {
        public virtual T Load(Int32 id)
        {
            if (id <= 0) throw new ArgumentOutOfRangeException("id", "Load");

            using (var session = SessionFactory.GetFactory().OpenSession())
            {
                return session.Get<T>(id);
            }
        }

        public virtual IEnumerable<T> Load()
        {
            using (var session = SessionFactory.GetFactory().OpenSession())
            {
                return session.CreateCriteria(typeof(T))
                    .AddOrder(Order.Asc("Id")).List<T>();
            }
        }

        public virtual IEnumerable<T> Load(object template)
        {
            return Load();
        }

        public virtual T Save(T item)
        {
            if (item == null) throw new ArgumentNullException("item", "Save");

            using (var session = SessionFactory.GetFactory().OpenSession())
            {
                using (var transaction = session.BeginTransaction())
                {
                    session.Save(item);
                    transaction.Commit();
				}
            }

            return item;
        }

        public virtual T SaveOrUpdate(T item)
        {
            if (item == null) throw new ArgumentNullException("item", "SaveOrUpdate");

            using (var session = SessionFactory.GetFactory().OpenSession())
            {
                using (var transaction = session.BeginTransaction())
                {
                    session.SaveOrUpdate(item);
                    transaction.Commit();
                }
            }

            return item;
        }

        public virtual T Update(T item)
        {
            if (item == null) throw new ArgumentNullException("item", "Update");

            using (var session = SessionFactory.GetFactory().OpenSession())
            {
                using (var transaction = session.BeginTransaction())
                {
                    session.Update(item);
                    transaction.Commit();
                }
            }

            return item;
        }

        public virtual void Delete(T item)
        {
            if (item == null) throw new ArgumentNullException("item", "Delete");

            using (var session = SessionFactory.GetFactory().OpenSession())
            {
                using (var transaction = session.BeginTransaction())
                {
                    session.Delete(item);
                    transaction.Commit();
                }
            }
        }

		public IQueryable<T> Query()
		{
			try
			{
				var session = SessionFactory.GetFactory().OpenSession();
				return session.Query<T>();
			}
			catch (Exception e)
			{
				return null;
			}
		}
    }
}
