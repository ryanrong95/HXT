<%@ Page Title="" Language="C#" MasterPageFile="~/Uc/Works.Master" AutoEventWireup="true" CodeBehind="List.aspx.cs" Inherits="Yahv.CrmPlus.WebApp.Crm.Client.Approvals.PublicSeas.List" %>

<%@ Import Namespace="Yahv.Underly" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cphHead" runat="server">
    <script>
        $(function () {

            $("#clientType").fixedCombobx({
                type: 'ClientType',
                isAll: true
            });
            $("#Sale").combobox({
                data: model.Sales,
                valueField: 'value',
                textField: 'text',
                panelHeight: 'auto', //自适应
                multiple: false,
                limitToList: true,
                collapsible: true,
            });

            var getQuery = function () {
                var params = {
                    action: 'data',
                    r_name: $.trim($('#r_name').textbox("getValue")), //认领人
                    s_name: $.trim($('#s_name').textbox("getValue")),
                    clientType: $("#clientType").fixedCombobx('getValue')
                };
                return params;
            };
            //设置表格
            window.grid = $("#dg").myDatagrid({
                toolbar: '#tb',
                pagination: true,
                singleSelect: false,
                method: 'get',
                queryParams: getQuery(),
                fit: true,
                rownumbers: true,
                nowrap: false,
            });
            //搜索
            $("#btnSearch").click(function () {
                grid.myDatagrid('search', getQuery());
            })
            //清空
            $("#btnClear").click(function () {
                location.reload();
                return false;
            });
        });

        function btnformatter(value, rowData) {
            var arry = ['<span class="easyui-formatted">'];
            arry.push('<a id="btnApprove" href="#" class="easyui-linkbutton" data-options="iconCls:\'icon-yg-approvalPass\'" onclick="approval(true,\'' + rowData.ID + '\',\'' + rowData.ConductType + '\')">审批通过</a> ');
            arry.push('<a id="btnApprove" href="#" class="easyui-linkbutton" data-options="iconCls:\'icon-yg-approvalNopass\'" onclick="approval(false,\'' + rowData.ID + '\',\'' + rowData.ConductType + '\')">审批不通过</a> ');
            arry.push('</span>');
            return arry.join('');
        }

        function approval(result, id, conductType) {
            $.messager.confirm("操作提示", "您确定要审批通过吗", function (r) {
                if (r) {
                    $.post('?action=Approve', { id: id, result: result, conductType: conductType }, function (data) {
                        var result = JSON.parse(data)
                        if (result.success) {
                            top.$.timeouts.alert({
                                position: "TC",
                                msg: "操作成功",
                                type: "success"
                            });
                            grid.myDatagrid('flush');
                        }
                        else {
                            top.$.timeouts.alert({
                                position: "TC",
                                msg: "操作失败" + result.message,
                                type: "error"
                            });
                        }
                    });
                }

            })

        }

        function claim(id) {
            $.myDialog({
                title: "认领",
                url: 'Claim.aspx?ID=' + id,
                width: "60%",
                height: "80%",
            });

        };

        //function edit(id) {
        // $.myDialog({
        //     title: "认领",
        //     url: 'Edit.aspx?ID=' + id,
        //     width: "60%",
        //     height: "80%",
        // });

        //};
        function detail(id) {
            $.myWindow({
                title: '综合信息',
                url: '../Index.aspx?id=' + id + "&Source=fromPublic",
                width: '80%',
                height: '80%',
                onClose: function () {
                    window.grid.myDatagrid('flush');
                }
            });
        }
        //function auditedClaim(id) {
        //    $.myDialog({
        //        title: "认领",
        //        url: 'Edit.aspx?&enterpriseid=' + id, onClose: function () {
        //            window.grid.myDatagrid('flush');
        //        },
        //        width: "30%",
        //        height: "40%",
        //    });

        //};
        function showAddPage() {
            $.myDialog({
                title: "新增",
                url: 'Add.aspx', onClose: function () {
                    window.grid.myDatagrid('flush');
                },
                width: "450px",
                height: "200px",
            });

        }
        function nameformatter(value, rowData) {
            var result = "";
            result += '<span class="vip' + rowData.Vip + '"></span>';
            result += '<span class="level' + rowData.Grade + '"></span>';
            if (rowData.IsMajor) {
                result += '<span class="star"></span>';
            }
            result += rowData.Name;
            return result;
        }
        function isformatter(value, rowData) {
            return value ? "是" : "否";
        }
    </script>
    <script type="text/javascript">
        function onSelect1(sd) {
            $('#s_enddate').datebox('calendar').calendar({
                validator: function (date) {
                    return sd <= date;
                }
            });
        }
        function onSelect2(ed) {
            $('#s_startdate').datebox('calendar').calendar({
                validator: function (date) {
                    return ed >= date;
                }
            });
        }
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphForm" runat="server">
    <div id="tb">
        <div>
            <table class="liebiao-compact">
                <tr>
                    <td style="width: 100px;">客户名称</td>
                    <td>
                        <input id="s_name" data-options="prompt:'客户名称',validType:'length[1,75]'" class="easyui-textbox" /></td>
                    <td>客户类型</td>
                    <td>
                        <input id="clientType" name="clientType" />
                        <%-- <select id="clientType" name="clientType" class="easyui-combobox" data-options="editable:false,panelheight:'auto'"></select>--%>
                    </td>

                    <td style="width: 100px;">认领人</td>
                    <td>
                        <input id="r_name" data-options="prompt:'认领人',validType:'length[1,75]'" class="easyui-textbox" />

                        <%--<select id="Sale" name="Sale" class="easyui-combobox" data-options="editable:false,panelheight:'auto'"></select>--%>
                    </td>
                    <td style="width: 100px;">认领时间</td>

                    <td>
                        <input id="s_startdate" class="easyui-datebox" data-options="editable:false,prompt:'开始时间',onSelect:onSelect1" />
                        -
                        <input id="s_enddate" class="easyui-datebox" data-options="editable:false,prompt:'结束时间',onSelect:onSelect2" />
                    </td>
                    <td colspan="6">
                        <a id="btnSearch" class="easyui-linkbutton" data-options="iconCls:'icon-yg-search'">搜索</a>
                        <a id="btnClear" class="easyui-linkbutton" data-options="iconCls:'icon-yg-clear'">清空</a>
                        <em class="toolLine"></em>
                    </td>
                </tr>
                <tr>
                </tr>
            </table>
        </div>
    </div>
    <table id="dg" style="width: 100%">
        <thead>
            <tr>
                <%--<th data-options="field:'Ck',checkbox:true"></th>--%>
                <th data-options="field:'Name',formatter:nameformatter,width:280">客户名称</th>
                <th data-options="field:'ClientType',width:120">客户类型</th>
                <th data-options="field:'StatusDes',width:100">审核状态</th>
                <th data-options="field:'District',width:120">国别地区</th>
                <th data-options="field:'Industry',width:120">所属行业</th>
                 <th data-options="field: 'Company',width:120">合作公司</th>
                <%-- <th data-options="field: 'IsMajor',width:120">是否重点客户</th>--%>

                <%--<th data-options="field: 'ClientGrade',width:80">客户等级</th>--%>
                <th data-options="field:'ConductTypeDes',width:70">业务类型</th>
                <th data-options="field:'IsSpecial',formatter:isformatter,width:70">是否特殊</th>
                <th data-options="field:'IsInternational',formatter:isformatter,width:70">是否国际</th>
                <th data-options="field:'Claimant',width:80">认领人</th>
                <th data-options="field:'ClaimTime',width:140">认领时间</th>
                <th data-options="field:'CreateDate',width:140">注册时间</th>
                <th data-options="field:'Btn',formatter:btnformatter,width:200">操作</th>
            </tr>
        </thead>
    </table>

</asp:Content>
