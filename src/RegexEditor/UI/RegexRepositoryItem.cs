using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml.Serialization;
using System.Xml;

namespace Microsoft.VisualStudio.RegularExpression.UI
{
    /// <summary>
    /// Item to be saved or loaded from the user's regex repository.
    /// </summary>
    [Serializable]
    public class RegexRepositoryItem : ICloneable
    {
        /// <summary>
        /// Gets or sets the Author
        /// </summary>
        public string Author { get; set; }

        /// <summary>
        /// Gets or sets the category
        /// </summary>
        public string Category { get; set; }

        /// <summary>
        /// Gets or sets the Description
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the Matches
        /// </summary>
        public string Matches { get; set; }

        /// <summary>
        /// Gets or sets the Regex
        /// </summary>
        public string Regex { get; set; }

        /// <summary>
        /// Gets or sets the Title
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Clones an instance of <see cref="RegexRepositoryItem"/>
        /// </summary>
        /// <returns></returns>
        public object Clone()
        {
            RegexRepositoryItem item = new RegexRepositoryItem();
            item.Author = this.Author;
            item.Category = this.Category;
            item.Description = this.Description;
            item.Matches = this.Matches;
            item.Title = this.Title;
            item.Regex = this.Regex;

            return item;
        }
    }
}