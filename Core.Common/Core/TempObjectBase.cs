﻿using Core.Common.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Core.Common.Extensions;

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
                                      
        #region IDirtyCapable memebers

        [NotNavigable]
        public bool IsDirty
        {
            get { return _IsDirty; }
            set { _IsDirty = value; }
        }                       

        public List<TempObjectBase> GetDirtyObjects()
        {
            List<TempObjectBase> dirtyObjects = new List<TempObjectBase>();

            WalkObjectGraph(
                o =>
                {
                    if (o.IsDirty)
                        dirtyObjects.Add(o);

                    return false;
                }, coll => { });

            return dirtyObjects;
        }

        public void CleanAll()
        {
            WalkObjectGraph(
                o =>
                {
                    if (o.IsDirty)
                        o.IsDirty = false;

                    return false;
                }, coll => { });
        }

        public virtual bool IsAnythingDirty()
        {
            bool isDirty = false;

            WalkObjectGraph(
                o =>
                {
                    if (o.IsDirty)
                    {
                        IsDirty = true;
                        return true;
                    }
                    else
                        return false;//short circuit
                }, coll => { });
            
            return isDirty;
        }

        #endregion
        
        #region Protected methos

        protected void WalkObjectGraph(Func<TempObjectBase, bool> snippetForObject,
                                       Action<IList> snippetForCollection,
                                       params string[] exemptProperties)
        {
            List<TempObjectBase> visited = new List<TempObjectBase>();
            Action<TempObjectBase> walk = null;

            List<string> exemptions = new List<string>();
            if (exemptProperties != null)
                exemptions = exemptProperties.ToList();

            walk = (o) =>
                {
                    if (o != null && !visited.Contains(o))
                    {
                        visited.Add(o);

                        bool exitWalk = snippetForObject.Invoke(o);

                        if(!exitWalk)
                        {
                            PropertyInfo[] properties = o.GetBrowsableProperties();
                            foreach (PropertyInfo property in properties)
                            {
                                if (property.PropertyType.IsSubclassOf(typeof(TempObjectBase)))
                                {
                                    TempObjectBase obj = (TempObjectBase)(property.GetValue(o, properties));
                                    walk(obj);
                                }
                                else
                                {
                                    IList coll = property.GetValue(o, null) as IList;
                                    if (coll != null)
                                    {
                                        snippetForCollection.Invoke(coll);

                                        foreach (object item in coll)
                                        {
                                            if (item is TempObjectBase)
                                                walk((TempObjectBase)item);
                                        }

                                    }
                                }
                            }
                        }                 
                    }
                };

            walk(this);
        }

        #endregion
    }
}


