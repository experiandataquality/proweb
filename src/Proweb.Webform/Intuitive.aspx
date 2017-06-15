<%@ Page Language="C#" AutoEventWireup="true" Inherits="Experian.Qas.Prowebintegration.Intuitive" Codebehind="Intuitive.aspx.cs" %>

<%@ Import Namespace="Experian.Qas.Prowebintegration" %>
<%@ Import Namespace="Experian.Qas.Proweb" %>
<!doctype HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd" >
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>QAS Pro Web - Intuitive Search</title>
    <link href="style.css" type="text/css" rel="stylesheet" />
    <meta content="Microsoft Visual Studio 7.0" name="GENERATOR" />
    <meta content="C#" name="CODE_LANGUAGE" />
    <meta content="JavaScript" name="vs_defaultClientScript" />
    <meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema" />

    <script language="javascript" type="text/javascript">
            var gPostBackTimer = null;
            var gLastSubmit = 0;
            
            // If the Enter key is pressed in the address list box, this is treated as if the 
            // address had been clicked
            function AddressKeypress(selectObj, e)
            {
                if (!e) {
                    e = window.event;
                }
                key = (e.keyCode) ? e.keyCode : e.which;
                if(key==13 || key == 32) {
                    AddressClick(selectObj);
                }    
            }

            // If an address in the address list box is clicked select the address 
            //& enable the "Format" button
            function AddressClick(selectObj) 
            {
                var textBox = document.getElementById("InputBox");
                var listBox = document.getElementById("AddressList");
                var button = document.getElementById("SelectButton");
                var promptText = "Continue typing";

                if (textBox != null && listBox != null)
                {
                    for (i = 0; i < listBox.length; i++)
                    {
                        if (listBox[i].value == selectObj.value)
                        {
                            textBox.value = listBox[i].text;

                            if (textBox.value != promptText) 
                            {
                                button.disabled = false;
                            }
                            
                            break; 
                        }
                    }
                }
                __doPostBack('<%=AddressList.UniqueID %>', "onclick");
            }
            
            // If an address in the address list box double clicked, format the address
            function AddressDoubleClick() 
            {
                __doPostBack('<%=InputBox.UniqueID %>', "onenter");
            }

            // Handle keypresses in the input address box    
            function InputAddressKeyPressed(e)
            {
                if (!e) {
                    e = window.event;
                }
            
                key = (e.keyCode) ? e.keyCode : e.which;
            
                // 'Enter' key behaves in same way as clicking the Select button
                if (key == 13) {
                    __doPostBack('<%=InputBox.UniqueID %>', "onenter");
                }
                // Down arrow key gives focus to the address list box
                else if (key == 40)
                {
                    __doPostBack('<%=InputBox.UniqueID %>', "ondown");
                }
                // If a non-arrow key is pressed, the input address has changed
                else if (key <= 36 || key >= 41 && 
                            key != 16 )
                {
                    InputAddressChanged();
                }
            }
        
            // Post back the input address, causing an Intuitive Search to take place
            function InputAddressPostback()
            {
                gLastSubmit = new Date().getTime();
                __doPostBack('<%=InputBox.UniqueID %>', "onkeyup");
            }
        
            // If the input address has changed, we submit an Intuitive Search to the server
            // If several keys are pressed in succession, we impose a delay to avoid overloading the server
            // with unnecessary searches.
            //
            // We only perform a search if:
            // 1. No keys have been pressed for at least half a second, OR:
            // 2. It is at least one second since a search was last submitted. 
            function InputAddressChanged() 
            {
                clearTimeout(gPostBackTimer);
                theTime = new Date().getTime() - gLastSubmit;
            
                if (theTime > 1000) {
                    InputAddressPostback();
                }
                else if (theTime > 500) {
                    gPostBackTimer=setTimeout("InputAddressPostback()", 1000 - theTime);
                } else {
                    gPostBackTimer=setTimeout("InputAddressPostback()", 500);
                }
            }
    </script>

    <style type="text/css">
        .heading
        {
            background-color: #c5e6f0;
        }
        </style>
