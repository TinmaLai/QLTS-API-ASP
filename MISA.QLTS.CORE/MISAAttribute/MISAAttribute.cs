using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MISA.QLTS.CORE.MISAAttribute
{
    [AttributeUsage(AttributeTargets.Property|AttributeTargets.Field|AttributeTargets.Method)]
    public class PrimaryKey : Attribute
    {

    }
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Method)]
    public class IsNotNullOrEmpty : Attribute
    {

    }
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Method)]
    public class PropertyNameFriendly : Attribute
    {
        public string Name;
        public PropertyNameFriendly(string name)
        {
            Name = name;
        }

    }
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Method)]
    public class MaxLength : Attribute
    {
        public int Length;
        public MaxLength(int length)
        {
            Length = length;
        }
    }
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Method)]
    public class NotDuplicate : Attribute
    {

    }
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Method)]
    public class NotInsertColumn : Attribute
    { 

    }
    
}
