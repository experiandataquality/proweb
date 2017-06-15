<%@ Page Language="c#" AutoEventWireup="false" %>
<%@ Import Namespace="System.Collections.Generic" %>  
<%@ Import Namespace="Experian.Qas.Proweb" %>
<!doctype HTML PUBLIC "-//W3C//DTD HTML 4.01//EN" "http://www.w3.org/TR/html4/strict.dtd">
<html>
<head>
    <title>QAS Pro Web - Country Diagnostics</title>
    <link rel="stylesheet" href="style.css" type="text/css">
    <meta name="GENERATOR" content="Microsoft Visual Studio 7.0">
    <meta name="CODE_LANGUAGE" content="C#">
    <meta name="vs_defaultClientScript" content="JavaScript">
    <meta name="vs_targetSchema" content="http://schemas.microsoft.com/intellisense/ie5">
    <%
        // Retrieve server URL from web.config
        string sServerURL = ConfigurationSettings.AppSettings["com.qas.proweb.serverURL"];

        // Instance of the Pro Web service
        IAddressLookup addressLookup = null;

        // Retrieve DataID parameter (set by test overview page): the Dataset to display
        string sDataId = Request.QueryString["DataID"];
    %>
</head>
<body>
    <table class="std">
        <tr>
            <th class="banner">
                <a href="index.htm">
                    <h1>QAS Pro Web</h1>
                    <h2>C# .NET Samples</h2>
                </a>
            </th>
        </tr>
        <tr>
            <td class="std">
                <h1>Country Diagnostics</h1>
                <h2>Dataset: <%= sDataId %></h2>
                <h3>Prompt Sets</h3>

                <table class="indent" border="1" cellspacing="1">
                    <tr>
                        <%	try
                            {
                                addressLookup = QAS.GetSearchService();

                                // Array of search prompts to display
                                PromptSet.Types[] aPromptTypes = new PromptSet.Types[]
		{
			PromptSet.Types.Optimal,
			PromptSet.Types.Alternate,
			PromptSet.Types.Generic
		};
                                for (int i = 0; i < aPromptTypes.GetLength(0); i++)
                                {
                        %>
                        <th><%= aPromptTypes[i].ToString() %></th>
                        <%		}
                        %>
                    </tr>
                    <tr>
                        <%		for (int i = 0; i < aPromptTypes.GetLength(0); i++)
                                {
                                    PromptSet tPrompt = addressLookup.GetPromptSet(sDataId, aPromptTypes[i]);
                                    if (tPrompt == null || tPrompt.Lines == null)
                                    {
                                        Response.Write("\n<td>&nbsp;</td>");
                                    }
                                    else
                                    {
                        %>
                        <td>
                            <table cellpadding="2">
                                <%
                                List<PromptLine> lines = tPrompt.Lines;
                                foreach (PromptLine line in lines)
                                { 
                                    %>
                                    <tr>
                                        <td><%= line.Prompt %></td>
                                            <%if (!line.Example.Equals(""))
                                            {
                                                Response.Write("<td>(e.g. " + line.Example + ")</td>");
                                            }
                                            %>
                                    </tr>
                                    <%
                                }
                                %>
                            </table>
                        </td>
                        <%			}
        }
                        %>
                    </tr>
                </table>

                <h3>Layouts</h3>
                <table class="indent" border="1" cellspacing="1">
                    <tr>
                        <%
                        IList<Layout> layouts = addressLookup.GetAllLayouts(sDataId);
                        foreach (Layout layout in layouts)
                        {
                            %>
                            <th><%= HttpUtility.HtmlEncode(layout.Name) %></th>
                            <%
                        }
                        %>
                    </tr>
                    <tr>
                        <%
                        foreach (Layout layout in layouts)
                        {
                            IList<ExampleAddress> examples = addressLookup.GetExampleAddresses(sDataId, layout.Name);
                            // Take the labels from the first address
                            List<AddressLine> addLines = examples[0].AddressLines;
                            %>
                            <td>
                                <table>
                                    <%
                                    for (int j = 0; j < addLines.Count; j++)
                                    {
                                        string sLine = addLines[j].Label;
                                        %>
                                        <tr>
                                           <td><%= sLine.Equals("") ? "<i>No&nbsp;label</i>" : sLine %></td>
                                        </tr>
                                        <%
                                    }
                                    %>
                                </table>
                            </td>
                            <%
                        }
                        %>
                    </tr>
                </table>
                <%	}
                            catch (Exception x)
                            {
                %>
        </tr>
    </table>


    <% if (x is QAServerException)
       { %>
    <h3 class="error">Pro Web Server Error:</h3>
    <% }
       else
       { %>
    <h3 class="error">Error:</h3>
    <% }%>
    <p class="error"><%=x.Message.Replace("\n", "<br/>")%></p>
    <h3 class="error">Trace:</h3>
    <p class="error"><%=x.StackTrace.Replace(System.Environment.NewLine, "<br/>") %></p>

    <% if (x.InnerException != null)
       { %>
    <h3 class="error">Source:</h3>
    <p class="error"><%=x.InnerException.ToString().Replace( System.Environment.NewLine, "<br/>") %></p>
    <% } %>
    <%	}
    %>
    <br />
    <br />
    <hr />
    <br />
    <a href="test.aspx">Click here to return to the Diagnostics Overview page.</a>
    <br />
    <br />
    <a href="index.htm">Click here to return to the C# .NET Samples home page.</a>
</body>
</html>