</head>
<body>
    <form id="frmIntuitive" runat="server">
    <asp:ScriptManager ID="smIntuitive" runat="server"></asp:ScriptManager>
    <table class="std">
        <tr>
            <th class="banner">
                <h1><a href="index.htm">QAS Pro Web</a></h1>
                <h2><a href="index.htm">C# .NET Samples</a></h2>
            </th>
        </tr>
        <tr>
            <td class="std"><h1>Intuitive Search</h1>
                <p> Please select a datamap or country from the list below, and begin to enter your
                    address search.</p>
                <table>
                    <tr>
                        <td><label for="CountryList">Datamap or Country</label></td>
                        <td style="width:10px"></td>
                        <td><asp:DropDownList ID="CountryList" runat="server" OnSelectedIndexChanged="ClearButton_Click" OnInit="PopulateDatamaps" AutoPostBack="True"></asp:DropDownList></td>
                    </tr>
                    <tr>
                        <td><label for="Prompt"></label></td>
                        <td style="width: 10"></td>
                        <td><br /><asp:UpdatePanel ID="PromptUpdatePanel" runat="server" RenderMode="Inline">
                            <ContentTemplate>
                                <asp:Label ID="Prompt" runat="server"></asp:Label>
                            </ContentTemplate>
                            <Triggers>
                                <asp:AsyncPostBackTrigger ControlID="InputBox" />
                            </Triggers>
                        </asp:UpdatePanel></td>
                    </tr>
                    <tr>
                        <td><label for="InputBox">Search</label></td>
                        <td></td>
                        <td>
                            <asp:TextBox ID="InputBox" runat="server" Style="width: 40em" AutoCompleteType="Disabled" autocomplete="off"></asp:TextBox>&nbsp;
                            <asp:Button ID="SelectButton" runat="server" Text="Format" 
                                OnClick="SelectButton_Click" PostBackUrl="~/Intuitive.aspx" Enabled="False"></asp:Button>&nbsp;
                            <asp:Button ID="ClearButton" runat="server" Text="Clear" OnClick="ClearButton_Click" PostBackUrl="~/Intuitive.aspx"></asp:Button>&nbsp;
                        </td>
                    </tr>
                    <tr>
                        <td><br /></td>
                        <td></td>
                        <td></td>
                    </tr>
                    <tr>
                        <td></td>
                        <td></td>
                        <td></td>
                    </tr>
                    <tr><td></td>
                        <td></td>
                        <td><asp:UpdatePanel ID="UpdatePanelPicklist" runat="server" RenderMode="Inline">
                                <ContentTemplate>
                                    <label id="AddressListLabel" for="AddressList" runat="server"></label>
                                    <asp:ListBox ID="AddressList" runat="server" Visible="False" Style="width: 40em"></asp:ListBox>
                                    <br />
                                    <br />
                                    <label id="ResultBoxLabel" for="ResultBox" runat="server" style="font-weight: bold">Formatted Result</label><br />
                                    <asp:TextBox ID="ResultBox" runat="server" Rows="7" TextMode="MultiLine" Style="width: 300px"></asp:TextBox>
                                </ContentTemplate>
                                <Triggers>
                                    <asp:AsyncPostBackTrigger ControlID="InputBox" />
                                </Triggers>
                            </asp:UpdatePanel></td>
                    </tr>
                </table>
                <asp:UpdatePanel ID="UpdatePanelLogging" runat="server" RenderMode="Inline" UpdateMode="Conditional">
                    <ContentTemplate>
                        <p class="debug">
                            <% if (m_asError != null) { %>
                            <%="<img src='img/debug.gif' align='left'>" + m_asError %>
                            <% } else { %>
                            <%="&nbsp" %>
                            <% } %></p>
                    </ContentTemplate>
                </asp:UpdatePanel>
                <p></p>
                <hr style="width: 100%; size: 1" />
                <p></p>
                <p><a href="index.htm">Click here to return to the C# .NET Samples home page.</a></p>
            </td>
        </tr>
    </table>
    </form>
</body>
</html>
