using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace MVC_SYSTEM.Class
{
    public class Constans
    {
        public enum Month
        {
            None,
            [Description("Januari")]
            Januari,

            [Description("Februari")]
            Februari,

            [Description("Mac")]
            Mac,

            [Description("April")]
            April,

            [Description("Mei")]
            Mei,

            [Description("Jun")]
            Jun,

            [Description("Julai")]
            Julai,

            [Description("Ogos")]
            Ogos,

            [Description("Setember")]
            September,

            [Description("Oktober")]
            Oktober,

            [Description("November")]
            November,

            [Description("Disember")]
            Disember
        }

        public enum MonthNumber
        {
            None,
            [Description("01")]
            Januari,

            [Description("02")]
            Februari,

            [Description("03")]
            Mac,

            [Description("04")]
            April,

            [Description("05")]
            Mei,

            [Description("06")]
            Jun,

            [Description("07")]
            Julai,

            [Description("08")]
            Ogos,

            [Description("09")]
            September,

            [Description("10")]
            Oktober,

            [Description("11")]
            November,

            [Description("12")]
            Disember
        }
    }

    public static class EnumExtensionMethods
    {
        public static string GetEnumDescription(this Enum enumValue)
        {
            var fieldInfo = enumValue.GetType().GetField(enumValue.ToString());

            var descriptionAttributes = (DescriptionAttribute[])fieldInfo.GetCustomAttributes(typeof(DescriptionAttribute), false);

            return descriptionAttributes.Length > 0 ? descriptionAttributes[0].Description : enumValue.ToString();
        }
    }
}