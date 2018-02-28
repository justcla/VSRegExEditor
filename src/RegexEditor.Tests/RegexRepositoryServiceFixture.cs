using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.VisualStudio.RegularExpression.UI;
using System.IO;

namespace Microsoft.VisualStudio.RegularExpression.Test
{
    /// <summary>
    /// Summary description for UnitTest1
    /// </summary>
    [TestClass]
    public class RegexRepositoryServiceFixture
    {
        [TestMethod]
        public void ShouldAnEmptyItemsList()
        {
            string repositoryFile = Path.GetTempFileName();
            File.Delete(repositoryFile);
            RegexRepositoryService repository = new RegexRepositoryService(repositoryFile, true);

            Assert.IsNotNull(repository.Items);
            Assert.AreEqual(0, repository.Items.Count());
            Assert.IsFalse(File.Exists(repositoryFile));
        }

        [TestMethod]
        public void ShouldSaveNewRepositoryItem()
        {
            string repositoryFile = Path.GetTempFileName();
            RegexRepositoryService repository = new RegexRepositoryService(repositoryFile, false);

            RegexRepositoryItem item = repository.CreateItem("foo", "bar");
            repository.Save(item);

            // Create the repository again to check if it loads the saved item
            repository = new RegexRepositoryService(repositoryFile, true);

            Assert.IsNotNull(repository.Items);
            Assert.AreEqual(1, repository.Items.Count());
            Assert.AreEqual("foo", repository.Items.First().Title);
            Assert.AreEqual("bar", repository.Items.First().Regex);
        }

        [TestMethod]
        public void ShouldSaveReadOnlyRepositoryItem()
        {
            string repositoryFile = Path.GetTempFileName();
            File.SetAttributes(repositoryFile, FileAttributes.ReadOnly);
            RegexRepositoryService repository = new RegexRepositoryService(repositoryFile, false);

            RegexRepositoryItem item = repository.CreateItem("foo", "bar");
            repository.Save(item);

            // Create the repository again to check if it loads the saved item
            repository = new RegexRepositoryService(repositoryFile, true);

            Assert.IsNotNull(repository.Items);
            Assert.AreEqual(1, repository.Items.Count());
            Assert.AreEqual("foo", repository.Items.First().Title);
            Assert.AreEqual("bar", repository.Items.First().Regex);
        }

        [TestMethod]
        public void ShouldCreateRepositoryItemWithDefaultValues()
        {
            string repositoryFile = Path.GetTempFileName();
            RegexRepositoryService repository = new RegexRepositoryService(repositoryFile);
            RegexRepositoryItem item = repository.CreateItem("foo", "bar");

            Assert.IsNotNull(item);

            Assert.AreEqual("foo", item.Title);
            Assert.AreEqual(Environment.UserName, item.Author);
            Assert.AreEqual("default", item.Category);
            Assert.AreEqual("foo", item.Description);
            Assert.AreEqual("", item.Matches);
            Assert.AreEqual("bar", item.Regex); 
        }
    }
}
