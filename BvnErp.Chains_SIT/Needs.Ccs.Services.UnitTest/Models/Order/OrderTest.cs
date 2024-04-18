using Microsoft.VisualStudio.TestTools.UnitTesting;
using Needs.Ccs.Services.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models.Tests
{
    [TestClass()]
    public class DraftOrderTest
    {
        [TestMethod()]
        public void TestDelete()
        {
            var order = new Views.DraftOrdersView()["WL00220190510004"];
            order.Delete();

            using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
            {
                var linq = reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.Orders>().First(item => item.ID == order.ID);
                Assert.AreEqual((int)Enums.Status.Delete, linq.Status);

                //数据还原
                reponsitory.Update<Layer.Data.Sqls.ScCustoms.Orders>(new { Status = Enums.Status.Normal }, item => item.ID == order.ID);
            }
        }
    }
}