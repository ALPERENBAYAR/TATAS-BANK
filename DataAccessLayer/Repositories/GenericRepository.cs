
using DataAccessLayer.Concrete;
using DataAccessLayer.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace DataAccessLayer.Repositories
{
    public class GenericRepository<T> : IGenericDal<T> where T : class
    {
        public void Insert(T entity)
        {
            using var c = new Context();

            // Eğer entity ilişkili navigation property'ler içeriyorsa ve onlar zaten varsa,
            // EF'nin onları eklemeye çalışmaması için Attach gerekir.
            var entry = c.Entry(entity);

            foreach (var navigation in entry.Navigations)
            {
                if (navigation.Metadata is Microsoft.EntityFrameworkCore.Metadata.INavigation navMeta && navMeta.IsOnDependent)
                {
                    var relatedEntity = navigation.CurrentValue;
                    if (relatedEntity != null)
                    {
                        c.Attach(relatedEntity);
                    }
                }
            }

            c.Set<T>().Add(entity);
            c.SaveChanges();
        }


        public void Update(T entity)
        {
            using var c = new Context();
            c.Update(entity);
            c.SaveChanges();
        }

        public void Delete(T entity)
        {
            using var c = new Context();
            c.Remove(entity);
            c.SaveChanges();
        }

        public  List<T> GetAll()
        {
            using var c = new Context();
            return  c.Set<T>().ToList();
        }


        public T GetById(int id)
        {
            using var c = new Context();
            return c.Set<T>().Find(id);
        }

        }
}
