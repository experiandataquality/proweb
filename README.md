# Pro Web sample integration code

The Pro Web sample integration code allows you to browse and test out common strategies for integrating address capture.

The code supplied also includes a number of classes that can be used to integrate Pro Web into your own systems.

View the full [Pro Web documentation](http://edq.com/documentation/apis/pro-web) for more information.

## Prerequisites

* Pro Web and address data installed and licensed
* Web server set up - We recommend using IIS

## Getting started

Download or clone the repository. You will need to copy the files to your web server in order to test them out.


### Configuring The Server Path

You will need to configure the server path to point to your Pro Web installation.

To change the server path, edit the following line in the web.config

`<add key="com.qas.proweb.serverURL" value="http://localhost:2021/" />`


### Using The Sample Pages

Once the sample integration pages have been correctly configured, you can view and use them with the following steps:

1. Ensure the Pro Web Service is running.
2. Ensure your web server is running.
3. Ensure that datasets and correct licence keys are installed, and that the datasets have not expired.
4. In a browser, navigate to the index.htm page for the relevant integration pages. For example - "http://[server address]/ProWebCS/index.htm" where [server address] refers to the server hosting the C# sample pages.

