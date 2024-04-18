<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="SearchList.aspx.cs" Inherits="WebApp.Declaration.Notice.SearchList" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title></title>
    <uc:EasyUI runat="server" />
    <link href="../../App_Themes/xp/Style.css" rel="stylesheet" />
    <%-- <script>
        gvSettings.fatherMenu = '报关通知(XTD)';
        gvSettings.menu = '已制单';
        gvSettings.summary = '跟单员拆分报关';
    </script>--%>
    <script>
        $(function () {
            //下拉框数据初始化
            var NoticeStatus = eval('(<%=this.Model.NoticeStatus%>)');
            var VoyageType = eval('(<%=this.Model.VoyageType%>)');
            var DecHeadSpecialType = eval('(<%=this.Model.DecHeadSpecialType%>)');

            $('#NoticeStatus').combobox({
                data: NoticeStatus
            });

            var newVoyageType = [];
            newVoyageType.push({ "TypeValue": "0", "TypeText": "全部" });
            for (var i = 0; i < VoyageType.length; i++) {
                newVoyageType.push({ "TypeValue": VoyageType[i].TypeValue, "TypeText": VoyageType[i].TypeText });
            }
            $('#VoyageType').combobox({
                data: newVoyageType,
            });
            $('#VoyageType').combobox('setValue', "全部");

            //报关单特殊类型下拉框初始化
            $('#DecHeadSpecialType').combobox({
                data: DecHeadSpecialType,
                editable: false,
                valueField: 'TypeValue',
                textField: 'TypeText'
            });

            //订单列表初始化
            $('#orders').myDatagrid();
        });

        //查询
        function Search() {
            //var NoticeID = $('#NoticeID').textbox('getValue');
            var OrderID = $('#OrderID').textbox('getValue');
            var ClientName = $('#ClientName').textbox('getValue');
            var VoyageID = $('#VoyageID').textbox('getValue');
            var VoyageType = $('#VoyageType').combobox('getValues')[0];
            var ContrNo = $('#ContrNo').textbox('getValue');
            var DecHeadSpecialType = $('#DecHeadSpecialType').combobox('getValues');
            var StartDate = $('#StartDate').datebox('getValue');
            var EndDate = $('#EndDate').datebox('getValue');
            var DecHeadSpecialTypeArray = [];
            for (var i = 0; i < DecHeadSpecialType.length; i++) {
                DecHeadSpecialTypeArray.push({ "DecHeadSpecialTypeValue": DecHeadSpecialType[i] });
            }

            var parm = {
                //NoticeID: NoticeID,
                OrderID: OrderID,
                ClientName: ClientName,
                VoyageID: VoyageID,
                VoyageType: VoyageType,
                ContrNo: ContrNo,
                StartDate: StartDate,
                EndDate: EndDate,
                DecHeadSpecialType: JSON.stringify(DecHeadSpecialTypeArray),
            };
            $('#orders').myDatagrid('search', parm);
        }

        //重置查询条件
        function Reset() {
            //$('#NoticeID').textbox('setValue', null);
            $('#OrderID').textbox('setValue', null);
            $('#ClientName').textbox('setValue', null);
            $('#VoyageID').textbox('setValue', null);
            $('#VoyageType').combobox('setValue', "全部");
            $('#DecHeadSpecialType').combobox('clear');
            $('#StartDate').datebox('setValue', null);
            $('#EndDate').datebox('setValue', null);
            $('#ContrNo').textbox('setValue', null)
            Search();
        }

        //列表框按钮加载
        function Operation(val, row, index) {
            var buttons = '<a href="javascript:void(0);" class="easyui-linkbutton l-btn l-btn-small" style="margin:3px" onclick="SearchDec(\'' + row.NoticeID + '\')" group >' +
                '<span class =\'l-btn-left l-btn-icon-left\'>' +
                '<span class="l-btn-text">查看</span>' +
                '<span class="l-btn-icon icon-edit">&nbsp;</span>' +
                '</span>' +
                '</a>';
            return buttons;
        }

        function SearchDec(NoticeID) {
            var url = location.pathname.replace(/SearchList.aspx/ig, 'SearchListDetail.aspx?NoticeID=' + NoticeID);
            top.$.myWindow({
                iconCls: "",
                url: url,
                noheader: false,
                title: '报关单详情',
                width: '70%',
                height: '70%'
            });
        }

        function InspQuarNameShow(val, row, index) {
            var result = '';
            if (row.IsCharterBus == true) {
                result += '包车/';
            } else {
                //result += '-/';
            }

            if (row.IsHighValue == true) {
                result += '高价值/';
            } else {
                //result += '-/';
            }

            if (row.IsInspection == true) {
                result += '商检/';
            } else {
                //result += '-/';
            }

            if (row.IsQuarantine == true) {
                result += '检疫/';
            } else {
                //result += '-/';
            }

            if (row.IsCCC == true) {
                result += '3C/';
            } else {
                //result += '-/';
            }

            if (row.IsOrigin == true) {
                result += '加征/';
            } else {
                //result += '-';
            }

            if (row.IsSenOrigin == true) {
                result += '敏感产地';
            } else {
                //result += '-';
            }

            return result;
        }
    </script>
