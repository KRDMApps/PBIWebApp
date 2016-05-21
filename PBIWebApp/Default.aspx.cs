
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using System.Threading.Tasks;
using System.Net.Http;
using System.Collections.Specialized;
using System.Web.Script.Serialization;

namespace PBIWebApp
{
    /* NOTE: This sample is to illustrate how to authenticate a Power BI web app. 
    * In a production application, you should provide appropriate exception handling and refactor authentication settings into 
    * a configuration. Authentication settings are hard-coded in the sample to make it easier to follow the flow of authentication. */
    public partial class _Default : Page
    {
        public AuthenticationResult authResult { get; set; }

        protected void Page_Load(object sender, EventArgs e)
        {
            //Test for AuthenticationResult
            if (Session["authResult"] != null)
            {
                //Get the authentication result from the session
                authResult = (AuthenticationResult)Session["authResult"];

                //Show Power BI Panel
                PBIPanel.Visible = true;
                signinPanel.Visible = false;

                //Set user and toek from authentication result
                userLabel.Text = authResult.UserInfo.DisplayableId;
                accessTokenTextbox.Text = authResult.AccessToken;

            }
            else
            {
                PBIPanel.Visible = false;
            }
        }

        protected void signInButton_Click(object sender, EventArgs e)
        {
            try
            {
                lblError.Text = "";
                //Create a query string
                //Create a sign-in NameValueCollection for query string
                var @params = new NameValueCollection
                {
                    //Azure AD will return an authorization code. 
                    //See the Redirect class to see how "code" is used to AcquireTokenByAuthorizationCode
                    {"response_type", "code"},

                    //Client ID is used by the application to identify themselves to the users that they are requesting permissions from. 
                    //You get the client id when you register your Azure app.
                    {"client_id", Properties.Settings.Default.ClientID},

                    //Resource uri to the Power BI resource to be authorized
                    {"resource", "https://analysis.windows.net/powerbi/api"},

                    //After user authenticates, Azure AD will redirect back to the web app
                    {"redirect_uri", Properties.Settings.Default.RedirectUrl}
                };

                //Create sign-in query string
                var queryString = HttpUtility.ParseQueryString(string.Empty);
                queryString.Add(@params);

                //Redirect authority
                //Authority Uri is an Azure resource that takes a client id to get an Access token
                string authorityUri = "https://login.windows.net/common/oauth2/authorize/";
                Response.Redirect(String.Format("{0}?{1}", authorityUri, queryString));
            }
            catch (Exception ex)
            {
                lblError.Text = ex.Message;
            }
        }

