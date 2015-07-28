using System;
using System.Collections.Generic;
using System.ComponentModel;
using Core.Common.Contract;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Core.Common.Tests
{
    [TestClass]
    public class ObjectBaseTest
    {
        [TestMethod]
        public void test_clean_property_change()
        {
            TestClass objTest = new TestClass();

            bool propertyChange = false;

            objTest.PropertyChanged += (s, e) =>
                {
                    if (e.PropertyName == "CleanProp")
                        propertyChange = true;
                };

            objTest.CleanProp = "test value";

            Assert.IsTrue(propertyChange, "The property should have triggered change notification."); 
        }

        [TestMethod]
        public void test_dirty_set()
        {
            TestClass objTest = new TestClass();

            Assert.IsFalse(objTest.IsDirty, "Object should be clean.");

            objTest.DirtyProp = "test value";

            Assert.IsTrue(objTest.IsDirty, "object should be dirty.");
        }

        [TestMethod]
        public void test_property_change_single_subscription()
        {
            TestClass objTest = new TestClass();
            int changeCounter = 0;
            PropertyChangedEventHandler handler1 
                = new PropertyChangedEventHandler((s, e) => { changeCounter++; });
            PropertyChangedEventHandler handler2 
                = new PropertyChangedEventHandler((s, e) => { changeCounter++; });

            objTest.PropertyChanged += handler1;
            objTest.PropertyChanged += handler1;//should not be duplicate
            objTest.PropertyChanged += handler1;//should not be duplicate
            objTest.PropertyChanged += handler2;
            objTest.PropertyChanged += handler2;//should not be duplicate

            objTest.CleanProp = "test value";

            Assert.IsTrue(changeCounter == 2, "Property change notification should only have been called once.");
        }

        [TestMethod]
        public void test_object_validation()
        {
            TestClass objTest = new TestClass();

            Assert.IsFalse(objTest.IsValid, "Object should not be valid as one of its rules should be broken.");

            objTest.StringProp = "some values";

            Assert.IsTrue(objTest.IsValid, "Object should be valid as its property has been fixed.");
        }
    }
}
