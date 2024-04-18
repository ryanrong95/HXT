using Castle.DynamicProxy;
using Layer.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{
    public class TestHandler
    {
        private string Id { get; set; }

        private string AdminID { get; set; }

        private string Name { get; set; }

        private string[] IDs { get; set; }

        public TestHandler()
        {

        }

        public TestHandler(string id)
        {
            this.Id = id;
        }

        public TestHandler(string id, string adminID, string name)
        {
            this.Id = id;
            this.AdminID = adminID;
            this.Name = name;
        }

        public TestHandler(string[] ids)
        {
            this.IDs = ids;
        }

        public void Add()
        {
            using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
            {
                reponsitory.InsertWithLog<Layer.Data.Sqls.ScCustoms.ExitNoticeFiles>(new Layer.Data.Sqls.ScCustoms.ExitNoticeFiles
                {
                    ID = Guid.NewGuid().ToString("N"),
                    ExitNoticeID = this.Id,
                    AdminID = this.Id,
                    Name = this.Id,
                    URL = this.Id,
                    FileType = 1,
                    FileFormat = this.Id,
                    Status = 200,
                    CreateDate = DateTime.Now,
                    Summary = this.Id,
                });
            }
        }

        public void BarchInsert()
        {
            List<Layer.Data.Sqls.ScCustoms.ExitNoticeFiles> listExitNoticeFiles = new List<Layer.Data.Sqls.ScCustoms.ExitNoticeFiles>();
            foreach (var itemId in this.IDs)
            {
                listExitNoticeFiles.Add(new Layer.Data.Sqls.ScCustoms.ExitNoticeFiles()
                {
                    ID = Guid.NewGuid().ToString("N"),
                    ExitNoticeID = itemId,
                    AdminID = itemId,
                    Name = itemId,
                    URL = itemId,
                    FileType = 1,
                    FileFormat = itemId,
                    Status = 200,
                    CreateDate = DateTime.Now,
                    Summary = itemId,
                });
            }

            using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
            {
                reponsitory.InsertWithLog<Layer.Data.Sqls.ScCustoms.ExitNoticeFiles>(listExitNoticeFiles.ToArray());
            }
        }

        public void Delete()
        {
            using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
            {
                reponsitory.DeleteWithLog<Layer.Data.Sqls.ScCustoms.ExitNoticeFiles>(item => item.ExitNoticeID == this.Id);
            }
        }

        public void Update()
        {
            using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
            {
                reponsitory.UpdateWithLog<Layer.Data.Sqls.ScCustoms.ExitNoticeFiles>(new
                {
                    AdminID = this.AdminID,
                    Name = this.Name,
                    Summary = "uu",
                }, item => item.ExitNoticeID == this.Id);
            }
        }

        public void UpdateChangeds()
        {
            using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
            {
                reponsitory.UpdateWithLog<Layer.Data.Sqls.ScCustoms.ExitNoticeFiles>(new Layer.Data.Sqls.ScCustoms.ExitNoticeFiles
                {
                    AdminID = this.AdminID,
                    Name = this.Name,
                }, item => item.ExitNoticeID == this.Id);
            }
        }

        public void UpdateProblem()
        {
            using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
            {
                reponsitory.UpdateWithLog<Layer.Data.Sqls.ScCustoms.Orders>(new { OrderStatus = Enums.OrderStatus.Declared, }, item => item.ID == "ICG0120190802003");
            }
        }

        public void UpdateObjects()
        {
            using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
            {
                var ids = new string[]
                {
                    "a440781567af4e059693d73d492b4556",
                    "35119772492043c2a8cfc84df6b9010b",
                    "1c7aabc0ad954034acc40a102dd47ed2",
                    "cee1e6223001400babd61d07b64e90e8",
                };
                Expression<Func<Layer.Data.Sqls.ScCustoms.ExitNoticeFiles, bool>> whereLambda = oit => ids.Contains(oit.ID);
                Expression<Func<Layer.Data.Sqls.ScCustoms.ExitNoticeFiles, string>> orderbyLambda = oit => oit.ID;

                List<Layer.Data.Sqls.ScCustoms.ExitNoticeFiles> listaasda = new List<Layer.Data.Sqls.ScCustoms.ExitNoticeFiles>();
                
                listaasda.Add(new Layer.Data.Sqls.ScCustoms.ExitNoticeFiles
                {
                    ID = "cee1e6223001400babd61d07b64e90e8",
                    ExitNoticeID = "dddd",
                });

                listaasda.Add(new Layer.Data.Sqls.ScCustoms.ExitNoticeFiles
                {
                    ID = "1c7aabc0ad954034acc40a102dd47ed2",
                    ExitNoticeID = "cccc",
                });

                listaasda.Add(new Layer.Data.Sqls.ScCustoms.ExitNoticeFiles
                {
                    ID = "35119772492043c2a8cfc84df6b9010b",
                    ExitNoticeID = "bbbb",
                });

                listaasda.Add(new Layer.Data.Sqls.ScCustoms.ExitNoticeFiles
                {
                    ID = "a440781567af4e059693d73d492b4556",
                    ExitNoticeID = "aaaaa",
                });

                listaasda = listaasda.OrderBy(t => t.ID).ToList();

                var entities = listaasda.Select(t => new { ID = t.ID, ExitNoticeID = t.ExitNoticeID, }).ToArray();
                reponsitory.UpdateWithLog(whereLambda, orderbyLambda, entities);
            }
        }

        public void DeleteBoolean()
        {
            using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
            {
                //reponsitory.DeleteWithLog<Layer.Data.Sqls.ScCustoms.EntryNoticeItems>(item => item.ID == "aaa");

                //IcgooRequestLog System.Nullable<System.DateTime>     System.Nullable<bool>    System.Nullable<int>
                //SwapNoticeItems System.Nullable<decimal>

                reponsitory.InsertWithLog<Layer.Data.Sqls.ScCustoms.PreProductPostLog>(new Layer.Data.Sqls.ScCustoms.PreProductPostLog
                {
                    ID = Guid.NewGuid().ToString("N"),
                    CreateDate = DateTime.Now,
                });

                reponsitory.InsertWithLog<Layer.Data.Sqls.ScCustoms.SwapNoticeItems>(new Layer.Data.Sqls.ScCustoms.SwapNoticeItems
                {
                    ID = Guid.NewGuid().ToString("N"),
                    SwapNoticeID = "aa",
                    DecHeadID = "cc",
                    CreateDate = DateTime.Now,
                    Amount = (decimal?)2.1,
                    Status = 200,
                });

            }
        }

    }
}
