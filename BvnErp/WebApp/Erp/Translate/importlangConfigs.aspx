<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="importlangConfigs.aspx.cs" Inherits="WebApp.Erp.Translate.importlangConfigs" %>

<!DOCTYPE html>
<html>
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>翻译数据批量处理</title>
    <uc:EasyUI runat="server" />
    <style>
        h1 { font-size: 20px; }
    </style>
    <script>
        function importData() {
            $.get('/Erp/Languages/List.aspx?action=data', {}, function (data) {
                worker.langs = (function () {
                    var arry = [];
                    for (var i in data) {
                        arry.push(data[i].ShortName);
                    }
                    return arry;
                })();
                worker.loadJs();
            });
        };
        function intiData() {
            worker.createScript({ src: '/files/langfile.js' }, function () {
                $.post('TopobjectConfig.ashx?action=upload', { data: JSON.stringify(lang || {}) }, function (text) {
                    $.get('?', {
                        action: 'delete',
                        file: 'langfile.js'
                    });
                    document.getElementById('result1').innerText = '处理完成';
                });
            })
        };
        var worker = {
            createScript: function (attrs, callBack) {
                var script = document.createElement('script');
                for (var name in attrs) {
                    script.setAttribute(name, attrs[name]);
                }
                script.setAttribute('src', script.getAttribute('src') + '?_' + Math.random());
                script.onload = function (event) {
                    script.remove();
                    callBack(event.target);
                };
                script.onerror = function (e) {
                    script.remove();
                    alert('没有找到文件:' + (/^http:|^https:/.test(attrs['src']) ? attrs['src'] : (window.location.protocol + '//' + window.location.host + attrs['src'])));
                }
                document.head.insertBefore(script, document.getElementsByTagName('script')[0]);
            },
            counter: 0,
            langs: ['en-US', 'zh-CN', 'zh-Hant'],
            loadJs: function () {
                for (var i = 0, length = this.langs.length; i < length; i++) {
                    this.createScript({
                        src: '/files/result_' + this.langs[i] + '.js',
                        lang: this.langs[i]
                    }, function (script) {
                        try {
                            worker.post(script.getAttribute('lang'));
                        } catch (e) {
                            console.log(e);
                        }
                    });
                }
            },
            post: function (attr) {
                if (window.lang) {
                    var classer = window.lang[attr];
                    if (classer) {
                        $.ajax({
                            url: '?',
                            type: 'post',
                            data: {
                                action: 'import',
                                langType: attr,
                                data: $('<div>').html(JSON.stringify(classer)).text()
                            },
                            dataType: 'json',
                            success: function (result) {
                                document.getElementById('result').innerHTML = document.getElementById('result').innerHTML + '<p>' + attr + ' 处理完成<p>';
                                $.get('?', {
                                    action: 'delete',
                                    file: 'result_' + attr + '.js'
                                });
                            },
                            error: function (XMLHttpRequest, textStatus, errorThrown) {

                            }
                        });
                    }
                }
            }
        };
    </script>
</head>
<body>
    <%--<div style="width: 45%; display: inline-block; vertical-align: top;">
        <h1>翻译项数据格式</h1>
        <p>文件名：langfile.js</p>
        <p>js实例代码</p>
        <img src="imgs/3.png" />
    </div>--%>
    <div style="width: 45%; display: inline-block; vertical-align: top;">
        <h1>翻译数据格式</h1>
        <p>文件名：result_***.js</p>
        <img src="imgs/0.png" />
        <p>js实例代码</p>
        <img src="imgs/1.png" />
    </div>
    <hr />
    <form runat="server" method="post" enctype="multipart/form-data">
        <input type="file" multiple="multiple" name="files" />
        <input runat="server" onserverclick="upload_click" type="submit" value="上传" />
    </form>
    <hr />
    <%--<input type="button" value="增加翻译项" onclick="intiData()" />处理结果：<span id="result1"></span>
    <hr />--%>
    <input type="button" value="导入数据" onclick="importData()" />处理结果：<span id="result"></span>
    <hr />
</body>
</html>

