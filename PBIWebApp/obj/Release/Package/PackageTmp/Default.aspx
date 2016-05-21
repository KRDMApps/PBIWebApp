<%@ Page Title="Home Page" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="PBIWebApp._Default" %>
<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <!-- Sign in -->
    <div><h1 style="border-bottom:solid; border-bottom-color: silver">Power BI: Get datasets sample web applicaton</h1>
        <asp:Panel ID="signinPanel" runat="server" Visible="true">     
            <p><b class="step">Step 1</b>: Sign in to your Power BI account to link your account to this web application.</p>
            <p>
                <asp:Button ID="signInButton" runat="server" OnClick="signInButton_Click" Text="Sign in to Power BI" />
            </p>   
        </asp:Panel>   
    </div>
    <!-- Get datasets -->
    <div> 
        <asp:Panel ID="PBIPanel" runat="server" Visible="false">
            <asp:Label ID="lblError" ForeColor="Red" runat="server"></asp:Label>
            <p><b class="step">Step 2</b>: Get the data.</p>
            <table>
            <tr>
                <td><asp:Button ID="getGroupsButton" runat="server" OnClick="getGroupsButton_Click" Text="Get Groups" /></td>
                <td><asp:Button ID="clearGroupsButton" runat="server" OnClick="clearGroupsButton_Click" Text="Clear" /></td>
            </tr>
            <tr>  
                <td><asp:ListBox ID="groupResultsListBox" runat="server" Height="200px" Width="586px"></asp:ListBox></td>
            </tr>
            <tr>
                <td><asp:Button ID="getDatasetsButton" runat="server" OnClick="getDatasetsButton_Click" Text="Get Datasets" /></td>
                <td><asp:Button ID="clearDatasetsButton" runat="server" OnClick="clearDatasetsButton_Click" Text="Clear" /></td>
            </tr>
            <tr>  
                <td><asp:ListBox ID="datasetResultsListBox" runat="server" Height="200px" Width="586px"></asp:ListBox></td>
            </tr>
            <tr>
                <td><asp:Button ID="getTablesButton" runat="server" OnClick="getTablesButton_Click" Text="Get Tables" /></td>
                <td><asp:Button ID="clearTablesButton" runat="server" OnClick="clearTablesButton_Click" Text="Clear" /></td>
            </tr>
            <tr>  
                <td><asp:ListBox ID="tableResultsListBox" runat="server" Height="200px" Width="586px"></asp:ListBox></td>
            </tr>
            <tr>
                <td><asp:Button ID="clearTableRowsButton" runat="server" OnClick="clearTableRowsButton_Click" Text="Clear Table Rows" /></td>
                <td><asp:Button ID="clearAllButton" runat="server" OnClick="clearAllButton_Click" Text="Clear All" /></td>
            </tr>
            <tr>  
                <td><asp:TextBox ID="clearTablesResultTextBox" runat="server" Rows="1"  TextMode="SingleLine" Width="586px"></asp:TextBox></td>
            </tr>

            <tr>
                <td><b>Signed in as:</b></td>
            </tr>
            <tr>
                <td><asp:Label ID="userLabel" runat="server"></asp:Label></td>
            </tr>
        </table>
        </asp:Panel>
    </div>
</asp:Content>