using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Toolkit.DataAccess.UOW
{
    public interface IUnitOfWorkFactory
    {
        IUnitOfWork Create();
        string ConnectionString { get; }
    }
}
