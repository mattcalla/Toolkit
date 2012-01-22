using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Toolkit.DataAccess.UOW
{
    public interface IDatabaseSession
    {
        void Add(object obj);
        T Get<T>(object id);
        void Delete<T>(object id);
    }
}
