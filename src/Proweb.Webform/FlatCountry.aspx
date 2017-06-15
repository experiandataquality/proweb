<%@ Page language="c#" Inherits="Experian.Qas.Prowebintegration.FlatCountry" enableViewState="False" CodeFile="FlatCountry.aspx.cs" %>
<%@ Import namespace="Experian.Qas.Prowebintegration" %>
<%@ Import namespace="Experian.Qas.Proweb" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.01//EN" "http://www.w3.org/TR/html4/strict.dtd">
<html>
    <head>
        <title>QAS Pro Web - Address Capture</title>
        <link href="style.css" type="text/css" rel="stylesheet" />
        <meta name="GENERATOR" content="Microsoft Visual Studio 7.0" />
        <meta name="CODE_LANGUAGE" content="C#" />
        <meta name="vs_defaultClientScript" content="JavaScript" />
        <meta name="vs_targetSchema" content="http://schemas.microsoft.com/intellisense/ie5" />
        <script type="text/javascript">
        
            /* Reload the country selection and set the focus to the country dropdown */
            function init()
            {			
                var sDataID = "<%= GetDataID() %>";
                if (sDataID != "")
                {
                    document.frmFlatCountry.<%= Constants.FIELD_DATA_ID %>.value = sDataID;
                }
                document.frmFlatCountry.<%= Constants.FIELD_DATA_ID %>.focus();
            }

            /* Store the text of the DataID select control in the CountryName field */
            function setCountryValue(theForm)  
            {      
                var iSelected = theForm.<%= Constants.FIELD_DATA_ID %>.selectedIndex;
                var sCountry = theForm.<%= Constants.FIELD_DATA_ID %>.options[iSelected].text;
                
                if ( theForm.<%= Constants.FIELD_DATA_ID %>.options[iSelected].value.length > 0 )
                {
                    theForm.<%= Constants.FIELD_COUNTRY_NAME %>.value = theForm.<%= Constants.FIELD_DATA_ID %>.options[iSelected].text;
                }
                else 
                {
                    alert( "Please select a datamap or country");
                    return false;
                }
                
                return true;
            }
            
        </script>
        <style>
            .heading
            {
                background-color:#c5e6f0;
            }
        </style>
        
    </head>
    <body onload="init();">
        <form id="frmFlatCountry" method="post" runat="server" onsubmit="return setCountryValue(this);">
            <table class="std">
                <tr>
                    <th class="banner">
                        <a href="index.htm"><h1>QAS Pro Web</h1><h2>C# .NET Samples</h2></a>
                    </th>
                </tr>
                <tr>
                    <td class="std">
                        <h1>Address Capture</h1>
                        Please select a datamap or country from the list below.<br /><br />

                        <table>
                            <tr>
                                <td>Datamap or Country</td>
                                <td width="10"></td>
                                <td>
                                    <select name="<%= Constants.FIELD_DATA_ID %>">
                                    
                                    <%                                  
                                        if (m_atDatasets != null)
                                        {
                                                %> <option value="" class="heading">-- Datamaps available --</option> 
                                                <%
                                                                                                                    
                                            foreach (Dataset dset in m_atDatasets)
                                            {   
                                                %> <option value="<%=dset.ID%>"><%=dset.Name%></option> 
                                                <%
                                            }
                                                                                                            
                                                %> <option value="" class="heading">-- Other --</option>
                                                <%                                                                   
                                        }
                                        
                                        foreach( Dataset dset in Constants.AllCountries )
                                        {
                                            bool bDuplicate = false;

                                            if (m_atDatasets != null)
                                            {
                                                foreach (Dataset serverDset in m_atDatasets)
                                                {
                                                    if (serverDset.Name == dset.Name || serverDset.ID == dset.ID)
                                                    {
                                                        bDuplicate = true;
                                                        break;
                                                    }
                                                }
                                            }
                                            
                                            if ( !bDuplicate )
                                            {
                                            %> <option value="<%=dset.ID%>"><%=dset.Name%></option> 
                                            <%
                                            }
                                        }                                                                                                                                    
                                    %>
                                            
                                    </select>
                                </td>
                            </tr>
                        </table>
                        <br>
                        The next step is to enter a few address details, from which to search for your 
                        whole address.
                        <br />
                        <br />
                        <input type="hidden" name="<%= Constants.FIELD_COUNTRY_NAME %>">
                        <input type="submit" value="    Next >   " id="ButtonNext" runat="server" onserverclick="ButtonNext_ServerClick">
                        <br>
                        <br>
                        <p class="debug">
                            
                                            &nbsp
                                                    
                            <%
                                if ( m_asError != null ) 
                                { 
                                    %><%= "<img src='img/debug.gif' align='left'>The QAS server is not available<br>" + m_asError %><% 
                                }
                            %>
                        </p>
                        <hr>
                        <br>
                        <a href="index.htm">Click here to return to the C# .NET Samples home page.</a>
                    </td>
                </tr>
            </table>
        </form>
    </body>
</html>
