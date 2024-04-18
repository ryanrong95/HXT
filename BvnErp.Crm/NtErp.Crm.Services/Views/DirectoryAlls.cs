using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Needs.Linq;
using Layer.Data.Sqls;
using Needs.Utils.Converters;
using Newtonsoft.Json.Linq;
using NtErp.Crm.Services.Enums;
using NtErp.Crm.Services.Models;

namespace NtErp.Crm.Services.Views
{
    public class DirectoryAlls : UniqueView<Directory, BvCrmReponsitory>, Needs.Underly.IFkoView<Directory>
    {
        /// <summary>
        /// 无参构造函数
        /// </summary>
        public DirectoryAlls()
        {

        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="reponsitory">数据库实体</param>
        internal DirectoryAlls(BvCrmReponsitory reponsitory) : base(reponsitory)
        {

        }

        protected override IQueryable<Directory> GetIQueryable()
        {
            return from directory in this.Reponsitory.ReadTable<Layer.Data.Sqls.BvCrm.Directories>()
                   select new Directory
                   {
                       ID = directory.ID,
                       FatherID = directory.FatherID,
                       Name = directory.Name,
                       Status = (Status)directory.Status,
                       CreateDate = directory.CreateDate,
                       UpdateDate = directory.UpdateDate,
                   };
        }
    }

    /// <summary>
    /// 获取目录树形结构
    /// </summary>
    public class DirectoryTree
    {
        private Directory[] directories;
        private string directoryid;

        /// <summary>
        /// 区域树集合
        /// </summary>
        public JArray Tree
        {
            get
            {
                return this.GetTree();
            }
        }

        /// <summary>
        /// 路径集合
        /// </summary>
        public string Path
        {
            get
            {
                return this.GetPath(this.directoryid);
            }
        }

        /// <summary>
        /// 初始化
        /// </summary>
        public DirectoryTree(string id = null)
        {
            directoryid = id;
            using (BvCrmReponsitory reponsitory = new BvCrmReponsitory())
            {
                directories = new DirectoryAlls(reponsitory).Where(item => item.Status == Status.Normal).ToArray();
            }
        }

        /// <summary>
        /// 获取区域树形集合
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        private JArray GetTree(string id = null)
        {
            Func<Directory, bool> exp = t => t.FatherID == id;
            JArray arry = new JArray();
            var list = this.directories.Where(exp).ToArray();
            foreach (var item in list)
            {
                JObject obj = new JObject();
                obj.Add("id", item.ID);
                obj.Add("text", item.Name);

                var children = GetTree(item.ID);
                if (children != null && children.Count > 0)
                {
                    obj.Add("children", children);
                }
                arry.Add(obj);
            }

            return arry;
        }

        /// <summary>
        /// 获取路径
        /// </summary>
        /// <param name="path"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        private string GetPath(string id, string path = null)
        {
            if (string.IsNullOrEmpty(path))
            {
                path = string.Empty;
            }
            var directory = this.directories.Where(item => item.ID == id).SingleOrDefault();
            if (directory == null)
            {
                return string.Empty;
            }
            path = directory.Name + "\\" + path;
            if (!string.IsNullOrWhiteSpace(directory.FatherID))
            {
                path = GetPath(directory.FatherID, path);
            }
            return path;
        }
    }

}
