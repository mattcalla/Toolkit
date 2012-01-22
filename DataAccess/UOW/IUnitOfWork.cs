using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Toolkit.DataAccess.UOW
{
    public interface IUnitOfWork : IDisposable
    {
        void Commit();
        void Rollback();

        IDatabaseSession DatabaseSession { get; }
        int UserId { get; set; }
    }
}
