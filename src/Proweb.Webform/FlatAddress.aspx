<%@ Page language="c#" Inherits="Experian.Qas.Prowebintegration.FlatAddress" enableViewState="False" CodeFile="FlatAddress.aspx.cs" %>
<%@ Import namespace="Experian.Qas.Prowebintegration" %>
<%@ Import namespace="Experian.Qas.Proweb" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.01//EN" "http://www.w3.org/TR/html4/strict.dtd">
<html>
    <head>
        <title>QAS Pro Web - Address Capture</title>
        <link href="style.css" type="text/css" rel="stylesheet" />
        <meta content="Microsoft Visual Studio 7.0" name="GENERATOR" />
        <meta content="C#" name="CODE_LANGUAGE" />
        <meta content="JavaScript" name="vs_defaultClientScript" />
        <meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema" />
    </head>
    <body>
        <form method="post" runat="server">
            <table class="std">
                <tr>
                    <th class="banner">
                        <a href="index.htm"><h1>QAS Pro Web</h1><h2>C# .NET Samples</h2></a>
                    </th>
                </tr>
                <tr>
                    <td class="std">
                        <h1>Address Capture</h1>
                        <h3>Confirm Your Address</h3>
                        <P><asp:Literal id="LiteralMessage" runat="server" EnableViewState="False"></asp:Literal></P>

                        <table>
<%	for (int i=0; i < m_asAddressLines.Length; i++)
    {
%>							<tr>
                                <td><%= m_asAddressLabels[i] %></td>
                                <td width="10"></td>
                                <td>
                                    <input type="text" name="<%= Constants.FIELD_ADDRESS_LINES %>" size="50" value="<%= HttpUtility.HtmlEncode(m_asAddressLines[i]) %>" />
                                </td>
                            </tr>
<%	}
%>							<tr>
                                <td>Datamap or Country</td>
                                <td width="10"></td>
                                <td>
                                    <input type="text" name="<%= Constants.FIELD_COUNTRY_NAME %>" size="50" value="<%= GetCountryName() %>" readonly class="readonly" tabindex="-1" >
                                </td>
                            </tr>
<%	if (!GetRoute().Equals(Constants.Routes.Okay))
    {
%>							<tr class="debug">
                                <td colspan="3">
                                    <span class="debug"><img src="img/debug.gif" align="left">&nbsp;Integrator information: search result was <%= GetRoute().ToString() %>
<%	if (GetErrorInfo() != null)
    {
        Response.Write("<br />&nbsp;" + GetErrorInfo().Replace("\n", "<br />"));
    }
%>
                                    </span>
                                </td>
                            </tr>
<%	}
%>						</table>
                        <br>
                        This completes the address capture process.
                        <br /><br />
<%	// carry through values from earlier pages
    RenderRequestString(Constants.FIELD_DATA_ID);
    RenderRequestArray(Constants.FIELD_INPUT_LINES);
    RenderRequestString(FIELD_PROMPTSET);
    RenderRequestString(FIELD_PICKLIST_MONIKER);
    RenderRequestString(FIELD_REFINE_MONIKER);
    RenderRequestString(Constants.FIELD_MONIKER);
    RenderHiddenField(Constants.FIELD_ROUTE, GetRoute().ToString());
%>						<input type="button" value="   < Back   " id="ButtonBack" runat="server" onserverclick="ButtonBack_ServerClick">
                        <input type="submit" value="   Accept   " id="ButtonAccept" runat="server" onserverclick="ButtonAccept_ServerClick">

                        <br /><br /><br /><hr /><br />
                        <a href="index.htm">Click here to return to the C# .NET Samples home page.</a>
                    </td>
                </tr>
            </table>
        </form>
    </body>
</html>
