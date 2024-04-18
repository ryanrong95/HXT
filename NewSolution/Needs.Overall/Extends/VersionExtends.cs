using System;

namespace Needs.Overall.Extends
{
    static class VersionExtends
    {
        public static Layer.Data.Sqls.BvOveralls.Versions ToLinq(this Models.Version entity)
        {
            return new Layer.Data.Sqls.BvOveralls.Versions
            {
                Code = entity.Code,
                CreateDate = entity.CreateDate,
                ID = entity.ID,
                LastGenerationDate = entity.LastGenerationDate,
                Name = entity.Name,
                UpdateDate = entity.UpdateDate
            };
        }
    }
}
