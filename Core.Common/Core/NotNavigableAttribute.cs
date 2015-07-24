using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Core.Common.Extensions
{
    [AttributeUsage(AttributeTargets.Property)]
    public class NotNavigableAttribute : Attribute
    {
    }
}
