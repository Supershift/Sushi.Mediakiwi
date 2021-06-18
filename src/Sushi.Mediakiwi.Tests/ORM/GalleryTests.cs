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
    public class GalleryTests : BaseTest
    {
        #region Test Data
        private string _table = "wim_Galleries";
        private string _key = "Gallery_Key";

        private const string _TestGalleryCompletePathUpdate = "/Test_Gallery_Alteration";
        private Gallery _TestObj = new Gallery()
        {
            BaseGalleryID = 1,
            Created = DateTime.UtcNow,
            IsActive = true,
            IsFolder = true,
            Name = "xUNIT TESTx",
            TypeID = 0,
            CompletePath = "/Test_Gallery"
        };

        private Gallery _TestChildObj = new Gallery()
        {
            BaseGalleryID = 1,
            Created = DateTime.UtcNow,
            IsActive = true,
            IsFolder = true,
            Name = "xUNIT CHILD TESTx",
            TypeID = (short)GalleryType.None,
            CompletePath = "/Test_Gallery/Child"
        };

        private Gallery _TestObjAsync = new Gallery()
        {
            BaseGalleryID = 2,
            Created = DateTime.UtcNow,
            IsActive = true,
            IsFolder = true,
            Name = "xASYNC UNIT TESTx",
            TypeID = (short)GalleryType.None,
            CompletePath = "/Async_Test_Gallery"
        };

        private Gallery _AsyncTestChildObj = new Gallery()
        {
            BaseGalleryID = 2,
            Created = DateTime.UtcNow,
            IsActive = true,
            IsFolder = true,
            Name = "xASYNC UNIT CHILD TESTx",
            TypeID = 0,
            CompletePath = "/Async_Test_Gallery/Child"
        };
        #endregion Test Data

        #region  Create
        public void A_Create_TestObj()
        {
            // insert base
            _TestObj.Insert();
            Assert.IsTrue(_TestObj?.ID > 0);
            if (_TestObj?.ID > 0)
            {
                Trace.WriteLine($"CREATED Gallery: {_TestObj.ID}");
            }
            var newGalleries = Gallery.SearchAll_ByPath(_TestObj.CompletePath);
            Assert.IsFalse(newGalleries == null || newGalleries.Length == 0);
            if (newGalleries?.Length > 0)
                Trace.WriteLine($"CREATED Gallery FOUND: ({newGalleries[0].ID})");
            else
                Trace.WriteLine("CREATED Gallery NOT FOUND...");

            // insert child
            _TestChildObj.ParentID = _TestObj.ID;
            _TestChildObj.Insert();
            Assert.IsTrue(_TestChildObj?.ID > 0);
            if (_TestChildObj?.ID > 0)
            {
                Trace.WriteLine($"CREATED CHILD Gallery: {_TestChildObj.ID}");
            }
            newGalleries = Gallery.SearchAll_ByPath(_TestChildObj.CompletePath);
            Assert.IsFalse(newGalleries == null || newGalleries.Length == 0);
            if (newGalleries?.Length > 0)
                Trace.WriteLine($"CREATED CHILD Gallery FOUND: ({newGalleries[0].ID})");
            else
                Trace.WriteLine("CREATED CHILD Gallery NOT FOUND...");
        }

        public async Task A_Create_TestObjAsync()
        {
            // insert base
            await _TestObjAsync.InsertAsync();
            Assert.IsTrue(_TestObjAsync?.ID > 0);
            if (_TestObjAsync?.ID > 0)
            {
                Trace.WriteLine($"CREATED Gallery: {_TestObjAsync.ID}");
            }
            var newGalleries = Gallery.SearchAll_ByPath(_TestObjAsync.CompletePath);
            Assert.IsFalse(newGalleries == null || newGalleries.Length == 0);
            if (newGalleries?.Length > 0)
                Trace.WriteLine($"CREATED Gallery FOUND: ({newGalleries[0].ID})");
            else
                Trace.WriteLine("CREATED Gallery NOT FOUND...");

            // insert child
            _AsyncTestChildObj.ParentID = _TestObjAsync.ID;
            await _AsyncTestChildObj.InsertAsync();
            Assert.IsTrue(_AsyncTestChildObj?.ID > 0);
            if (_AsyncTestChildObj?.ID > 0)
            {
                Trace.WriteLine($"CREATED CHILD Gallery: {_AsyncTestChildObj.ID}");
            }
            newGalleries = Gallery.SearchAll_ByPath(_AsyncTestChildObj.CompletePath);
            Assert.IsFalse(newGalleries == null || newGalleries.Length == 0);
            if (newGalleries?.Length > 0)
                Trace.WriteLine($"CREATED CHILD Gallery FOUND: ({newGalleries[0].ID})");
            else
                Trace.WriteLine("CREATED CHILD Gallery NOT FOUND...");
        }
        #endregion Create

        #region Select ONE
        [TestMethod]
        public void B_SelectOneByID()
        {
            try
            {
                A_Create_TestObj();

                // Function that we are testing BELOW...
                var gallery = Gallery.SelectOne(_TestObj.ID);

                if (gallery?.ID > 0)
                    Trace.WriteLine($"FOUND Gallery: {gallery.ID}");
                else
                    Assert.Fail("Gallery NOT FOUND...");
            }
            finally
            {
                D_Delete_TestObj();
                F_Reset_AutoIndent();
            }
        }

        [TestMethod]
        public async Task B_SelectOneByIDAsync()
        {
            try
            {
                await A_Create_TestObjAsync();

                // Function that we are testing BELOW...
                var gallery = await Gallery.SelectOneAsync(_TestObjAsync.ID);

                if (gallery?.ID > 0)
                    Trace.WriteLine($"FOUND Gallery: {gallery.ID}");
                else
                    Assert.Fail("Gallery NOT FOUND...");
            }
            finally
            {
                D_Delete_TestObjAsync();
                await F_Reset_AutoIndentAsync();
            }
        }


        [TestMethod]
        public void X_SelectOneByGUID()
        {
            Guid g = new Guid("3752B19A-9051-4C77-906B-30CF9D502660");

            // Function that we are testing BELOW...
            var gallery = Gallery.SelectOne(g);

            if (gallery?.ID > 0)
                Trace.WriteLine($"FOUND Gallery: {gallery.ID}");
            else
                Assert.Fail("Gallery NOT FOUND...");
        }

        [TestMethod]
        public async Task X_SelectOneByGUIDAsync()
        {
            Guid g = new Guid("3752B19A-9051-4C77-906B-30CF9D502660");

            // Function that we are testing BELOW...
            var gallery = await Gallery.SelectOneAsync(g);

            if (gallery?.ID > 0)
                Trace.WriteLine($"FOUND Gallery: {gallery.ID}");
            else
                Assert.Fail("Gallery NOT FOUND...");
        }


        [TestMethod]
        public void B_SelectOneByParentAndName()
        {
            try
            {
                A_Create_TestObj();

                // Function that we are testing BELOW...
                var gallery = Gallery.SelectOne((int)_TestChildObj.ParentID, _TestChildObj.Name);

                if (gallery?.ID > 0)
                    Trace.WriteLine($"FOUND Gallery: {gallery.ID}");
                else
                    Assert.Fail("Gallery NOT FOUND...");
            }
            finally
            {
                D_Delete_TestObj();
                F_Reset_AutoIndent();
            }
        }

        [TestMethod]
        public async Task B_SelectOneByParentAndNameAsync()
        {
            try
            {
                await A_Create_TestObjAsync();

                // Function that we are testing BELOW...
                var gallery = await Gallery.SelectOneAsync((int)_AsyncTestChildObj.ParentID, _AsyncTestChildObj.Name);

                if (gallery?.ID > 0)
                    Trace.WriteLine($"FOUND Gallery: {gallery.ID}");
                else
                    Assert.Fail("Gallery NOT FOUND...");
            }
            finally
            {
                D_Delete_TestObjAsync();
                await F_Reset_AutoIndentAsync();
            }
        }


        [TestMethod]
        public void X_SelectOneRoot()
        {
            // Function that we are testing BELOW...
            var gallery = Gallery.SelectOneRoot();

            if (gallery?.ID > 0)
                Trace.WriteLine($"FOUND Gallery: {gallery.ID}");
            else
                Assert.Fail("Gallery NOT FOUND...");
        }

        [TestMethod]
        public async Task X_SelectOneRootAsync()
        {
            // Function that we are testing BELOW...
            var gallery = await Gallery.SelectOneRootAsync();

            if (gallery?.ID > 0)
                Trace.WriteLine($"FOUND Gallery: {gallery.ID}");
            else
                Assert.Fail("Gallery NOT FOUND...");
        }

        [TestMethod]
        public void X_SelectOneRootByType()
        {
            var type = GalleryType.None;

            // Function that we are testing BELOW...
            var gallery = Gallery.SelectOneRoot(type);

            if (gallery?.ID > 0)
                Trace.WriteLine($"FOUND Gallery: {gallery.ID}");
            else
                Assert.Fail("Gallery NOT FOUND...");
        }

        [TestMethod]
        public async Task X_SelectOneRootByTypeAsync()
        {
            var type = GalleryType.None;

            // Function that we are testing BELOW...
            var gallery = await Gallery.SelectOneRootAsync(type);

            if (gallery?.ID > 0)
                Trace.WriteLine($"FOUND Gallery: {gallery.ID}");
            else
                Assert.Fail("Gallery NOT FOUND...");
        }

        #endregion Select ONE

        #region Select ALL
        [TestMethod]
        public void X_SelectAll()
        {
            // Function that we are testing BELOW...
            var galleries = Gallery.SelectAll();

            if (galleries?.Length > 0)
                Trace.WriteLine($"FOUND Gallery: {galleries.Select(x => x.ID.ToString()).Aggregate((a, b) => a + ", " + b)}");
            else
                Assert.Fail("Gallery NOT FOUND...");
        }

        [TestMethod]
        public async Task X_SelectAllAsync()
        {
            // Function that we are testing BELOW...
            var galleries = await Gallery.SelectAllAsync();

            if (galleries?.Length > 0)
                Trace.WriteLine($"FOUND Gallery: {galleries.Select(x => x.ID.ToString()).Aggregate((a, b) => a + ", " + b)}");
            else
                Assert.Fail("Gallery NOT FOUND...");
        }

        [TestMethod]
        public void B_SelectAllBySearch()
        {
            try
            {
                A_Create_TestObj();

                // Function that we are testing BELOW...
                var galleries = Gallery.SelectAll(_TestObj.Name);

                if (galleries?.Length > 0)
                    Trace.WriteLine($"FOUND Gallery: {galleries.Select(x => x.ID.ToString()).Aggregate((a, b) => a + ", " + b)}");
                else
                    Assert.Fail("Gallery NOT FOUND...");
            }
            finally
            {
                D_Delete_TestObj();
                F_Reset_AutoIndent();
            }
        }

        [TestMethod]
        public async Task B_SelectAllBySearchAsync()
        {
            try
            {
                await A_Create_TestObjAsync();

                // Function that we are testing BELOW...
                var galleries = await Gallery.SelectAllAsync(_TestObjAsync.Name);

                if (galleries?.Length > 0)
                    Trace.WriteLine($"FOUND Gallery: {galleries.Select(x => x.ID.ToString()).Aggregate((a, b) => a + ", " + b)}");
                else
                    Assert.Fail("Gallery NOT FOUND...");
            }
            finally
            {
                D_Delete_TestObjAsync();
                await F_Reset_AutoIndentAsync();
            }
        }

        [TestMethod]
        public void B_SelectAllByType()
        {
            try
            {
                A_Create_TestObj();

                // Function that we are testing BELOW...
                var galleries = Gallery.SelectAll((GalleryType)_TestObj.TypeID, true);

                if (galleries?.Length > 0)
                    Trace.WriteLine($"FOUND Gallery: {galleries.Select(x => x.ID.ToString()).Aggregate((a, b) => a + ", " + b)}");
                else
                    Assert.Fail("Gallery NOT FOUND...");
            }
            finally
            {
                D_Delete_TestObj();
                F_Reset_AutoIndent();
            }
        }

        [TestMethod]
        public async Task B_SelectAllByTypeAsync()
        {
            try
            {
                await A_Create_TestObjAsync();

                // Function that we are testing BELOW...
                var galleries = await Gallery.SelectAllAsync((GalleryType)_TestObjAsync.TypeID, true);

                if (galleries?.Length > 0)
                    Trace.WriteLine($"FOUND Gallery: {galleries.Select(x => x.ID.ToString()).Aggregate((a, b) => a + ", " + b)}");
                else
                    Assert.Fail("Gallery NOT FOUND...");
            }
            finally
            {
                D_Delete_TestObjAsync();
                await F_Reset_AutoIndentAsync();
            }
        }

        [TestMethod]
        public void X_SelectAllAccessibleByUser()
        {
            var user = ApplicationUser.SelectOne("Mark Rienstra");

            // Function that we are testing BELOW...
            var galleries = Gallery.SelectAllAccessible(user);

            if (galleries?.Length > 0)
                Trace.WriteLine($"FOUND Gallery: {galleries.Select(x => x.ID.ToString()).Aggregate((a, b) => a + ", " + b)}");
            else
                Assert.Fail("Gallery NOT FOUND...");
        }

        [TestMethod]
        public async Task X_SelectAllAccessibleByUserAsync()
        {
            var user = ApplicationUser.SelectOne("Mark Rienstra");

            // Function that we are testing BELOW...
            var galleries = await Gallery.SelectAllAccessibleAsync(user);

            if (galleries?.Length > 0)
                Trace.WriteLine($"FOUND Gallery: {galleries.Select(x => x.ID.ToString()).Aggregate((a, b) => a + ", " + b)}");
            else
                Assert.Fail("Gallery NOT FOUND...");
        }

        [TestMethod]
        public void X_SelectAllByBackwardTrail()
        {
            int topId = 2;

            // Function that we are testing BELOW...
            var galleries = Gallery.SelectAllByBackwardTrail(topId);

            if (galleries?.Length > 0)
                Trace.WriteLine($"FOUND Gallery: {galleries.Select(x => x.ID.ToString()).Aggregate((a, b) => a + ", " + b)}");
            else
                Assert.Fail("Gallery NOT FOUND...");
        }

        [TestMethod]
        public async Task X_SelectAllByBackwardTrailAsync()
        {
            int topId = 2;

            // Function that we are testing BELOW...
            var galleries = await Gallery.SelectAllByBackwardTrailAsync(topId);

            if (galleries?.Length > 0)
                Trace.WriteLine($"FOUND Gallery: {galleries.Select(x => x.ID.ToString()).Aggregate((a, b) => a + ", " + b)}");
            else
                Assert.Fail("Gallery NOT FOUND...");
        }

        [TestMethod]
        public void B_SelectAllByBase()
        {
            _TestObj.BaseGalleryID = 669;
            try
            {
                A_Create_TestObj();

                // Function that we are testing BELOW...
                var galleries = Gallery.SelectAllByBase((int)_TestObj.BaseGalleryID);

                if (galleries?.Length > 0)
                    Trace.WriteLine($"FOUND Gallery: {galleries.Select(x => x.ID.ToString()).Aggregate((a, b) => a + ", " + b)}");
                else
                    Assert.Fail("Gallery NOT FOUND...");
            }
            finally
            {
                D_Delete_TestObj();
                F_Reset_AutoIndent();
            }
        }

        [TestMethod]
        public async Task B_SelectAllByBaseAsync()
        {
            _TestObjAsync.BaseGalleryID = 661;
            try
            {
                await A_Create_TestObjAsync();

                // Function that we are testing BELOW...
                var galleries = await Gallery.SelectAllByBaseAsync((int)_TestObjAsync.BaseGalleryID);

                if (galleries?.Length > 0)
                    Trace.WriteLine($"FOUND Gallery: {galleries.Select(x => x.ID.ToString()).Aggregate((a, b) => a + ", " + b)}");
                else
                    Assert.Fail("Gallery NOT FOUND...");
            }
            finally
            {
                D_Delete_TestObjAsync();
                await F_Reset_AutoIndentAsync();
            }
        }


        [TestMethod]
        public void B_SelectAllByParent()
        {
            try
            {
                A_Create_TestObj();

                // Function that we are testing BELOW...
                var galleries = Gallery.SelectAllByParent((int)_TestChildObj.ParentID);

                if (galleries?.Length > 0)
                    Trace.WriteLine($"FOUND Gallery: {galleries.Select(x => x.ID.ToString()).Aggregate((a, b) => a + ", " + b)}");
                else
                    Assert.Fail("Gallery NOT FOUND...");
            }
            finally
            {
                D_Delete_TestObj();
                F_Reset_AutoIndent();
            }
        }

        [TestMethod]
        public async Task B_SelectAllByParentAsync()
        {
            try
            {
                await A_Create_TestObjAsync();

                // Function that we are testing BELOW...
                var galleries = await Gallery.SelectAllByParentAsync((int)_AsyncTestChildObj.ParentID);

                if (galleries?.Length > 0)
                    Trace.WriteLine($"FOUND Gallery: {galleries.Select(x => x.ID.ToString()).Aggregate((a, b) => a + ", " + b)}");
                else
                    Assert.Fail("Gallery NOT FOUND...");
            }
            finally
            {
                D_Delete_TestObjAsync();
                await F_Reset_AutoIndentAsync();
            }
        }

        [TestMethod]
        public void B_SelectAllByParentReturnHidden()
        {
            try
            {
                A_Create_TestObj();

                // Function that we are testing BELOW...
                var galleries = Gallery.SelectAllByParent((int)_TestChildObj.ParentID, true);

                if (galleries?.Length > 0)
                    Trace.WriteLine($"FOUND Gallery: {galleries.Select(x => x.ID.ToString()).Aggregate((a, b) => a + ", " + b)}");
                else
                    Assert.Fail("Gallery NOT FOUND...");
            }
            finally
            {
                D_Delete_TestObj();
                F_Reset_AutoIndent();
            }
        }


        [TestMethod]
        public async Task B_SelectAllByParentReturnHiddenAsync()
        {
            try
            {
                await A_Create_TestObjAsync();

                // Function that we are testing BELOW...
                var galleries = await Gallery.SelectAllByParentAsync((int)_AsyncTestChildObj.ParentID, true);

                if (galleries?.Length > 0)
                    Trace.WriteLine($"FOUND Gallery: {galleries.Select(x => x.ID.ToString()).Aggregate((a, b) => a + ", " + b)}");
                else
                    Assert.Fail("Gallery NOT FOUND...");
            }
            finally
            {
                D_Delete_TestObjAsync();
                await F_Reset_AutoIndentAsync();
            }
        }
        #endregion Select ALL

        #region Search 
        [TestMethod]
        public void B_SearchAll_ByPath()
        {
            try
            {
                A_Create_TestObj();

                // Function that we are testing BELOW...
                var galleries = Gallery.SearchAll_ByPath(_TestObj.CompletePath);

                if (galleries?.Length > 0)
                    Trace.WriteLine($"FOUND Gallery: {galleries.Select(x => x.ID.ToString()).Aggregate((a, b) => a + ", " + b)}");
                else
                    Assert.Fail("Gallery NOT FOUND...");
            }
            finally
            {
                D_Delete_TestObj();
                F_Reset_AutoIndent();
            }
        }

        [TestMethod]
        public async Task B_SearchAll_ByPathAsync()
        {
            try
            {
                await A_Create_TestObjAsync();

                // Function that we are testing BELOW...
                var galleries = await Gallery.SearchAll_ByPathAsync(_TestObjAsync.CompletePath);

                if (galleries?.Length > 0)
                    Trace.WriteLine($"FOUND Gallery: {galleries.Select(x => x.ID.ToString()).Aggregate((a, b) => a + ", " + b)}");
                else
                    Assert.Fail("Gallery NOT FOUND...");
            }
            finally
            {
                D_Delete_TestObjAsync();
                await F_Reset_AutoIndentAsync();
            }
        }
        #endregion Search 

        #region Updates
        [TestMethod]
        public void C_Update()
        {
            try
            {
                A_Create_TestObj();

                var newGalleries = Gallery.SearchAll_ByPath(_TestObj.CompletePath);
                Assert.IsFalse(newGalleries == null || newGalleries.Length == 0);

                if (newGalleries?.Length > 0)
                {
                    var gallery = newGalleries[0];
                    gallery.TypeID = (int)GalleryType.Mixed;
                    // Function that we are testing BELOW...
                    gallery.Update();

                    newGalleries = Gallery.SearchAll_ByPath(_TestObj.CompletePath);

                    Assert.IsFalse(newGalleries == null || newGalleries.Length == 0, "Gallery NOT FOUND...");
                    if (newGalleries?.Length > 0)
                    {
                        Trace.WriteLine($"FOUND Gallery: {newGalleries.Select(x => x.ID.ToString()).Aggregate((a, b) => a + ", " + b)}");
                        Assert.IsFalse(gallery.TypeID != (int)GalleryType.Mixed);
                    }
                }
            }
            finally
            {
                D_Delete_TestObj();
                F_Reset_AutoIndent();
            }
        }

        [TestMethod]
        public async Task C_UpdateAsync()
        {
            try
            {
                await A_Create_TestObjAsync();

                var newGalleries = Gallery.SearchAll_ByPath(_TestObjAsync.CompletePath);
                Assert.IsFalse(newGalleries == null || newGalleries.Length == 0);

                if (newGalleries?.Length > 0)
                {
                    var gallery = newGalleries[0];
                    gallery.TypeID = (int)GalleryType.Mixed;
                    // Function that we are testing BELOW...
                    await gallery.UpdateAsync();

                    newGalleries = Gallery.SearchAll_ByPath(_TestObjAsync.CompletePath);
                    Assert.IsFalse(newGalleries == null || newGalleries.Length == 0, "Gallery NOT FOUND...");

                    if (newGalleries?.Length > 0)
                    {
                        Trace.WriteLine($"FOUND Gallery: {newGalleries.Select(x => x.ID.ToString()).Aggregate((a, b) => a + ", " + b)}");
                        Assert.IsFalse(gallery.TypeID != (int)GalleryType.Mixed);
                    }
                }
            }
            finally
            {
                D_Delete_TestObjAsync();
                await F_Reset_AutoIndentAsync();
            }
        }

        [TestMethod]
        public void C_UpdateCount()
        {

            A_Create_TestObj();

            var newGalleries = Gallery.SearchAll_ByPath(_TestObj.CompletePath);
            Assert.IsFalse(newGalleries == null || newGalleries.Length == 0);
            var gallery = newGalleries[0];
            int currentAssetCount = gallery.AssetCount;

            // Test Asset is needed for this test
            AssetTests assetTests = new AssetTests();
            assetTests.A_Create_TestObj(gallery.ID);

            try
            {
                // Function that we are testing BELOW...
                gallery.UpdateCount();

                var dbGallery = Gallery.SearchAll_ByPath(_TestObj.CompletePath)[0];
                Assert.IsTrue(currentAssetCount < dbGallery.AssetCount);

                if (currentAssetCount < dbGallery.AssetCount)
                    Trace.WriteLine($"UPDATED: (Count {dbGallery.AssetCount})");
                else
                    Trace.WriteLine("NOT UPDATED...");
            }
            finally
            {
                assetTests.D_Delete_TestObj();
                D_Delete_TestObj();

                assetTests.F_Reset_AutoIndent();
                F_Reset_AutoIndent();
            }
        }


        [TestMethod]
        public async Task C_UpdateCountAsync()
        {

            await A_Create_TestObjAsync();

            var newGalleries = Gallery.SearchAll_ByPath(_TestObjAsync.CompletePath);
            Assert.IsFalse(newGalleries == null || newGalleries.Length == 0);
            var gallery = newGalleries[0];
            int currentAssetCount = gallery.AssetCount;

            // Test Asset is needed for this test
            AssetTests assetTests = new AssetTests();
            await assetTests.A_Create_TestObjAsync(gallery.ID);

            try
            {
                // Function that we are testing BELOW...
                await gallery.UpdateCountAsync();

                var dbGallery = Gallery.SearchAll_ByPath(_TestObjAsync.CompletePath)[0];
                Assert.IsTrue(currentAssetCount < dbGallery.AssetCount);

                if (currentAssetCount < dbGallery.AssetCount)
                    Trace.WriteLine($"UPDATED: (Count {dbGallery.AssetCount})");
                else
                    Trace.WriteLine("NOT UPDATED...");
            }
            finally
            {
                await assetTests.D_Delete_TestObjAsync();
                D_Delete_TestObjAsync();

                await assetTests.F_Reset_AutoIndentAsync();
                await F_Reset_AutoIndentAsync();
            }
        }

        [TestMethod]
        public void C_UpdateChildren()
        {
            try
            {
                A_Create_TestObj();

                var newGalleries = Gallery.SearchAll_ByPath(_TestObj.CompletePath);
                Assert.IsFalse(newGalleries == null || newGalleries.Length == 0);
                var gallery = newGalleries[0];

                // Function that we are testing BELOW...
                var result = gallery.UpdateChildren(_TestGalleryCompletePathUpdate);
                Assert.IsTrue(result);
                _TestObj.CompletePath = _TestGalleryCompletePathUpdate;

                newGalleries = Gallery.SearchAll_ByPath(_TestGalleryCompletePathUpdate);
                Assert.IsFalse(newGalleries == null || newGalleries.Length == 0, "Test did not find the mutated Gallery");
            }
            finally
            {
                D_Delete_TestObj();
                F_Reset_AutoIndent();
            }
        }

        [TestMethod]
        public async Task C_UpdateChildrenAsync()
        {
            try
            {
                await A_Create_TestObjAsync();

                var newGalleries = Gallery.SearchAll_ByPath(_TestObjAsync.CompletePath);
                Assert.IsFalse(newGalleries == null || newGalleries.Length == 0);
                var gallery = newGalleries[0];

                // Function that we are testing BELOW...
                var result = gallery.UpdateChildren(_TestGalleryCompletePathUpdate);
                Assert.IsTrue(result);
                _TestObj.CompletePath = _TestGalleryCompletePathUpdate;

                newGalleries = Gallery.SearchAll_ByPath(_TestGalleryCompletePathUpdate);
                Assert.IsFalse(newGalleries == null || newGalleries.Length == 0, "Test did not find the mutated Gallery");
            }
            finally
            {
                D_Delete_TestObjAsync();
                await F_Reset_AutoIndentAsync();
            }
        }

        [TestMethod]
        public void C_Deactivate()
        {
            try
            {
                A_Create_TestObj();

                // Set activity to TRUE
                var newGalleries = Gallery.SearchAll_ByPath(_TestObj.CompletePath);
                Assert.IsFalse(newGalleries == null || newGalleries.Length == 0);
                var gallery = newGalleries[0];
                gallery.IsActive = true;
                gallery.Update();

                // validate if activity is TRUE
                newGalleries = Gallery.SearchAll_ByPath(_TestObj.CompletePath);
                Assert.IsFalse(newGalleries == null || newGalleries.Length == 0);
                gallery = newGalleries[0];
                Assert.IsTrue(gallery.IsActive);

                // Function that we are testing BELOW...
                Gallery.Deactivate(_TestObj.CompletePath);

                // validate if activity is FALSE
                newGalleries = Gallery.SearchAll_ByPath(_TestObj.CompletePath);
                Assert.IsFalse(newGalleries == null || newGalleries.Length == 0);
                gallery = newGalleries[0];
                Assert.IsFalse(gallery.IsActive);
            }
            finally
            {
                D_Delete_TestObj();
                F_Reset_AutoIndent();
            }
        }

        [TestMethod]
        public async Task C_DeactivateAsync()
        {
            try
            {
                await A_Create_TestObjAsync();

                // Set activity to TRUE
                var newGalleries = Gallery.SearchAll_ByPath(_TestObjAsync.CompletePath);
                Assert.IsFalse(newGalleries == null || newGalleries.Length == 0);
                var gallery = newGalleries[0];
                gallery.IsActive = true;
                await gallery.UpdateAsync();

                // validate if activity is TRUE
                newGalleries = await Gallery.SearchAll_ByPathAsync(_TestObjAsync.CompletePath);
                Assert.IsFalse(newGalleries == null || newGalleries.Length == 0);
                gallery = newGalleries[0];
                Assert.IsTrue(gallery.IsActive);

                // Function that we are testing BELOW...
                await Gallery.DeactivateAsync(_TestObjAsync.CompletePath);

                // validate if activity is FALSE
                newGalleries = await Gallery.SearchAll_ByPathAsync(_TestObjAsync.CompletePath);
                Assert.IsFalse(newGalleries == null || newGalleries.Length == 0);
                gallery = newGalleries[0];
                Assert.IsFalse(gallery.IsActive);
            }
            finally
            {
                D_Delete_TestObjAsync();
                await F_Reset_AutoIndentAsync();
            }
        }
        #endregion Updates

        #region Delete
        public void D_Delete_TestObj()
        {
            // delete child
            var newGalleries = Gallery.SearchAll_ByPath(_TestChildObj.CompletePath);
            foreach (var gallery in newGalleries)
            {
                Trace.WriteLine($"DELETE CHILD Gallery: {gallery.ID}");
                gallery._DeleteForTest();
            }

            // check if deleted
            newGalleries = Gallery.SearchAll_ByPath(_TestChildObj.CompletePath);
            Assert.IsTrue(newGalleries == null || newGalleries.Length == 0);

            // delete base
            newGalleries = Gallery.SearchAll_ByPath(_TestObj.CompletePath);
            foreach (var gallery in newGalleries)
            {
                Trace.WriteLine($"DELETE Gallery: {gallery.ID}");
                gallery._DeleteForTest();
            }

            // check if deleted
            newGalleries = Gallery.SearchAll_ByPath(_TestObj.CompletePath);
            Assert.IsTrue(newGalleries == null || newGalleries.Length == 0);
        }

        public void D_Delete_TestObjAsync()
        {
            // delete child
            var newGalleries = Gallery.SearchAll_ByPath(_AsyncTestChildObj.CompletePath);
            foreach (var gallery in newGalleries)
            {
                Trace.WriteLine($"DELETE CHILD Gallery: {gallery.ID}");
                gallery._DeleteForTest();
            }

            // check if deleted
            newGalleries = Gallery.SearchAll_ByPath(_AsyncTestChildObj.CompletePath);
            Assert.IsTrue(newGalleries == null || newGalleries.Length == 0);

            // delete base
            newGalleries = Gallery.SearchAll_ByPath(_TestObjAsync.CompletePath);
            foreach (var gallery in newGalleries)
            {
                Trace.WriteLine($"DELETE Gallery: {gallery.ID}");
                gallery._DeleteForTest();
            }

            // check if deleted
            newGalleries = Gallery.SearchAll_ByPath(_TestObjAsync.CompletePath);
            Assert.IsTrue(newGalleries == null || newGalleries.Length == 0);
        }
        #endregion Delete

        #region ResetAutoIndent
        public void F_Reset_AutoIndent()
        {
            var connector = ConnectorFactory.CreateConnector<Gallery>();
            var filter = connector.CreateDataFilter();

            // Query for resetting the autoincrement in the table
            string sql = $"DECLARE @max int;  SELECT @max = ISNULL(MAX([{_key}]), 1) FROM [dbo].[{_table}];  DBCC CHECKIDENT({_table}, RESEED, @max)";

            try
            {
                connector.ExecuteNonQuery(sql, filter);
            }
            catch (Exception ex)
            {
                Assert.Fail($"Error during resetting of autoincrement: {ex.Message}");
            }
        }

        public async Task F_Reset_AutoIndentAsync()
        {
            var connector = ConnectorFactory.CreateConnector<Gallery>();
            var filter = connector.CreateDataFilter();

            // Query for resetting the autoincrement in the table
            string sql = $"DECLARE @max int;  SELECT @max = ISNULL(MAX([{_key}]), 1) FROM [dbo].[{_table}];  DBCC CHECKIDENT({_table}, RESEED, @max)";

            try
            {
                await connector.ExecuteNonQueryAsync(sql, filter);
            }
            catch (Exception ex)
            {
                Assert.Fail($"Error during resetting of autoincrement: {ex.Message}");
            }
        }
        #endregion ResetAutoIndent
    }
}
