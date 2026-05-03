using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using WA_StoreControl.Models;

namespace WA_StoreControl.Utilidades
{
    public abstract class CRUDBaseService<T> where T : class
    {
        public DBStore db { get; set; }

        public CRUDBaseService(DBStore context)
        {
            db = context;
        }

        /// <summary>
        /// Extrae todo los registros
        /// </summary>
        /// <returns></returns>
        public IQueryable<T> GetAll()
        {
            return db.Set<T>().AsNoTracking();
        }

        /// <summary>
        /// Extrae todo los registro depediendo de una exprecion como filtrado
        /// </summary>
        /// <param name="predicate">Expresion lambda, usado las propiedades del modelo</param>
        /// <returns></returns>
        public IQueryable<T> GetAll(Expression<Func<T, bool>> predicate)
        {
            return db.Set<T>().Where(predicate);
        }

        /// <summary>
        /// Crear un nievo registro
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public bool Create(T entity)
        {
            db.Entry(entity).State = EntityState.Added;
            return db.SaveChanges() > 0;
        }

        /// <summary>
        /// Crear Multiples registros
        /// </summary>
        /// <param name="entities">Lista de registros a crear</param>
        /// <returns></returns>
        public bool CreateMultiples(IEnumerable<T> entities)
        {
            db.Set<T>().AddRange(entities);
            return db.SaveChanges() > 0;
        }

        /// <summary>
        /// Actualiza un registro
        /// </summary>
        /// <param name="entity"></param>
        public bool Update(T entity)
        {
            db.Entry(entity).State = EntityState.Modified;
            return db.SaveChanges() > 0;
        }

        /// <summary>
        /// Elimina un registro
        /// </summary>
        /// <param name="entityId"></param>
        /// <returns></returns>
        public bool Delete(int entityId)
        {
            var entity = db.Set<T>().Find(entityId);

            if (entity != null)
            {
                db.Entry(entity).State = EntityState.Deleted;
                return db.SaveChanges() > 0;
            }
            else
                return false;
        }

        /// <summary>
        /// Encuentra un registro con el Identificador especificado
        /// </summary>
        /// <param name="id">Identificador del registro a especificar</param>
        /// <returns></returns>
        public T Find(int id)
        {
            var entity = db.Set<T>().Find(id);

            return entity ?? (T)Activator.CreateInstance(typeof(T));
        }

        public bool Any(Expression<Func<T, bool>> predicate)
        {
            return db.Set<T>().Any(predicate);
        }

        /// <summary>
        /// Metodo que sirve para devolver una lista paginada. 
        /// </summary>
        /// <param name="entities">lista de elementos. tiene que ir ordenada</param>
        /// <param name="viewModel">Propiedades de paginacion</param>
        /// <returns></returns>
        public IQueryable<T> PaginateData(IQueryable<T> entities, SearchViewModel viewModel)
        {
            // Acualizar valores del viewModel...
            viewModel.TotalRecords = entities.Count();
            viewModel.TotalPages = viewModel.TotalRecords / viewModel.RecordsPerPage;

            // En aso de que hayan registros que no alcanzaron en la pagina fina o si el calculo de pagina = 0, agregar una pagina mas...
            if (viewModel.TotalRecords % viewModel.RecordsPerPage > 0)
                viewModel.TotalPages++;

            //En caso de que la pagina solicitada ya no exista, retroceder una pagina hacia atras...
            if (viewModel.TotalPages > 0 && viewModel.Page > viewModel.TotalPages)
                viewModel.Page--;

            //Paginar consulta...
            entities = entities.Skip((viewModel.Page - 1) * viewModel.RecordsPerPage).Take(viewModel.RecordsPerPage);

            return entities;
        }
    }
}