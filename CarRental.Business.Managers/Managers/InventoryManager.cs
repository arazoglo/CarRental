using CarRental.Business.Contracts;
using CarRental.Business.Entities;
using CarRental.Data.Contracts;
using Core.Common.Contract;
using Core.Common.Core;
using Core.Common.Exceptions;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace CarRental.Business.Managers.Managers
{
    public class InventoryManager : IInventoryService
    {
        public InventoryManager()
        {
            ObjectBase.Container.SatisfyImportsOnce(this);
        }

        public InventoryManager(IDataRepositoryFactory dataRepositoryFatory)
        {
            _DataRepositoryFactory = dataRepositoryFatory;
        }
        [Import]
        IDataRepositoryFactory _DataRepositoryFactory;

        public Entities.Car GetCar(int carId)
        {
            try
            {
                ICarRepository carRepository =
                    _DataRepositoryFactory.GetDataRepository<ICarRepository>();

                Car carEntity = carRepository.Get(carId);
                if (carEntity == null)
                {
                    NotFoundException ex =
                        new NotFoundException(string.Format("Car with ID of {0} is not in the database.", carId));

                    throw new FaultException<NotFoundException>(ex, ex.Message);
                }

                return carEntity;
            }
            catch(FaultException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                throw new FaultException(ex.Message);
            }

        }

        public Entities.Car[] GetAllCars()
        {
            throw new NotImplementedException();
        }
    }
}
