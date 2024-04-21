using Layers.Data.Sqls;
using Layers.Linq;
using System;
using System.Linq;
using Yahv.Underly;
using Yahv.Underly.Attributes;
using Yahv.Underly.Erps;

namespace Yahv.CrmPlus.Service
{
    public class InitialEnum
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        static InitialEnum()
        {

        }
        static object locker = new object();

        /// <summary>
        /// EnterError
        /// </summary>
        static public void Execute()
        {
            lock (locker)
            {
                var k = new InitialEnum();
                k.Enter<FixedSource>();
                k.Enter<FixedArea>();
                k.Enter<FixedIndustry>();
            }
        }

        void Enter<T>()
        {
            using (var repository = new PvdCrmReponsitory())
            {
                var type = typeof(T);
                var query = Enum.GetValues(type).Cast<Enum>();
                var baseType = Enum.GetUnderlyingType(type);

                var exsits = repository.ReadTable<Layers.Data.Sqls.PvdCrm.EnumsDictionaries>().
                    Where(ed => query.Select(item => item.GetFixedID()).Contains(ed.ID));

                foreach (var item in query)
                {
                    var description = item.GetDescription();
                    var id = item.GetFixedID();
                    if (exsits.Any(t => t.ID == id))
                    {
                        repository.Update<Layers.Data.Sqls.PvdCrm.EnumsDictionaries>(new
                        {
                            Description = description,
                            Value = Convert.ChangeType(item, baseType).ToString()
                        }, u => u.ID == id);
                    }
                    else
                    {
                        repository.Insert(new Layers.Data.Sqls.PvdCrm.EnumsDictionaries
                        {
                            ID = id,
                            Enum = type.Name,
                            IsFixed = true,
                            Field = item.ToString(),
                            Description = description,
                            Value = Convert.ChangeType(item, baseType).ToString(),
                            CreatorID = Npc.Robot.Obtain(),
                            CreateDate = DateTime.Now
                        });
                    }
                }
            }
        }
    }
}
