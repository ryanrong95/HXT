using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Needs.Wl.CustomsTool.WinForm.Services
{
    public class IconImageProvider
    {
        ImageList _SmallImageList;
        ImageList _LargeImageList;
        Dictionary<string, int> _ImageIndexDict = new Dictionary<string, int>();

        public IconImageProvider(ImageList smallImageList, ImageList largeImageList)
        {
            _SmallImageList = smallImageList;
            _LargeImageList = largeImageList;
        }

        /// <summary>
        /// Get Iamge Index of this filename's Icon
        /// </summary>
        /// <param name="fileName">file name</param>
        /// <returns></returns>
        public int GetIconImageIndex(string fileName)
        {
            String extension = System.IO.Path.GetExtension(fileName).ToLower().Trim();

            int index = -1;

            if (_ImageIndexDict.TryGetValue(extension, out index))
            {
                return index;
            }

            if (_SmallImageList != null)
            {
                _SmallImageList.Images.Add(FileIcon.GetFileIcon(fileName, false));
                index = _SmallImageList.Images.Count - 1;
            }

            if (_LargeImageList != null)
            {
                _LargeImageList.Images.Add(FileIcon.GetFileIcon(fileName, true));
                index = _LargeImageList.Images.Count - 1;
            }

            _ImageIndexDict.Add(extension, index);
            return index;
        }
    }
}
