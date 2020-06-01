<%@ Page Language="C#" AutoEventWireup="true" Inherits="Default" EnableViewState="false"
    ValidateRequest="false" CodeBehind="Default.aspx.cs" %>
<%@ Register Assembly="DevExpress.ExpressApp.Web.v19.2, Version=19.2.6.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a" 
    Namespace="DevExpress.ExpressApp.Web.Templates" TagPrefix="cc3" %>
<%@ Register Assembly="DevExpress.ExpressApp.Web.v19.2, Version=19.2.6.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a"
    Namespace="DevExpress.ExpressApp.Web.Controls" TagPrefix="cc4" %>
<!DOCTYPE html>
<html>
<head runat="server">
    <title>Main Page</title>
    <meta http-equiv="Expires" content="0" />
    <style type="text/css">  
        .Caption {
            color: black;  
        }
        .GroupHeader .Label {
            color: black;  
        }
        .dxeBase_XafTheme
        {
            color: black;  
        }
        .dxeDisabled_XafTheme
        {
            color: black;  
        }
        .dxeDisabled_XafTheme.dxeTextBox_XafTheme,
        .dxeDisabled_XafTheme.dxeButtonEdit_XafTheme,
        .dxeDisabled_XafTheme.dxeEditArea_XafTheme {  
            color: black;  
            background-color: whitesmoke;  
        }
        /*.dxeDisabled_XafTheme.dxeHyperlink_XafTheme {
            color: black;  
        }*/


        .dxgvHeader_XafTheme table {  
            color: black;  
        }

        .dxnbLite_XafTheme .dxnb-header,
        .dxnbLite_XafTheme .dxnb-headerCollapsed {
            color: black;
            /*font-weight: bold; */
        }
    </style>  
</head>
<body class="VerticalTemplate">
    <form id="form2" runat="server">
    <cc4:ASPxProgressControl ID="ProgressControl" runat="server" />
    <div runat="server" id="Content" />
    </form>
</body>
</html>