using System.Data.Entity.Infrastructure;
using System.Text.RegularExpressions;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Linq.Expressions;

namespace SignixDemo.DataLayer
{
    public class Repository : IRepository
    {
        [ThreadStatic]
        private static Repository _repository;

        private DataContext DataContext { get; set; }

        public static Repository Instance
        {
            get
            {
                return _repository ?? (_repository = new Repository());
            }
        }

        private Repository()
        {
            DataContext = new DataContext();

            DataContext.Configuration.LazyLoadingEnabled = true;
        }

        private DbSet<T> DbSet<T>() where T : class
        {
            return DataContext.Set<T>();
        }

        public string GetTableName<T>() where T : class
        {
            var objectContext = ((IObjectContextAdapter)DataContext).ObjectContext;
            var sql = objectContext.CreateObjectSet<T>().ToTraceString();
            var regex = new Regex(@"FROM\s+(?<table>.+)\s+AS");
            var match = regex.Match(sql);
            return match.Groups["table"].Value;
        }

        public IQueryable<T> All<T>() where T : class
        {
            return DbSet<T>().AsQueryable();
        }

        public static IQueryable<T> Table<T>() where T : class
        {
            return Instance.DbSet<T>();
        }

        public IQueryable<T> Filter<T>(Expression<Func<T, bool>> predicate, params string[] includes) where T : class
        {
            var dbSet = DbSet<T>().Where(predicate).AsQueryable();

            if (includes == null || !includes.Any()) return dbSet;

            foreach (var include in includes)
            {
                dbSet = dbSet.Include(include);
            }
            return dbSet;
        }

        //public void Filter<T>(Expression<Func<T, bool>> predicate, PagedView<T> pagedView) where T : class
        //{
        //    var sortExpression = string.Format("{0} {1}", pagedView.Sort ?? "Id", pagedView.SortDir ?? "ASC");

        //    var dbSet = DbSet<T>().Where(predicate).OrderBy(sortExpression).AsQueryable();

        //    pagedView.TotalRecords = dbSet.Count();

        //    pagedView.RecordIndex = ((pagedView.Page ?? 1) - 1) * PageSize;

        //    pagedView.ResultSet = dbSet.Skip(pagedView.RecordIndex).Take(PageSize);
        //}

        public T Find<T>(params object[] keys) where T : class
        {
            return DbSet<T>().Find(keys);
        }

        public T Find<T>(Expression<Func<T, bool>> predicate) where T : class
        {
            return DbSet<T>().Where(predicate).FirstOrDefault();
        }

        public bool IsExists<T>(Expression<Func<T, bool>> predicate) where T : class
        {
            return DbSet<T>().Any(predicate);
        }

        public T Insert<T>(T entity, bool commit = true) where T : class
        {
            var newEntry = DbSet<T>().Add(entity);

            if (commit)
            {
                DataContext.SaveChanges();
            }

            return newEntry;
        }

        public int Update<T>(T entity) where T : class
        {
            var entry = DataContext.Entry(entity);
            DbSet<T>().Attach(entity);
            entry.State = EntityState.Modified;
            return DataContext.SaveChanges();
        }

        public int Update<T>(T entity, bool commit) where T : class
        {
            var entry = DataContext.Entry(entity);

            var primaryKey = 0;

            var prop = entity.GetType().GetProperty("Id");

            if (prop != null)
            {
                primaryKey = (int)prop.GetValue(entity, null);
            }

            if (entry.State != EntityState.Detached) return commit ? DataContext.SaveChanges() : 0;

            var currentEntry = DbSet<T>().Find(primaryKey);

            if (currentEntry != null)
            {
                var attachedEntry = DataContext.Entry(currentEntry);

                attachedEntry.CurrentValues.SetValues(entity);
            }
            else
            {
                DbSet<T>().Attach(entity);

                entry.State = EntityState.Modified;
            }

            return commit ? DataContext.SaveChanges() : 0;
        }

        public void DetachEntity<T>(T entity) where T : class
        {
            DataContext.Entry(entity).State = EntityState.Detached;
        }

