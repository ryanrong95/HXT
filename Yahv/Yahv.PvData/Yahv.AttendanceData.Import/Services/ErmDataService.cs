using Layers.Data.Sqls;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.AttendanceData.Import.Extends;

namespace Yahv.AttendanceData.Import.Services
{
    public class ErmDataService
    {
        private string connectString = "Data Source=172.30.10.51,6522;Initial Catalog=PvbErm;Persist Security Info=True;User ID=u_v0;Password=V6e1W9MiA84t6FJ05TXCU9uRY9THg3EpT60v9X1qwMbLg895Oc";
        private SqlConnection conn;
        public ErmDataService()
        {

        }

        public void DataImport()
        {
            //List<Layers.Data.Sqls.PvbErm.VoteSteps> VoteSteps;
            //List<Layers.Data.Sqls.PvbErm.VoteFlows> VoteFlows;
            List<Layers.Data.Sqls.PvbErm.Calendars> Calendars;
            List<Layers.Data.Sqls.PvbErm.RegionsAc> RegionsAc;
            List<Layers.Data.Sqls.PvbErm.Schedulings> Schedulings;
            List<Layers.Data.Sqls.PvbErm.Schedules> Schedules;
            List<Layers.Data.Sqls.PvbErm.SchedulesPublic> SchedulesPublic;

            using (PvbErmReponsitory repository = new PvbErmReponsitory())
            {
                //VoteSteps = repository.ReadTable<Layers.Data.Sqls.PvbErm.VoteSteps>().ToList();
                //VoteFlows = repository.ReadTable<Layers.Data.Sqls.PvbErm.VoteFlows>().ToList();
                Calendars = repository.ReadTable<Layers.Data.Sqls.PvbErm.Calendars>().ToList();
                RegionsAc = repository.ReadTable<Layers.Data.Sqls.PvbErm.RegionsAc>().ToList();
                Schedulings = repository.ReadTable<Layers.Data.Sqls.PvbErm.Schedulings>().ToList();
                Schedules = repository.ReadTable<Layers.Data.Sqls.PvbErm.Schedules>().ToList();
                SchedulesPublic = repository.ReadTable<Layers.Data.Sqls.PvbErm.SchedulesPublic>().ToList();
            }
            using (conn = new SqlConnection(this.connectString))
            {
                //conn.BulkInsert(VoteSteps);
                //conn.BulkInsert(VoteFlows);
                conn.BulkInsert(Calendars);
                conn.BulkInsert(RegionsAc);
                conn.BulkInsert(Schedulings);
                conn.BulkInsert(Schedules);
                conn.BulkInsert(SchedulesPublic);
            }
        }
    }
}
