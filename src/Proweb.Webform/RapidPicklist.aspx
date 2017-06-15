<%@ Page language="c#" Inherits="Experian.Qas.Prowebintegration.RapidPicklist" enableViewState="False" EnableEventValidation="false" Codebehind="RapidPicklist.aspx.cs" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<HTML>
    <HEAD>
        <title>QAS Pro</title>
        <%-- QAS Pro Web > (c) Experian > www.edq.com --%>
        <%-- Intranet > Rapid Addressing > Standard > RapidPicklist --%>
        <%-- Results frame > Picklist table > Text, score, postcode; moniker & warning behind the scenes --%>
        <meta name="GENERATOR" Content="Microsoft Visual Studio 7.0" />
        <meta name="CODE_LANGUAGE" Content="C#" />
        <meta name="vs_defaultClientScript" content="JavaScript" />
        <meta name="vs_targetSchema" content="http://schemas.microsoft.com/intellisense/ie5" />
        <link href="rapid.css" type="text/css" rel="stylesheet" />
        <script type="text/javascript">
            // Picklist content (when picklist populated directly)
<% RenderPicklistData(m_Picklist, HistoryDepth); %>

            // EVENT HANDLERS //


            function init()
            {
<% if (m_Picklist != null)
    {
        // Inform parent of new picklist properties
%>				parent.updatePicklist("<%= JavascriptEncode(SearchString) %>", <%= m_Picklist.Total %>);
<%	}
%>				// Pick up picklist HTML from the appropriate source
                document.getElementById("picklist").innerHTML = <%= DataSource %>sPicklistHTML;
            }


            // ACTIONS //


            // Call the appropriate picklist action function, defined in the asActions array
            //  bAuto indicates if they used <enter> to automatically hit the first item
            function action(iIndex, bAuto)
            {
                var sAction = (iIndex >= 0 && iIndex < <%= DataSource %>asActions.length) ? <%= DataSource %>asActions[iIndex] : "";
                if (sAction != "")
                {
                    if (bAuto && sAction.charAt(sAction.length - 2) != "(")
                    {
                        // Add an extra argument to indicate it was an automatic action using the <enter> key
                        sAction = sAction.substring(0, sAction.length - 1) + ",true)";
                    }
                    Function("parent." + sAction)();
                }
            }


            // Update self: submit a refinement on the terms passed
            function refine(sDataID, sMoniker, iDepth, sSearchText)
            {
                document.getElementById("<%= HiddenDataID.ID %>").value = sDataID;
                document.getElementById("<%= HiddenMoniker.ID %>").value = sMoniker;
                document.getElementById("<%= HiddenHistoryDepth.ID %>").value = iDepth;
                document.getElementById("<%= HiddenSearchText.ID %>").value = sSearchText;
                <%= ClientScript.GetPostBackEventReference(ActionUpdate, string.Empty) %>;
            }
        </script>
    </HEAD>
    <body onload="init();">
        <div id="picklist" class="picklist">
        </div>
        <form method="post" runat="server">
            <%-- Details of dynamic refinement - set by parent javascript --%>
            <asp:button id="ActionUpdate" runat="server" EnableViewState="False" Visible="False" onclick="ActionUpdate_Click"></asp:button>
            <input id="HiddenDataID" type="hidden" runat="server" NAME="HiddenDataID"> <input id="HiddenMoniker" type="hidden" runat="server" NAME="HiddenMoniker">
            <input id="HiddenHistoryDepth" type="hidden" runat="server" NAME="HiddenHistoryDepth">
            <input id="HiddenSearchText" type="hidden" runat="server" NAME="HiddenSearchText">
        </form>
    </body>
</HTML>
