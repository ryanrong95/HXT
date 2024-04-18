using System;

namespace Yahv.Underly.Attributes
{
    [AttributeUsage(AttributeTargets.All, Inherited = false, AllowMultiple = true)]
    class OriginAttribute : Attribute, IOrigin
    {
        readonly string code;
        readonly string chineseName;
        readonly string englishName;

        public OriginAttribute(string code, string chineseName, string englishName)
        {
            this.code = code;
            this.chineseName = chineseName;
            this.englishName = englishName;
        }

        public string Code
        {
            get { return this.code; }
        }

        public string ChineseName
        {
            get { return this.chineseName; }
        }

        public string EnglishName
        {
            get { return this.englishName; }
        }
    }
}
