<%@ Page language="c#" Inherits="Experian.Qas.Prowebintegration.FlatPrompt" enableViewState="False" Codebehind="FlatPrompt.aspx.cs" %>
<%@ Import namespace="Experian.Qas.Prowebintegration" %>
<!doctype HTML PUBLIC "-//W3C//DTD HTML 4.01//EN" "http://www.w3.org/TR/html4/strict.dtd">
<html>
    <head>
        <title>QAS Pro Web - Address Capture</title>
        <link href="style.css" type="text/css" rel="stylesheet" />
        <meta content="Microsoft Visual Studio 7.0" name="GENERATOR" />
        <meta content="C#" name="CODE_LANGUAGE" />
        <meta content="JavaScript" name="vs_defaultClientScript" />
        <meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema" />
        <script type="text/javascript">
            /* Set the focus to the first input line */
            function init()
            {
                document.frmFlatPrompt.<%= Experian.Qas.Proweb.Constants.FIELD_INPUT_LINES %>[0].focus();
            }
            
            /* Ensure at least one address line has been entered */
            function validate()
            {
                var aUserInput = document.frmFlatPrompt.<%= Experian.Qas.Proweb.Constants.FIELD_INPUT_LINES %>;
                for (var i=0; i < aUserInput.length; i++)
                {
                    if (aUserInput[i].value != "")
                    {
                        return true;
                    }
                }
                document.frmFlatPrompt.<%= Experian.Qas.Proweb.Constants.FIELD_INPUT_LINES %>[0].focus();
                alert("Please enter some address details.");
                return false;
            }
        </script>
    </head>
    <body onload="init();">
        <form id="frmFlatPrompt" method="post" runat="server">
            <table class="std">
                <tr>
                    <th class="banner">
                        <a href="index.htm"><h1>QAS Pro Web</h1><h2>C# .NET Samples</h2></a>
                    </th>
                </tr>
                <tr>
                    <td class="std">
                        <h1>Address Capture</h1>
                        <h3>Enter Your Address</h3>
                        Enter the address elements requested below.<br /><br />

                        <table>
                            <%
                            string[] input = GetInputLines();
                            for (int i=0; i < m_atPromptLines.Count; i++)
                            {
                                string sValue = "";
                                if (i < input.Length)
                                {
                                    sValue = HttpUtility.HtmlEncode(input[i]);
                                }%>
                            <tr>
                                <td>
                                    <%= m_atPromptLines[i].Prompt %>
                                </td>
                                <td width="10"></td>
                                <td><input type="text" size="<%= m_atPromptLines[i].SuggestedInputLength %>" value="<%= sValue %>" name="<%= Experian.Qas.Proweb.Constants.FIELD_INPUT_LINES %>" >
                                    &nbsp;&nbsp;
                                    
                                    <% 
                                    if (m_atPromptLines[i].Example != "")
                                    {%>
                                        <i>(e.g. <%= HttpUtility.HtmlEncode(m_atPromptLines[i].Example) %>)</i>
                                    <%}%>
                                </td>
                            </tr>
                            <%
                            }%>
                            <tr>
                                <td colSpan="3">
                                    <P><br>
                                        <asp:linkbutton id="HyperlinkAlternate" runat="server" EnableViewState="False" onclick="HyperlinkAlternate_Click">If you do not know the postal/ZIP code then click here.</asp:linkbutton></P>
                                </td>
                            </tr>
                        </table>
                        <br>
                        We will now find your address from the information you have entered above.
                        <br>
                        <br>
                        <%
                            RenderRequestString(Experian.Qas.Proweb.Constants.FIELD_DATA_ID);
                            RenderRequestString(Experian.Qas.Proweb.Constants.FIELD_COUNTRY_NAME);%>
                        <input id="HiddenPromptSet" type="hidden" runat="server" />
                        <input id="ButtonBack" type="button" value="   < Back   " runat="server" onserverclick="ButtonBack_ServerClick" />
                        <input id="ButtonNext" type="submit" value="    Next >   " runat="server" onclick="return validate();" onserverclick="ButtonNext_ServerClick" />
                        <br>
                        <br>
                        <br>
                        <hr>
                        <br>
                        <a href="index.htm">Click here to return to the C# .NET Samples home page.</a>
                    </td>
                </tr>
            </table>
        </form>
    </body>
</html>
