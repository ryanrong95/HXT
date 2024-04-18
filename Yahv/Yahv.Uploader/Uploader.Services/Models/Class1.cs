using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Uploader.Services.Models
{
    class Class1
    {

        public Class1()
        {
            var arry = new string[10];
            var viwe = new Views.FilesDescriptionView();

            viwe.Modify(new
            {
                PayID = "1231231"
            }, arry);
        }

    }
}
