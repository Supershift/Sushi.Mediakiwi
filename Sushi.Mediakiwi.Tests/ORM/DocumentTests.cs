using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Sushi.Mediakiwi.Data;
using Sushi.Mediakiwi.Data.MicroORM;

namespace Sushi.Mediakiwi.Tests.ORM
{
    [TestClass]
    public class DocumentTests : BaseTest
    {

        #region Select ONE

        [TestMethod]
        public void X_SelectOneByID()
        {
            int Id = 10;

            // Function that we are testing BELOW...
            var document = Document.SelectOne(Id);

            if (document?.ID > 0)
                Trace.WriteLine($"FOUND Document: {document.ID}");
            else
                Assert.Fail("Document NOT FOUND...");
        }

        [TestMethod]
        public async Task X_SelectOneByIDAsync()
        {
            int Id = 10;

            // Function that we are testing BELOW...
            var document = await Document.SelectOneAsync(Id);

            if (document?.ID > 0)
                Trace.WriteLine($"FOUND Document: {document.ID}");
            else
                Assert.Fail("Document NOT FOUND...");
        }

        [TestMethod]
        public void X_SelectOneByGUID()
        {
            Guid g = new Guid("CED27A87-1909-4E94-813E-46FC8E7C15DA");

            // Function that we are testing BELOW...
            var document = Document.SelectOne(g);

            if (document?.ID > 0)
                Trace.WriteLine($"FOUND Document: {document.ID}");
            else
                Assert.Fail("Document NOT FOUND...");
        }

        [TestMethod]
        public async Task X_SelectOneByGUIDAsync()
        {
            Guid g = new Guid("CED27A87-1909-4E94-813E-46FC8E7C15DA");

            // Function that we are testing BELOW...
            var document = await Document.SelectOneAsync(g);

            if (document?.ID > 0)
                Trace.WriteLine($"FOUND Document: {document.ID}");
            else
                Assert.Fail("Document NOT FOUND...");
        }

        #endregion Select ONE

        #region Select ALL

        [TestMethod]
        public void X_SelectAllForGallery()
        {
            int galleryId = 1;

            // Function that we are testing BELOW...
            var documents = Document.SelectAll(galleryId);

            if (documents?.Length > 0)
                Trace.WriteLine($"FOUND Document: {documents.Select(x => x.ID.ToString()).Aggregate((a, b) => a + ", " + b)}");
            else
                Assert.Fail("Document NOT FOUND...");
        }

        [TestMethod]
        public async Task X_SelectAllForGalleryAsync()
        {
            int galleryId = 1;

            // Function that we are testing BELOW...
            var documents = await Document.SelectAllAsync(galleryId);

            if (documents?.Length > 0)
                Trace.WriteLine($"FOUND Document: {documents.Select(x => x.ID.ToString()).Aggregate((a, b) => a + ", " + b)}");
            else
                Assert.Fail("Document NOT FOUND...");
        }

        #endregion Select ALL
    }
}
