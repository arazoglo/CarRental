using Core.Common.Contract;
using Core.Common.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Common.Contract;
using Core.Common.Core;
using System.ComponentModel.Composition;

namespace CarRental.Data.Contracts
{
    [Export(typeof(IDataRepositoryFactory))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class DataRepositoryFactory : IDataRepositoryFactory
    {

        T IDataRepositoryFactory.GetDataRepository<T>()
        {                                                   
            return ObjectBase.Container.GetExportedValue<T>();
        }
    }
}
