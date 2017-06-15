<%@ Page language="c#" Inherits="Experian.Qas.Prowebintegration.VerifyRefine" enableViewState="False" CodeFile="VerifyRefine.aspx.cs" %>
<%@ Import namespace="Experian.Qas.Prowebintegration" %>
<!doctype HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<html> 
    <head>
        <title>QAS Pro Web - Address Verification</title>
        <%-- QAS Pro Web > (c) Experian > www.edq.com --%>
        <%-- Web > Verification > VerifyRefine --%>
        <%-- Additional details > Query user for premises details, which are then checked --%>
        <link href="style.css" type="text/css" rel="stylesheet" />
        <meta name="GENERATOR" Content="Microsoft Visual Studio 7.0" />
        <meta name="CODE_LANGUAGE" Content="C#" />
        <meta name="vs_defaultClientScript" content="JavaScript" />
        <meta name="vs_targetSchema" content="http://schemas.microsoft.com/intellisense/ie5">
        <script type="text/javascript">
            /* Set the focus */
            function init()
            {
                document.frmVerifyRefine.<%= TextRefinement.ID %>.focus();
                document.frmVerifyRefine.<%= TextRefinement.ID %>.select();
            }

            /* Ensure something has been entered */
            function validate()
            {
                if (document.frmVerifyRefine.TextRefinement.value != "")
                {
                    return true;
                }
                document.frmVerifyRefine.TextRefinement.focus();
                alert("Please enter exact details.");
                return false;
            }
        </script>
    </head>
    <body onload="init();">
        <form id="frmVerifyRefine" method="post" runat="server" autocomplete="off">
            <table class="std">
                <tr>
                    <th class="banner">
                        <a href="index.htm"><h1>QAS Pro Web</h1><h2>C# .NET Samples</h2></a>
                    </th>
                </tr>
                <tr>
                    <td class="std">
                        <h1>Address Verification</h1>
                        <h3>Enter your address</h3>
                        <p><asp:Literal id="LiteralMessage" runat="server" EnableViewState="False"></asp:Literal></p>
                        <br>
                        <table>
                            <tr>
                                <td>Address</td>
                                <td width="10"></td>
                                <td><input type="text" size="10" id="TextRefinement" runat="server" NAME="TextRefinement"></td>
                                <td>&nbsp;&nbsp;in&nbsp;</td>
                                <td><asp:Literal id="LiteralRefineLine" runat="server" EnableViewState="False"></asp:Literal></td>
                            </tr>
                            <tr>
                                <td></td>
                                <td></td>
                                <td></td>
                                <td></td>
                                <td><asp:Literal id="LiteralRefineAddress" runat="server" EnableViewState="False"></asp:Literal></td>
                            </tr>
                        </table>
                        <br>
                        <input type="submit" value="    Next >   " id="ButtonNext" runat="server" onclick="return validate();">
                        <input type="hidden" id="HiddenMoniker" runat="server">
                        <input type="hidden" id="HiddenCountry" runat="server">
<%	/* Need to pass along the user input in case the next stage fails */
    for (int i = 0; i < GetInputAddress.Length; i++)
    {
%>						<input type="hidden" name="<%= Experian.Qas.Proweb.Constants.FIELD_INPUT_LINES %>" value="<%= HttpUtility.HtmlEncode(GetInputAddress[i]) %>" />
<%	}
%>						<p>&nbsp;</p>
                        <p><hr /></p>
                        <p><a href="index.htm">Click here to return to the C# .NET Samples home page.</a></p>
                    </td>
                </tr>
            </table>
        </form>
    </body>
</html>
