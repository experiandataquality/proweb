<%@ Page language="c#" Inherits="Experian.Qas.Prowebintegration.HierSearch" contentType="text/html" EnableEventValidation="false" Codebehind="HierSearch.aspx.cs" %>
<%@ Import namespace="Experian.Qas.Proweb" %>
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

        <style type="text/css">
            table.pickle {
                BORDER-COLLAPSE: collapse
            }
            td.pickle {
                PADDING-RIGHT: 0px; PADDING-LEFT: 0px; PADDING-BOTTOM: 0px; PADDING-TOP: 0px
            }
        </style>

        <script type="text/javascript">

            // Set the focus to the prompt/refinement text box
            function onLoadPage()
            {
                document.forms['frmHierSearch'].TextBoxRefine.focus();
                document.forms['frmHierSearch'].TextBoxRefine.select();
            }

            // Fire the event for Back button clicked
            function goBack()
            {
                <%= ClientScript.GetPostBackEventReference(ButtonBack, string.Empty) %>;
            }

            // Pick up item's details from the hidden form and submit Format command
            function <%= Commands.Format.ToString() %>(iIndex)
            {
                document.forms['frmHierSearch'].<%= HiddenMoniker.ID %>.value = document.getElementsByName("<%= FIELD_MONIKER %>")[iIndex].value;
                document.forms['frmHierSearch'].<%= HiddenWarning.ID %>.value = document.getElementsByName("<%= FIELD_WARNING %>")[iIndex].value;
                <%= ClientScript.GetPostBackEventReference(ButtonFormat, string.Empty) %>;
            }

            // Pick up item's details from the hidden form and submit StepIn command
            function <%= Commands.StepIn.ToString() %>(iIndex)
            {
                document.forms['frmHierSearch'].<%= HiddenMoniker.ID %>.value = document.getElementsByName("<%= FIELD_MONIKER %>")[iIndex].value;
                document.forms['frmHierSearch'].<%= HiddenText.ID %>.value = document.getElementsByName("<%= FIELD_PICKTEXT %>")[iIndex].value;
                document.forms['frmHierSearch'].<%= HiddenPostcode.ID %>.value = document.getElementsByName("<%= FIELD_POSTCODE %>")[iIndex].value;
                document.forms['frmHierSearch'].<%= HiddenScore.ID %>.value = document.getElementsByName("<%= FIELD_SCORE %>")[iIndex].value;
                document.forms['frmHierSearch'].<%= HiddenWarning.ID %>.value = document.getElementsByName("<%= FIELD_WARNING %>")[iIndex].value;
                <%= ClientScript.GetPostBackEventReference(ButtonStepIn, string.Empty) %>;
            }

            // Explain why incomplete address item cannot be stepped into
            function <%= Commands.HaltIncomplete.ToString() %>(iIndex)
            {
                document.forms['frmHierSearch'].TextBoxRefine.focus();
                // display some on-screen warning
                alert ("Please enter the premise details.");
            }
            
            // Explain why unresolvable range item cannot be stepped into
            function <%= Commands.HaltRange.ToString() %>(iIndex)
            {
                document.forms['frmHierSearch'].TextBoxRefine.focus();
                // display some on-screen warning
                alert ("Please enter a value within the range.");
            }

        </script>

        <style type="text/css">
            em { text-decoration: underline; font-style: normal; }
        </style>

    </head>

    <body onload="onLoadPage();">

        <form id="frmHierSearch" name="frmHierSearch" method="post" runat="server" defaultbutton="ButtonRefine">
            <table class="std">
                <tr>
                    <th class="banner">
                        <a href="index.htm"><h1>QAS Pro Web</h1><h2>C# .NET Samples</h2></a>
                    </th>
                </tr>
                <tr>
                    <td class="std">
                        <h1>Rapid Addressing &#8211; Single Line</h1>
                        <h3>Select from Address Picklist</h3>
                        <p>Enter text to refine your search, or select one of the addresses below.</p>
                        
                            <table>
                                <tr>
                                    
                                    <td><button id="ButtonNew" accesskey="N" runat="server" onserverclick="ButtonNew_ServerClick"><em>N</em>ew</button></td>
                                    <td><button id="ButtonBack" accesskey="B" runat="server" onserverclick="ButtonBack_ServerClick"><em>B</em>ack</button></td>
                                    <td>&nbsp;<label id="LabelPrompt" for="TextBoxRefine" runat="server">Prompt</label>:</td>
                                    <td><asp:textbox id="TextBoxRefine" runat="server" EnableViewState="False"></asp:textbox></td>
                                    <td>&nbsp;<asp:button id="ButtonRefine" runat="server" EnableViewState="False" Text="Search" onclick="ButtonRefine_Click"></asp:button></TD>
                                </tr>
                            </table>
                        
                        
                        <table class="pickle">

                            <asp:placeholder id="PlaceHolderWarning" runat="server" EnableViewState="False" Visible="False">
                                <tr>
                                    <td nowrap=nowrap colspan="5"><img class="icon" src="img/warning.gif">
                                        <b><asp:Literal id="LiteralWarning" runat="server" EnableViewState="False"></asp:Literal></b><br />&nbsp;
                                    </td>
                                </tr>
                            </asp:placeholder>
                            <% RenderHistory(); %>
                            <tr>
                                <td colspan="5">
                                    <hr />
                                </td>
                            </tr>
                            <% RenderPicklist(); %>

                        </table>
                        
                        <p></p>
                        <p>
                            <input id="HiddenMoniker" type="hidden" runat="server" /> 
                            <input id="HiddenText" type="hidden" runat="server" />
                            <input id="HiddenPostcode" type="hidden" runat="server" /> 
                            <input id="HiddenScore" type="hidden" runat="server" />
                            <input id="HiddenWarning" type="hidden" runat="server" />

                            <asp:button id="ButtonFormat" runat="server" EnableViewState="False" Text="Format" Visible="False" onclick="ButtonFormat_Click"></asp:button>
                            <asp:button id="ButtonStepIn" runat="server" EnableViewState="False" Text="StepIn" Visible="False" onclick="ButtonStepIn_Click"></asp:button>
                        </p>
                        <p></p>
                        <hr width="100%" size="1" />
                        <p></p>
                        <p><A href="index.htm">Click here to return to the C# .NET Samples home page.</A></p>
                    </td>
                </tr>
            </table>
        </form>

        <form name="PrivateData">
            <% RenderPrivateData(); %>
        </form>

    </body>

</html>
