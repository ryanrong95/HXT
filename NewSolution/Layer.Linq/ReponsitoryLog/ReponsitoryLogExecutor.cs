using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Layer.Linq
{
    public class ReponsitoryLogExecutor
    {
        private LinqReponsitory LinqReponsitory { get; set; }

        public ReponsitoryLogExecutor(LinqReponsitory linqReponsitory)
        {
            this.LinqReponsitory = linqReponsitory;
        }

        public void RowBatchInsert(ReponsitoryLogRow[] rows)
        {
            if (rows == null || rows.Length <= 0)
            {
                return;
            }

            StringBuilder sbSql = new StringBuilder();

            sbSql.Append(@"
                INSERT  INTO dbo.ReponsitoryLogRow
                        ( ID, DataSource, DatabaseName, TableName, Operation, Status,
                          CreateTime, UpdateTime, Summary )
                VALUES   ");

            for (int i = 0; i < rows.Length; i++)
            {
                sbSql.AppendFormat(@"
                        ( N'{0}', -- ID - nvarchar(64)
                          N'{1}', -- DataSource - nvarchar(64)
                          N'{2}', -- DatabaseName - nvarchar(128)
                          N'{3}', -- TableName - nvarchar(128)
                          {4}, -- Operation - int
                          {5}, -- Status - int
                          '{6}', -- CreateTime - datetime
                          '{7}', -- UpdateTime - datetime
                          N'{8}'  -- Summary - nvarchar(1024)
                          )",
                          rows[i].ID,  //-- ID - nvarchar(64)
                          rows[i].DataSource,  //-- DataSource - nvarchar(64)
                          rows[i].DatabaseName,  //-- DatabaseName - nvarchar(128)
                          rows[i].TableName,  //-- TableName - nvarchar(128)
                          rows[i].Operation.GetHashCode(),  //-- Operation - int
                          rows[i].Status.GetHashCode(),  //-- Status - int
                          rows[i].CreateTime.ToString("yyyy-MM-dd HH:mm:ss.fff"),  //-- CreateTime - datetime
                          rows[i].UpdateTime.ToString("yyyy-MM-dd HH:mm:ss.fff"),  //-- UpdateTime - datetime
                          rows[i].Summary  //-- Summary - nvarchar(1024)
                    );

                if (i == rows.Length - 1)
                {
                    sbSql.Append(@";");
                }
                else
                {
                    sbSql.Append(@",");
                }

            }

            this.LinqReponsitory.Command(sbSql.ToString());
        }

        public void ColumnBatchInsert(ReponsitoryLogColumn[] columns)
        {
            if (columns == null || columns.Length <= 0)
            {
                return;
            }

            StringBuilder sbSql = new StringBuilder();

            sbSql.Append(@"
                    INSERT  INTO dbo.ReponsitoryLogColumn
                            ( ID ,
                              RowID ,
                              ColumnName ,
                              ColumnType ,
                              OldPrimaryKey ,
                              OldValue ,
                              NewPrimaryKey ,
                              NewValue ,
                              Status ,
                              CreateTime ,
                              UpdateTime ,
                              Summary
                            )
                    VALUES    ");

            for (int i = 0; i < columns.Length; i++)
            {
                sbSql.AppendFormat(@"
                        ( N'{0}' , -- ID - nvarchar(64)
                          N'{1}' , -- RowID - nvarchar(64)
                          N'{2}' , -- ColumnName - nvarchar(128)
                          N'{3}' , -- ColumnType - nvarchar(64)
                          N'{4}' , -- OldPrimaryKey - nvarchar(64)
                          N'{5}' , -- OldValue - nvarchar(1024)
                          N'{6}' , -- NewPrimaryKey - nvarchar(64)
                          N'{7}' , -- NewValue - nvarchar(64)
                          {8} , -- Status - int
                          '{9}' , -- CreateTime - datetime
                          '{10}' , -- UpdateTime - datetime
                          N'{11}'  -- Summary - nvarchar(1024)
                        )",
                        columns[i].ID,
                        columns[i].RowID,
                        columns[i].ColumnName,
                        columns[i].ColumnType,
                        columns[i].OldPrimaryKey,
                        columns[i].OldValue,
                        columns[i].NewPrimaryKey,
                        columns[i].NewValue,
                        columns[i].Status.GetHashCode(),
                        columns[i].CreateTime.ToString("yyyy-MM-dd HH:mm:ss.fff"),
                        columns[i].UpdateTime.ToString("yyyy-MM-dd HH:mm:ss.fff"),
                        columns[i].Summary
                    );

                if (i == columns.Length - 1)
                {
                    sbSql.Append(@";");
                }
                else
                {
                    sbSql.Append(@",");
                }

            }

            this.LinqReponsitory.Command(sbSql.ToString());
        }

    }
}
