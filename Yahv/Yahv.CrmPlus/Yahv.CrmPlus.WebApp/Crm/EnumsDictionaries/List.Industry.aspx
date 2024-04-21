<%@ Page Title="" Language="C#" AutoEventWireup="true" CodeBehind="List.aspx.cs" Inherits="Yahv.CrmPlus.WebApp.Crm.EnumsDictionaries.List" %>

<%
    Response.Clear();
    Server.Execute("List.aspx?Enum=FixedIndustry");
    Response.End();
%>