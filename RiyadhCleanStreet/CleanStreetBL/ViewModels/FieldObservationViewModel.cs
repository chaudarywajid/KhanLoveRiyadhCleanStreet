using CleanStreetBL.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleanStreetBL.ViewModels
{
    class FieldObservationViewModel : INotifyPropertyChanged
    {
        public FieldObservationViewModel()
        {

            Model = new FieldObservation();

        }

        FieldObservation _model;
        public FieldObservation Model
        {
            get
            {
                return _model;
            }
            set
            {

                _model = value;
                OnPropertyChanged("Model");

            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

    }
}
