<%@ Import namespace="Experian.Qas.Prowebintegration" %>
<%@ Import namespace="Experian.Qas.Proweb" %>
<%@ Page language="c#" AutoEventWireup="false" Inherits="Experian.Qas.Prowebintegration.BulkVerifySearch" enableViewState="False" Codebehind="BulkVerifySearch.aspx.cs" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<html>
    <head>
        <title>QAS Pro Web - Bulk Verification</title>
        <%-- QAS Pro Web > (c) Experian > www.edq.com --%>
        <%-- Intrant > Bulk verification > BulkVerifySearch --%>
        <%-- Results page > Display original addresses along with verification level and found address --%>
        <link href="style.css" type="text/css" rel="stylesheet" />
        <meta content="Microsoft Visual Studio 7.0" name="GENERATOR" />
        <meta content="C#" name="CODE_LANGUAGE" />
        <meta content="JavaScript" name="vs_defaultClientScript" />
        <meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema" />
        <script type="text/javascript">
            <%-- Set the focus --%>
            function init()
            {
            }

            <%-- Select the chosen option button, then submit the form --%>
            function submitCommand(iIndex)
            {
                document.getElementsByName("<%= Constants.FIELD_MONIKER %>")[iIndex].checked = true;
                document.VerifyPicklist.<%= Constants.FIELD_MONIKER %>.selectedIndex = iIndex;
                document.VerifyPicklist.<%= FIELD_MUST_REFINE %>.value = g_abRefine[iIndex];
                document.VerifyPicklist.submit();
            }

            <%-- Check the user has selected something from the picklist then submit the form --%>
            function validate(theForm)
            {
                var aItems = document.getElementsByName("<%= Constants.FIELD_MONIKER %>");
                for (var iIndex=0; iIndex < aItems.length; iIndex++)
                {
                    if (aItems[iIndex].checked == true)
                    {
                        document.VerifyPicklist.<%= FIELD_MUST_REFINE %>.value = g_abRefine[iIndex];
                        return true;
                    }
                }
                alert ("Please select an address first.");
                return false;
            }
        </script>
    </head>
    <body onload="init();" MS_POSITIONING="FlowLayout">
        <table class="std">
            <tr>
                <th class="banner">
                    <a href="index.htm">
                        <h1>QAS Pro Web</h1>
                        <h2>C# .NET Samples</h2>
                    </a>
                </th>
            </tr>
            <tr>
                <td class="std">
                    <h1>Bulk Verification</h1>
                    <h3>Verification results</h3>
                    <p>
                        <% if (GetRoute().Equals(Constants.Routes.Okay)) { %>
                        <table border="1">
                            <tr>
                                <th>Input address</th>
                                <th>Verification Level</th>
                                <th>Formatted address</th>
                            </tr>
                            <%    int i;
                                for (i = 0; i < this.Count; i++)
                                {
                            %>
                            <tr>
                                <td>
                                    <%= HttpUtility.HtmlEncode(this.BulkOriginalAddress(i)) %>
                                </td>
                                <td>
                                    <%= HttpUtility.HtmlEncode(this.BulkVerifyLevel(i).ToString()) %>
                                </td>
                                <td>
                                    <table border="2">
                                        <tr>
                                            <th>Label</th>
                                            <th>Line</th>
                                            <th>Line Type</th>
                                        </tr>
                                        <%    int j;
                                            for (j = 0; j < this.FormattedAddressLength(i); j++)
                                            {%>
                                        <tr>
                                            <td>
                                                <%= HttpUtility.HtmlEncode(this.BulkSearchItems[i].Address.AddressLines[j].Label) %>
                                            </td>
                                            <td>
                                                <%= HttpUtility.HtmlEncode(this.BulkSearchItems[i].Address.AddressLines[j].Line) %>
                                            </td>
                                            <td>
                                                <%= HttpUtility.HtmlEncode(this.BulkSearchItems[i].Address.AddressLines[j].LineType.ToString()) %>
                                            </td>
                                        </tr>
                                            <%}%>
                                    </table>
                                </td>
                            </tr>
                              <%}%>
                            <%
                            if (this.ErrorCode() != 0)
                            {
                            %>
                            <tr>
                                <td>Error = <%= HttpUtility.HtmlEncode(System.Convert.ToString(this.ErrorCode())) %></td>
                                <td><%= HttpUtility.HtmlEncode(System.Convert.ToString(this.ErrorMessage()))%></td>
                                <td />
                            </tr>
                            <%
                            }
                            %>
                    </table>
                    </p>
                        <%
                       } else {
            %>
    
        <p class="debug">
        <img src="img/debug.gif" align="left">&nbsp;Integrator information: search result was <%= GetRoute() %> <br />

<%
    if (GetErrorInfo() != null) {
%>
        <%= HttpUtility.HtmlEncode(GetErrorInfo()).Replace("\n", "<br />")%> 
<%
    }
%>
    </p>
                      <%  } %>

                        <p>&nbsp;</p>
                        <p>This completes the bulk verification process.</p>
                    <br/>
                    <hr/>
                    <p>
                        <a href="index.htm">Click here to return to the C# .NET Samples home page.</a>
                    </p>
                </td>
            </tr>
        </table>
    </body>
</html>
