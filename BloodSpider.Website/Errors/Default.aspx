<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="BloodSpider.Website.Errors" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Reported Errors</title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
     
        <asp:GridView ID="GridView1" runat="server" AllowPaging="True" AllowSorting="True" AutoGenerateColumns="False" DataKeyNames="bug_id" DataSourceID="SqlDataSource1" EmptyDataText="There are no data records to display.">
            <Columns>
                <asp:CommandField ShowDeleteButton="True" ShowEditButton="True" />
                <asp:BoundField DataField="bug_id" HeaderText="bug_id" ReadOnly="True" SortExpression="bug_id" />
                <asp:BoundField DataField="application_id" HeaderText="application_id" SortExpression="application_id" />
                <asp:BoundField DataField="errorcode" HeaderText="errorcode" SortExpression="errorcode" />
                <asp:BoundField DataField="message" HeaderText="message" SortExpression="message" />
                <asp:BoundField DataField="stacktrace" HeaderText="stacktrace" SortExpression="stacktrace" />
                <asp:BoundField DataField="reportedat" HeaderText="reportedat" SortExpression="reportedat" />
                <asp:BoundField DataField="application_version" HeaderText="application_version" SortExpression="application_version" />
            </Columns>
        </asp:GridView>
        <asp:SqlDataSource ID="SqlDataSource1" runat="server" ConnectionString="<%$ ConnectionStrings:BloodSpiderConnectionString %>" DeleteCommand="DELETE FROM [Bugs] WHERE [bug_id] = @bug_id" InsertCommand="INSERT INTO [Bugs] ([application_id], [errorcode], [message], [stacktrace], [reportedat], [application_version]) VALUES (@application_id, @errorcode, @message, @stacktrace, @reportedat, @application_version)" ProviderName="<%$ ConnectionStrings:BloodSpiderConnectionString.ProviderName %>" SelectCommand="SELECT [bug_id], [application_id], [errorcode], [message], [stacktrace], [reportedat], [application_version] FROM [Bugs]" UpdateCommand="UPDATE [Bugs] SET [application_id] = @application_id, [errorcode] = @errorcode, [message] = @message, [stacktrace] = @stacktrace, [reportedat] = @reportedat, [application_version] = @application_version WHERE [bug_id] = @bug_id">
            <DeleteParameters>
                <asp:Parameter Name="bug_id" Type="Int64" />
            </DeleteParameters>
            <InsertParameters>
                <asp:Parameter Name="application_id" Type="Int32" />
                <asp:Parameter Name="errorcode" Type="String" />
                <asp:Parameter Name="message" Type="String" />
                <asp:Parameter Name="stacktrace" Type="String" />
                <asp:Parameter Name="reportedat" Type="DateTime" />
                <asp:Parameter Name="application_version" Type="String" />
            </InsertParameters>
            <UpdateParameters>
                <asp:Parameter Name="application_id" Type="Int32" />
                <asp:Parameter Name="errorcode" Type="String" />
                <asp:Parameter Name="message" Type="String" />
                <asp:Parameter Name="stacktrace" Type="String" />
                <asp:Parameter Name="reportedat" Type="DateTime" />
                <asp:Parameter Name="application_version" Type="String" />
                <asp:Parameter Name="bug_id" Type="Int64" />
            </UpdateParameters>
        </asp:SqlDataSource>
    
    </div>
    </form>
</body>
</html>
