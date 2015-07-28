using Core.Common.Utils;
using Core.Common.Extensions;
using Core.Common.Contract;   
using System.Text;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using FluentValidation;
using FluentValidation.Results;
using System.Runtime.Serialization;
using System.ComponentModel.Composition.Hosting;

namespace Core.Common.Core
{
    public class ObjectBase : NotificationObject, IDirtyCapable, IExtensibleDataObject, IDataErrorInfo 
    {
        public ObjectBase()
        {
            _Validator = GetValidator();
            Validate();
        }
                                       
        protected bool _IsDirty = false;
        protected IValidator _Validator = null;
        protected IEnumerable<ValidationFailure> _ValidationErrors = null;
        public static CompositionContainer Container { get; set; }


        #region IDataErrorInfo memebers
        string IDataErrorInfo.Error
        {
            get { return string.Empty; }
        }

        string IDataErrorInfo.this[string columnName]
        {
            get
            {
                StringBuilder errors = new StringBuilder();

                if(_ValidationErrors != null && _ValidationErrors.Count()>0)
                {
                    foreach(ValidationFailure validationError in _ValidationErrors)
                    {
                        if (validationError.PropertyName == columnName)
                            errors.AppendLine(validationError.ErrorMessage);
                    }
                }

                return errors.ToString();
            }
        }

        #endregion

        #region Validation
        protected virtual IValidator GetValidator()
        {
            return null;
        }   

        public void Validate()
        {
            if(_Validator != null)
            {
                ValidationResult results = _Validator.Validate(this);
                _ValidationErrors = results.Errors;
            }
        }

        [NotNavigable]
        public virtual bool IsValid
        {
            get
            {
                if (_ValidationErrors != null && _ValidationErrors.Count() > 0)
                    return false;
                else
                    return true;

            }
        }

        #endregion

        #region IExtensibleDataObject Members
        
        public ExtensionDataObject ExtensionData { get; set; }
        
        #endregion

        #region Property change notification
        /*
        protected event PropertyChangedEventHandler PropertyChanged
        {
            add
            {
                if (!_PropertyChangedSubscribers.Contains(value))
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

        public event PropertyChangedEventHandler _PropertyChanged;

        List<PropertyChangedEventHandler> _PropertyChangedSubscribers
            = new List<PropertyChangedEventHandler>();
        */
        protected override void OnPropertyChanged(string propertyName)
        {
            OnPropertyChanged(propertyName, true);
        }

        protected void OnPropertyChanged(string propertyName, bool makeDirty)
        {
            base.OnPropertyChanged(propertyName);

            if (makeDirty)
                IsDirty = true;

            Validate();
        }

        protected void OnPropertyChanged<T>(Expression<Func<T>> propertyExpression, bool makeDirty)
        {
            string propertyName = PropertySupport.ExtractPropertyName(propertyExpression);
            OnPropertyChanged(propertyName, makeDirty);
        }

        #endregion Property change notification

        #region IDirtyCapable memebers

        [NotNavigable]
        public virtual bool IsDirty
        {
            get { return _IsDirty; }
            set { _IsDirty = value; }
        }

        public List<IDirtyCapable> GetDirtyObjects()
        {
            List<IDirtyCapable> dirtyObjects = new List<IDirtyCapable>();

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

        protected void WalkObjectGraph(Func<ObjectBase, bool> snippetForObject,
                                       Action<IList> snippetForCollection,
                                       params string[] exemptProperties)
        {
            List<ObjectBase> visited = new List<ObjectBase>();
            Action<ObjectBase> walk = null;

            List<string> exemptions = new List<string>();
            if (exemptProperties != null)
                exemptions = exemptProperties.ToList();

            walk = (o) =>
            {
                if (o != null && !visited.Contains(o))
                {
                    visited.Add(o);

                    bool exitWalk = snippetForObject.Invoke(o);

                    if (!exitWalk)
                    {
                        PropertyInfo[] properties = o.GetBrowsableProperties();
                        foreach (PropertyInfo property in properties)
                        {
                            if (property.PropertyType.IsSubclassOf(typeof(ObjectBase)))
                            {
                                ObjectBase obj = (ObjectBase)(property.GetValue(o, properties));
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
                                        if (item is ObjectBase)
                                            walk((ObjectBase)item);
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


