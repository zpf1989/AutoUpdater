using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoUpdater.Components
{
    public static class ObjectExtension
    {
        /// <summary>
        /// 获取对象的DescriptionAttribute值
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string GetDescription(this object value)
        {
            string str = value.ToString();
            var field = value.GetType().GetField(str);
            object[] attrs = field.GetCustomAttributes(typeof(DescriptionAttribute), false);
            if (attrs == null || attrs.Length < 1)
            {
                return str;
            }
            var da = (DescriptionAttribute)attrs[0];
            if (da == null)
            {
                return str;
            }
            return da.Description;
        }
    }
}
