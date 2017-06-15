<%@ Page language="c#" Inherits="Experian.Qas.Prowebintegration.RapidSearch" EnableEventValidation="false" CodeFile="RapidSearch.aspx.cs" %>
<%@ Import namespace="Experian.Qas.Prowebintegration" %>
<!doctype HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<html>
    <head>
        <title>QAS Pro</title>
        <%-- QAS Pro Web > (c) Experian > www.edq.com --%>
        <%-- Intranet > Rapid Addressing > Standard > RapidSearch --%>
        <%-- Main searching page > Toolbar, prompt, history, results iframe, status bar --%>
        <meta content="Microsoft Visual Studio 7.0" name="GENERATOR" />
        <meta content="C#" name="CODE_LANGUAGE" />
        <meta content="JavaScript" name="vs_defaultClientScript" />
        <meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema" />
        <link href="rapid.css" type="text/css" rel="stylesheet" />
        <script type="text/javascript">
            // STATE VARIABLES //


            // Remember search text at last picklist update (as currently displayed)
            var g_sSubmitText = "<%= JavascriptEncode(SearchString) %>";
            // Remember search text at last refinement submission (soon to be displayed)
            var g_sPicklistText = "<%= JavascriptEncode(SearchString) %>";
            // Remember ID of the keypress timer (so we can cancel it)
            var g_timerId = null;
            // Pre-load troublesome Busy image
            (new Image()).src = "img/wait.gif";
            // Picklist content (when initial search): picked up by results iframe
            <% RenderPicklistData(m_Picklist); %>


            // EVENT HANDLERS //

            // Set focus, etc., on page load
            function init()
            {
            
                document.formAction.setAttribute("autocomplete","<%= IsAutoComplete %>");
                document.getElementById("<%= searchText.ID %>").focus();
                
                // Prevent the default action of clicking on the 'Database' label in IE
                document.getElementById("lblCountry").onclick = function() { if ( window.event ) { window.event.returnValue = false; } };
                
<%  if ( IsSearchDynamic )
    { 
%>        
                g_timerId = setTimeout( 'dynamicSearch()', 300 );
<%  } 
%>
                
<%	if (IsInitialSearch)
    {
        // If the user is starting a new search we want the whole field selected
%>				document.getElementById("<%= searchText.ID %>").select();
<%	}
    else
    {
        // A side-effect of setting the value is to put the focus at the end:
%>				document.getElementById("<%= searchText.ID %>").value = "<%= JavascriptEncode(SearchString) %>";
<%	}
%>			}


            // Track keypresses into input text box
            function onKeyUp(e)
            {
                // If <Enter> key pressed
                if (e.keyCode == 13)
                {
                    handleSubmit();
                    //do not allow form to submit again as <Enter> by default causes form submission
                    return false;
                }
<%	// Perform dynamic searching if enabled
    if (IsSearchDynamic)
    {
%>				else
                {
                    // Clear and restart timer with a 300ms delay
                    clearTimeout(g_timerId);
                    g_timerId = setTimeout('dynamicSearch()', 300);
                    return true;
                }
<%	}
%>			}


            // Handle submit action: <Enter> key or button clicked
            function handleSubmit()
            {
<%	if (IsSearchDynamic)
    {
%>				// If there's something to do
                var sCurrent = document.getElementById("<%= searchText.ID %>").value;
                if (sCurrent != g_sSubmitText)
                {
                    // Don't wait for timer, submit immediately
                    clearTimeout(g_timerId);
                    dynamicSearch();
                }
                else if (sCurrent == g_sPicklistText)
                {
                    // Picklist is up to date: perform the action on the first picklist item
                    window.frames.results.action(0, true);
                }				
<%	}
    else
    {
%>				// Perform the initial search
                submitSearch();
<%	}
%>
                return false;
            }


            // ACTIONS //


            // Update dialog pre-submit
            function preSubmit()
            {
                document.getElementById("statusData").className = "status working";
                document.getElementById("infoStatus").innerHTML = "Working&#8230;";
            }


            // Step back to previous picklist
            function stepBack()
            {
                preSubmit();
                <%= ClientScript.GetPostBackEventReference(ButtonBack, string.Empty) %>;
            }


            // Perform initial search - updating the whole page
            function submitSearch()
            {
                // Is there something to search on?
                var sCurrent = document.getElementById("<%= searchText.ID %>").value;
                if (sCurrent != "")
                {
                    preSubmit();
                    <%= ClientScript.GetPostBackEventReference(ActionSearch, string.Empty) %>;
                }
            }


            // Perform dynamic search/refinement - updating the picklist iframe
            function dynamicSearch()
            {				
                clearTimeout(g_timerId);
                
                // Has the search text really changed since the last submission?
                var sCurrent = document.getElementById("<%= searchText.ID %>").value;
                if (sCurrent != g_sSubmitText)
                {
                    g_sSubmitText = sCurrent;
                    preSubmit();
                    window.frames.results.refine("<%= DataID %>", "<%= Moniker %>", <%= m_aHistory.Count %>, sCurrent);
                }
                
                g_timerId = setTimeout('dynamicSearch()', 300);
            }


            // Step in to the selected picklist item -> child picklist
            function <%= Commands.StepIn %>(sMoniker, sPickText, sPostcode, iScore, sWarning)
            {
                preSubmit();
                // Send details of picklist item stepped into
                document.getElementById("<%= HiddenMoniker.ID %>").value = sMoniker;
                document.getElementById("<%= HiddenPickText.ID %>").value = sPickText;
                document.getElementById("<%= HiddenPostcode.ID %>").value = sPostcode;
                document.getElementById("<%= HiddenScore.ID %>").value = iScore;
                document.getElementById("<%= HiddenWarning.ID %>").value = sWarning;
                <%= ClientScript.GetPostBackEventReference(ActionStepIn, string.Empty) %>;
            }


            // Format the selected picklist item -> final address
            function <%= Commands.Format %>(sMoniker, sWarning)
            {
                preSubmit();
                // Send details of picklist item stepped into
                document.getElementById("<%= HiddenMoniker.ID %>").value = sMoniker;
                document.getElementById("<%= HiddenWarning.ID %>").value = sWarning;
                <%= ClientScript.GetPostBackEventReference(ActionFormat, string.Empty) %>;
            }


            // Force-format the unrecognised address -> final address/warning, must click
            //  bAuto indicates if they used <enter> to automatically hit the first item
            function <%= Commands.ForceFormat %>(sMoniker, bAuto)
            {
                if (bAuto)
                {
                    displayWarning("Address not recognised &#8211; click to <a href='javascript:window.frames.results.action(0);'>override</a>");
                }
                else
                {
                    <%= Commands.Format %>(sMoniker, "<%= StepinWarnings.ForceAccept %>");
                }
            }


            // Explain (in statusbar) why they cannot proceed -> unresolvable range
            function <%= Commands.HaltRange %>()
            {
                displayWarning("Enter a value within the displayed range");
            }


            // Explain (in statusbar) why they cannot proceed -> incomplete address
            function <%= Commands.HaltIncomplete %>(iIndex)
            {
                displayWarning("Enter the premise details");
            }


            // UPDATE STATUS DISPLAY //


            // Picklist iframe has updated - it has called to tell us new picklist properties
            function updatePicklist(sPickText, iMatchCount)
            {
                // Remember the text used to produce the current picklist
                g_sPicklistText = sPickText;
                // Update statusbar
                updateMatchCount(iMatchCount)
                document.getElementById("infoStatus").innerHTML = "&nbsp;";
                document.getElementById("statusData").className = "status";
            }


            // Update the match count displayed in the statusbar
            function updateMatchCount(iMatchCount)
            {
                document.getElementById("matchCount").innerHTML = (iMatchCount < 9999) ? "Matches: " + iMatchCount : "Too many";
            }


            // Display a warning in the statusbar
            function displayWarning(sWarning)
            {
                document.getElementById("statusData").className = "status warning";
                document.getElementById("infoStatus").innerHTML = sWarning;
                document.getElementById("<%= searchText.ID %>").focus();
            }
            
            function validate( theForm )
            {
                var oSelect = document.getElementById("country");
                
                if ( oSelect.options[oSelect.options.selectedIndex].value == "" )
                {
                    alert( "Please select a datamap or country");
                }

                var sSelectedCountry = oSelect.options[oSelect.options.selectedIndex].value;		
                document.getElementById("DataID").value = sSelectedCountry;
                
                return true;

            }
            
        </script>
    </head>
    <body onload="init();">
        <form id="formAction" method="post" runat="server" onsubmit="return validate(this);">
            <table class="main">
                <tr>
                    <td class="main">
                        <div class="toolbar">
                            <span class="first margin">
                                <button id="ButtonNew" type="button" runat="server" accessKey="N" onclick="preSubmit();" onserverclick="ButtonNew_ServerClick"><EM>N</EM>ew</button>
                                &nbsp;
                                <button id="ButtonBack" type="button" runat="server" accessKey="B" onclick="preSubmit();" onserverclick="ButtonBack_ServerClick"><EM>B</EM>ack</button>
                            </span>
                            <span class="margin">
                                <asp:RadioButton id="RadioTypedown" accessKey="T" runat="server" Text="<em>T</em>ypedown" AutoPostBack="True" GroupName="GroupEngine" oncheckedchanged="RadioEngine_Changed"></asp:RadioButton>
                                <asp:RadioButton id="RadioSingleline" accessKey="S" runat="server" Text="<em>S</em>ingle Line" AutoPostBack="True" GroupName="GroupEngine" oncheckedchanged="RadioEngine_Changed"></asp:RadioButton>
                                <asp:RadioButton id="RadioKeyfinder" accessKey="K" runat="server" Text="<em>K</em>ey Search" AutoPostBack="True" GroupName="GroupEngine" oncheckedchanged="RadioEngine_Changed"></asp:RadioButton>
                            </span>
                            <span class="last margin">
                                <label for="country" id="lblCountry">Datamap:</label>
                                <asp:dropdownlist id="country" runat="server" AutoPostBack="True" accessKey="D" onchange="preSubmit();" onselectedindexchanged="country_SelectedIndexChanged"></asp:dropdownlist>
                            </span>
                        </div>
                        <div class="prompt">
                            <asp:Label id="LabelPrompt" runat="server" EnableViewState="False">Label</asp:Label><br>
                            <asp:textbox id="searchText" runat="server" onKeyPress="return onKeyUp(event);" EnableViewState="False"></asp:textbox>
                            <INPUT id="ButtonSearch" type="button" value="Select" runat="server" onclick="handleSubmit();" tabindex="-1" >
                            <%-- Details of picklist item selected - set by javascript --%>
                            <input id="HiddenMoniker" type="hidden" runat="server" NAME="HiddenMoniker"> <input id="HiddenPickText" type="hidden" runat="server" NAME="HiddenPickText">
                            <input id="HiddenPostcode" type="hidden" runat="server" NAME="HiddenPostcode"> <input id="HiddenScore" type="hidden" runat="server" NAME="HiddenScore">
                            <input id="HiddenWarning" type="hidden" runat="server" NAME="HiddenWarning">
                            <asp:button id="ActionFormat" runat="server" EnableViewState="False" Visible="False" onclick="ActionFormat_Click"></asp:button>
                            <asp:button id="ActionStepIn" runat="server" EnableViewState="False" Visible="False" onclick="ActionStepIn_Click"></asp:button>
                            <asp:button id="ActionSearch" runat="server" EnableViewState="False" Visible="False" onclick="ButtonSearch_ServerClick"></asp:button>
                            
                        </div>
                        <div class="history">
                            <% RenderHistory(); %>
                        </div>
                    </td>
                </tr>
                <tr>
                    <td class="results">
                        <iframe class="results" id="results" name="results" src="RapidPicklist.aspx"></iframe>
                        <noframes>Sorry, this integration requires support for iframes</noframes>
                    </td>
                </tr>
                <tr class="status">
                    <td class="status" id="statusData" runat="server">
                        <div class="count" id="matchCount" runat="server"></div>
                        <div class="info" id="infoStatus" runat="server">&nbsp;</div>
                    </td>
                </tr>
            </table>
            <input type=hidden id="<%=Experian.Qas.Proweb.Constants.FIELD_DATA_ID %>" name="<%=Experian.Qas.Proweb.Constants.FIELD_DATA_ID %>" value="" />
        </form>
    </body>
</html>
