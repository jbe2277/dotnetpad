using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace Waf.DotNetPad.Samples
{
    public static class NameOfOperator
    {
        private static ObservableCollection<string> list;
        
        public static void Main()
        {
            // Use nameof to compare with the property name provided by the event args.
            list = new ObservableCollection<string>();
            ((INotifyPropertyChanged)list).PropertyChanged += ListPropertyChanged;
            list.Add("Luke");
            list.Add("Han");

            // Use nameof with the ArgumentNullException constructor.
            ArgumentNullCheck(null);
        }

        private static void ListPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(list.Count))
            {
                Console.WriteLine("Count: " + list.Count);
            }
        }

        private static void ArgumentNullCheck(IReadOnlyList<string> list)
        {
            if (list == null) { throw new ArgumentNullException(nameof(list)); }
        }
    }
}
