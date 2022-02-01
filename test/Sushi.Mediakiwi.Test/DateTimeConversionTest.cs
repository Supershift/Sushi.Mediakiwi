using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace Sushi.Mediakiwi.Test
{
    [TestClass]
    public class DateTimeConversionTest
    {
        private const string nlDateFormat = "dd-MM-yyyy HH:mm";

        public class TestDateTimeObjectNL 
        {
            public string Created { get; set; } 
        }

        public class TestDateTimeObjectUS
        { 
            public DateTime Created { get; set; }
        }

        [TestMethod]
        public void ConvertFromNLtoUS()
        {
            // Create a source object with NL culture.
            var objectFrom = new TestDateTimeObjectNL()
            {
                Created = DateTime.Now.ToString(nlDateFormat, new System.Globalization.CultureInfo("nl-NL"))
            };

            // Create a target object
            var objectTo = new TestDateTimeObjectUS();

            // Reflect properties
            Utils.ReflectProperty(objectFrom, objectTo, false, "nl-NL", "en-US");

            Assert.IsTrue(objectFrom.Created.Equals(objectTo.Created.ToString(nlDateFormat)), $"Conversion failed, outcome: {objectTo.Created:nlDateFormat}");

        }
    }
}
