

using NtErp.Wss.Sales.Services.Utils.Convertibles;
using System;
using System.Collections;
using System.Collections.Generic;

/*
建议不要建立太多
*/
namespace NtErp.Wss.Sales.Services.Underly
{
    public partial class Elements
    {
        #region Enum

        public static implicit operator Enum(Elements entity)
        {
            Type type = entity.Value.GetType();
            if (type.IsEnum)
            {
                return (Enum)entity.Value.ChangeType(type);
            }

            throw new NotSupportedException($"Type:{type.FullName} does not support conversion!");
        }
        public static implicit operator Elements(Enum v)
        {
            return new Elements(v);
        }

        #endregion

        #region Currency

        public static implicit operator Currency(Elements entity)
        {
            Type type = entity.Value.GetType();
            if (type.IsEnum)
            {
                return (Currency)entity.Value.ChangeType(type);
            }

            throw new NotSupportedException($"Type:{type.FullName} does not support conversion!");

        }
        public static implicit operator Elements(Currency v)
        {
            return new Elements(v);
        }

        public static implicit operator List<Currency>(Elements entity)
        {
            if (entity.Value is List<Currency>)
            {
                return entity.Value as List<Currency>;
            }

            Type currentType = typeof(Currency);
            List<Currency> list = new List<Currency>();
            if (entity.Value is IEnumerable)
            {
                foreach (var item in entity.Value as IEnumerable)
                {
                    list.Add((Currency)item.ChangeType(currentType));
                }
            }
            else
            {
                list.Add((Currency)entity.Value.ChangeType(currentType));
            }
            entity.Value = list;
            return list;
        }
        public static implicit operator Elements(List<Currency> v)
        {
            return new Elements(v);
        }

        public static implicit operator Currency[] (Elements entity)
        {
            if (entity.Value is double[])
            {
                return entity.Value as Currency[];
            }

            Type currentType = typeof(Currency);

            if (entity.Value is IEnumerable)
            {
                ArrayList arryList = new ArrayList();
                foreach (var item in entity.Value as IEnumerable)
                {
                    arryList.Add(item.ChangeType(currentType));
                }

                return (entity.Value = arryList.ToArray(currentType)) as Currency[];
            }
            else
            {
                return (entity.Value = new Currency[] { (Currency)entity.Value.ChangeType(currentType) }) as Currency[];
            }
        }
        public static implicit operator Elements(Currency[] v)
        {
            return new Elements(v);
        }

        #endregion

        #region District

        public static implicit operator District(Elements entity)
        {
            Type type = entity.Value.GetType();
            if (type.IsEnum)
            {
                return (District)entity.Value.ChangeType(type);
            }

            throw new NotSupportedException($"Type:{type.FullName} does not support conversion!");

        }
        public static implicit operator Elements(District v)
        {
            return new Elements(v);
        }

        public static implicit operator List<District>(Elements entity)
        {
            if (entity.Value is List<District>)
            {
                return entity.Value as List<District>;
            }

            Type currentType = typeof(District);
            List<District> list = new List<District>();
            if (entity.Value is IEnumerable)
            {
                foreach (var item in entity.Value as IEnumerable)
                {
                    list.Add((District)item.ChangeType(currentType));
                }
            }
            else
            {
                list.Add((District)entity.Value.ChangeType(currentType));
            }
            entity.Value = list;
            return list;
        }
        public static implicit operator Elements(List<District> v)
        {
            return new Elements(v);
        }

        public static implicit operator District[] (Elements entity)
        {
            if (entity.Value is double[])
            {
                return entity.Value as District[];
            }

            Type currentType = typeof(District);

            if (entity.Value is IEnumerable)
            {
                ArrayList arryList = new ArrayList();
                foreach (var item in entity.Value as IEnumerable)
                {
                    arryList.Add(item.ChangeType(currentType));
                }

                return (entity.Value = arryList.ToArray(currentType)) as District[];
            }
            else
            {
                return (entity.Value = new District[] { (District)entity.Value.ChangeType(currentType) }) as District[];
            }
        }
        public static implicit operator Elements(District[] v)
        {
            return new Elements(v);
        }

        #endregion


    }
}
