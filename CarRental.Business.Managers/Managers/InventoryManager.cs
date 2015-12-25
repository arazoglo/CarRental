using CarRental.Business.Contracts;
using CarRental.Business.Entities;
using CarRental.Common;
using CarRental.Data.Contracts;
using Core.Common.Contract;
using Core.Common.Core;
using Core.Common.Exceptions;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Security.Permissions;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace CarRental.Business.Managers
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall,
        ConcurrencyMode=ConcurrencyMode.Multiple,
        ReleaseServiceInstanceOnTransactionComplete=false)]
    public class InventoryManager : ManagerBase, IInventoryService
    {
        public InventoryManager()              
        {
            
        }

        public InventoryManager(IDataRepositoryFactory dataRepositoryFatory)
        {
            _DataRepositoryFactory = dataRepositoryFatory;
        }
        [Import]
        IDataRepositoryFactory _DataRepositoryFactory;

        [PrincipalPermission(SecurityAction.Demand, Role = Security.CarRentalAdminRole)]
        [PrincipalPermission(SecurityAction.Demand, Name = Security.CarRentalUser)]
        public Entities.Car GetCar(int carId)
        {
            return ExecuteFaultHandledOperation(() =>
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
            });

        }

        [PrincipalPermission(SecurityAction.Demand, Role = Security.CarRentalAdminRole)]
        [PrincipalPermission(SecurityAction.Demand, Name = Security.CarRentalUser)]
        public Entities.Car[] GetAllCars()
        {
            return ExecuteFaultHandledOperation(()=>
            {
                ICarRepository carRepository =
                        _DataRepositoryFactory.GetDataRepository<ICarRepository>();

                IRentalRepository rentalRepository =
                    _DataRepositoryFactory.GetDataRepository<IRentalRepository>();

                IEnumerable<Car> cars = carRepository.Get();
                IEnumerable<Rental> rentedCars = rentalRepository.GetCurrentlyRentedCars();

                foreach (Car car in cars)
                {

                    Rental rentedCar = rentedCars.Where(item => item.CarId == car.CarId).FirstOrDefault();
                    car.CurrentlyRented = (rentedCar != null);
                }

                return cars.ToArray();
            });
        }

        [OperationBehavior(TransactionScopeRequired = true)]
        public Car UpdateCar(Car car)
        {
            return ExecuteFaultHandledOperation(() =>
            {
                ICarRepository carRepository = 
                    _DataRepositoryFactory.GetDataRepository<ICarRepository>();

                Car updatedEntity = null;

                if (car.CarId == 0)
                    updatedEntity = carRepository.Add(car);
                else
                    updatedEntity = carRepository.Update(car);

                return updatedEntity;
            });
        }

        [OperationBehavior(TransactionScopeRequired = true)]
        public void DeleteCar(int carId)
        {
            ExecuteFaultHandledOperation(() =>
            {
                ICarRepository carRepository = 
                    _DataRepositoryFactory.GetDataRepository<ICarRepository>();

                carRepository.Remove(carId);
            });
        }

        public Car[] GetAvailableCars(DateTime pickupDate, DateTime returnDate)
        {
            return ExecuteFaultHandledOperation(() =>
            {
                ICarRepository carRepository= 
                    _DataRepositoryFactory.GetDataRepository<ICarRepository>();
                IRentalRepository rentalRepository =
                    _DataRepositoryFactory.GetDataRepository<IRentalRepository>();
                IReservationRepository  reservationRepository =
                    _DataRepositoryFactory.GetDataRepository<IReservationRepository>();

                IEnumerable<Car> allCArs = carRepository.Get();
                IEnumerable<Rental> rentedCars = rentalRepository.GetCurrentlyRentedCars();
                IEnumerable<Reservation> reservedCars = reservationRepository.Get();

                List<Car> availableCars = new List<Car>();

                foreach(Car car in allCArs)
                {   
                    
                }

                return availableCars.ToArray();
            });
        }
    }
}
