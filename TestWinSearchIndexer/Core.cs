using Microsoft.VisualStudio.TestTools.UnitTesting;
using WinSearchIndexer;

namespace TestWinSearchIndexer
{
    [TestClass]
    public class Core
    {
        const string NonExcludedPath = "C:\\Test";
        const string ExcludedPath = "C:\\Test\\DontPickMe";

        [TestMethod]
        public void SearchIndexingIsAvailable()
        {
            // Ensures the service is running, and (if applicable) the server feature is available
            Assert.IsTrue(Indexer.FindSearchIndexStatus());
        }

        [TestMethod]
        public void TestDirectoriesExist()
        {
            // Use a test directory on the root drive so not to interfere with any real settings
            // C:\Users is usually indexed, so C:\Users\YOURUSERNAME and C:\Users\Public is a no-go
            // A test directory is the best way to start fresh
            Assert.IsTrue(System.IO.Directory.Exists(NonExcludedPath) &&
                System.IO.Directory.Exists(ExcludedPath));
        }

        [TestMethod]
        public void GetSearchIndexIncludedLocations()
        {
            // There's usually always something indexed by the indexer
            // Otherwise, an exception will get thrown (unsure what those could be)
            Assert.IsTrue(Indexer.GetSearchIndexIncludedLocations().Count > 0);
        }

        [TestMethod]
        public void AddSearchIndexIncludedLocation()
        {
            // Index our entire test folder and all children
            // OverrideChildRules is true to start fresh
            Indexer.AddSearchIndexIncludedLocation(NonExcludedPath, true, true, true);
            Assert.IsTrue(Indexer.FindSearchIndexLocationIncluded(NonExcludedPath));
        }

        [TestMethod]
        public void ExcludeSearchIndexIncludedLocation()
        {
            // Now exclude a sub-folder
            Indexer.RemoveSearchIndexIncludedLocation(ExcludedPath);
            Assert.IsFalse(Indexer.FindSearchIndexLocationIncluded(ExcludedPath));
        }

        [TestMethod]
        public void OverrideSearchIndexIncludedLocation()
        {
            // Re-apply indexing to the parent, so this should make all child folders included again
            Indexer.AddSearchIndexIncludedLocation(ExcludedPath, true, true, true);
            Assert.IsTrue(Indexer.FindSearchIndexLocationIncluded(ExcludedPath));
        }
    }
}
