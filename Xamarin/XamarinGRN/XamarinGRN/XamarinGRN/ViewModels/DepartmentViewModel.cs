using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace XamarinGRN.ViewModels
{
    class DepartmentViewModel : IDisposable, INotifyPropertyChanged
    {
        private string _BoCode;
        public string BoCode
        {
            get { return _BoCode; }
            set
            {
                _BoCode = value;
                PropertyChanged(this, new PropertyChangedEventArgs("BoCode"));
            }
        }
        private string _BoName;
        public string BoName
        {
            get { return _BoName; }
            set
            {
                _BoName = value;
                PropertyChanged(this, new PropertyChangedEventArgs("BoName"));
            }
        }
        private bool _IsActive;
        public bool IsActive
        {
            get { return _IsActive; }
            set
            {
                _IsActive = value;
                PropertyChanged(this, new PropertyChangedEventArgs("IsActive"));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged = delegate { };
        /// <summary>
        /// Handle the on property changed, value update to screen
        /// </summary>
        /// <param name="propertyname"></param>
        public void OnPropertyChanged(string propertyname)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyname));
        }

        /// <summary>
        /// Dispose
        /// </summary>
        public void Dispose()
        {
            GC.Collect();
        }
    }
}