        public int Delete<T>(T entity) where T : class
        {
            DbSet<T>().Remove(entity);
            return DataContext.SaveChanges();
        }

        public void Delete<T>(T entity, bool commit = true) where T : class
        {
            DbSet<T>().Remove(entity);
            if (commit)
                DataContext.SaveChanges();
        }

        public int Delete<T>(Expression<Func<T, bool>> predicate, bool commit = true) where T : class
        {
            var objects = Filter(predicate);

            foreach (var obj in objects)
            {
                DbSet<T>().Remove(obj);
            }

            return commit ? DataContext.SaveChanges() : 0;
        }

        public int Count<T>(Expression<Func<T, bool>> predicate) where T : class
        {
            return DbSet<T>().Count(predicate);
        }

        public IEnumerable<T> ExecuteSqlQuery<T>(string commandText, params SqlParameter[] parameters) where T : struct
        {
            return DataContext.Database.SqlQuery<T>(commandText, parameters);
        }

        public int ExecuteSqlCommand(string commandText, params SqlParameter[] parameters)
        {
            return DataContext.Database.ExecuteSqlCommand(commandText, parameters);
        }

        public int ExecuteNonQuery(string commandText, CommandType commandType, object parameters)
        {
            var rowCount = 0;
            var cmd = DataContext.Database.Connection.CreateCommand();
            cmd.CommandText = commandText;
            cmd.CommandType = commandType;
            AddParameters(cmd, parameters);
            var isMyConnection = false;
            if (DataContext.Database.Connection.State != ConnectionState.Open)
            {
                DataContext.Database.Connection.Open();
                isMyConnection = true;
            }
            try
            {
                rowCount = cmd.ExecuteNonQuery();
            }
            finally
            {
                if (isMyConnection)
                    DataContext.Database.Connection.Close();
            }

            return rowCount;
        }

        public DataSet ExecuteReader(string commandText, CommandType commandType, object parameters, params string[] resultSets)
        {
            var ds = new DataSet();
            var cmd = DataContext.Database.Connection.CreateCommand();
            cmd.CommandText = commandText;
            cmd.CommandType = commandType;

            AddParameters(cmd, parameters);


            var isMyConnection = false;
            if (DataContext.Database.Connection.State != ConnectionState.Open)
            {
                DataContext.Database.Connection.Open();
                isMyConnection = true;
            }
            try
            {
                using (var reader = cmd.ExecuteReader())
                {
                    ds.Load(reader, LoadOption.OverwriteChanges, resultSets);
                }
            }
            finally
            {
                if (isMyConnection)
                    DataContext.Database.Connection.Close();
            }
            return ds;
        }

        public object ExecuteScalor(string commandText, CommandType commandType, object parameters, params string[] resultSets)
        {
            var cmd = DataContext.Database.Connection.CreateCommand();
            cmd.CommandText = commandText;
            cmd.CommandType = commandType;
            AddParameters(cmd, parameters);
            var isMyConnection = false;
            if (DataContext.Database.Connection.State != ConnectionState.Open)
            {
                DataContext.Database.Connection.Open();
                isMyConnection = true;
            }
            try
            {
                return cmd.ExecuteScalar();
            }
            finally
            {
                if (isMyConnection)
                    DataContext.Database.Connection.Close();
            }
        }

        //public List<T> ExecuteReader<T>(string commandText, CommandType commandType, object parameters) where T : class
        //{
        //    var entityList = new List<T>();
        //    var cmd = DataContext.Database.Connection.CreateCommand();
        //    cmd.CommandText = commandText;
        //    cmd.CommandType = commandType;

        //    AddParameters(cmd, parameters);

        //    var isMyConnection = false;

        //    if (DataContext.Database.Connection.State != ConnectionState.Open)
        //    {
        //        DataContext.Database.Connection.Open();

        //        isMyConnection = true;
        //    }

        //    try
        //    {
        //        var dr = cmd.ExecuteReader(CommandBehavior.CloseConnection);

