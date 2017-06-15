<%@ Page language="c#" Inherits="Experian.Qas.Prowebintegration.RapidAddress" EnableEventValidation="false" Codebehind="RapidAddress.aspx.cs" %>
<%@ Import namespace="Experian.Qas.Prowebintegration" %>
<!doctype HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<html>
    <head>
        <title>QAS Pro</title>
        <%-- QAS Pro Web > (c) Experian > www.edq.com --%>
        <%-- Intranet > Rapid Addressing > Standard > RapidAddress --%>
        <%-- Final address page > Formatted address, or manual entry --%>
        <meta name="GENERATOR" Content="Microsoft Visual Studio 7.0" />
        <meta name="CODE_LANGUAGE" Content="C#" />
        <meta name="vs_defaultClientScript" content="JavaScript" />
        <meta name="vs_targetSchema" content="http://schemas.microsoft.com/intellisense/ie5" />
        <link href="rapid.css" type="text/css" rel="stylesheet" />
        <script type="text/javascript" src="Qas.js"></script>
        <script type="text/javascript" src="Qas.UI.MultiDPCtrl.js"></script>
        <script type="text/javascript">
            // EVENT HANDLERS
            
            window.onload = init;
            
            <%
                MultiDataplusControl[] ctrls = GetMultiDPControls();
                
                if ( ctrls != null && ctrls.Length > 0 )
                {
                    %>var gMultiDataplusCtrl = new Qas.UI.MultiDataplusControl();<%
                }
             %>
            // Populate engine and data, set focus
            function init()
            {
                try 
                {
                    document.getElementById("<%= Experian.Qas.Proweb.Constants.FIELD_ADDRESS_LINES %>" + "0" ).focus();
                }
                catch(e) 
                { 
                    /*  If setting the focus fails, it's probably because we arrived at this page as
                        the result of an error, so there's no text box to set the focus to. */
                }
                
                // Prevent the default action of clicking on the 'Database' label in IE
                document.getElementById("lblCountry").onclick = function() { if ( window.event ) { window.event.returnValue = false; } };
                
                <%
                    string sWrite = "";
                    
                    if ( ctrls != null && ctrls.Length > 0 )
                    {
                        foreach( MultiDataplusControl ctrl in ctrls )
                        {
                            if ( ctrl.bRender == true )
                            {
                                sWrite = "gMultiDataplusCtrl.addControls( ";
                                sWrite += "'" + ctrl.sGroup + "', ";
                                sWrite += "'" + ctrl.sFwdID + "', ";
                                sWrite += "'" + ctrl.sBackID + "', ";
                                sWrite += "'" + ctrl.sReturnID + "', ";
                                sWrite += "'" + ctrl.sIndexID + "');\n";
                                
                                Response.Write( sWrite );
                            }
                        }
                    }
                    
                    if ( m_MultiDataplusDisplayGroups != null )
                    {
                        foreach( MultiDataplusDisplayGroup grp in m_MultiDataplusDisplayGroups )
                        {
                            sWrite = "gMultiDataplusCtrl.addItems( ";
                            sWrite += "'" + grp.sGroup + "', ";
                            sWrite += grp.iLineNum.ToString() + ", ";
                            sWrite += "'" + grp.sElemID + "', ";
                            
                            for ( int i = 0; i < grp.asItems.Length; ++i )
                            {
                                if ( i != 0 )
                                {
                                    sWrite += ", ";
                                }
                                
                                sWrite += "'" + grp.asItems[i] + "'";
                            }
                            
                            sWrite += ");\n";
                            
                            Response.Write( sWrite );
                        }
                    }
                    
                 %>
            } 

            // ACTIONS

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

            // Return the formatted address back to calling window
            function finish()
            {
                // Build array of address lines and pass back to opener
                var elem = null;
                var i = 0;
                var asAddress = new Array();
                
                while( ( elem = Qas.getElement("<%= Experian.Qas.Proweb.Constants.FIELD_ADDRESS_LINES %>" + i ) ) != null )
                {
                    if ( elem.className.indexOf("multidp") != -1 )
                    {
                        asAddress.push( gMultiDataplusCtrl.getResult.call( gMultiDataplusCtrl, i ) );
                    }
                    else
                    {
                        asAddress.push( elem.value || elem.innerHTML || "" );
                    }
                    
                    ++i;
                }
                
                try
                {
                    self.opener.<%= CallBackFunction %>(asAddress);
                    self.close();
                }
                catch( e )
                {
                    var sMessage = "Unable to locate original window for pasting.\n\nWould you like to close the window?";
                    
                    var bClose = window.confirm( sMessage );
                    
                    if ( bClose )
                    {
                        self.close();
                    }
                }
                
                return false;
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
    <body>
        <form method="post" runat="server" autocomplete="off" onsubmit="return validate(this);">
            <table class="main">
                <tr>
                    <td class="main">
                        <div class="toolbar">
                            <span class="first margin">
                                <button id="ButtonNew" type="button" runat="server" accessKey="N" onserverclick="ButtonNew_ServerClick"><EM>N</EM>ew</button>
                                &nbsp;
                                <button id="ButtonBack" type="button" runat="server" accessKey="B" onserverclick="ButtonBack_ServerClick"><EM>B</EM>ack</button> 
                            </span>
                            <span class="margin">
                                <asp:RadioButton id="RadioTypedown" accessKey="T" runat="server" Text="<em>T</em>ypedown" AutoPostBack="True" GroupName="GroupEngine" oncheckedchanged="RadioEngine_Changed"></asp:RadioButton>
                                <asp:RadioButton id="RadioSingleline" runat="server" Text="<em>S</em>ingle Line" AutoPostBack="True" GroupName="GroupEngine" accessKey="S" oncheckedchanged="RadioEngine_Changed"></asp:RadioButton>
                                <asp:RadioButton id="RadioKeyfinder" accessKey="K" runat="server" Text="<em>K</em>ey Search" AutoPostBack="True" GroupName="GroupEngine" oncheckedchanged="RadioEngine_Changed"></asp:RadioButton>
                            </span>
                            <span class="last margin">
                                <label id="lblCountry" for="country">Datamap:</label>
                                <asp:dropdownlist id="country" runat="server" AutoPostBack="True" accessKey="D" onselectedindexchanged="Country_Changed"></asp:dropdownlist>
                            </span>
                        </div>
                        <div class="prompt">
                            <asp:Label id="LabelPrompt" runat="server" EnableViewState="False">Label</asp:Label>
                            <br>
                            <asp:textbox id="searchText" runat="server" onKeyUp="onKeyUp(event);" EnableViewState="False" CssClass="disabled" ReadOnly="true"></asp:textbox>
                            <input id="Accept" type="button" onclick="finish();" value="Accept">
                        </div>
                    </td>
                </tr>
                <tr>	
                    <td class="address">
                        <asp:table cssclass="address" id="TableAddress" runat="server" EnableViewState="False"></asp:table>
                        <br />
                        <asp:table id="TableMultiDPCtrl" runat="server" EnableViewState="False"></asp:table>
                        <asp:PlaceHolder id="PlaceholderInfo" runat="server" EnableViewState="False" Visible="False">
                            <P class="debug">Integrator information: search result was
                                <asp:Literal id="LiteralRoute" runat="server" EnableViewState="False"></asp:Literal>
                                <asp:Literal id="LiteralError" runat="server" EnableViewState="False"></asp:Literal></P>
                        </asp:PlaceHolder>
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
