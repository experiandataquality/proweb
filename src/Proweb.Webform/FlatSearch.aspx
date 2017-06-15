<%@ Page language="c#" Inherits="Experian.Qas.Prowebintegration.FlatSearch" enableViewState="False" Codebehind="FlatSearch.aspx.cs" %>
<%@ Import namespace="Experian.Qas.Prowebintegration" %>
<%@ Import namespace="Experian.Qas.Proweb" %>
<%@ Import Namespace="System.Collections.Generic" %>  
<!doctype HTML PUBLIC "-//W3C//DTD HTML 4.01//EN" "http://www.w3.org/TR/html4/strict.dtd">
<html>
    <head>
        <title>QAS Pro Web - Address Capture</title>
        <link href="style.css" type="text/css" rel="stylesheet" />
        <meta name="GENERATOR" Content="Microsoft Visual Studio 7.0" />
        <meta name="CODE_LANGUAGE" Content="C#" />
        <meta name="vs_defaultClientScript" content="JavaScript" />
        <meta name="vs_targetSchema" content="http://schemas.microsoft.com/intellisense/ie5" />
        <script type="text/javascript">
            /* Hyperlink clicked: select the appropriate radio button, click the Next button */
            function submitCommand(iIndex)
            {
                document.getElementsByName("<%= Constants.FIELD_MONIKER %>")[iIndex].checked = true;
                document.frmFlatSearch.<%= Constants.FIELD_MONIKER %>.selectedIndex = iIndex;
                document.frmFlatSearch.ButtonNext.click();
            }

            /* Next button clicked: ensure radio button selected, and pick up private data */
            function submitForm()
            {
                var aItems = document.getElementsByName("<%= Constants.FIELD_MONIKER %>");
                for (var i=0; i < aItems.length; i++)
                {
                    if (aItems[i].checked == true)
                    {
                        document.frmFlatSearch.<%= FIELD_MUST_REFINE %>.value = document.PrivateData.getElementsByTagName("input")[i].value;
                        return true;
                    }
                }
                alert ("Please select an address first.");
                return false;
            }
        </script>
    </head>
    <body>
        <form id="frmFlatSearch" method="post" runat="server">
            <table class="std">
                <tr>
                    <th class="banner">
                        <a href="index.htm"><h1>QAS Pro Web</h1><h2>C# .NET Samples</h2></a>
                    </th>
                </tr>
                <tr>
                    <td class="std">
                        <h1>Address Capture</h1>
                        <h3>Select Your Address</h3>
                        Select one of the following addresses that matched your selection.<br /><br />

                        <table>
                            <%
                            List<PicklistItem> items = m_Picklist.PicklistItems;
                            for (int i = 0; i < items.Count; i++)
                            {
                                %>
                                <tr>
                                    <td>
                                        <input type="radio" name="<%= Constants.FIELD_MONIKER %>" value="<%= items[i].Moniker %>" ><a
                                               class="<%= HttpUtility.HtmlEncode(items[i].IsSubsidiaryData ? "sub" : "pre") %>"
                                               href="javascript:submitCommand('<%= i %>');"><%= HttpUtility.HtmlEncode(items[i].Text) %></a>
                                </td>
                                <td width="15"></td>
                                <td>
                                    <%= HttpUtility.HtmlEncode(items[i].Postcode) %>
                                </td>
                            </tr>
                            <%
                            }%>
                        </table>
                        <br>
                        The next step is to confirm your address.
                        <br>
                        <br>
                        <%
                        // carry through values from earlier pages
                        RenderRequestString(Constants.FIELD_DATA_ID);
                        RenderRequestString(Constants.FIELD_COUNTRY_NAME);
                        RenderRequestArray(Constants.FIELD_INPUT_LINES);
                        RenderRequestString(FIELD_PROMPTSET);
                        RenderRequestString(FIELD_PICKLIST_MONIKER);
                        // hidden field to be populated by client JavaScript, picked out from form PrivateData
                        RenderHiddenField(FIELD_MUST_REFINE, null);
                        %>
                        <input type="button" value="   < Back   " id="ButtonBack" runat="server" onserverclick="ButtonBack_ServerClick">
                        <input type="submit" value="    Next >   " id="ButtonNext" runat="server" onclick="return submitForm();" onserverclick="ButtonNext_ServerClick">
                        <br>
                        <br>
                        <br>
                        <hr>
                        <br>
                        <A href="index.htm">Click here to return to the C# .NET Samples home page.</A>
                    </TD>
                </TR>
            </TABLE>
        </form>
        <form name="PrivateData">
            <%
            List<PicklistItem> items = m_Picklist.PicklistItems;
            foreach (PicklistItem item in items)
            {
                RenderHiddenField(FIELD_MUST_REFINE, MustRefine(item));
            }
            %>
        </form>
    </body>
</html>