        //        var schemaTable = dr.GetSchemaTable();

        //        while (dr.Read())
        //        {
        //            var instance = Activator.CreateInstance<T>();

        //            var isEmptyRow = true;

        //            foreach (var property in typeof(T).GetProperties())
        //            {
        //                var sqlColName = property.GetCustomAttributes(typeof(SqlColumn), true).FirstOrDefault() as SqlColumn;

        //                if (schemaTable != null && (sqlColName == null
        //                                            || !schemaTable.Rows.OfType<DataRow>().Any(row => row["ColumnName"].ToString().Equals(sqlColName.Name)))) continue;

        //                if (sqlColName != null && dr[sqlColName.Name] == DBNull.Value) continue;

        //                if (sqlColName != null) property.SetValue(instance, dr[sqlColName.Name], null);

        //                isEmptyRow = false;
        //            }

        //            if (!isEmptyRow)
        //                entityList.Add(instance);
        //        }
        //        dr.Close();
        //    }

        //    finally
        //    {
        //        if (isMyConnection)
        //            DataContext.Database.Connection.Close();
        //    }

        //    return entityList;

        //}

        public bool IsExists(string commandText, CommandType commandType, object parameters)
        {
            bool flag = false;
            var cmd = DataContext.Database.Connection.CreateCommand();
            cmd.CommandText = commandText;
            cmd.CommandType = commandType;

            AddParameters(cmd, parameters);

            var isMyConnection = false;

            if (DataContext.Database.Connection.State != ConnectionState.Open)
            {
                DataContext.Database.Connection.Open();

                isMyConnection = true;
            }

            try
            {
                var dr = cmd.ExecuteReader(CommandBehavior.CloseConnection);

                var schemaTable = dr.GetSchemaTable();

                if (dr.Read())
                {
                    flag = true;
                }
                dr.Close();
            }

            finally
            {
                if (isMyConnection)
                    DataContext.Database.Connection.Close();
            }

            return flag;

        }

        private static void AddParameters(System.Data.Common.DbCommand cmd, object parameters)
        {
            if (parameters == null) return;

            var type = parameters.GetType();
            var props = type.GetProperties();

            foreach (var propertyInfo in props)
            {
                var parameter = cmd.CreateParameter();
                parameter.ParameterName = "@" + propertyInfo.Name;
                parameter.Value = propertyInfo.GetValue(parameters, null);
                cmd.Parameters.Add(parameter);
            }
        }

        public void Commit()
        {
            DataContext.SaveChanges();
        }

        public static void Dispose()
        {
            if (_repository == null)
                return;

            _repository.DataContext.Dispose();
            _repository.DataContext = null;
            _repository = null;
        }

        //public static void GetPagedView<T>(IQueryable<T> itemList, Expression<Func<T, bool>> predicate, PagedView<T> pagedView) where T : class
        //{
        //    var sortExpression = string.Format("{0} {1}", pagedView.Sort ?? "Id", pagedView.SortDir ?? "ASC");

        //    var dbSet = itemList.Where(predicate).OrderBy(sortExpression);

        //    pagedView.TotalRecords = dbSet.Count();

        //    pagedView.RecordIndex = ((pagedView.Page ?? 1) - 1) * PageSize;

        //    pagedView.ResultSet = dbSet.Skip(pagedView.RecordIndex).Take(PageSize);
        //}

        //public static void GetPagedView<T>(IEnumerable<T> itemList, PagedView<T> pagedView) where T : class
        //{
        //    var sortExpression = string.Format("{0} {1}", pagedView.Sort ?? "Id", pagedView.SortDir ?? "ASC");

        //    var dbSet = itemList.OrderBy(sortExpression);

        //    pagedView.TotalRecords = dbSet.Count();

        //    pagedView.RecordIndex = ((pagedView.Page ?? 1) - 1) * PageSize;

        //    pagedView.ResultSet = dbSet.Skip(pagedView.RecordIndex).Take(PageSize);
        //}
    }
}
