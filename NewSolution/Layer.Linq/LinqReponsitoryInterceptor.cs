using Castle.DynamicProxy;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Layer.Linq
{
    public class EntityInsertInterceptor<T> : IInterceptor where T : class, INotifyPropertyChanging, INotifyPropertyChanged
    {
        private object Entity { get; set; }

        private LinqReponsitory Reponsitory { get; set; }

        public EntityInsertInterceptor(object entity, LinqReponsitory reponsitory)
        {
            this.Entity = entity;
            this.Reponsitory = reponsitory;
        }

        public void Intercept(IInvocation invocation)
        {
            invocation.Proceed();

            ParameterInfo[] parameters = invocation.Method.GetParameters();

            foreach (var parameter in parameters)
            {
                string parameterTypeName = parameter.ParameterType.Name;

                //Assembly assembly = Assembly.Load(LinqReponsitoryLogConst.AssemblyName);
                Type type = typeof(T);  //assembly.GetType(LinqReponsitoryLogConst.DataTypeNamePrefix + parameterTypeName);

                PropertyInfo[] propertyInfos = type.GetProperties();
                if (propertyInfos != null && propertyInfos.Any())
                {
                    List<ReponsitoryLogRow> logRows = new List<ReponsitoryLogRow>();
                    List<ReponsitoryLogColumn> logColumns = new List<ReponsitoryLogColumn>();

                    string rowID = Guid.NewGuid().ToString("N");

                    logRows.Add(new ReponsitoryLogRow()
                    {
                        ID = rowID,
                        DataSource = this.Reponsitory.DataContext.Connection.DataSource,
                        DatabaseName = this.Reponsitory.DataContext.Connection.Database,
                        TableName = type.Name,
                        Operation = RowOperationEnum.Insert,
                        Status = Status.Normal,
                        CreateTime = DateTime.Now,
                        UpdateTime = DateTime.Now,
                    });

                    foreach (var propertyInfo in propertyInfos)
                    {
                        string propertyTypeFullName = propertyInfo.PropertyType.FullName;
                        object propertyObject = type.GetProperty(propertyInfo.Name).GetValue(this.Entity);

                        if (LinqReponsitoryLogCommon.CheckRightPropertyTypeName(ref propertyTypeFullName))
                        {
                            logColumns.Add(new ReponsitoryLogColumn()
                            {
                                ID = Guid.NewGuid().ToString("N"),
                                RowID = rowID,
                                ColumnName = propertyInfo.Name,
                                ColumnType = propertyTypeFullName,
                                NewPrimaryKey = (string)type.GetProperty("ID").GetValue(this.Entity),
                                NewValue = LinqReponsitoryLogCommon.ConvertObjectToString(propertyObject, propertyTypeFullName),//propertyObject == null ? LinqReponsitoryLogConst.NULL : Convert.ToString(propertyObject),
                                Status = Status.Normal,
                                CreateTime = DateTime.Now,
                                UpdateTime = DateTime.Now,
                            });
                        }
                    }

                    ReponsitoryLogExecutor logExecutor = new ReponsitoryLogExecutor(this.Reponsitory);
                    logExecutor.RowBatchInsert(logRows.ToArray());
                    logExecutor.ColumnBatchInsert(logColumns.ToArray());


                }
            }

        }
    }

    public class EntitiesInsertInterceptor<T> : IInterceptor where T : class, INotifyPropertyChanging, INotifyPropertyChanged
    {
        public T[] Entities { get; set; }

        private LinqReponsitory Reponsitory { get; set; }

        public EntitiesInsertInterceptor(T[] entities, LinqReponsitory reponsitory)
        {
            this.Entities = entities;
            this.Reponsitory = reponsitory;
        }

        public void Intercept(IInvocation invocation)
        {
            invocation.Proceed();

            Type type = typeof(T);
            PropertyInfo[] propertyInfos = type.GetProperties();

            if (this.Entities != null && this.Entities.Any())
            {
                List<ReponsitoryLogRow> logRows = new List<ReponsitoryLogRow>();
                List<ReponsitoryLogColumn> logColumns = new List<ReponsitoryLogColumn>();

                foreach (var entity in this.Entities)
                {
                    if (propertyInfos != null && propertyInfos.Any())
                    {
                        string rowID = Guid.NewGuid().ToString("N");

                        logRows.Add(new ReponsitoryLogRow()
                        {
                            ID = rowID,
                            DataSource = this.Reponsitory.DataContext.Connection.DataSource,
                            DatabaseName = this.Reponsitory.DataContext.Connection.Database,
                            TableName = type.Name,
                            Operation = RowOperationEnum.Insert,
                            Status = Status.Normal,
                            CreateTime = DateTime.Now,
                            UpdateTime = DateTime.Now,
                        });

                        foreach (var propertyInfo in propertyInfos)
                        {
                            string propertyTypeFullName = propertyInfo.PropertyType.FullName;
                            object propertyObject = type.GetProperty(propertyInfo.Name).GetValue(entity);

                            if (LinqReponsitoryLogCommon.CheckRightPropertyTypeName(ref propertyTypeFullName))
                            {
                                logColumns.Add(new ReponsitoryLogColumn()
                                {
                                    ID = Guid.NewGuid().ToString("N"),
                                    RowID = rowID,
                                    ColumnName = propertyInfo.Name,
                                    ColumnType = propertyTypeFullName,
                                    NewPrimaryKey = (string)type.GetProperty("ID").GetValue(entity),
                                    NewValue = LinqReponsitoryLogCommon.ConvertObjectToString(propertyObject, propertyTypeFullName),//propertyObject == null ? LinqReponsitoryLogConst.NULL : Convert.ToString(propertyObject),
                                    Status = Status.Normal,
                                    CreateTime = DateTime.Now,
                                    UpdateTime = DateTime.Now,
                                });
                            }
                        }

                    }
                }

                ReponsitoryLogExecutor logExecutor = new ReponsitoryLogExecutor(this.Reponsitory);
                logExecutor.RowBatchInsert(logRows.ToArray());
                logExecutor.ColumnBatchInsert(logColumns.ToArray());
            }


        }
    }

    public class DeleteInterceptor<T> : IInterceptor where T : class, INotifyPropertyChanging, INotifyPropertyChanged
    {
        private Expression<Func<T, bool>> Lambda { get; set; }

        private LinqReponsitory Reponsitory { get; set; }

        public DeleteInterceptor(Expression<Func<T, bool>> lambda, LinqReponsitory reponsitory)
        {
            this.Lambda = lambda;
            this.Reponsitory = reponsitory;
        }

        public void Intercept(IInvocation invocation)
        {
            List<ReponsitoryLogRow> allLogRows = new List<ReponsitoryLogRow>();
            List<ReponsitoryLogColumn> allLogColumns = new List<ReponsitoryLogColumn>();

            var deletings = Reponsitory.ReadTable<T>().Where(this.Lambda).ToList();

            if (deletings != null && deletings.Any())
            {
                foreach (var deleting in deletings)
                {
                    Type type = typeof(T);
                    PropertyInfo[] propertyInfos = type.GetProperties();
                    if (propertyInfos != null && propertyInfos.Any())
                    {
                        List<ReponsitoryLogRow> logRows = new List<ReponsitoryLogRow>();
                        List<ReponsitoryLogColumn> logColumns = new List<ReponsitoryLogColumn>();

                        string rowID = Guid.NewGuid().ToString("N");

                        logRows.Add(new ReponsitoryLogRow()
                        {
                            ID = rowID,
                            DataSource = this.Reponsitory.DataContext.Connection.DataSource,
                            DatabaseName = this.Reponsitory.DataContext.Connection.Database,
                            TableName = type.Name,
                            Operation = RowOperationEnum.Delete,
                            Status = Status.Normal,
                            CreateTime = DateTime.Now,
                            UpdateTime = DateTime.Now,
                        });

                        foreach (var propertyInfo in propertyInfos)
                        {
                            string propertyTypeFullName = propertyInfo.PropertyType.FullName;
                            object propertyObject = type.GetProperty(propertyInfo.Name).GetValue(deleting);

                            if (LinqReponsitoryLogCommon.CheckRightPropertyTypeName(ref propertyTypeFullName))
                            {
                                logColumns.Add(new ReponsitoryLogColumn()
                                {
                                    ID = Guid.NewGuid().ToString("N"),
                                    RowID = rowID,
                                    ColumnName = propertyInfo.Name,
                                    ColumnType = propertyTypeFullName,
                                    OldPrimaryKey = (string)type.GetProperty("ID").GetValue(deleting),
                                    OldValue = LinqReponsitoryLogCommon.ConvertObjectToString(propertyObject, propertyTypeFullName),//propertyObject == null ? LinqReponsitoryLogConst.NULL : Convert.ToString(propertyObject),
                                    Status = Status.Normal,
                                    CreateTime = DateTime.Now,
                                    UpdateTime = DateTime.Now,
                                });
                            }
                        }


                        allLogRows.AddRange(logRows);
                        allLogColumns.AddRange(logColumns);


                    }
                }
            }

            invocation.Proceed();

            ReponsitoryLogExecutor logExecutor = new ReponsitoryLogExecutor(this.Reponsitory);
            logExecutor.RowBatchInsert(allLogRows.ToArray());
            logExecutor.ColumnBatchInsert(allLogColumns.ToArray());

        }
    }

    public class ObjectUpdateInterceptor<T> : IInterceptor where T : class, INotifyPropertyChanging, INotifyPropertyChanged
    {
        private object Entity { get; set; }

        private Expression<Func<T, bool>> Lambda { get; set; }

        private LinqReponsitory Reponsitory { get; set; }

        public ObjectUpdateInterceptor(object entity, Expression<Func<T, bool>> lambda, LinqReponsitory reponsitory)
        {
            this.Entity = entity;
            this.Lambda = lambda;
            this.Reponsitory = reponsitory;
        }

        public void Intercept(IInvocation invocation)
        {
            List<ReponsitoryLogRow> allLogRows = new List<ReponsitoryLogRow>();
            List<ReponsitoryLogColumn> allLogColumns = new List<ReponsitoryLogColumn>();

            var updatings = Reponsitory.ReadTable<T>().Where(this.Lambda).ToList();

            invocation.Proceed();

            if (updatings != null && updatings.Any())
            {
                foreach (var updating in updatings)
                {
                    Type entityType = this.Entity.GetType();
                    PropertyInfo[] propertyInfos = entityType.GetProperties();
                    if (propertyInfos != null && propertyInfos.Any())
                    {
                        List<ReponsitoryLogRow> logRows = new List<ReponsitoryLogRow>();
                        List<ReponsitoryLogColumn> logColumns = new List<ReponsitoryLogColumn>();

                        string rowID = Guid.NewGuid().ToString("N");

                        logRows.Add(new ReponsitoryLogRow()
                        {
                            ID = rowID,
                            DataSource = this.Reponsitory.DataContext.Connection.DataSource,
                            DatabaseName = this.Reponsitory.DataContext.Connection.Database,
                            TableName = typeof(T).Name,
                            Operation = RowOperationEnum.Update,
                            Status = Status.Normal,
                            CreateTime = DateTime.Now,
                            UpdateTime = DateTime.Now,
                        });

                        foreach (var propertyInfo in propertyInfos)
                        {
                            string propertyTypeFullName = propertyInfo.PropertyType.FullName;
                            string propertyBaseTypeName = propertyInfo.PropertyType.BaseType.Name;

                            object oldPropertyObject = typeof(T).GetProperty(propertyInfo.Name).GetValue(updating);
                            object newPropertyObject = entityType.GetProperty(propertyInfo.Name).GetValue(this.Entity);

                            string oldPropertyStr = LinqReponsitoryLogCommon.ConvertObjectToString(oldPropertyObject, propertyTypeFullName);//oldPropertyObject == null ? LinqReponsitoryLogConst.NULL : Convert.ToString(oldPropertyObject);
                            string newPropertyStr = LinqReponsitoryLogCommon.ConvertObjectToString(newPropertyObject, propertyTypeFullName);//newPropertyObject == null ? LinqReponsitoryLogConst.NULL : Convert.ToString(newPropertyObject);


                            if (propertyBaseTypeName == "Enum")
                            {
                                string dllFilePath = propertyInfo.PropertyType.Assembly.CodeBase.Replace("file:///", "");
                                var asm = Assembly.LoadFile(dllFilePath);
                                Type enumType = asm.GetType(propertyTypeFullName, true);
                                foreach (var item in Enum.GetValues(enumType).Cast<Enum>())
                                {
                                    int key = item.GetHashCode();
                                    string value = Enum.GetName(enumType, item);
                                    if (newPropertyStr == value)
                                    {
                                        newPropertyStr = Convert.ToString(key);
                                    }
                                }
                            }
                            


                            if (oldPropertyStr != newPropertyStr)
                            {
                                if (LinqReponsitoryLogCommon.CheckRightPropertyTypeName(ref propertyTypeFullName) || propertyBaseTypeName == "Enum")
                                {
                                    if (propertyBaseTypeName == "Enum")
                                    {
                                        propertyTypeFullName = "System.Int32";
                                    }

                                    string newPrimaryKey = string.Empty;
                                    string[] newColumns = propertyInfos.Select(t => t.Name).ToArray();
                                    if (newColumns.Contains("ID"))
                                    {
                                        newPrimaryKey = (string)typeof(T).GetProperty("ID").GetValue(this.Entity);
                                    }
                                    else
                                    {
                                        newPrimaryKey = (string)typeof(T).GetProperty("ID").GetValue(updating);
                                    }


                                    logColumns.Add(new ReponsitoryLogColumn()
                                    {
                                        ID = Guid.NewGuid().ToString("N"),
                                        RowID = rowID,
                                        ColumnName = propertyInfo.Name,
                                        ColumnType = propertyTypeFullName,
                                        OldPrimaryKey = (string)typeof(T).GetProperty("ID").GetValue(updating),
                                        OldValue = oldPropertyStr,
                                        NewPrimaryKey = newPrimaryKey,
                                        NewValue = newPropertyStr,
                                        Status = Status.Normal,
                                        CreateTime = DateTime.Now,
                                        UpdateTime = DateTime.Now,
                                    });
                                }
                            }
                        }


                        allLogRows.AddRange(logRows);
                        allLogColumns.AddRange(logColumns);

                    }




                }
            }


            ReponsitoryLogExecutor logExecutor = new ReponsitoryLogExecutor(this.Reponsitory);
            logExecutor.RowBatchInsert(allLogRows.ToArray());
            logExecutor.ColumnBatchInsert(allLogColumns.ToArray());


        }
    }

    public class ObjectsUpdateInterceptor<T> where T : class, INotifyPropertyChanging, INotifyPropertyChanged
    {
        //private object[] Entities { get; set; }

        //private Expression<Func<T, bool>> WhereLambda { get; set; }

        //private Expression<Func<T, string>> OrderbyLambda { get; set; }

        //private LinqReponsitory Reponsitory { get; set; }

        //public ObjectsUpdateInterceptor(object[] entities, Expression<Func<T, bool>> whereLambda, Expression<Func<T, string>> orderbyLambda, LinqReponsitory reponsitory)
        //{
        //    this.Entities = entities;
        //    this.WhereLambda = whereLambda;
        //    this.OrderbyLambda = orderbyLambda;
        //    this.Reponsitory = reponsitory;
        //}

        private T[] Updatings { get; set; }

        private T[] Updateds { get; set; }

        private LinqReponsitory Reponsitory { get; set; }

        public ObjectsUpdateInterceptor(T[] updatings, T[] updateds, LinqReponsitory reponsitory)
        {
            this.Updatings = updatings;
            this.Updateds = updateds;
            this.Reponsitory = reponsitory;
        }

        public void Intercept()
        {
            List<ReponsitoryLogRow> allLogRows = new List<ReponsitoryLogRow>();
            List<ReponsitoryLogColumn> allLogColumns = new List<ReponsitoryLogColumn>();

            //var updatings = Reponsitory.ReadTable<T>().Where(this.WhereLambda).OrderBy(this.OrderbyLambda).ToList();

            //List<string> idList = new List<string>();
            //if (updatings != null && updatings.Any())
            //{
            //    foreach (var updating in updatings)
            //    {
            //        idList.Add(Convert.ToString(typeof(T).GetProperty("ID").GetValue(updating)));
            //    }
            //}

            //string[] ids = idList.ToArray();

            //invocation.Proceed();

            //var updateds = Reponsitory.ReadTable<T>().Where(this.WhereLambda).OrderBy(this.OrderbyLambda).ToList();

            if (this.Updatings != null && this.Updatings.Any())
            {
                foreach (var updating in this.Updatings)
                {
                    Type type = typeof(T);
                    PropertyInfo[] propertyInfos = type.GetProperties();
                    if (propertyInfos != null && propertyInfos.Any())
                    {
                        List<ReponsitoryLogRow> logRows = new List<ReponsitoryLogRow>();
                        List<ReponsitoryLogColumn> logColumns = new List<ReponsitoryLogColumn>();

                        string rowID = Guid.NewGuid().ToString("N");

                        logRows.Add(new ReponsitoryLogRow()
                        {
                            ID = rowID,
                            DataSource = this.Reponsitory.DataContext.Connection.DataSource,
                            DatabaseName = this.Reponsitory.DataContext.Connection.Database,
                            TableName = typeof(T).Name,
                            Operation = RowOperationEnum.Update,
                            Status = Status.Normal,
                            CreateTime = DateTime.Now,
                            UpdateTime = DateTime.Now,
                        });

                        string dataID = LinqReponsitoryLogCommon.GetObjectID<T>(updating);
                        T updated = LinqReponsitoryLogCommon.GetEntityByID<T>(this.Updateds, dataID);

                        foreach (var propertyInfo in propertyInfos)
                        {
                            string propertyTypeFullName = propertyInfo.PropertyType.FullName;

                            object oldPropertyObject = typeof(T).GetProperty(propertyInfo.Name).GetValue(updating);
                            object newPropertyObject = typeof(T).GetProperty(propertyInfo.Name).GetValue(updated);

                            string oldPropertyStr = LinqReponsitoryLogCommon.ConvertObjectToString(oldPropertyObject, propertyTypeFullName);
                            string newPropertyStr = LinqReponsitoryLogCommon.ConvertObjectToString(newPropertyObject, propertyTypeFullName);

                            if (oldPropertyStr != newPropertyStr)
                            {
                                if (LinqReponsitoryLogCommon.CheckRightPropertyTypeName(ref propertyTypeFullName))
                                {
                                    string newPrimaryKey = string.Empty;
                                    string[] newColumns = propertyInfos.Select(t => t.Name).ToArray();
                                    if (newColumns.Contains("ID"))
                                    {
                                        newPrimaryKey = (string)typeof(T).GetProperty("ID").GetValue(updated);
                                    }
                                    else
                                    {
                                        newPrimaryKey = (string)typeof(T).GetProperty("ID").GetValue(updating);
                                    }


                                    logColumns.Add(new ReponsitoryLogColumn()
                                    {
                                        ID = Guid.NewGuid().ToString("N"),
                                        RowID = rowID,
                                        ColumnName = propertyInfo.Name,
                                        ColumnType = propertyTypeFullName,
                                        OldPrimaryKey = (string)typeof(T).GetProperty("ID").GetValue(updating),
                                        OldValue = oldPropertyStr,
                                        NewPrimaryKey = newPrimaryKey,
                                        NewValue = newPropertyStr,
                                        Status = Status.Normal,
                                        CreateTime = DateTime.Now,
                                        UpdateTime = DateTime.Now,
                                    });
                                }
                            }

                        }


                        allLogRows.AddRange(logRows);
                        allLogColumns.AddRange(logColumns);

                    }
                }
            }

            ReponsitoryLogExecutor logExecutor = new ReponsitoryLogExecutor(this.Reponsitory);
            logExecutor.RowBatchInsert(allLogRows.ToArray());
            logExecutor.ColumnBatchInsert(allLogColumns.ToArray());



        }
    }

    public class TUpdateInterceptor<T> : IInterceptor where T : class, INotifyPropertyChanging, INotifyPropertyChanged
    {
        //private T Entity { get; set; }

        private Expression<Func<T, bool>> Lambda { get; set; }

        private LinqReponsitory Reponsitory { get; set; }

        public TUpdateInterceptor(T entity, Expression<Func<T, bool>> lambda, LinqReponsitory reponsitory)
        {
            //this.Entity = entity;
            this.Lambda = lambda;
            this.Reponsitory = reponsitory;
        }

        public void Intercept(IInvocation invocation)
        {
            List<ReponsitoryLogRow> allLogRows = new List<ReponsitoryLogRow>();
            List<ReponsitoryLogColumn> allLogColumns = new List<ReponsitoryLogColumn>();

            var updatings = Reponsitory.ReadTable<T>().Where(this.Lambda).ToList();

            invocation.Proceed();

            var updateds = Reponsitory.ReadTable<T>().Where(this.Lambda).ToList();

            if (updatings != null && updatings.Any())
            {
                foreach (var updating in updatings)
                {
                    Type type = typeof(T);
                    PropertyInfo[] propertyInfos = type.GetProperties();
                    if (propertyInfos != null && propertyInfos.Any())
                    {
                        List<ReponsitoryLogRow> logRows = new List<ReponsitoryLogRow>();
                        List<ReponsitoryLogColumn> logColumns = new List<ReponsitoryLogColumn>();

                        string rowID = Guid.NewGuid().ToString("N");

                        logRows.Add(new ReponsitoryLogRow()
                        {
                            ID = rowID,
                            DataSource = this.Reponsitory.DataContext.Connection.DataSource,
                            DatabaseName = this.Reponsitory.DataContext.Connection.Database,
                            TableName = typeof(T).Name,
                            Operation = RowOperationEnum.Update,
                            Status = Status.Normal,
                            CreateTime = DateTime.Now,
                            UpdateTime = DateTime.Now,
                        });

                        string dataID = LinqReponsitoryLogCommon.GetObjectID<T>(updating);
                        T updated = LinqReponsitoryLogCommon.GetEntityByID<T>(updateds.ToArray(), dataID);

                        foreach (var propertyInfo in propertyInfos)
                        {
                            string propertyTypeFullName = propertyInfo.PropertyType.FullName;

                            object oldPropertyObject = typeof(T).GetProperty(propertyInfo.Name).GetValue(updating);
                            object newPropertyObject = typeof(T).GetProperty(propertyInfo.Name).GetValue(updated);

                            string oldPropertyStr = LinqReponsitoryLogCommon.ConvertObjectToString(oldPropertyObject, propertyTypeFullName);
                            string newPropertyStr = LinqReponsitoryLogCommon.ConvertObjectToString(newPropertyObject, propertyTypeFullName);

                            if (oldPropertyStr != newPropertyStr)
                            {
                                if (LinqReponsitoryLogCommon.CheckRightPropertyTypeName(ref propertyTypeFullName))
                                {
                                    string newPrimaryKey = string.Empty;
                                    string[] newColumns = propertyInfos.Select(t => t.Name).ToArray();
                                    if (newColumns.Contains("ID"))
                                    {
                                        newPrimaryKey = (string)typeof(T).GetProperty("ID").GetValue(updated);
                                    }
                                    else
                                    {
                                        newPrimaryKey = (string)typeof(T).GetProperty("ID").GetValue(updating);
                                    }


                                    logColumns.Add(new ReponsitoryLogColumn()
                                    {
                                        ID = Guid.NewGuid().ToString("N"),
                                        RowID = rowID,
                                        ColumnName = propertyInfo.Name,
                                        ColumnType = propertyTypeFullName,
                                        OldPrimaryKey = (string)typeof(T).GetProperty("ID").GetValue(updating),
                                        OldValue = oldPropertyStr,
                                        NewPrimaryKey = newPrimaryKey,
                                        NewValue = newPropertyStr,
                                        Status = Status.Normal,
                                        CreateTime = DateTime.Now,
                                        UpdateTime = DateTime.Now,
                                    });
                                }
                            }

                        }


                        allLogRows.AddRange(logRows);
                        allLogColumns.AddRange(logColumns);

                    }
                }
            }


            ReponsitoryLogExecutor logExecutor = new ReponsitoryLogExecutor(this.Reponsitory);
            logExecutor.RowBatchInsert(allLogRows.ToArray());
            logExecutor.ColumnBatchInsert(allLogColumns.ToArray());


        }
    }

    public class LinqReponsitoryLogModel
    {
        public string ColumnName { get; set; } = string.Empty;

        public string TypeName { get; set; } = string.Empty;

        public string OldValue { get; set; } = string.Empty;

        public string NewValue { get; set; } = string.Empty;
    }

    public class LinqReponsitoryLogConst
    {
        //public const string AssemblyName = "Layer.Data";

        //public const string DataTypeNamePrefix = "Layer.Data.Sqls.ScCustoms.";

        public const string NULL = "**NULL**";

        //public const string RightPropertyTypeFullNamePrefix = "System.";

        public static readonly string[] RightPropertyTypeFullNames =
        {
            "System.String",
            "System.Boolean",
            "System.Byte",
            "System.Char",
            "System.DateTime",
            "System.Decimal",
            "System.Double",
            "System.Int16",
            "System.Int32",
            "System.Int64",
            "System.UInt16",
            "System.UInt32",
            "System.UInt64",
        };
    }

    public class LinqReponsitoryLogCommon
    {
        public static string ConvertObjectToString(object obj, string propertyTypeFullName)
        {
            string result = string.Empty;

            if (obj == null)
            {
                result = LinqReponsitoryLogConst.NULL;
            }
            else if (propertyTypeFullName == "System.DateTime")
            {
                result = Convert.ToDateTime(obj).ToString("yyyy-MM-dd HH:mm:ss.fff");
            }
            else
            {
                result = Convert.ToString(obj);
            }

            return result;
        }

        public static string GetObjectID<T>(T entity)
        {
            return Convert.ToString(typeof(T).GetProperty("ID").GetValue(entity));
        }

        public static T GetEntityByID<T>(T[] datas, string targetID)
        {
            T resultEntity = default(T);

            if (datas == null || !datas.Any())
            {
                return resultEntity;
            }

            foreach (var data in datas)
            {
                string id = GetObjectID<T>(data);
                if (id == targetID)
                {
                    resultEntity = data;
                    break;
                }
            }

            return resultEntity;
        }

        public static bool CheckRightPropertyTypeName(ref string theTypeName)
        {
            bool result = false;

            if (LinqReponsitoryLogConst.RightPropertyTypeFullNames.Contains(theTypeName))
            {
                result = true;
            }
            else if (theTypeName.Contains("System.Nullable") && theTypeName.Contains("mscorlib"))
            {
                foreach (var item in LinqReponsitoryLogConst.RightPropertyTypeFullNames)
                {
                    if (theTypeName.Contains(item))
                    {
                        theTypeName = item;
                        result = true;
                        break;
                    }
                }
            }

            return result;
        }
    }
}
