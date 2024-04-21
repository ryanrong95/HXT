<%@ Page Title="" Language="C#" MasterPageFile="~/Uc/Works.Master" AutoEventWireup="true" CodeBehind="Edit.aspx.cs" Inherits="Yahv.CrmPlus.WebApp.Crm.Projects.Edit" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cphHead" runat="server">
    <script>
        $(function () {
            $('#Contact').contactCrmPlus({
                required: true,
                isAdd: true
            });
            $("#ClientName").clientCrmPlus({
                onChange: function (Value) {
                    ////获取当前选中的值，返回json
                    var json = $('#ClientName').clientCrmPlus('getValue');
                    $('#Contact').contactCrmPlus('setEnterpriseID', json.id);
                }
            });
            $("#OrderClient").clientCrmPlus({
                required: false
            });

            if (!jQuery.isEmptyObject(model.Entity)) {

                $('#form1').form('load', {
                    ProjectName: model.Entity.Name,
                    EstablishDate: model.Entity.EstablishDate,
                    RDDate: model.Entity.RDDate,
                    ProductDate: model.Entity.ProductDate,
                    Summary: model.Entity.Summary,
                    //ClientName:model.Entity.EndClientID
                });
                $("#ClientName").clientCrmPlus("setValue", model.Entity.EndClientID);
                $("#OrderClient").clientCrmPlus("setValue", model.Entity.AssignClientID)
                $("#Contact").contactCrmPlus('loadSetValue', { enterpriseID: model.Entity.EndClientID, contactID: model.Entity.ClientContactID });
            };
            window.grid = $('#dg').myDatagrid({
                pageSize: 50,
                toolbar: '#tb',
                nowrap: false,
                fitColumns: true,
                fit: false,
                pagination: false,
            });

        });


        //操作
        function btnformatter(value, rowData) {
            var arry = ['<span class="easyui-formatted">'];
            arry.push('<a id="btnEdit" href="#" class="easyui-linkbutton" data-options="iconCls:\'icon-yg-edit\'" onclick="showEditPage(\'' + rowData.ID + '\')">编辑</a> ');
            arry.push('<a id="btnapply" href="#" class="easyui-linkbutton" data-options="iconCls:\'icon-yg-submit\'" onclick="showApplyPage(\'' + rowData.ID + '\')">送样申请</a> ');
            arry.push('</span>');
            return arry.join('');
        }

        function statusStyle(val, row)
        {
            if (row.Status =='<%=(int)Yahv.Underly.AuditStatus.Waiting %>') {
                return "<a href='javascript:void(0);' style='color:Red'>" + "[审核中]" + val + "</a>";
            }            else {
                return val;
            }
        }
        function addPartNumber(id) {
            var id = model.Entity.ID;
            var clientid = model.Entity.AssignClientID ?? "";
            $.myDialog({
                title: "型号新增",
                url: './PartNumbers/Add.aspx?ProjectID=' + id + '&ClientID=' +clientid, onClose: function () {
                    window.grid.myDatagrid('flush');
                },
                width: "60%",
                height: "70%",
            });
            return false;
        }


        function showEditPage(id) {
            $.myDialog({
                title: "型号编辑",
                url: './PartNumbers/Edit.aspx?ID=' + id, onClose: function () {
                    window.grid.myDatagrid('flush');
                },
                width: "60%",
                height: "70%",
            });
            return false;
        }

        //送样申请
        function showApplyPage(id) {
            $.myDialog({
                title: "送样申请",
                url: './Samples/Add.aspx?ID=' + id + "&ProjectID=" + model.Entity.ID, onClose: function () {
                    window.grid.myDatagrid('flush');
                },
                width: "60%",
                height: "70%",
            });
            return false;
        }

     
    </script>