</head>
<body class="easyui-layout">
    <div id="topBar">
        <div id="search">
            <ul style="margin-left: 12px;">
                <li>
                    <span class="lbl">订单编号: </span>
                    <input class="easyui-textbox" id="OrderID" />
                    <span class="lbl">客户名称: </span>
                    <input class="easyui-textbox" id="ClientName" />
                    <span class="lbl">报关单特殊类型: </span>
                    <input class="easyui-combobox" id="DecHeadSpecialType" name="DecHeadSpecialType" data-options="multiple:true,panelHeight:'auto'," />

                </li>
            </ul>
            <ul style="margin-left: 12px;">
                <li>
                    <span class="lbl">制单日期:</span>
                    <input class="easyui-datebox" id="StartDate" data-options="editable:false" />
                    <span class="lbl" style="margin-left: 46px">至:</span>
                    <input class="easyui-datebox" id="EndDate" data-options="editable:false" />
                    <span class="lbl" style="margin-left: 59px">合同号: </span>
                    <input class="easyui-textbox" id="ContrNo" style="margin-left: 20px" />
                </li>
            </ul>
            <ul>
                <li>
                    <span class="lbl" style="margin-left: 10px;">运输批次号: </span>
                    <input class="easyui-textbox" id="VoyageID" />
                    <span class="lbl" style="margin-left: 10px;">运输类型: </span>
                    <input class="easyui-combobox" id="VoyageType" name="VoyageType" data-options="valueField:'TypeValue',textField:'TypeText',editable:false," />
                    <a id="btnSearch" href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-search'" onclick="Search()">查询</a>
                    <a id="btnReset" href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-redo'" onclick="Reset()">重置</a>
                </li>
            </ul>
        </div>
    </div>

    <div id="data" data-options="region:'center',border:false">
        <table id="orders" title="报关通知已制单列表" data-options="fitColumns:true,fit:true,nowrap:false,toolbar:'#topBar'">
            <thead>
                <tr>
                    <th data-options="field:'OrderID',align:'left'" style="width: 8%">订单编号</th>
                    <th data-options="field:'ClientName',align:'left'" style="width: 20%">客户名称</th>
                    <th data-options="field:'ContrNo',align:'left'" style="width: 8%">合同号</th>
                    <th data-options="field:'InspQuarName',align:'center',formatter:InspQuarNameShow" style="width: 8%">报关单特殊类型</th>
                    <th data-options="field:'VoyageID',align:'left'" style="width: 6%">运输批次号</th>
                    <th data-options="field:'VoyageType',align:'left'" style="width: 6%">运输类型</th>
                    <th data-options="field:'PackNo',align:'left'" style="width: 4%">件数</th>
                    <th data-options="field:'TotalDeclarePrice',align:'left'" style="width: 6%">报关总价</th>
                    <th data-options="field:'Currency',align:'center'" style="width: 5%">币种</th>
                    <th data-options="field:'TotalQty',align:'left'" style="width: 5%">总数量</th>
                    <th data-options="field:'TotalListQty',align:'center'" style="width: 5%">型号数量</th>
                    <th data-options="field:'DeclarantName',align:'center'" style="width: 6%">跟单员</th>
                    <th data-options="field:'CreateDate',align:'center'" style="width: 6%">创建日期</th>
                    <th data-options="field:'Btn',align:'center',formatter:Operation" style="width: 6%">操作</th>
                </tr>
            </thead>
        </table>
    </div>
</body>
</html>
