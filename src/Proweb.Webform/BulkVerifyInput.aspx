<%@ Page language="c#" CodeFile="BulkVerifyInput.aspx.cs" AutoEventWireup="false" Inherits="Experian.Qas.Prowebintegration.BulkVerifyInput" enableViewState="False" %>
<%@ Import namespace="Experian.Qas.Prowebintegration" %>
<%@ Import namespace="Experian.Qas.Proweb" %>
<%@ Import Namespace="System.Collections.Generic" %>  
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<html>
    <head>
        <title>QAS Pro Web - Rapid Addressing - Bulk Processing</title>
        <%-- QAS Pro Web > (c) Experian > www.edq.com --%>
        <%-- Intranet > BulkVerifyInput --%>
        <%-- Bulk Address input page > Enter complete addresses and selected country --%>
        <link href="style.css" type="text/css" rel="stylesheet" />
        <meta content="Microsoft Visual Studio 7.0" name="GENERATOR" />
        <meta content="C#" name="CODE_LANGUAGE" />
        <meta content="JavaScript" name="vs_defaultClientScript" />
        <meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema" />
        <script type="text/javascript">
            <%-- Set the focus to the first address line --%>
            function init()
            {
                document.VerifyInput.<%= Constants.FIELD_INPUT_LINES %>.focus();
            }

            <%-- Update the CountryName field and check the user has entered something --%>
            function validate(theForm)
            {
                if ( setCountryValue(theForm) )
                {
                    var aUserInput = theForm.<%= Constants.FIELD_INPUT_LINES %>;
                    if (aUserInput.value != "")
                    {
                        return true;
                    }
                }
                else 
                {
                    return false;
                }
                
                alert("Please enter the address.");
                return false;
            }

            <%-- Store the text of the DataID select control in the CountryName field --%>
            function setCountryValue(theForm)
            {
                try
                {
                    var iSelected = theForm.<%= Constants.FIELD_DATA_ID %>.selectedIndex;
                    theForm.<%= Constants.FIELD_COUNTRY_NAME %>.value = theForm.<%= Constants.FIELD_DATA_ID %>.options[iSelected].text;
                    
                    if ( theForm.<%= Constants.FIELD_DATA_ID %>.options[iSelected].value.length > 0 )
                    {
                        theForm.<%= Constants.FIELD_COUNTRY_NAME %>.value = theForm.<%= Constants.FIELD_DATA_ID %>.options[iSelected].text;
                    }
                    else 
                    {
                        alert("Please select a datamap or country");
                        return false;
                    }
                    
                }
                catch(x)
                {
                    alert("No datamaps available");
                    return false;
                }
                
                return true;
            }
        </script>
    </head>
    <body MS_POSITIONING="FlowLayout" onload="init();">
        <form name="VerifyInput" onsubmit="return validate(this);" action="<%= Constants.PAGE_BULK_VERIFY_SEARCH %>" method="post">
            <table class="std">
                <tr>
                    <th class="banner">
                        <a href="index.htm"><h1>QAS Pro Web</h1><h2>C# .NET Samples</h2></a>
                    </th>
                </tr>
                <tr>
                    <td class="std">
                        <h1>Rapid Addressing - Bulk Processing</h1>
                        <h3>Enter your Searches</h3>
                        <p>
                            <table>
                                <tr>
                                    <td>Addresses</td>
                                    <td width="10"></td>
                                    <td><textarea rows=8 cols=50 name="<%=  Experian.Qas.Proweb.Constants.FIELD_INPUT_LINES %>"></textarea></td>
                                </tr>
                                <tr>
                                    <td>Datamap</td>
                                    <td width="10"></td>
                                    <td>
                                        <%
                                            IList<Dataset> atDatasets = new List<Dataset>();
                                            string sError = "";

                                            try
                                            {
                                                atDatasets = AddressLookup.GetAllDatasets();
                                            }
                                            catch (Exception x)
                                            {
                                                sError = x.Message;
                                            }

                                            if (atDatasets.Count > 0)
                                            {
                                                %><select name="<%= Experian.Qas.Proweb.Constants.FIELD_DATA_ID %>"><%
                                                
                                                %> <option value="" class="heading">-- Datamaps available --</option> 
                                                <%
                                                
                                                foreach (Dataset dset in atDatasets)
                                                {
                                                    %><option value='<%=dset.ID%>'><%=dset.Name%><%
                                                }
                                                
                                                %></select><%
                                            }
                                            else
                                            {
                                                %><span class="debug">Unable to retrieve datamaps from QAS Server<br /> <%=sError%></span><%
                                            } 							    							    
                                        %> 

                                    </td>
                                </tr>
                            </table>
                        </p>
                        <p><input type="submit" value="    Next >  "> <input type=hidden name="<%= Constants.FIELD_COUNTRY_NAME %>"></p>
                        <p>&nbsp;</p>
                        <p><hr /></p>
                        <p><a href="index.htm">Click here to return to the C# .NET Samples home page.</a></p>
                    </td>
                </tr>
            </table>
        </form>
    </body>
</html>
