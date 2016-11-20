using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using static System.Console;

namespace Waf.DotNetPad.Applications.Samples
{
    public static class NullConditionalOperator
    {
        public static void Main()
        {
            // Use ?. to access the Name property
            Person person = null;
            WriteLine("person?.Name: {0}", person?.Name ?? "null");
            List<Person> persons = null;
            WriteLine("persons?[0].Name: {0}", persons?[0].Name ?? "null");

            person = new Person() { Name = "Luke" };
            WriteLine("person?.Name: {0}", person?.Name ?? "null");
            persons = new List<Person>() { person };
            WriteLine("persons?[0].Name: {0}", persons?[0].Name ?? "null");

            // Use ?. to raise the PropertyChanged event
            WriteLine();
            person.PropertyChanged += PersonPropertyChanged;
            person.Name = "Han";
        }

        private static void PersonPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(Person.Name))
            {
                WriteLine("New name: " + ((Person)sender).Name);
            }
        }
    }

    public class Person : INotifyPropertyChanged
    {
        private string name;

        public string Name
        {
            get { return name; }
            set
            {
                if (name != value)
                {
                    name = value;
                    RaisePropertyChanged();
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void RaisePropertyChanged([CallerMemberName] string propertyName = null)
        {
            OnPropertyChanged(new PropertyChangedEventArgs(propertyName));
        }

        protected virtual void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            PropertyChanged?.Invoke(this, e);
        }
    }
}
