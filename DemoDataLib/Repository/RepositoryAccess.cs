using DemoDataLib.Core.Accessor;
using DemoDataLib.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DemoDataLib.Core.Attribute;
using DemoDataLib.Data;
using DemoDataLib.Core.Interface;

namespace DemoDataLib.Repository
{
    public abstract class RepositoryAccess<DBC> where DBC : DbContext, new()
    {
        internal DBC DbContext { get; set; }
        public RepositoryAccess(Connection conn)
        {
            DbContext = new DBC();
            DbContext.Init(conn);
        }

        public List<TReturn> GetByFilter<TParam, TReturn>(Filter<TParam, TReturn> filter, Stack<object> stack = null)
            where TParam : ICustomParam
            where TReturn : ICustomReturn
        {
            return DbContext.GetByFilter(filter);
        }

        public TReturn HandleByActuator<TParam, TReturn>(Actuator<TParam, TReturn> actuator, Stack<object> stack = null)
            where TParam : ICustomParam
            where TReturn : class
        {
            return DbContext.HandleByActuator(actuator);
        }
    }

    public abstract class RepositoryAccess<TDataTable, DBC> : RepositoryAccess<DBC> where TDataTable : IDatabaseTable
        where DBC : DbContext, new()
    {
        private TableAttribute TableAttribute;
        private DataAccess<TDataTable> TableContext;
        private Type TableType;

        public RepositoryAccess(Connection conn) : base(conn)
        {
            string TableName = typeof(TDataTable).Name;
            TableContext = (DataAccess<TDataTable>)DbContext.GetType().GetProperty(TableName).GetValue(DbContext);
            TableType = TableContext.GetType();
        }

        public TDataTable Insert(TDataTable param)
        {

            InsertBefore(param, TableAttribute, TableContext);

            var insertMethod = TableType.GetMethod("Insert");
            int id = (int)insertMethod.Invoke(TableContext, new object[] { param });
            param.ID = id;

            InsertAfter(param, param, TableAttribute, TableContext);

            return param;
        }

        public virtual TDataTable Update(TDataTable param)
        {

            UpdateBefore(param, TableAttribute, TableContext);

            var getMethod = TableType.GetMethod("GetByPrimaryKey");
            var oldData = (TDataTable)getMethod.Invoke(TableContext, new object[] { param, DbAccess.LockTypes.UPDLOCK });
            var newData = param;

            var updateMethod = TableType.GetMethod("Update");
            updateMethod.Invoke(TableContext, new object[] { oldData, newData });

            UpdateAfter(param, newData, TableAttribute, TableContext);

            return newData;
        }

        public virtual TDataTable GetByPrimaryKey(TDataTable param, DbAccess.LockTypes lockType)
        {
            GetByPrimaryKeyBefore(param, TableAttribute, TableContext, lockType);

            var getMethod = TableType.GetMethod("GetByPrimaryKey");
            TDataTable data = (TDataTable)getMethod.Invoke(TableContext, new object[] { param, lockType });

            GetByPrimaryKeyAfter(param, data, TableAttribute, TableContext, lockType);

            return data;
        }

        protected virtual void InsertBefore(TDataTable param, TableAttribute tableAttr, DataAccess<TDataTable> table)
        {
            InsertBeforeEvent?.Invoke(param, TableAttribute, TableContext);
        }
        protected virtual void InsertAfter(TDataTable param, TDataTable data, TableAttribute tableAttr, DataAccess<TDataTable> table)
        {
            InsertAfterEvent?.Invoke(param, data, TableAttribute, TableContext);
        }
        protected virtual void UpdateBefore(TDataTable param, TableAttribute tableAttr, DataAccess<TDataTable> table)
        {
            UpdateBeforeEvent?.Invoke(param, TableAttribute, TableContext);

        }
        protected virtual void UpdateAfter(TDataTable param, TDataTable data, TableAttribute tableAttr, DataAccess<TDataTable> table)
        {
            UpdateAfterEvent?.Invoke(param, data, TableAttribute, TableContext);

        }
        protected virtual void GetByPrimaryKeyBefore(TDataTable param, TableAttribute tableAttr, DataAccess<TDataTable> table, DbAccess.LockTypes lockType)
        {
            GetByPrimaryKeyBeforeEvent?.Invoke(param, TableAttribute, TableContext, lockType);
        }
        protected virtual void GetByPrimaryKeyAfter(TDataTable param, TDataTable data, TableAttribute tableAttr, DataAccess<TDataTable> table, DbAccess.LockTypes lockType)
        {
            GetByPrimaryKeyAfterEvent?.Invoke(param, data, TableAttribute, TableContext, lockType);
        }

        public delegate void InsertBeforeEventHandler(TDataTable param, TableAttribute tableAttr, DataAccess<TDataTable> table);
        public delegate void InsertAfterEventHandler(TDataTable param, TDataTable data, TableAttribute tableAttr, DataAccess<TDataTable> table);
        public delegate void UpdateBeforeEventHandler(TDataTable param, TableAttribute tableAttr, DataAccess<TDataTable> table);
        public delegate void UpdateAfterEventHandler(TDataTable param, TDataTable data, TableAttribute tableAttr, DataAccess<TDataTable> table);
        public delegate void GetByPrimaryKeyBeforeEventHandler(TDataTable param, TableAttribute tableAttr, DataAccess<TDataTable> table, DbAccess.LockTypes lockType);
        public delegate void GetByPrimaryKeyAfterEventHandler(TDataTable param, TDataTable data, TableAttribute tableAttr, DataAccess<TDataTable> table, DbAccess.LockTypes lockType);

        public event InsertBeforeEventHandler InsertBeforeEvent;
        public event InsertAfterEventHandler InsertAfterEvent;
        public event UpdateBeforeEventHandler UpdateBeforeEvent;
        public event UpdateAfterEventHandler UpdateAfterEvent;
        public event GetByPrimaryKeyBeforeEventHandler GetByPrimaryKeyBeforeEvent;
        public event GetByPrimaryKeyAfterEventHandler GetByPrimaryKeyAfterEvent;

    }

}
