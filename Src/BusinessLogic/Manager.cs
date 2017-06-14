using System;
using System.Collections.Generic;
using System.Linq;
using DataSchema;

namespace BusinessLogic
{
    public abstract class Manager<T> where T: Entity, INamedEntity, new()
    {
        /// <summary>
        /// Message from last performed operation
        /// </summary>
        public string Message { get; protected set; }

        /// <summary>
        /// Last performed operation
        /// </summary>
        public string Operation { get; protected set; }

        public IRepository<T> Repository { get; set; }

        public virtual T Load(int id)
        {
            return Repository.Load(id);
        }

        public virtual IList<T> GetAll()
        {
            return Repository.Load().ToList();
        }

        public abstract bool ValidateOnAdd(T item);
        public abstract bool ValidateOnUpdate(T item);
        public abstract bool ValidateOnDelete(T item);

        public virtual bool TryAdd(T item)
        {
            Operation = Localization.Instance.AddOperation;

            if (ValidateOnAdd(item) == false)
                return false;

            Repository.Save(item);
            return true;
        }

        public virtual bool TryUpdate(T item)
        {
            Operation = Localization.Instance.UpdateOperation;

            if (ValidateOnAdd(item) == false)
                return false;

            if (item.Id <= 0)
            {
                Message = Localization.Instance.EntityError;
                return false;
            }

            if (ValidateOnUpdate(item) == false)
                return false;

            Repository.Update(item);
            return true;
        }

        public virtual bool TryDelete(T item)
        {
            Operation = Localization.Instance.DeleteOperation;

            if (item == null)
            {
                Message = Localization.Instance.NullError;
                return false;
            }

            if (ValidateOnDelete(item) == false)
                return false;

            Repository.Delete(item);

            return true;
        }        
    }
}