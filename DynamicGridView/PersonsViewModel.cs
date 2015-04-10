using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using MVVM;

namespace DynamicGridView
{
    //http://stackoverflow.com/questions/2643545/wpf-mvvm-how-to-bind-gridviewcolumn-to-viewmodel-collection
    public class Person
    {
        public string Name { get; set; }
        public string FirstName { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string Locality { get; set; }
    }

    public class ColumnDescriptor
    {
        public string HeaderText { get; set; }
        public string DisplayMember { get; set; }
    }

    public class PersonsViewModel
    {
        public PersonsViewModel()
        {
            Persons = new ObservableCollection<Person>
                {
                    new Person
                        {
                            Name = "Doe",
                            FirstName = "John",
                            DateOfBirth = new DateTime(1981, 9, 12),
                            Locality = "Bruxelles"
                        },
                    new Person
                        {
                            Name = "Black",
                            FirstName = "Jack",
                            DateOfBirth = new DateTime(1950, 1, 15),
                            Locality = "London"
                        },
                    new Person
                        {
                            Name = "Smith",
                            FirstName = "Jane",
                            DateOfBirth = new DateTime(1987, 7, 23),
                            Locality = "Paris"
                        }
                };

            AvailableColumns = new ObservableCollection<ColumnDescriptor>
                {
                    new ColumnDescriptor
                        {
                            HeaderText = "Last name",
                            DisplayMember = "Name"
                        },
                    new ColumnDescriptor
                        {
                            HeaderText = "First name",
                            DisplayMember = "FirstName"
                        },
                    new ColumnDescriptor
                        {
                            HeaderText = "Date of birth",
                            DisplayMember = "DateOfBirth"
                        },
                    new ColumnDescriptor
                        {
                            HeaderText = "Locality",
                            DisplayMember = "Locality"
                        }
                };

            CurrentColumns = new ObservableCollection<ColumnDescriptor>
                {
                    AvailableColumns.FirstOrDefault(x => x.DisplayMember == "Name"),
                    AvailableColumns.FirstOrDefault(x => x.DisplayMember == "FirstName"),
                    AvailableColumns.FirstOrDefault(x => x.DisplayMember == "DateOfBirth"),
                };
        }

        public ObservableCollection<Person> Persons { get; private set; }

        public ObservableCollection<ColumnDescriptor> CurrentColumns { get; private set; }
        public ObservableCollection<ColumnDescriptor> AvailableColumns { get; private set; }

        public ColumnDescriptor SelectedColumnDescriptor { get; set; }

        private ICommand _addColumnCommand;
        public ICommand AddColumnCommand
        {
            get
            {
                _addColumnCommand = _addColumnCommand ?? new RelayCommand<string>(AddColumn);
                return _addColumnCommand;
            }
        }

        private ICommand _removeColumnCommand;
        public ICommand RemoveColumnCommand
        {
            get
            {
                _removeColumnCommand = _removeColumnCommand ?? new RelayCommand<string>(RemoveColumn);
                return _removeColumnCommand;
            }
        }

        private void AddColumn(string s)
        {
            ColumnDescriptor column = AvailableColumns.FirstOrDefault(x => x.DisplayMember == s);
            if (column != null)
                CurrentColumns.Add(column);
        }

        private void RemoveColumn(string s)
        {
            ColumnDescriptor column = AvailableColumns.FirstOrDefault(x => x.DisplayMember == s);
            if (column != null)
                CurrentColumns.Remove(column);
        }
    }
}
