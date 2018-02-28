using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml.Serialization;
using System.Xml;
using System.Collections.ObjectModel;

namespace Microsoft.VisualStudio.RegularExpression.UI
{
    /// <summary>
    /// 
    /// </summary>
    internal class RegexRepositoryService
    {
        private const string RepositoryPath = @"expressions.xml";
        private string repositoryFile;
        private ObservableCollection<RegexRepositoryItem> items;
        private ObservableCollection<RegexRepositoryItem> filteredItems;

        internal RegexRepositoryService()
            : this(Path.Combine(Path.GetDirectoryName(typeof(RegexRepositoryService).Assembly.Location), RepositoryPath))
        { }

        internal RegexRepositoryService(string repositoryFile)
            : this(repositoryFile, false)
        { }

        internal RegexRepositoryService(string repositoryFile, bool throwOnFailure)
        {
            this.repositoryFile = repositoryFile;

            try
            {
                this.LoadItems();
                this.filteredItems = new ObservableCollection<RegexRepositoryItem>();
                foreach (RegexRepositoryItem item in items)
                {
                    filteredItems.Add(item);
                }
            }
            catch
            {
                this.items = new ObservableCollection<RegexRepositoryItem>();

                if (throwOnFailure)
                    throw;
            }
        }

        private void EnsureRepositoryFile()
        {
            if (File.Exists(this.repositoryFile))
            {
                if ((File.GetAttributes(this.repositoryFile) & FileAttributes.ReadOnly) != 0)
                {
                    File.SetAttributes(this.repositoryFile, FileAttributes.Normal);
                }
            }
            else
            {
                string path = Path.GetDirectoryName(this.repositoryFile);

                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
            }
        }

        private void SaveItems()
        {
            EnsureRepositoryFile();

            XmlSerializer serializer = new XmlSerializer(typeof(List<RegexRepositoryItem>));
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Indent = true;

            using (FileStream stream = new FileStream(this.repositoryFile, FileMode.Create, FileAccess.Write))
            {
                using (XmlWriter writer = XmlWriter.Create(stream, settings))
                {
                    serializer.Serialize(writer, this.items.ToList());
                }
            }
        }

        private void LoadItems()
        {
            if (File.Exists(this.repositoryFile))
            {
                XmlSerializer serializer = new XmlSerializer(typeof(List<RegexRepositoryItem>));

                using (FileStream stream = new FileStream(this.repositoryFile, FileMode.Open, FileAccess.Read))
                {
                    using (XmlReader reader = XmlReader.Create(stream))
                    {
                        this.items = new ObservableCollection<RegexRepositoryItem>(serializer.Deserialize(reader) as List<RegexRepositoryItem>);                        
                    }
                }
            }
            else
            {
                this.items = new ObservableCollection<RegexRepositoryItem>();
            }
        }


        private string filter;

        public string Filter
        {
            get
            {
                return filter;
            }
            set
            {
                filter = value;
                UpdateFilteredItems();
            }
        }

        private void UpdateFilteredItems()
        {
            if (this.filteredItems != null)
            {
                this.filteredItems.Clear();

                foreach (RegexRepositoryItem item in items)
                {
                    if (filter == string.Empty || (item.Title.ToUpper().Contains(Filter.ToUpper()) || item.Description.ToUpper().Contains(Filter.ToUpper())))
                    {
                        filteredItems.Add(item);
                    }
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public IEnumerable<RegexRepositoryItem> Items
        {
            get { return this.items; }
        }

        public IEnumerable<RegexRepositoryItem> FilteredItems
        {
            get 
            {
                return this.filteredItems;
            }
        }

        internal RegexRepositoryItem CreateItem(string name, string regex)
        {
            return new RegexRepositoryItem()
            {
                Author = Environment.UserName,
                Category = "default",
                Title = name,
                Description = name,
                Matches = string.Empty,
                Regex = regex
            };
        }

        internal void Save(RegexRepositoryItem item)
        {
            if (!this.items.Contains(item))
            {
                this.items.Add(item);
            }

            SaveItems();
        }

        internal void Delete(RegexRepositoryItem item)
        {
            if (this.items.Contains(item))
            {
                this.items.Remove(item);
            }

            SaveItems();
        }

        internal void GenerateUniqueTitle(RegexRepositoryItem item)
        {
            item.Title = GenerateUniqueName(title => this.items.Where(i => string.Compare(i.Title, title, true) == 0).Count() == 0, item.Title, 1);
        }

        internal string GenerateUniqueName(Predicate<string> uniqueNamePredicate, string name, int index)
        {
            string uniqueName = name + index.ToString();
            if (uniqueNamePredicate(uniqueName))
            {
                return uniqueName;
            }
            else
            {
                return GenerateUniqueName(uniqueNamePredicate, name, ++index);
            }
        }
    }
}