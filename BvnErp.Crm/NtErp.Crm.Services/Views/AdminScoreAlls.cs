using Layer.Data.Sqls;
using Needs.Linq;
using NtErp.Crm.Services.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NtErp.Crm.Services.Views
{
    public class AdminScoreAlls : UniqueView<Score, BvCrmReponsitory>
    {
        public AdminScoreAlls()
        {

        }

        protected override IQueryable<Score> GetIQueryable()
        {
            AdminTopView Admins = new AdminTopView(this.Reponsitory);

            var linq1 = from map in Reponsitory.ReadTable<Layer.Data.Sqls.BvCrm.MapsDistrict>()
                       join district in Reponsitory.ReadTable<Layer.Data.Sqls.BvCrm.Districts>()
                       on map.DistrictID equals district.ID
                       select new
                       {
                           map.AdminID,
                           distictId = district.Level == 11 ? district.ID : district.FatherID,
                       };

            var linq2 = from map in Reponsitory.ReadTable<Layer.Data.Sqls.BvCrm.MapsDistrict>()
                        join district in Reponsitory.ReadTable<Layer.Data.Sqls.BvCrm.Districts>()
                        on map.DistrictID equals district.ID
                        select new
                        {
                            AdminID = map.LeadID,
                            distictId = district.Level == 11 ? district.ID : district.FatherID,
                        };

            var districtlinq = from map in linq1.Union(linq2)
                               join district in Reponsitory.ReadTable<Layer.Data.Sqls.BvCrm.Districts>()
                               on map.distictId equals district.ID
                               select new
                               {
                                   map.AdminID,
                                   district
                               };

            return from Score in Reponsitory.ReadTable<Layer.Data.Sqls.BvCrm.Scores>()
                   join Admin in Admins on Score.AdminID equals Admin.ID
                   join district in districtlinq on Score.AdminID equals district.AdminID into _districts
                   select new Score
                   {
                       ID = Score.ID,
                       ScoreType = (Enums.ScoreType)Score.ScoreType,
                       ReportScore = Score.ReportScore,
                       ProjectScore = Score.ProjectScore,
                       ClientScore = Score.ClientScore,
                       DIScore = Score.DIScore,
                       DWScore = Score.DWScore,
                       Year = Score.Year,
                       Month = Score.Month,
                       TotalScore = Score.TotalScore,
                       Bonus = Score.Bonus,
                       Admin = Admin,
                       DistrictNames = _districts.Select(item => item.district.Name).ToArray(),
                   };
        }
    }
}
