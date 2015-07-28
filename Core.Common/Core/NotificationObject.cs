using Core.Common.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Core.Common.Core
{
    public class NotificationObject : INotifyPropertyChanged
    {
        private event PropertyChangedEventHandler _PropertyChangeEvent;

        protected List<PropertyChangedEventHandler> _PropertyChangeSubscribers = new List<PropertyChangedEventHandler>();
 
        public event PropertyChangedEventHandler PropertyChanged
        {
            add
            {
                if(!_PropertyChangeSubscribers.Contains(value))
                {
                    _PropertyChangeEvent += value;
                    _PropertyChangeSubscribers.Add(value);
                }
            }
            remove
            {
                _PropertyChangeEvent -= value;
                _PropertyChangeSubscribers.Remove(value);
            }
        }

        protected virtual void OnPropertyChanged(string propertyName)
        {
            if (_PropertyChangeEvent != null)
                _PropertyChangeEvent(this, new PropertyChangedEventArgs(propertyName));
        }

        protected virtual void OnPropertyChanged<T>(Expression<Func<T>> propertyExpression)
        {
            string propertyName = PropertySupport.ExtractPropertyName(propertyExpression);
            OnPropertyChanged(propertyName);
        }
    }
}