        protected void getGroupsButton_Click(object sender, EventArgs e)
        {
            try
            {
                lblError.Text = "";
                string responseContent = string.Empty;
                //The resource Uri to the Power BI REST API resource
                string groupsUri = "https://api.powerbi.com/v1.0/myorg/groups";

                //Configure groups request
                System.Net.WebRequest request = System.Net.WebRequest.Create(groupsUri) as System.Net.HttpWebRequest;
                request.Method = "GET";
                request.ContentLength = 0;
                request.Headers.Add("Authorization", String.Format("Bearer {0}", authResult.AccessToken));

                //Get groups response from request.GetResponse()
                using (var response = request.GetResponse() as System.Net.HttpWebResponse)
                {
                    //Get reader from response stream
                    using (var reader = new System.IO.StreamReader(response.GetResponseStream()))
                    {
                        responseContent = reader.ReadToEnd();

                        //Deserialize JSON string
                        //JavaScriptSerializer class is in System.Web.Script.Serialization
                        JavaScriptSerializer json = new JavaScriptSerializer();
                        Groups groups = (Groups)json.Deserialize(responseContent, typeof(Groups));

                        groupResultsListBox.Items.Clear();
                        //Get each group from 
                        foreach (group grp in groups.value)
                        {
                            groupResultsListBox.Items.Add(new ListItem(grp.Name, grp.Id));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                lblError.Text = ex.Message;
            }
        }

        protected void getDatasetsButton_Click(object sender, EventArgs e)
        {
            try
            {
                lblError.Text = "";
                string responseContent = string.Empty;
                //The resource Uri to the Power BI REST API resource
                string datasetsUri = "https://api.powerbi.com/v1.0/myorg/datasets";

                if (groupResultsListBox.Items.Count > 0 && groupResultsListBox.SelectedItem == null)
                {
                    lblError.Text = "Groups have been retrieved but none have been selected";
                    return;
                }

                if (groupResultsListBox.SelectedItem != null)
                {
                    string groupId = groupResultsListBox.SelectedValue.ToString();
                    datasetsUri = "https://api.powerbi.com/v1.0/myorg/groups/" + groupId + "/datasets";
                }
                //Configure datasets request
                System.Net.WebRequest request = System.Net.WebRequest.Create(datasetsUri) as System.Net.HttpWebRequest;
                request.Method = "GET";
                request.ContentLength = 0;
                request.Headers.Add("Authorization", String.Format("Bearer {0}", authResult.AccessToken));

                //Get datasets response from request.GetResponse()
                using (var response = request.GetResponse() as System.Net.HttpWebResponse)
                {
                    //Get reader from response stream
                    using (var reader = new System.IO.StreamReader(response.GetResponseStream()))
                    {
                        responseContent = reader.ReadToEnd();

                        //Deserialize JSON string
                        //JavaScriptSerializer class is in System.Web.Script.Serialization
                        JavaScriptSerializer json = new JavaScriptSerializer();
                        Datasets datasets = (Datasets)json.Deserialize(responseContent, typeof(Datasets));

                        datasetResultsListBox.Items.Clear();
                        //Get each Dataset from 
                        foreach (dataset ds in datasets.value)
                        {
                            datasetResultsListBox.Items.Add(new ListItem(ds.Name, ds.Id));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                lblError.Text = ex.Message;
            }
        }

        protected void getTablesButton_Click(object sender, EventArgs e)
        {
            try
            {
                lblError.Text = "";
                if (datasetResultsListBox.SelectedItem == null)
                {
                    lblError.Text = "Please choose a dataset list above";
                    return;
                }

                string responseContent = string.Empty;
                string dataSetId = datasetResultsListBox.SelectedValue.ToString();
                //The resource Uri to the Power BI REST API resource
                string tablesUri = "https://api.powerbi.com/v1.0/myorg/datasets/" + dataSetId + "/tables";

                if (groupResultsListBox.SelectedItem != null)
                {
                    string groupId = groupResultsListBox.SelectedValue.ToString();
                    tablesUri = "https://api.powerbi.com/v1.0/myorg/groups/" + groupId + "/datasets/" + dataSetId + "/tables";
                }
                //Configure tables request
                System.Net.WebRequest request = System.Net.WebRequest.Create(tablesUri) as System.Net.HttpWebRequest;
                request.Method = "GET";
                request.ContentLength = 0;
                request.Headers.Add("Authorization", String.Format("Bearer {0}", authResult.AccessToken));

                //Get tables response from request.GetResponse()
                using (var response = request.GetResponse() as System.Net.HttpWebResponse)
                {
                    //Get reader from response stream
                    using (var reader = new System.IO.StreamReader(response.GetResponseStream()))
                    {
                        responseContent = reader.ReadToEnd();

                        //Deserialize JSON string
                        //JavaScriptSerializer class is in System.Web.Script.Serialization
                        JavaScriptSerializer json = new JavaScriptSerializer();
                        Tables tables = (Tables)json.Deserialize(responseContent, typeof(Tables));

                        tableResultsListBox.Items.Clear();
                        //Get each table from 
                        foreach (table tbl in tables.value)
                        {
                            tableResultsListBox.Items.Add(new ListItem(tbl.Name, tbl.Name));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                lblError.Text = ex.Message;
            }
        }

        protected void clearTableRowsButton_Click(object sender, EventArgs e)
        {
            try
            {
                lblError.Text = "";
                if (datasetResultsListBox.SelectedItem == null || tableResultsListBox.SelectedItem == null)
                {
                    lblError.Text = "Please choose a dataset and a table from the lists above";
                    return;
                }

                string responseContent = string.Empty;
                string dataSetId = datasetResultsListBox.SelectedValue.ToString();
                string tableName = tableResultsListBox.SelectedValue.ToString();
                //The resource Uri to the Power BI REST API resource
                string clearRowsUri = "https://api.powerbi.com/v1.0/myorg/datasets/" + dataSetId + "/tables/" + tableName + "/rows";

                if (groupResultsListBox.SelectedItem != null)
                {
                    string groupId = groupResultsListBox.SelectedValue.ToString();
                    clearRowsUri = "https://api.powerbi.com/v1.0/myorg/groups/" + groupId + "/datasets/" + dataSetId + "/tables/" + tableName + "/rows";
                }
                //Configure tables request
                System.Net.WebRequest request = System.Net.WebRequest.Create(clearRowsUri) as System.Net.HttpWebRequest;
                request.Method = "DELETE";
                request.ContentLength = 0;
                request.Headers.Add("Authorization", String.Format("Bearer {0}", authResult.AccessToken));

                //Get tables response from request.GetResponse()
                using (var response = request.GetResponse() as System.Net.HttpWebResponse)
                {
                    //Get reader from response stream
                    clearTablesResultTextBox.Text = response.StatusCode.ToString();
                }
            }
            catch (Exception ex)
            {
                lblError.Text = ex.Message;
            }

        }

        protected void clearGroupsButton_Click(object sender, EventArgs e)
        {
            groupResultsListBox.Items.Clear();
        }

        protected void clearDatasetsButton_Click(object sender, EventArgs e)
        {
            datasetResultsListBox.Items.Clear();
        }

        protected void clearTablesButton_Click(object sender, EventArgs e)
        {
            tableResultsListBox.Items.Clear();
        }

        protected void clearAllButton_Click(object sender, EventArgs e)
        {
            groupResultsListBox.Items.Clear();
            datasetResultsListBox.Items.Clear();
            tableResultsListBox.Items.Clear();
            clearTablesResultTextBox.Text = "";
            lblError.Text = "";
        }
    }
}