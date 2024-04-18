<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="List.aspx.cs" Inherits="WebApp.NoticeBoard.List" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>公告-发布消息</title>
    <uc:EasyUI runat="server" />
    <script src="../Scripts/Ccs.js"></script>
    <link href="../App_Themes/xp/Style.css" rel="stylesheet" />
    <script type="text/javascript">
        var AdminID = '<%=this.Model.AdminID%>';

        $(function () {
            //if (AdminID == "" || AdminID == null) {
            //    $('#btnAdd').css('display', 'none');
            //}
            //公告列表初始化
            $('#NoticeList').myDatagrid({
                actionName: 'NoticeData',
                fitColumns: true,
                fit: true,
                loadFilter: function (data) {
                    for (var index = 0; index < data.rows.length; index++) {
                        var row = data.rows[index];
                        for (var name in row.item) {
                            row[name] = row.item[name];
                        }
                        delete row.item;
                    }
                    return data;
                }
            });

            $("#SelfNotice").click(function () {
                if ($(this).is(":checked")) {
                    Search();
                }
            });
        });

        //查询
        function Search() {

            var CreateDateBegin = $('#CreateDateBegin').datebox('getValue');
            var CreateDateEnd = $('#CreateDateEnd').datebox('getValue');
            var AdminID = "";
            if ($('#SelfNotice').is(':checked')) {
                AdminID = '<%=Needs.Wl.Admin.Plat.AdminPlat.Current.ID.ToString() %>';
            }
            var parm = {
                CreateDateBegin: CreateDateBegin,
                CreateDateEnd: CreateDateEnd,
                AdminID: AdminID,
            };

            //公告列表初始化
            $('#NoticeList').myDatagrid({
                actionName: 'NoticeData',
                queryParams: parm,
                fitColumns: true,
                fit: true,
                loadFilter: function (data) {
                    for (var index = 0; index < data.rows.length; index++) {
                        var row = data.rows[index];
                        for (var name in row.item) {
                            row[name] = row.item[name];
                        }
                        delete row.item;
                    }
                    return data;
                }
            });

        }

        //重置查询条件
        function Reset() {
            $('#CreateDateBegin').datebox('setValue', null);
            $('#CreateDateEnd').datebox('setValue', null);
            $('#SelfNotice').removeAttr("checked");
            Search();
        }

        //新增公告
        function ShowAddWindow() {
            var url = location.pathname.replace(/List.aspx/ig, 'Edit.aspx');
            top.$.myWindow({
                iconCls: "",
                url: url,
                noheader: false,
                title: '新增公告',
                width: 1050,
                height: 820,
                onClose: function () {
                    Search();
                }
            });
        }
        //查看公告内容详情
        function View(ID) {
            var url = location.pathname.replace(/List.aspx/ig, 'Edit.aspx')
                + '?From=View&ID=' + ID;
            top.$.myWindow({
                iconCls: "",
                url: url,
                noheader: false,
                title: '查看',
                width: 1050,
                height: 820,
                onClose: function () {
                    Search();
                }
            });
        }
        //列表框按钮加载
        function Operation(val, row, index) {
            var buttons = '<a href="javascript:void(0);" class="easyui-linkbutton l-btn l-btn-small" style="margin:3px" onclick="View(\''
                + row.ID + '\')" group >' +
                '<span class =\'l-btn-left l-btn-icon-left\'>' +
                '<span class="l-btn-text">查看</span>' +
                '<span class="l-btn-icon icon-search">&nbsp;</span>' +
                '</span>' +
                '</a>';
            return buttons;
        }
    </script>
</head>
<body class="easyui-layout">
    <div id="topBar">
        <div id="search">
            <ul>
                <li>
                    <a id="btnAdd" href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-add'" style="margin-left: 5px;" onclick="ShowAddWindow()">新增公告</a>
                </li>
            </ul>
            <ul>
                <li>
                    <span class="lbl">发布日期：</span>
                    <input class="easyui-datebox" id="CreateDateBegin" data-options="width:160,validType:'length[1,50]'" />
                    <span class="lbl">至</span>
                    <input class="easyui-datebox" id="CreateDateEnd" data-options="width:160,validType:'length[1,50]'" />
                    <input type="checkbox" name="Notice" value="0" id="SelfNotice" title="本人已发公告" class="checkbox checkboxlist" /><label for="SelfNotice" style="margin-right: 20px">本人已发公告</label>
                    <a id="btnSearch" href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-search'" style="margin-left: 10px;" onclick="Search()">查询</a>
                    <a id="btnReset" href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-redo'" onclick="Reset()">重置</a>
                </li>
            </ul>
        </div>
    </div>
    <div id="data" data-options="region:'center',border:false">
        <table id="NoticeList" title="公告发布(消息)" data-options="toolbar:'#topBar',">
            <thead>
                <tr>
                    <th data-options="field:'ID',align:'left', hidden: true" style="width: 16%;">编号</th>
                    <th data-options="field:'NoticeTitle',align:'left'" style="width: 35%;">标题</th>
                    <th data-options="field:'RoleName',align:'center'" style="width: 16%;">角色</th>
                    <th data-options="field:'CreateDate',align:'center'" style="width: 16%;">发布日期</th>
                    <%--<th data-options="field:'Status',align:'center'" style="width: 16%;">状态</th>--%>
                    <th data-options="field:'Btn',align:'center',formatter:Operation" style="width: 16%;">操作</th>
                </tr>
            </thead>
        </table>
    </div>
</body>
</html>
