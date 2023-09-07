<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ActiveworkerReport.aspx.cs" Inherits="MVC_SYSTEM.Reports.ActiveworkerReport" %>
<%@ Register Assembly="Microsoft.ReportViewer.WebForms, Version=11.0.0.0, Culture=neutral, PublicKeyToken=89845DCD8080CC91" Namespace="Microsoft.Reporting.WebForms" TagPrefix="rsweb" %>
<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
        <div>
            <asp:ScriptManager ID="ScriptManager2" runat="server"></asp:ScriptManager>
            <rsweb:ReportViewer ID="WorkerActiveReportViewer" runat="server" AsyncRendering="false"></rsweb:ReportViewer>
        </div>
    </form>
</body>
</html>
