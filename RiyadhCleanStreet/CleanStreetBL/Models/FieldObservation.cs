using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleanStreetBL.Models
{

    public class FieldObservation : INotifyPropertyChanged
    {
        private int observationId;
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ObservationId
        {
            get { return observationId; }
            set { observationId = value; OnPropertyChanged("ObservationId"); }
        }

        private DateTime reportDateTime;
        public DateTime ReportDateTime
        {
            get
            {
                return reportDateTime;
            }
            set
            {
                reportDateTime = value;
                OnPropertyChanged("ReportDateTime");
            }
        }

        private DateTime hijraDate;
        public DateTime HijraDate
        {
            get
            {
                return hijraDate;
            }
            set
            {
                hijraDate = value;
                OnPropertyChanged("HijraDate");
            }
        }

        private double lat;
        public double Lat
        {
            get
            {
                return lat;
            }
            set
            {
                lat = value;
                OnPropertyChanged("Lat");
            }
        }

        private double lng;
        public double Lng
        {
            get
            {
                return lng;
            }
            set
            {
                lng = value;
                OnPropertyChanged("Lng");
            }
        }

        private string observationName;
        public string ObservationName
        {
            get
            {
                return observationName;
            }
            set
            {
                observationName = value;
                OnPropertyChanged("ObservationName");
            }
        }

        private string description;
        public string Description
        {
            get
            {
                return description;
            }
            set
            {
                description = value;
                OnPropertyChanged("Description");
            }
        }

        private string streetName;
        public string StreetName
        {
            get
            {
                return streetName;
            }
            set
            {
                streetName = value;
                OnPropertyChanged("StreetName");
            }
        }

        private string districtName;
        public string DistrictName
        {
            get
            {
                return districtName;
            }
            set
            {
                districtName = value;
                OnPropertyChanged("DistrictName");
            }
        }

        private string blgNoORLoc;
        public string BlgNoORLoc
        {
            get
            {
                return blgNoORLoc;
            }
            set
            {
                blgNoORLoc = value;
                OnPropertyChanged("BlgNoORLoc");
            }
        }

        private string status;
        public string Status
        {
            get
            {
                return status;
            }
            set
            {
                status = value;
                OnPropertyChanged("Status");
            }
        }

        private string userId;
        public string UserId
        {
            get
            {
                return userId;
            }
            set
            {
                userId = value;
                OnPropertyChanged("UserId");
            }
        }

        private string fileName;
        public string FileName
        {
            get
            {
                return fileName;
            }
            set
            {
                fileName = value;
                OnPropertyChanged("FileName");
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
