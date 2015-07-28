using Core.Common.Contract;
using Core.Common.Data;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using Core.Common.Data;
using Core.Common.Contract;

namespace CarRental.Data
{
    public abstract class DataRepositoryBase<T> : DataRepositoryBase<T,CarRentalContext>
        where T : class, IIdentifiableEntity, new()
    {
    }
}
