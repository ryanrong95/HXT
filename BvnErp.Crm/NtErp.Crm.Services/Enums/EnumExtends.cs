using Needs.Utils.Descriptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//using System.ComponentModel;
using System.Reflection;

namespace NtErp.Crm.Services.Enums
{
    /// <summary>
    /// 建议使用：Needs.Utils.Descriptions.EnumUtils
    /// </summary>
    //[Obsolete("建议废弃：shenchen")]
    //public static class EnumExtends
    //{
    //    [Obsolete("建议废弃：shenchen")]
    //    public static List<DropDownList> ToDropDownList<T>(this T EnumType)
    //    {
    //        List<DropDownList> DDLlist = new List<DropDownList>();
    //        Type enumtype = typeof(T);
    //        foreach (int value in Enum.GetValues(enumtype))
    //        {
    //            ////获取枚举name
    //            //string name = Enum.GetName(enumtype, value);

    //            ////方法一
    //            //MemberInfo[] fields = enumtype.GetMember(name);
    //            //Attribute[] attributes = Attribute.GetCustomAttributes(fields[0]);
    //            //DescriptionAttribute attr = attributes.SingleOrDefault(item => item is DescriptionAttribute) as DescriptionAttribute;

    //            ////方法二
    //            ////FieldInfo info = enumtype.GetField(name);
    //            ////DescriptionAttribute[] attrarray = info.GetCustomAttributes(false) as DescriptionAttribute[];
    //            ////DescriptionAttribute attr1 = attrarray.SingleOrDefault();
    //            ////string text2 = attr1.Description;

    //            //DDLlist.Add(new DropDownList
    //            //{
    //            //    text = attr.Description,
    //            //    value = value.ToString()
    //            //});
    //        }
    //        return DDLlist;
    //    }

    //    /// <summary>
    //    /// 这是我给你写的
    //    /// 这样的方法不适合写泛型方法
    //    /// 也不适合写在这里
    //    /// </summary>
    //    /// <typeparam name="T">泛型就是任意的</typeparam>
    //    /// <returns></returns>
    //    public static List<DropDownList> ToDropDownList_chenhan<T>()
    //    {
    //        List<DropDownList> list = new List<DropDownList>();
    //        Type type = typeof(T);
    //        return new List<DropDownList>(Enum.GetValues(type).Cast<Enum>().Select(item => new DropDownList
    //        {
    //            text = item.GetDescription(),
    //            value = ((int)Enum.ToObject(type, item)).ToString()
    //        }));
    //    }

    //    [Obsolete("建议废弃：shenchen")]
    //    public static string GetDescription(this Type e, int value)
    //    {
    //        string name = Enum.GetName(e, value);
    //        MemberInfo[] fields = e.GetMember(name);
    //        Attribute[] attributes = Attribute.GetCustomAttributes(fields[0]);
    //        DescriptionAttribute attr = attributes.SingleOrDefault(item => item is DescriptionAttribute) as DescriptionAttribute;

    //        return "";
    //    }
    //}

    /// <summary>
    /// 这个更加不需要
    /// 命名也没有考虑到通用性
    /// </summary>

    //[Obsolete("建议废弃：shenchen")]
    //public class DropDownList
    //{
    //    //文本显示
    //    public string text { get; set; }
    //    //值
    //    public string value { get; set; }
    //}
}
