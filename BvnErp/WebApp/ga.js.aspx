<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ga.js.aspx.cs" Inherits="WebApp.ga_js" %>

<!DOCTYPE html>

<html>
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title></title>
    <script>

        /*  颗粒化 */
        (function ($) {
            /* 默认值 */
            var defaults = {
                menu: '', /* 菜单名称*/
                rawurl: document.URL.replace(/\?.*?$/ig, '').toLowerCase(), /* 当前页面url*/
                actionurl: '/ga.js.aspx?action=roleunites', /* 写入action */
                readurl: '/ga.js.aspx?action=readunites', /* 配置读取action */
                enterSuccess: function () { }, /* callback */
                readSuccess: function () { } /* callback */
            };
            /* 标签属性名称 */
            var uniteLabel = {
                names: ['v_name', 'g_name'],
                title: 'v_title'
            };
            /* 配置类型 */
            var roleUniteType = {
                /* 正常 */
                normal: 1,
                /* grid */
                grid: 2
            };

            $.extend({
                roleUnites: function (options) {

                    options = options || {};

                    var obj = new Object();

                    /* 项 */
                    obj.options = $.extend({}, defaults, options);

                    /* 标签集合 */
                    obj.labels = $('[' + uniteLabel.names.join('],[') + ']');

                    /* roleUnite 对象集合 */
                    /* obj.options.rawurl = getRelativeUrl(obj.options.rawurl); */
                    obj.sets = getSets(obj.options.menu, obj.options.rawurl, obj.labels);

                    /* 读取入库 */
                    obj.enter = function () {
                        /* 验证 menu 必需项 */
                        if ($.trim(obj.options.menu) == '' || obj.options.menu == defaults.menu) {
                            throw new Error("roleUnites enter options.menu undefined!");
                            return;
                        }
                        /* 验证 action 必需项 */
                        if ($.trim(obj.options.actionurl) == '') {
                            throw new Error("roleUnites enter options.actionurl undefined!");
                            return;
                        }
                        /* 写入读取的配置项 */

                        $.ajax({
                            type: 'post',
                            url: obj.options.actionurl,
                            data: { menu: obj.options.menu, url: obj.options.rawurl, sets: JSON.stringify(obj.sets) },
                            dataType: 'json',
                            success: function (data) {
                                console.log(obj.options.menu + ' unites write success');
                                /* callback */
                                obj.options.enterSuccess();
                            }
                        });

                        return obj; /* 返回当前对象，支持链式调用 */
                    };

                    /* 配置 */
                    obj.read = function (target) {
                        var arry = eval('(<%=this.Model %>)');
                        if (arry && arry.length > 0) {
                            $.each(arry, function (index, item) {
                                if (item.Type == 1) {
                                    $('[' + uniteLabel.names[item.Type - 1] + '="' + item.Name + '"]').hide();
                                }
                                else if (item.Type == 2) {
                                    var gname = $('[' + uniteLabel.names[item.Type - 1] + '="' + item.Name + '"]');
                                    /*gname.remove()*/;
                                    var tabid = getParentTable(gname).attr('id');
                                    $("#" + tabid).datagrid('hideColumn', item.Name);
                                }
                            });
                        }
                        console.log(obj.options.menu + ' unites read success');
                        /* callback */
                        obj.options.readSuccess();

                        return obj; /* 返回当前对象，支持链式调用 */
                    };

                    return obj;
                }
            });

           /* g_name table */
           var getParentTable = function (gname) {
               var nodename = gname.prop("nodeName").toLowerCase();
               if (nodename != 'table') {
                   return getParentTable(gname.parent());
               }
               return gname;
           };

           /* 根据标签生成 roleUnite 对象集合 */
           var getSets = function (menu, url, labels) {
               var arry = [];
               var nameValues = [];
               labels.each(function () {

                   var vtype = 1;
                   var vname = '';
                   for (var i = 0; i < uniteLabel.names.length; i++) {
                       vname = $(this).attr(uniteLabel.names[i]);
                       if (vname) {
                           vtype = i + 1;
                           break;
                       }
                   }
                   if (nameValues.indexOf(vname) == -1) {
                       nameValues.push(vname);
                       arry.push({
                           Menu: menu, /* 菜单 */
                           Type: vtype, /* 类型 grid 还是正常 */
                           Name: vname, /* 标识名称 */
                           Title: $(this).attr(uniteLabel.title), /* 标识title */
                           Url: url, /* 当前页面url */
                       });

                   }

               });
               return arry;
           };


       })(jQuery);
    </script>
    <script>
        /*
           * 获取当前相对路径
           * 比如 http://www.xx.com/public/item.aspx?t=110 结果为：/public/item.aspx
           */
        function getRelativeUrl(url) {
            var arrUrl = url.split("//");

            var start = arrUrl[1].indexOf("/");
            var relUrl = arrUrl[1].substring(start);/* stop省略，截取从start开始到结尾的所有字符 */

            if (relUrl.indexOf("?") != -1) {
                relUrl = relUrl.split("?")[0];
            }
            return relUrl;
        }

        var writeMenu = function () {
            /* 写入菜单 */
            if ($.trim(gvSettings.fatherMenu) != '' && $.trim(gvSettings.menu) != '') {
                var obj = {
                    Name: $.trim(gvSettings.menu),
                    FatherID: $.trim(gvSettings.fatherMenu),
                    Url: getRelativeUrl(document.URL.replace(/\?.*?$/ig, '').toLowerCase()),
                    Summary: $.trim(gvSettings.summary)
                };
                $.ajax({
                    type: 'post',
                    url: '/ga.js.aspx?action=writeMenu',
                    data: { menu: JSON.stringify(obj) },
                    dataType: 'json',
                    success: function (data) {
                        /* callback */
                        console.log(gvSettings.menu + ' menu success');
                    }
                });
            }
        };
        var doUnites = function (target) {
            /* 写入颗粒化 验证是否有menu， 没有就不执行匹配 */
            gvSettings.rawurl = getRelativeUrl(document.URL.replace(/\?.*?$/ig, '').toLowerCase());
            if ($.trim(gvSettings.menu) != '') {

                var options = $.extend({}, gvSettings);
                var unites = $.roleUnites(options);
                unites.enter().read(target); /* 写入并配置 */

            }
        };


        /* 扩展 easyui datagrid 视图加载完成之后的事件:$.fn.datagrid.defaults.view.onAfterRender */
        var onAfterRender = $.fn.datagrid.defaults.view.onAfterRender;
        $.extend($.fn.datagrid.defaults.view, {
            onAfterRender: function (target) {
                onAfterRender.call(this, target);

                doUnites(target);
            }
        });

        $(function () {

            writeMenu();
            if ($('[g_name]').length == 0) {
                doUnites();
            }

        });
    </script>
</head>
<body>
</body>
</html>
