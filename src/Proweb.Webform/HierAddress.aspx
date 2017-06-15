<%@ Page language="c#" Inherits="Experian.Qas.Prowebintegration.HierAddress" contentType="text/html" CodeFile="HierAddress.aspx.cs" %>
<%@ Import namespace="Experian.Qas.Prowebintegration" %>
<!doctype HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<html>
    <head>
        <title>QAS Pro Web - Rapid Addressing - Single Line</title>
        <link href="style.css" type="text/css" rel="stylesheet" />
        <meta content="Microsoft Visual Studio 7.0" name="GENERATOR" />
        <meta content="C#" name="CODE_LANGUAGE" />
        <meta content="JavaScript" name="vs_defaultClientScript" />
        <meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema" />
        <script type="text/javascript">
            // Set the focus to the first address line
            function onLoadPage()
            {
                document.frmHierAddress.<%= Experian.Qas.Proweb.Constants.FIELD_ADDRESS_LINES %>[0].focus();
            }
            
            // Fire the event for Back button clicked
            function goBack()
            {
                <%= ClientScript.GetPostBackEventReference(ButtonBack, string.Empty) %>;
            }
        </script>
    </head>
    <body onload="onLoadPage();">
        <form id="frmHierAddress" method="post" runat="server">
            <table class="std">
                <tr>
                    <th class="banner">
                        <a href="index.htm"><h1>QAS Pro Web</h1><h2>C# .NET Samples</h2></a>
                    </th>
                </tr>
                <tr>
                    <td class="std">
                        <h1>Rapid Addressing &#8211; Single Line</h1>
                        <h3>Confirm Final Address</h3>
                        <p><asp:literal id="LiteralMessage" runat="server" EnableViewState="False" Text="Please confirm your address below."></asp:literal></p>
                        <asp:placeholder id="PlaceHolderWarning" runat="server" EnableViewState="False" Visible="False">
                            <p><img class="icon" src="img/warning.gif"> <b>
                                    <asp:Literal id="LiteralWarning" runat="server" EnableViewState="False"></asp:Literal></b></p>
                        </asp:placeholder>
                        <p></p>
                        <p><asp:table id="TableAddress" runat="server" EnableViewState="False"></asp:table></p>
                        <p>This completes the address capture process.</P>
                        <asp:placeholder id="PlaceholderInfo" runat="server" EnableViewState="False" Visible="False">
                            <P class="debug"><img src="img/debug.gif" align="left"> Integrator information: search result was
                                <asp:Literal id="LiteralRoute" runat="server" EnableViewState="False"></asp:Literal>
                                <asp:Literal id="LiteralError" runat="server" EnableViewState="False"></asp:Literal></p>
                        </asp:placeholder>
                        <p><input id="ButtonNew" accessKey="N" type="button" value="   New   " runat="server" onserverclick="ButtonNew_ServerClick">
                            <input id="ButtonBack" accessKey="B" type="button" value="  Back  " runat="server" onserverclick="ButtonBack_ServerClick">
                            <asp:button id="ButtonAccept" runat="server" EnableViewState="False" Text=" Accept " onclick="ButtonAccept_Click"></asp:button></p>
                        <p>&nbsp;</p>
                        <p>
                            <hr width="100%" SIZE="1">
                        <p></p>
                        <p><a href="index.htm">Click here to return to the C# .NET Samples home page.</a></p>
                    </td>
                </tr>
            </table>
        </form>
    </body>
</html>
