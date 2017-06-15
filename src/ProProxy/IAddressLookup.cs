namespace Experian.Qas.Proweb
{
    using System.Collections.Generic;

    /// <summary>
    /// There are two sources for address capture - Pro Web the Experian Data Quality on site solution
    /// and Pro OnDemand - our hosted solution. It is easy to switch between these or use them in a mixed mode
    /// if you only integrate against this interface.
    /// </summary>
    public interface IAddressLookup
    {
        Engine CurrentEngine
        {
            get;
            set;
        }

        CanSearch CanSearch(string dataId, string layoutName, PromptSet.Types? promptSet);

        SearchResult Search(string dataId, string search, PromptSet.Types promptSet, string layout);

        BulkSearchResult BulkSearch(string dataId, IList<string> searches, PromptSet.Types promptSet, string layout);

        Picklist StepIn(string moniker, string layout);

        Picklist Refine(string moniker, string refinementText, string layout);

        FormattedAddress GetFormattedAddress(string moniker, string layout);

        IList<Dataset> GetAllDatasets();

        IList<LicensedSet> GetDataMapDetail(string sID);

        IList<Layout> GetAllLayouts(string dataId);

        IList<ExampleAddress> GetExampleAddresses(string dataId, string layout);

        IList<LicensedSet> GetLicenceInfo();

        PromptSet GetPromptSet(string dataId, PromptSet.Types promptSet);

        IList<string> GetSystemInfo();
    }
}
