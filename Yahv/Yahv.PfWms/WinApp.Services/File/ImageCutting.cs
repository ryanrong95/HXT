using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Yahv.Usually;

namespace WinApp.Services
{
    /// <summary>
    ///图片处理类
    /// </summary>
    public class ImageCutting
    {
        public ImageCutting()
        {
            //
            //TODO: 在此处添加构造函数逻辑
            //
        }

        /// <summary>
        /// 剪切图片
        /// </summary>
        /// <param name="path_source">原始图片路径</param>
        /// <param name="path_save">目标图片路径</param>
        /// <param name="x">剪切位置的左上角x坐标</param>
        /// <param name="y">剪切位置的左上角y坐标</param>
        /// <param name="width">要剪切的宽度</param>
        /// <param name="height">要剪切的高度</param>
        public void Cut(string path_source, string path_save, int x, int y, int width, int height)
        {
            //加载底图
            Image img = Image.FromFile(path_source);
            int w = img.Width;
            int h = img.Height;
            //设置画布
            width = width >= w ? w : width;
            height = height >= h ? h : height;
            Bitmap map = new Bitmap(width, height);
            //绘图
            Graphics g = Graphics.FromImage(map);
            g.DrawImage(img, 0, 0, new Rectangle(x, y, width, height), GraphicsUnit.Pixel);
            //保存
            map.Save(path_save);

        }


        /// <summary>
        /// 生成缩略图
        /// </summary>
        /// <param name="path_source">原始图片路径</param>
        /// <param name="path_save">缩略图路径</param>
        /// <param name="times">缩略倍数</param>
        /// <param name="b">缩略或放大（true缩略）</param>
        public void Thumbnail(string path_source, string path_save, int times, bool b)
        {
            //加载底图
            Image img = Image.FromFile(path_source);
            int w = img.Width;
            int h = img.Height;
            //设置画布
            int width = 0;
            int height = 0;
            if (b)
            {
                width = w / times;
                height = h / times;
            }
            else
            {
                width = w * times;
                height = h * times;
            }
            Bitmap map = new Bitmap(width, height);
            //绘图
            Graphics g = Graphics.FromImage(map);
            g.DrawImage(img, 0, 0, width, height);
            //保存
            map.Save(path_save);

        }

        /// <summary>
        /// 图片任意拉伸
        /// </summary>
        /// <param name="path_source">原始图片路径</param>
        /// <param name="path_save">缩略图路径</param>
        /// <param name="times">缩略倍数</param>
        /// <param name="b">缩略或放大（true缩略）</param>
        public void ResetImageSize(string path_source, string path_save,int width, int height)
        {
            //加载底图
            Image img = Image.FromFile(path_source);
            
            Bitmap map = new Bitmap(width,height);
            //绘图
            Graphics g = Graphics.FromImage(map);
            g.DrawImage(img, 0, 0, width, height);
            //保存
            map.Save(path_save);

        }

        /// <summary>
        /// 图片合成
        /// </summary>
        /// <param name="p1">图片1</param>
        /// <param name="p2">图片2</param>
        /// <param name="p2">图片3</param>
        /// <param name="path_save">新图片路径</param>
        public void Compound(string p1, string p2, string p3, string path_save)
        {
            //加载底图
            Image img1 = Image.FromFile(p1);
            Image img2 = Image.FromFile(p2);
            Image img3 = Image.FromFile(p3);


            //int w = img1.Width + img2.Width;
            //int h = img1.Height > img2.Height ? img1.Height : img2.Height;

            int w = img3.Width > (img1.Width > img2.Width ? img1.Width : img2.Width) ? img3.Width : (img1.Width > img2.Width ? img1.Width : img2.Width);
            int h = img1.Height + img2.Height + img3.Height;
            //设置画布


            Bitmap map = new Bitmap(w, h);
            //绘图
            Graphics g = Graphics.FromImage(map);
            g.DrawImage(img1, 0, 0, new Rectangle(0, 0, img1.Width, img1.Height), GraphicsUnit.Pixel);
            g.DrawImage(img2, 0, img1.Height, new Rectangle(0, 0, img2.Width, img2.Height), GraphicsUnit.Pixel);
            g.DrawImage(img3, 0, img1.Height+img2.Height, new Rectangle(0, 0, img3.Width, img3.Height), GraphicsUnit.Pixel);
            //g.DrawImage(img, 0, 0, new Rectangle(0, 0, width, height), GraphicsUnit.Pixel);
            //保存
            map.Save(path_save);
        }

        static object locker = new object();

        static ImageCutting current;
        static public ImageCutting Current
        {
            get
            {
                if (current == null)
                {
                    lock (locker)
                    {
                        if (current == null)
                        {
                            current = new ImageCutting();
                        }
                    }
                }

                return current;
            }
        }


    }
}