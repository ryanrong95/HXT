<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Tester.aspx.cs" Inherits="WebApp.Tester" %>

<!DOCTYPE html>

<html>
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title></title>
</head>
<body>
    <form id="form1" runat="server" enctype="multipart/form-data">
        <p id="box" style="width: 800px; height: 400px; border: 1px solid;" contenteditable="true">
        </p>
        <input type="file" name="test_file"/>
        <input type="submit" value="上传吧" />
        <%--<script type="text/javascript" src="require.js"></script>--%>
        <script>
            //function UploadImage(id, url, key) {
            //    this.element = document.getElementById(id);
            //    this.url = url; //后端处理图片的路径
            //    this.imgKey = key || "PasteAreaImgKey"; //提到到后端的name

            //}
            //UploadImage.prototype.paste = function (callback, formData) {
            //    var thatthat = this;
            //    this.element.addEventListener('paste', function (e) {//处理目标容器（id）的paste事件

            //        if (e.clipboardData && e.clipboardData.items[0].type.indexOf('image') > -1) {
            //            var that = this,
            //                reader = new FileReader();
            //            file = e.clipboardData.items[0].getAsFile();//读取e.clipboardData中的数据：Blob对象

            //            reader.onload = function (e) { //reader读取完成后，xhr上传
            //                var xhr = new XMLHttpRequest(),
            //                    fd = formData || (new FormData());;
            //                xhr.open('POST', thatthat.url, true);
            //                xhr.onload = function () {
            //                    callback.call(that, xhr);
            //                }
            //                fd.append(thatthat.imgKey, this.result); // this.result得到图片的base64
            //                xhr.send(fd);
            //            }
            //            reader.readAsDataURL(file);//获取base64编码
            //        }
            //    }, false);
            //}
        </script>
        <script>
            //require(['UploadImage'], function (UploadImage) {
            //    new UploadImage("box", "UploadHandler.ashx").upload(function (xhr) {//上传完成后的回调
            //        var img = new Image();
            //        img.src = xhr.responseText;
            //        this.appendChild(img);
            //    });
            //})
        </script>
    </form>
</body>
</html>
