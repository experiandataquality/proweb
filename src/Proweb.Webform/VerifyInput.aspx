<%@ Page language="c#" Inherits="Experian.Qas.Prowebintegration.VerifyInput" enableViewState="False" Codebehind="VerifyInput.aspx.cs" %>
<%@ Import namespace="Experian.Qas.Prowebintegration" %>
<%@ Import namespace="Experian.Qas.Proweb" %>
<!doctype HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<html>
    <head>
        <title>QAS Pro Web - Address Verification</title>
        <%-- QAS Pro Web > (c) Experian > www.edq.com --%>
        <%-- Web > Verification > VerifyInput --%>
        <%-- Address input page > Enter complete address and selected country --%>
        <link href="style.css" type="text/css" rel="stylesheet" />
        <script type="text/javascript">
            <%-- Set the focus to the first address line --%>
            function init()
            {
                document.frmVerifyInput.<%= Constants.FIELD_INPUT_LINES %>[0].focus();
            }
                        

            <%-- Update the CountryName field and check the user has entered something --%>
            function validate(theForm)
            {

                var aUserInput = theForm.<%= Constants.FIELD_INPUT_LINES %>;
                for (var i=0; i < aUserInput.length; i++)
                {
                    if (aUserInput[i].value != "")
                    {
                        return setCountryValue(theForm);
                    }
                }
                
                alert("Please enter the address.");
                return false;
                    
            }

            <%-- Store the text of the DataID select control in the CountryName field --%>
            function setCountryValue(theForm)
            {
                var iSelected = theForm.<%= Constants.FIELD_DATA_ID %>.selectedIndex;
                var sCountry = theForm.<%= Constants.FIELD_DATA_ID %>.options[iSelected].value;
                
                if ( theForm.<%= Constants.FIELD_DATA_ID %>.options[iSelected].value.length > 0 )
                {
                    theForm.<%= Constants.FIELD_COUNTRY_NAME %>.value = theForm.<%= Constants.FIELD_DATA_ID %>.options[iSelected].text;
                }
                else 
                {
                    alert("Please select a datamap or country");
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
        <form name="frmVerifyInput" onsubmit="return validate(this);" action="<%= Constants.PAGE_VERIFY_SEARCH %>" method="post">
            <table class="std">
                <tr>
                    <th class="banner">
                        <a href="index.htm"><h1>QAS Pro Web</h1><h2>C# .NET Samples</h2></a>
                    </th>
                </tr>
                <tr>
                    <td class="std">
                        <h1>Address Verification</h1>
                        <h3>Enter &amp; Verify Address</h3>
                        <p>
                            <table>
                                <tr>
                                    <td>Address Line 1</td>
                                    <td width="10"></td>
                                    <td><input type="text" size="50" name="<%= Constants.FIELD_INPUT_LINES %>" /></td>
                                </tr>
                                <tr>
                                    <td>Address Line 2</td>
                                    <td width="10"></td>
                                    <td><input type="text" size="50" name="<%= Constants.FIELD_INPUT_LINES %>" /></td>
                                </tr>
                                <tr>
                                    <td>Address Line 3</td>
                                    <td width="10"></td>
                                    <td><input type="text" size="50" name="<%= Constants.FIELD_INPUT_LINES %>" /></td>
                                </tr>
                                <tr>
                                    <td>City</td>
                                    <td width="10"></td>
                                    <td><input type="text" size="50" name="<%= Constants.FIELD_INPUT_LINES %>" /></td>
                                </tr>
                                <tr>
                                    <td>State or Province</td>
                                    <td width="10"></td>
                                    <td><input type="text" size="50" name="<%= Constants.FIELD_INPUT_LINES %>" /></td>
                                </tr>
                                <tr>
                                    <td>ZIP or Postal Code</td>
                                    <td width="10"></td>
                                    <td><input type="text" size="50" name="<%= Constants.FIELD_INPUT_LINES %>" /></td>
                                </tr>
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

                                                foreach (Dataset dset in Constants.AllCountries)
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
                        </p>
                        <p><input type="submit" value="    Next >  "> <input type=hidden name="<%= Constants.FIELD_COUNTRY_NAME %>" /></p>
                        <p class="debug">
                            <%
                                if (m_asError != null)
                                {
                                    %><%="<img src='img/debug.gif' align='left'>The QAS server is not available</br>" + m_asError%><%
                                }
                                else
                                {
                                    %><%="&nbsp" %><%
                                }
                            %>
                        </p>
                        <br />
                        <hr />
                        <p><a href="index.htm">Click here to return to the C# .NET Samples home page.</a></p>
                    </td>
                </tr>
            </table>
        </form>
    </body>
</html>
