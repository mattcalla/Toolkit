using System;
using Toolkit.DataAccess.LocalData;
using Toolkit.DataAccess.SimpleDatabase;

namespace Toolkit.DataAccess.UOW
{
    public static class UnitOfWork
    {
        private static IUnitOfWorkFactory _factory;

        private static SimpleDb _simpleDb;

        public static void Initialise(IUnitOfWorkFactory factory)
        {
            _factory = factory;
        }

        public static IUnitOfWork Create()
        {
            CurrentUnitOfWork = _factory.Create();
            return CurrentUnitOfWork;
        }

        public static IUnitOfWork Current 
        { 
            get
            {
                if (CurrentUnitOfWork == null) throw new ApplicationException("No UnitOfWork exists, please start a new UnitOfWork.");
                return CurrentUnitOfWork;
            }
        }

        private const string CurrentUnitOfWorkKey = "CurrentUnitOfWork.Key";
        private static IUnitOfWork CurrentUnitOfWork
        {
            get { return ThreadSafeLocalData.Data[CurrentUnitOfWorkKey] as IUnitOfWork; }
            set { ThreadSafeLocalData.Data[CurrentUnitOfWorkKey] = value; }
        }

        public static void Commit()
        {
            CurrentUnitOfWork.Commit();
        }

        public static void Rollback()
        {
            CurrentUnitOfWork.Rollback();
        }

        public static void Dispose()
        {
            CurrentUnitOfWork = null;
        }

        public static SimpleDb SimpleDb
        {
            get
            {
                return _simpleDb ?? (_simpleDb = new SimpleDb(DatabaseType.SqlServer, _factory.ConnectionString));
            }
        }
    }
}
