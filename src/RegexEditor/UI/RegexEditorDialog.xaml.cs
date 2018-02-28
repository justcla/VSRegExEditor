using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Text.RegularExpressions;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Text;

namespace Microsoft.VisualStudio.RegularExpression.UI
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class RegexEditorDialog : Window
    {
        RegexDataContext context;
        IWpfTextViewHost regexEditor;

        /// <summary>
        /// 
        /// </summary>
        public RegexEditorDialog(IWpfTextViewHost regexEditor)
        {
            InitializeComponent();

            this.regexEditor = regexEditor;
            TextViewEventManager textViewEventManager = this.regexEditor.TextView.Properties.GetProperty<TextViewEventManager>(TextViewEventManager.Key);
            textViewEventManager.IntellisenseSessionStart +=new EventHandler(OnIntellisenseSessionStart);
            textViewEventManager.IntellisenseSessionEnd +=new EventHandler(OnIntellisenseSessionEnd);

            context = new RegexDataContext();
            context.RegexRepositoryService = new RegexRepositoryService();
            context.SelectedItem = context.RegexRepositoryService.CreateItem("[new]", string.Empty);

            this.expressionContainer.Children.Add(regexEditor.HostControl);

            this.DataContext = context;

            this.regexEditor.TextView.Caret.MoveTo(new SnapshotPoint(this.regexEditor.TextView.TextBuffer.CurrentSnapshot, this.regexEditor.TextView.TextBuffer.CurrentSnapshot.Length));
            this.regexEditor.HostControl.Focus();
        }

        private void OnIntellisenseSessionStart(object sender, EventArgs e)
        {
            DisableCancelAndDefaultButtons();
        }

        private void OnIntellisenseSessionEnd(object sender, EventArgs e)
        {
            EnableCancelAndDefaultButtons();
        }

        internal static bool? ShowDialog(string regex, IWpfTextViewHost regexEditor, out string result)
        {
            RegexEditorDialog editorDialog = new RegexEditorDialog(regexEditor);
            RegexRepositoryItem selectedRepositoryItem = editorDialog.context.RegexRepositoryService.Items.FirstOrDefault(item => string.Compare(item.Regex, regex, true) == 0);

            if (selectedRepositoryItem != null)
            {
                editorDialog.context.SelectedItem = selectedRepositoryItem;
                editorDialog.regexRepositoryItemsDataGrid.SelectedItem = selectedRepositoryItem;
            }
            else
            {
                editorDialog.context.SelectedItem.Regex = regex;
            }

            bool? dialogResult = editorDialog.ShowDialog();
            result = editorDialog.GetExpressionText();

            return dialogResult;
        }

        internal void EnableCancelAndDefaultButtons()
        {
            this.buttonAccept.IsDefault = this.buttonCancel.IsCancel = true;
        }

        internal void DisableCancelAndDefaultButtons()
        {
            this.buttonAccept.IsDefault = this.buttonCancel.IsCancel = false;
        }

        private void Expander_Expanded(object sender, RoutedEventArgs e)
        {
            this.Height = this.MinHeight = this.MaxHeight = 580;
        }

        private void Expander_Collapsed(object sender, RoutedEventArgs e)
        {
            this.Height = this.MinHeight = this.MaxHeight = 230;
        }

        private string GetExpressionText()
        {
            return this.regexEditor.TextView.TextBuffer.CurrentSnapshot.GetText();
        }

        [SuppressMessage("Microsoft.Design", "CA1031")]
        private void ButtonMatch_Click(object sender, RoutedEventArgs e)
        {
            this.treeViewResult.Items.Clear();

            Regex regex = null;
            try
            {
                regex = new Regex(GetExpressionText(), RegexOptions.Multiline);
            }
            catch (Exception ex)
            {
                this.treeViewResult.Items.Add(new TreeViewItem { Header = ex.Message, IsExpanded = true });
                return;
            }


            MatchCollection matches = regex.Matches(this.textBoxMatches.Text);

            if (matches.Count > 0)
            {
                foreach (Match match in matches)
                {
                    if (match.Success)
                    {
                        TreeViewItem matchTreeItem = new TreeViewItem 
                        {
                            Header = NormalizeMatchValue(match.Value), 
                            IsExpanded = true ,
                            Tag = match
                        };
                        this.treeViewResult.Items.Add(matchTreeItem);

                        for (int i = 1; i < match.Groups.Count; i++)
                        {
                            string groupName = regex.GroupNameFromNumber(i);

                            matchTreeItem.Items.Add(new TreeViewItem 
                            { 
                                Header = string.Format("{0}: '{1}'", groupName, NormalizeMatchValue(match.Groups[i].Value)),
                                Tag = match.Groups[i]
                            });
                        }
                    }
                }
            }
            else
            {
                this.treeViewResult.Items.Add(new TreeViewItem { Header = "No results" });
            }
        }

        private string NormalizeMatchValue(string value)
        {
            return value.Replace(Environment.NewLine, string.Empty);
        }

        [SuppressMessage("Microsoft.Design", "CA1031")]
        private void ButtonAccept_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (this.context.SelectedItem != null)
                {
                    this.context.SelectedItem.Regex = GetExpressionText();
                    this.context.RegexRepositoryService.Save(this.context.SelectedItem);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(string.Format("The repository could not be saved. Error: '{0}'", ex.Message));
            }

            DialogResult = true;
            this.Close();
        }

        private void RegexRepositoryItemsDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            this.context.SelectedItem = this.regexRepositoryItemsDataGrid.SelectedItem as RegexRepositoryItem;

            if (this.context.SelectedItem != null)
            {
                this.regexEditor.TextView.TextBuffer.Replace(new Microsoft.VisualStudio.Text.Span(0, this.regexEditor.TextView.TextBuffer.CurrentSnapshot.Length), this.context.SelectedItem.Regex);
            }

            this.treeViewResult.Items.Clear();

            this.menuItemDeleteRepositoryItem.IsEnabled = this.menuItemDuplicateRepositoryItem.IsEnabled = this.context.SelectedItem != null;
        }

        private void MenuItemDeleteRepositoryItem_Click(object sender, RoutedEventArgs e)
        {
            if (this.regexRepositoryItemsDataGrid.SelectedItems != null)
            {
                foreach (RegexRepositoryItem selectedItem in this.regexRepositoryItemsDataGrid.SelectedItems.Cast<RegexRepositoryItem>().ToList())
                {
                    this.context.RegexRepositoryService.Delete(selectedItem);
                }
            }
        }

        class RegexDataContext : INotifyPropertyChanged
        {
            public RegexRepositoryService RegexRepositoryService { get; set; }

            private RegexRepositoryItem selectedItem;
            public RegexRepositoryItem SelectedItem
            {
                get { return this.selectedItem; }
                set
                {
                    this.selectedItem = value;
                    OnPropertyChanged("SelectedItem");
                }
            }

            public event PropertyChangedEventHandler PropertyChanged;

            protected virtual void OnPropertyChanged(string propertyName)
            {
                if (this.PropertyChanged != null)
                {
                    this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
                }
            }
        }

        private void MenuItemDuplicateRepositoryItem_Click(object sender, RoutedEventArgs e)
        {
            if (this.regexRepositoryItemsDataGrid.SelectedItems != null)
            {
                foreach (RegexRepositoryItem selectedItem in this.regexRepositoryItemsDataGrid.SelectedItems.Cast<RegexRepositoryItem>().ToList())
                {
                    RegexRepositoryItem item = selectedItem.Clone() as RegexRepositoryItem;
                    this.context.RegexRepositoryService.GenerateUniqueTitle(item);

                    this.context.RegexRepositoryService.Save(item);
                    this.regexRepositoryItemsDataGrid.SelectedItem = item;
                }
            }
        }

        private void ButtonCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void TreeViewResult_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            TreeViewItem selectedItem = e.NewValue as TreeViewItem;
            if(selectedItem != null)
            {
                Capture capture = selectedItem.Tag as Capture;
                if (capture != null)
                {
                    this.textBoxMatches.Focus();
                    this.textBoxMatches.Select(capture.Index, capture.Length);
                    this.textBoxMatchesShouldKeepFocus = true;
                    selectedItem.Focus();
                }
            }
        }

        private bool textBoxMatchesShouldKeepFocus;

        private void TextBoxMatches_LostFocus(object sender, RoutedEventArgs e)
        {
            if(textBoxMatchesShouldKeepFocus)
            {
                textBoxMatchesShouldKeepFocus = false;
                e.Handled = true;
            }
        }

        private void textBoxFilter_TextChanged(object sender, TextChangedEventArgs e)
        {
            context.RegexRepositoryService.Filter = textBoxFilter.Text;
            e.Handled = true;
        }
    }
}