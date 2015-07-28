using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Core.Common.Core;
using CarRental.Business.Bootstrapper;

namespace CarRental.Data.Test
{
    [TestClass]
    public class DataLayerTests
    {
        [TestInitialize]    
        public void Initialize()
        {
            ObjectBase.Container = MEFLoader.Init();   
        }
    }

    public class RepositoryTestClass
    {
        public RepositoryTestClass()
        {

        }
    }
}
