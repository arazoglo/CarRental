using CarRental.Business.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarRental.Business.Managers
{
    public class RentalManager : ManagerBase, IRentalService
    {

        public IEnumerable<Entities.Rental> GetRentalHistory(string loginEmail)
        {
            throw new NotImplementedException();
        }
    }
}