</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="cphForm" runat="server">
    <div class="easyui-layout" data-options="fit:true">
        <div data-options="region:'north'" style="height: 253px">
            <table class="liebiao" title="项目信息">
                <%--<tr>
                    <td colspan="4" class="title">
                        <p>项目信息</p>
                    </td>
                </tr>--%>
                <tr>
                    <td>客户名称：</td>
                    <td>
                        <%
                            if (string.IsNullOrWhiteSpace(this.Model.Entity?.Client?.Name))
                            {
                        %>
                        <input id="ClientName" name="ClientName" style="width: 200px" value="" readonly="readonly" />
                        <%
                            }
                            else
                            {
                        %>

                        <%=this.Model.Entity.Client.Name %>
                        <%
                            }
                        %>
                    </td>
                    <td>项目名称：</td>
                    <td>
                        <input class="easyui-textbox" id="ProjectName" name="ProjectName" style="width: 200px" data-options="required:true, validType:'length[1,50]'" /></td>
                </tr>

                <tr>
                    <td>立项日期：</td>
                    <td>
                        <input class="easyui-datebox" id="EstablishDate" name="EstablishDate" data-options="required:true" style="width: 200px" /></td>
                    <td>预计研发日期：</td>
                    <td>
                        <input class="easyui-datebox" id="RDDate" name="RDDate" data-options="required:false" style="width: 200px" /></td>
                </tr>
                <tr>
                    <td>预计量产日期</td>
                    <td>
                        <input class="easyui-datebox" id="ProductDate" name="ProductDate" data-options="required:false" style="width: 200px" />
                    </td>
                    <td>联系人：</td>
                    <td colspan="3">
                        <input id="Contact" name="Contact" style="width: 200px" />
                        <%--  <input class="easyui-combobox" id="Contact" name="Contact" style="width: 200px;" data-options="required:true, editable:false" />--%>
                        <%--<a id="btnContact" class="easyui-linkbutton" data-options="iconCls:'icon-yg-add'"></td>--%>
                </tr>
                <tr>
                    <td>下单客户</td>
                    <td colspan="3">
                        <%if (string.IsNullOrEmpty(this.Model.Entity?.OrderClient?.Name))
                            {
                        %>
                        <input id = "OrderClient" name = "OrderClient" style = "width: 200px" value = "" />
                        <%
                            }
                            else { 
                        %>
                        <%=this.Model.Entity.OrderClient.Name %>
                        <%
                        }
                        %>
                    </td>

                </tr>
                <tr>
                    <td>主要项目描述：</td>
                    <td colspan="3">
                        <input class="easyui-textbox" id="Summary" name="Summary"
                            data-options="required:false, multiline:true,validType:'length[1,150]',tipPosition:'right'" style="width: 700px; height: 100px" />
                    </td>
                </tr>
            </table>
            <div style="text-align: center; padding: 5px; display: none;">
                <asp:Button ID="btnSubmit" runat="server" Text="保存" Style="display: none;" OnClick="btnSubmit_Click" />
                <a class="easyui-linkbutton" data-options="iconCls:'icon-yg-save'" onclick="$('#<%=btnSubmit.ID%>').click();">保存</a>
                <a class="easyui-linkbutton" data-options="iconCls:'icon-yg-cancel'" onclick="Close()">取消</a>
                <input hidden="hidden" runat="server" id="hideSuccess" value="保存成功" />
            </div>
        </div>

        <div data-options="region:'center'">
            <table id="dg" class="easyui-datagrid" style="width: auto; height: auto" data-options="toolbar: '#tb'" title="产品列表">
                <thead>
                    <tr>
                        <th data-options="field:'SpnName',width:150">产品型号</th>
                        <th data-options="field:'BrandName',width:100">品牌</th>
                        <th data-options="field:'UnitProduceQuantity',width:60">单机用量</th>
                        <th data-options="field:'ProduceQuantity',width:60">项目用量</th>
                        <th data-options="field:'ProjectStatusDes',width:120,formatter:statusStyle">当前状态</th>
                        <th data-options="field:'CurrencyDes',width:70">币种</th>
                        <th data-options="field:'ExpectUnitPrice',width:80">参考单价</th>
                        <th data-options="field:'StatusDes',width:100">审核状态</th>
                        <%--  <th data-options="field:'FAE',width:120">FAE</th>--%>
                        <th data-options="field:'Btn',formatter:btnformatter,width:150">操作</th>
                    </tr>
                </thead>
            </table>
            <div id="tb" style="height: auto">
                <a href="javascript:void(0)" class="easyui-linkbutton" data-options="iconCls:'icon-add'" onclick="addPartNumber()">新增型号</a>
            </div>
        </div>
    </div>
</asp:Content>
