using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Underly.Attributes;

namespace Yahv.Erm.Services.Common
{
    public sealed class XDTOrganization
    {
        public static List<StaffOrganization> StaffList = new List<StaffOrganization>()
        {
            new StaffOrganization
            {
                Name = "张庆永",
                DepartmentType = DepartmentType.风控部,
                PostType = PostType.President,
                AdminID="Admin00526",
            },
            new StaffOrganization
            {
                Name = "黄巧如",
                DepartmentType = DepartmentType.审计部,
                PostType = PostType.Manager,
                AdminID="Admin00832",
            },
            new StaffOrganization
            {
                Name = "赖翠红",
                DepartmentType = DepartmentType.业务部,
                PostType = PostType.Manager,
                AdminID="Admin00534",
            },
            new StaffOrganization
            {
                Name = "陈红燕",
                DepartmentType = DepartmentType.业务部,
                PostType = PostType.Staff,
                AdminID="Admin00525",
            },
            new StaffOrganization
            {
                Name = "曹丽平",
                DepartmentType = DepartmentType.业务部,
                PostType = PostType.Staff,
                AdminID="Admin00541",
            },
            new StaffOrganization
            {
                Name = "施思静",
                DepartmentType = DepartmentType.财务部,
                PostType = PostType.Manager,
                AdminID="Admin00731",
            },
            new StaffOrganization
            {
                Name = "郝红梅",
                DepartmentType = DepartmentType.财务部,
                PostType = PostType.Staff,
                AdminID="Admin00530",
            },
            new StaffOrganization
            {
                Name = "鲁亚慧",
                DepartmentType = DepartmentType.行政部,
                PostType = PostType.Manager,
                AdminID="Admin00523",
            },
            new StaffOrganization
            {
                Name = "朱小钗",
                DepartmentType = DepartmentType.行政部,
                PostType = PostType.Staff,
                AdminID="Admin00548",
            },
            new StaffOrganization
            {
                Name = "荣检",
                DepartmentType = DepartmentType.信息IT部,
                PostType = PostType.Manager,
                AdminID="Admin00469",
            },
            new StaffOrganization
            {
                Name = "邵晨华",
                DepartmentType = DepartmentType.信息IT部,
                PostType = PostType.Staff,
                AdminID="Admin00583",
            },
            new StaffOrganization
            {
                Name = "魏晓毅",
                DepartmentType = DepartmentType.关务部,
                PostType = PostType.Manager,
                AdminID="Admin00554",
            },
            new StaffOrganization
            {
                Name = "杨文",
                DepartmentType = DepartmentType.关务部,
                PostType = PostType.Staff,
                AdminID="Admin00549",
            },
            new StaffOrganization
            {
                Name = "丁丹",
                DepartmentType = DepartmentType.关务部,
                PostType = PostType.Staff,
                AdminID="Admin00893",
            },
            new StaffOrganization
            {
                Name = "辛青濛",
                DepartmentType = DepartmentType.关务部,
                PostType = PostType.Staff,
                AdminID="Admin00533",
            },
            new StaffOrganization
            {
                Name = "黄柏云",
                DepartmentType = DepartmentType.关务部,
                PostType = PostType.Staff,
                AdminID="Admin00536",
            },
            new StaffOrganization
            {
                Name = "韦松丽",
                DepartmentType = DepartmentType.关务部,
                PostType = PostType.Staff,
                AdminID="Admin00798",
            },
            new StaffOrganization
            {
                Name = "曾勤",
                DepartmentType = DepartmentType.关务部,
                PostType = PostType.Staff,
                AdminID="Admin00834",
            },
            new StaffOrganization
            {
                Name = "李文忠",
                DepartmentType = DepartmentType.关务部,
                PostType = PostType.Staff,
                AdminID="Admin00765",
            },
            new StaffOrganization
            {
                Name = "刘艳",
                DepartmentType = DepartmentType.关务部,
                PostType = PostType.Staff,
                AdminID="Admin00553",
            },
             new StaffOrganization
            {
                Name = "赵兴",
                DepartmentType = DepartmentType.仓库部,
                PostType = PostType.Manager,
                AdminID="Admin00565",
            },
            new StaffOrganization
            {
                Name = "黎璐",
                DepartmentType = DepartmentType.仓库部,
                PostType = PostType.Staff,
                AdminID="Admin00571",
            },
            new StaffOrganization
            {
                Name = "赵亚博",
                DepartmentType = DepartmentType.仓库部,
                PostType = PostType.Staff,
                AdminID="Admin00570",
            },
            new StaffOrganization
            {
                Name = "吴师敏",
                DepartmentType = DepartmentType.仓库部,
                PostType = PostType.Staff,
                AdminID="Admin00573",
            },
            new StaffOrganization
            {
                Name = "邓学林",
                DepartmentType = DepartmentType.仓库部,
                PostType = PostType.Staff,
                AdminID="Admin00564",
            },
            new StaffOrganization
            {
                Name = "张孟雨",
                DepartmentType = DepartmentType.风控部,
                PostType = PostType.Staff,
                AdminID="Admin00532",
            },
        };
    }

    /// <summary>
    /// 部门类型
    /// </summary>
    public enum DepartmentType
    {
        [Description("审计部")]
        审计部 = 1,
        [Description("业务部")]
        业务部 = 2,
        [Description("财务部")]
        财务部 = 3,
        [Description("行政部")]
        行政部 = 4,
        [Description("关务部")]
        关务部 = 5,
        [Description("仓库部")]
        仓库部 = 6,
        [Description("风控部")]
        风控部 = 7,
        [Description("信息IT部")]
        信息IT部 = 8,
        [Description("管理部门")]
        管理部门 = 99,
    }

    /// <summary>
    /// 职务类型
    /// </summary>
    public enum PostType
    {
        [Description("普通员工")]
        Staff = 100,
        [Description("部门负责人")]
        Manager = 200,
        [Description("总经理")]
        President = 300,
    }

    public class StaffOrganization
    {
        public string Name { get; set; }

        public DepartmentType DepartmentType { get; set; }

        public PostType PostType { get; set; }

        public string AdminID { get; set; }
    }
}
