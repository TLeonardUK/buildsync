using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BuildSync.Core.Utils
{
    public static class EnumUtils
    {
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="EnumVal"></param>
        /// <returns></returns>
        public static T GetAttributeOfType<T>(this Enum EnumVal) 
            where T : System.Attribute
        {
            var EnumType = EnumVal.GetType();
            var MemberInfo = EnumType.GetMember(EnumVal.ToString());
            var Attributes = MemberInfo[0].GetCustomAttributes(typeof(T), false);
            return (Attributes.Length > 0) ? (T)Attributes[0] : null;
        }
    }
}
