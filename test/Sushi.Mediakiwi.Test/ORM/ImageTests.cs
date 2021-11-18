using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sushi.Mediakiwi.Data;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace Sushi.Mediakiwi.Test.ORM
{
    [TestClass]
    public class ImageTests : BaseTest
    {

        #region Select ONE

        [TestMethod]
        public void X_SelectOneByID()
        {
            int Id = 15;

            // Function that we are testing BELOW...
            var image = Image.SelectOne(Id);

            if (image?.ID > 0)
                Trace.WriteLine($"FOUND Image: {image.ID}");
            else
                Assert.Fail("Image NOT FOUND...");
        }

        [TestMethod]
        public async Task X_SelectOneByIDAsync()
        {
            int Id = 15;

            // Function that we are testing BELOW...
            var image = await Image.SelectOneAsync(Id);

            if (image?.ID > 0)
                Trace.WriteLine($"FOUND Image: {image.ID}");
            else
                Assert.Fail("Image NOT FOUND...");
        }

        [TestMethod]
        public void X_SelectOneByGUID()
        {
            Guid g = new Guid("F7B4BCB3-BFE8-42F1-BCBD-69DF05539319");

            // Function that we are testing BELOW...
            var image = Image.SelectOne(g);

            if (image?.ID > 0)
                Trace.WriteLine($"FOUND Image: {image.ID}");
            else
                Assert.Fail("Image NOT FOUND...");
        }

        [TestMethod]
        public async Task X_SelectOneByGUIDAsync()
        {
            Guid g = new Guid("F7B4BCB3-BFE8-42F1-BCBD-69DF05539319");

            // Function that we are testing BELOW...
            var image = await Image.SelectOneAsync(g);

            if (image?.ID > 0)
                Trace.WriteLine($"FOUND Image: {image.ID}");
            else
                Assert.Fail("Image NOT FOUND...");
        }

        #endregion Select ONE

        #region Select ALL

        [TestMethod]
        public void X_SelectAllForGallery()
        {
            int galleryId = 1;

            // Function that we are testing BELOW...
            var images = Image.SelectAll(galleryId);

            if (images?.Length > 0)
                Trace.WriteLine($"Image FOUND: {images.Select(x => x.ID.ToString()).Aggregate((a, b) => a + ", " + b)}");
            else
                Assert.Fail("Image NOT FOUND...");
        }

        [TestMethod]
        public async Task X_SelectAllForGalleryAsync()
        {
            int galleryId = 1;

            // Function that we are testing BELOW...
            var images = await Image.SelectAllAsync(galleryId);

            if (images?.Length > 0)
                Trace.WriteLine($"Image FOUND: {images.Select(x => x.ID.ToString()).Aggregate((a, b) => a + ", " + b)}");
            else
                Assert.Fail("Image NOT FOUND...");
        }

        [TestMethod]
        public void B_SelectAllForGalleryAndAssetType()
        {
            int galleryID = 66677;
            int assetTypeID = 1;
            bool isImage = true;
            bool isActive = true;
            bool isArchived = false;

            // Asset is needed for this test
            AssetTests assetTest = new AssetTests();
            assetTest.A_Create_TestObj(galleryID, assetTypeID, isImage, isActive, isArchived);

            try
            {
                // Function that we are testing BELOW...
                var images = Image.SelectAll(galleryID, assetTypeID);

                if (images?.Length > 0)
                    Trace.WriteLine($"Image FOUND: {images.Select(x => x.ID.ToString()).Aggregate((a, b) => a + ", " + b)}");
                else
                    Assert.Fail("Image NOT FOUND...");
            }
            finally
            {
                assetTest.D_Delete_TestObj();
                assetTest.F_Reset_AutoIndent();
            }
        }

        [TestMethod]
        public async Task B_SelectAllForGalleryAndAssetTypeAsync()
        {
            int galleryID = 66678;
            int assetTypeID = 1;
            bool isImage = true;
            bool isActive = true;
            bool isArchived = false;

            // Asset is needed for this test
            AssetTests assetTest = new AssetTests();
            await assetTest.A_Create_TestObjAsync(galleryID, assetTypeID, isImage, isActive, isArchived);

            try
            {
                // Function that we are testing BELOW...
                var images = await Image.SelectAllAsync(galleryID, assetTypeID);

                if (images?.Length > 0)
                    Trace.WriteLine($"Image FOUND: {images.Select(x => x.ID.ToString()).Aggregate((a, b) => a + ", " + b)}");
                else
                    Assert.Fail("Image NOT FOUND...");
            }
            finally
            {
                await assetTest.D_Delete_TestObjAsync();
                await assetTest.F_Reset_AutoIndentAsync();
            }
        }


        [TestMethod]
        public void X_SelectAllByRange()
        {
            List<int> IDs = new List<int>() { 15, 16, 17, 18, 19, 20 };

            // Function that we are testing BELOW...
            var images = Image.SelectRange(IDs);

            if (images?.Count > 0)
                Trace.WriteLine($"Image FOUND: {images.Select(x => x.ID.ToString()).Aggregate((a, b) => a + ", " + b)}");
            else
                Assert.Fail("Image NOT FOUND...");
        }

        [TestMethod]
        public async Task X_SelectAllByRangeAsync()
        {
            List<int> IDs = new List<int>() { 15, 16, 17, 18, 19, 20 };

            // Function that we are testing BELOW...
            var images = await Image.SelectRangeAsync(IDs);

            if (images?.Count > 0)
                Trace.WriteLine($"Image FOUND: {images.Select(x => x.ID.ToString()).Aggregate((a, b) => a + ", " + b)}");
            else
                Assert.Fail("Image NOT FOUND...");
        }

        #endregion Select ALL
    }
}
