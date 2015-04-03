﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
            this.Persons = new ObservableCollection<Person>
                {
                    new Person
                        {
                            Name = "Doe",
                            FirstName = "John",
                            DateOfBirth = new DateTime(1981, 9, 12)
                        },
                    new Person
                        {
                            Name = "Black",
                            FirstName = "Jack",
                            DateOfBirth = new DateTime(1950, 1, 15)
                        },
                    new Person
                        {
                            Name = "Smith",
                            FirstName = "Jane",
                            DateOfBirth = new DateTime(1987, 7, 23)
                        }
                };

            this.Columns = new ObservableCollection<ColumnDescriptor>
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
                        }
                };
        }

        public ObservableCollection<Person> Persons { get; private set; }

        public ObservableCollection<ColumnDescriptor> Columns { get; private set; }

        private ICommand _addColumnCommand;
        public ICommand AddColumnCommand
        {
            get
            {
                if (_addColumnCommand == null)
                {
                    _addColumnCommand = new RelayCommand<string>(
                        s => this.Columns.Add(new ColumnDescriptor
                            {
                                HeaderText = s,
                                DisplayMember = s
                            }));
                }
                return _addColumnCommand;
            }
        }

        private ICommand _removeColumnCommand;
        public ICommand RemoveColumnCommand
        {
            get
            {
                if (_removeColumnCommand == null)
                {
                    _removeColumnCommand = new RelayCommand<string>(
                        s => this.Columns.Remove(this.Columns.FirstOrDefault(d => d.DisplayMember == s)));
                }
                return _removeColumnCommand;
            }
        }
    }
}
