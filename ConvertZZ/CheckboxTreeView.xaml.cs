﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Input;

namespace ConvertZZ
{
    /// <summary>
    /// CheckboxTreeView.xaml 的互動邏輯
    /// </summary>
    public partial class CheckboxTreeView : UserControl, INotifyPropertyChanged
    {
        public CheckboxTreeView()
        {
            this.DataContext = this;
            InitializeComponent();
        }
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string name) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

        public delegate void CheckedChangedEventHandler(CheckBox sender);
        public event CheckedChangedEventHandler CheckedChanged;

        private IList<Node> _ItemSources;
        public IList<Node> ItemSources { get => _ItemSources; set { _ItemSources = value; OnPropertyChanged("ItemSources"); } }
        
        private void CheckBox_Checked(object sender, System.Windows.RoutedEventArgs e)
        {
            CheckedChanged?.Invoke(sender as CheckBox);
        }
    }
    public class Node : INotifyPropertyChanged
    {
        public Node(Node Parent)
        {
            if (Parent == null)
                Generation = 1;
            else
                Generation = Parent.Generation + 1;
            this.Parent = Parent;
        }
        private string _DisplayName;
        public string DisplayName { get => _DisplayName; set { _DisplayName = value; OnPropertyChanged("DisplayName"); } }
        private bool _IsChecked;
        public bool IsChecked { get => _IsChecked; set { _IsChecked = value; OnPropertyChanged("IsChecked"); } }
        public bool IsFile { get; set; }
        private Node _Parent;
        public Node Parent {
            get => _Parent;
            private set {
                if (value == null)
                    Generation = 1;
                else
                {
                    Generation = value.Generation + 1;
                    if (Nodes != null)
                        foreach (Node child in Nodes)
                            child.Parent = this;
                }
                _Parent = value;
                OnPropertyChanged("Parent");
            }
        }
        public int Generation { get; private set; }
        private List<Node> _Nodes=new List<Node>();
        public List<Node> Nodes {
            get => _Nodes;
            set
            {
                _Nodes = value;
                if (value != null)
                    foreach (var child in _Nodes)
                        child.Parent = this;
                OnPropertyChanged("Nodes");
            }
        }
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string name) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
    public class RelayCommand : ICommand
    {
        #region Fields 
        readonly Action<object> _execute;
        readonly Predicate<object> _canExecute;
        #endregion // Fields 
        #region Constructors 
        public RelayCommand(Action<object> execute) : this(execute, null) { }
        public RelayCommand(Action<object> execute, Predicate<object> canExecute)
        {
            _execute = execute ?? throw new ArgumentNullException("execute"); _canExecute = canExecute;
        }
        #endregion // Constructors 
        #region ICommand Members 
        [System.Diagnostics.DebuggerStepThrough]
        public bool CanExecute(object parameter)
        {
            return _canExecute == null ? true : _canExecute(parameter);
        }
        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }
        public void Execute(object parameter) { _execute(parameter); }
        #endregion // ICommand Members 
    }
}
