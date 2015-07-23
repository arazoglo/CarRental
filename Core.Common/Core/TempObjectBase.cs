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
    public class TempObjectBase : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler _PropertyChanged;     

        List<PropertyChangedEventHandler> _PropertyChangedSubscribers
            = new List<PropertyChangedEventHandler>();

        public event PropertyChangedEventHandler PropertyChanged
        {
            add
            {
                if(!_PropertyChangedSubscribers.Contains(value))
                {
                    _PropertyChanged += value;
                    _PropertyChangedSubscribers.Add(value);
                }
            }
            remove
            {
                _PropertyChanged -= value;
            }
        }

        protected virtual void OnPropertyChanged(string propertyName)
        {
            OnPropertyChanged(propertyName, true);
        }

        protected virtual void OnPropertyChanged(string propertyName, bool makeDirty)
        {
            if (_PropertyChanged != null)
                _PropertyChanged(this, new PropertyChangedEventArgs(propertyName)); 
 
            if (makeDirty)
                _IsDirty = true;
        }       
        
        protected virtual void OnPropertyChanged<T>(Expression<Func<T>> propertyExpression)
        {
            string propertyName = PropertySupport.ExtractPropertyName(propertyExpression);
            OnPropertyChanged(propertyName);
        }  

        bool _IsDirty;

        public bool IsDirty
        {
            get { return _IsDirty; }
        }

        protected List<TempObjectBase> GetDirtyObjects()
        {
            List<TempObjectBase> dirtyObjects = new List<TempObjectBase>();

            List<TempObjectBase> visited = new List<TempObjectBase>();
            Action<TempObjectBase> walk = null;

            walk = (o) =>
                {
                    if (o != null && !visited.Contains(o))
                    {
                        visited.Add(o);

                        if (o.IsDirty)
                            dirtyObjects.Add(o);

                        bool exitWalk = false;

                        if(!exitWalk)
                        {

                        }
                    }
                };

            return dirtyObjects;
        }
    }
}


