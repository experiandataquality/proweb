namespace Experian.Qas.Proweb
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Experian.Qas.Proweb.soap;

    /// <summary>
    /// This class is a facade into Pro Web and provides the main functionality of the package.
    /// It uses the Experian.Qas.Proweb.soap package in a stateless manner, but some optional settings
    /// are maintained between construction and the main "business" call to the soap package.
    /// An instance of this class is not intended to be preserved across different pages.
    /// The intended usage idiom is:
    ///   1. Construct a QAS object.
    ///   2. Set optional settings
    ///   3. Call main method (e.g. search) - until user interaction is required.
    ///   4. Discard object.
    /// This approach should be used instead of storing a reference to a single
    /// QAS object in the calling code (for example, you should not pass this
    /// type of reference from one page to another).
    /// The methods of the QAS class generally take String parameters and
    /// return objects (or arrays of objects) of the wrapper classes.
    /// </summary>
    public class ProWeb : IAddressLookup
    {
        private Engine currentEngine = new Engine();

        /// <summary>
        /// Initializes a new instance of the <see cref="ProWeb" /> class.
        /// Create a new ProWeb address lookup instance. This is a local install, generated from 
        /// a path to a port e.g. http://localhost:2020.
        /// </summary>
        /// <param name="endpointURL">The path to Pro Web set during install.</param>
        public ProWeb(string endpointURL)
        {
            SearchService = new ProxyProWeb();
            SearchService.Url = endpointURL;
        }

        /// <summary>
        /// Gets or sets the current search method.
        /// </summary>
        public Engine CurrentEngine
        {
            get
            {
                return currentEngine;
            }

            set
            {
                currentEngine = value;
            }
        }

        /// <summary>
        /// Gets or sets our handle on the underlying soap implementation.
        /// </summary>
        private ProxyProWeb SearchService
        {
            get;
            set;
        }

        /// <summary>
        /// This method checks that the combination of data mapping, engine and layout are
        /// valid to search with. This method returns a CanSearch object with IsOk False if you try
        /// to use a data mapping which is not installed or which does not have a valid licence,
        /// or if a layout has not been defined.
        /// This does take time to complete, so you probably do not want to do this before each search (the
        /// same internal checks are made) but should be used heavily during implementation and trouble 
        /// shooting.
        /// </summary>
        /// <param name="dataId">ID of the data mapping to be searched with.</param>
        /// <param name="layoutName">(Optional) Layout to be used in the final address.</param>
        /// <param name="promptSet">(Optional) Name of the Prompt Set to be used for searching.</param>
        /// <returns>CanSearch object, contains the availability of searching, and any reasons for searching
        /// being unavailable.</returns>
        public CanSearch CanSearch(string dataId, string layoutName, PromptSet.Types? promptSet)
        {
            // Create the search arguments
            QACanSearch param = new QACanSearch();
            param.Country = dataId;
            param.Engine = ConvertToEngineType(currentEngine);
            param.Layout = layoutName;

            if (promptSet != null)
            {
                param.Engine.PromptSet = (PromptSetType)promptSet;
            }

            CanSearch result = new CanSearch();
            try
            {
                // Perform the search
                QASearchOk cansearchResult = SearchService.DoCanSearch(param);

                // Prepare the results
                result.IsOk = cansearchResult.IsOk;

                if (cansearchResult.ErrorCode != null)
                {
                    result.ErrorCode = System.Convert.ToInt32(cansearchResult.ErrorCode);
                }

                if (result.ErrorMessage != null)
                {
                    result.ErrorMessage = cansearchResult.ErrorMessage;
                }
            }
            catch (Exception x)
            {
                MapException(x);
            }

            return result;
        }

        /// <summary>
        /// The actual address search! Using the supplied data and the current engine to look up an address.
        /// </summary>
        /// <param name="dataId">ID of the data mapping to be searched with.</param>
        /// <param name="search">The search collected from the end user.</param>        
        /// <param name="promptSet">(Optional) Name of the Prompt Set to be used for searching.</param>        
        /// <param name="layout">(Optional) Layout to be used in the final address.</param>
        /// <returns>The search result. This may include a picklist of suggestions or a final address.</returns>
        public SearchResult Search(string dataId, string search, PromptSet.Types promptSet, string layout)
        {
            // Create the search term
            QASearch param = new QASearch();
            param.Country = dataId;
            param.Engine = ConvertToEngineType(currentEngine);
            param.Engine.PromptSet = (PromptSetType)promptSet;
            param.Engine.PromptSetSpecified = true;
            param.Layout = layout;
            param.Search = search;

            SearchResult result = new SearchResult();
            try
            {
                // Perform the lookup
                QASearchResult searchResult = SearchService.DoSearch(param);

                // Prepare the results - translate the soap responses into something we can work with
                result.VerifyLevel = (VerificationLevel)searchResult.VerifyLevel;
                result.Address = CreateFormattedAddress(searchResult.QAAddress);
                result.Picklist = CreatePicklist(searchResult.QAPicklist);
            }
            catch (Exception x)
            {
                MapException(x);
            }

            return result;
        }

        /// <summary>
        /// A bulk search can take multiple addresses as an input and will perform and return a result for
        /// each of them. 
        /// </summary>
        /// <param name="dataId">ID of the data mapping to be searched with.</param>
        /// <param name="searches">A collection of searches.</param>
        /// <param name="promptSet">(Optional) Name of the Prompt Set to be used for searching.</param>
        /// <param name="layout">(Optional) Layout to be used in the final address.</param>
        /// <returns>The result of several address lookups.</returns>
        public BulkSearchResult BulkSearch(string dataId, IList<string> searches, PromptSet.Types promptSet, string layout)
        {
            int length = searches.Count;

            // Set up the parameter for the SOAP call
            QABulkSearch param = new QABulkSearch();
            param.Country = dataId;
            param.Layout = layout;
            param.Engine = ConvertToEngineType(currentEngine);
            param.BulkSearchTerm = new QASearchType();
            param.BulkSearchTerm.Search = new string[length];

            for (int i = 0; i < length; i++)
            {
                param.BulkSearchTerm.Search[i] = searches[i];
            }

            BulkSearchResult result = null;
            try
            {
                // Make the call to the server
                QABulkSearchResult bulkSearchResult = SearchService.DoBulkSearch(param);

                result = CreateBulkSearchResult(bulkSearchResult);
            }
            catch (Exception x)
            {
                MapException(x);
            }

            return result;
        }

        /// <summary>
        /// A step in is just an empty refine on the moniker of the picklist item you wish to step into. We
        /// provide this function just so that it marries better with the language used in our integration
        /// guides.
        /// </summary>
        /// <param name="moniker">The moniker of the picklist item you want to step into.</param>
        /// <param name="layout">(Optional) the current layout you are using.</param>
        /// <returns>A picklist -the result of the step in.</returns>
        public Picklist StepIn(string moniker, string layout)
        {
            return Refine(moniker, string.Empty, layout);
        }

        /// <summary>
        /// After you have performed an initial search or are at stage two of the typedown workflow you
        /// may need or want to refine on a Picklist to reduce the number of suggestion returned to the user. 
        /// Because the service is stateless, we use a Search Point Moniker (moniker) to allow us to recreate
        /// and work with previous results. By supplying this with additional information we can improve the 
        /// information returned to the user. 
        /// </summary>
        /// <param name="moniker">The moniker of the picklist item you want to refine on.</param>
        /// <param name="refinementText">What is the search term that you want to use to reduce the picklist
        /// of suggested addresses.</param>
        /// <param name="layout">(Optional) the current layout you are using.</param>
        /// <returns>A picklist -the result of the refine.</returns>
        public Picklist Refine(string moniker, string refinementText, string layout)
        {
            // Create the refinement
            QARefine param = new QARefine();
            param.Moniker = moniker;
            param.Refinement = refinementText;
            param.Threshold = currentEngine.Threshold;
            param.Timeout = currentEngine.Timeout;
            param.Layout = layout;

            Picklist result = null;
            try
            {
                // Perform the refinement
                QAPicklistType picklist = SearchService.DoRefine(param).QAPicklist;

                // Prepare the results
                result = CreatePicklist(picklist);
            }
            catch (Exception x)
            {
                MapException(x);
            }

            return result;
        }

        /// <summary>
        /// When the user reaches an address that they want to accept, a call to GetFormattedAddress will
        /// take the moniker from the selected entry and format it according to your layout.
        /// </summary>
        /// <param name="moniker">The moniker of the picklist item we want to format.</param>
        /// <param name="layout">Which layout we want to use to format the result.</param>
        /// <returns>A formatted address ready for use.</returns>
        public FormattedAddress GetFormattedAddress(string moniker, string layout)
        {
            // Create the format request
            QAGetAddress param = new QAGetAddress();
            param.Layout = layout;
            param.Moniker = moniker;

            FormattedAddress result = null;
            try
            {
                // Perform the call to Get the Address
                Address address = SearchService.DoGetAddress(param);

                // Prepare the results
                result = CreateFormattedAddress(address.QAAddress);
            }
            catch (Exception x)
            {
                MapException(x);
            }

            return result;
        }

        /// <summary>
        /// Depending upon which countries and additional sets you have licensed from Experian Data 
        /// Quality, there may be several possible datasets the user can select. This will return a list
        /// of all the datasets configured on the server.
        /// </summary>
        /// <returns>A list of all available datasets.</returns>
        public IList<Dataset> GetAllDatasets()
        {
            List<Dataset> results = new List<Dataset>();
            try
            {
                // Ask for the available datasets
                QADataSet[] datasets = SearchService.DoGetData(new QAGetData());

                // Prepare the results
                foreach (QADataSet dataset in datasets)
                {
                    results.Add(new Dataset(dataset.ID, dataset.Name));
                }
            }
            catch (Exception x)
            {
                MapException(x);
            }

            return results;
        }

        /// <summary>
        /// Depending upon which countries and additional sets you have licensed from Experian Data 
        /// Quality, there may be several possible datasets available. To get additional information about 
        /// them (such as expiry information) use this method. This method is useful for monitoring and
        /// implementation but would be unlikely to be used as part of a best practice address capture
        /// workflow.
        /// </summary>
        /// <param name="datamap">The datamap to return information for.</param>
        /// <returns>A collection of all the LicensedSets. there may be multiple of these if, for example, you
        /// have UK with Utilities information. You will get multiple entries one for each set.</returns>
        public IList<LicensedSet> GetDataMapDetail(string datamap)
        {
            // Prepare the request
            QAGetDataMapDetail mapDetailRequest = new QAGetDataMapDetail();
            mapDetailRequest.DataMap = datamap;

            List<LicensedSet> datasets = new List<LicensedSet>();
            try
            {
                // Perform the request
                QADataMapDetail mapDetail = SearchService.DoGetDataMapDetail(mapDetailRequest);
                
                // Prepare the response
                foreach (QALicensedSet setType in mapDetail.LicensedSet)
                {
                    LicensedSet set = CreateLicensedSet(setType);
                    datasets.Add(set);
                }
            }
            catch (Exception x)
            {
                MapException(x);
            }

            return datasets;
        }

        /// <summary>
        /// Depending upon your integration, you may wish users to or the integration to choose
        /// a different layout. If you do, a call to this will return all the valid layouts configured
        /// one your server. The Configuration Editor can be used to create and modify layouts on Pro Web.
        /// </summary>
        /// <param name="dataId">Return the layouts for this datamap.</param>
        /// <returns>All the layouts available for the supplied datamap.</returns>
        public IList<Layout> GetAllLayouts(string dataId)
        {
            // Prepare the request
            QAGetLayouts param = new QAGetLayouts();
            param.Country = dataId;

            List<Layout> results = new List<Layout>();
            try
            {
                // Ask for all the layouts
                QALayout[] layouts = SearchService.DoGetLayouts(param);

                // Prepare the response. 
                foreach (QALayout layoutType in layouts)
                {
                    Layout layout = new Layout();
                    layout.Name = layoutType.Name;
                    layout.Comment = layoutType.Comment;
                    results.Add(layout);
                }
            }
            catch (Exception x)
            {
                MapException(x);
            }

            return results;
        }

        /// <summary>
        /// Example addresses can be useful to prompt untrained users on the type of input they are 
        /// expected to enter. 
        /// </summary>
        /// <param name="dataId">The datamap to get example addresses for.</param>
        /// <param name="layout">(Optional)The layout to format the example with.</param>
        /// <returns>A collection of example addresses.</returns>
        public IList<ExampleAddress> GetExampleAddresses(string dataId, string layout)
        {
            // Prepare the request
            QAGetExampleAddresses param = new QAGetExampleAddresses();
            param.Country = dataId;
            param.Layout = layout;

            List<ExampleAddress> results = new List<ExampleAddress>();
            try
            {
                // Ask for some examples
                QAExampleAddress[] addresses = SearchService.DoGetExampleAddresses(param);

                // Prepare the response
                foreach (QAExampleAddress addressType in addresses)
                {
                    ExampleAddress example = new ExampleAddress();
                    example.Comment = addressType.Comment;
                    example.ExAddress = CreateFormattedAddress(addressType.Address);
                    results.Add(example);
                }
            }
            catch (Exception x)
            {
                MapException(x);
            }

            return results;
        }

        /// <summary>
        /// Useful for monitoring the status and trouble shooting, this method can return a list
        /// of all LicensedSets - one for each set configured. These will report on key stats such as
        /// clicks and days until data expiry.
        /// </summary>
        /// <returns>A collection of information about all configured data.</returns>
        public IList<LicensedSet> GetLicenceInfo()
        {
            List<LicensedSet> results = new List<LicensedSet>();
            try
            {
                // Make the call to the server
                QALicenceInfo info = SearchService.DoGetLicenseInfo(new QAGetLicenseInfo());

                // Prepare the results.
                foreach (QALicensedSet set in info.LicensedSet)
                {
                    results.Add(CreateLicensedSet(set));
                }                
            }
            catch (Exception x)
            {
                MapException(x);
            }

            return results;
        }

        /// <summary>
        /// PromptSets can be used to guide the user to enter the correct address elements, maximizing the 
        /// chances of them getting the correct result the first time.
        /// </summary>
        /// <param name="dataId">Which datamap to get the prompt for.</param>
        /// <param name="prompt">Which prompt to get.</param>
        /// <returns>The prompt set.</returns>
        public PromptSet GetPromptSet(string dataId, PromptSet.Types prompt)
        {
            // Prepare the request
            QAGetPromptSet param = new QAGetPromptSet();
            param.Country = dataId;
            param.Engine = ConvertToEngineType(currentEngine);
            param.PromptSet = (PromptSetType)prompt;

            PromptSet result = new PromptSet();
            try
            {
                // Make the call to the server
                QAPromptSet promptSetType = SearchService.DoGetPromptSet(param);

                // Prepare the result
                result = new PromptSet();
                result.IsDynamic = promptSetType.Dynamic;

                foreach (soap.PromptLine lineType in promptSetType.Line)
                {
                    Proweb.PromptLine line = new Proweb.PromptLine();
                    line.Example = lineType.Example;
                    line.Prompt = lineType.Prompt;
                    line.SuggestedInputLength = System.Convert.ToInt32(lineType.SuggestedInputLength);
                    result.AddPromptLine(line);
                }
            }
            catch (Exception x)
            {
                MapException(x);
            }

            return result;
        }

        /// <summary>
        /// This will not be required for an address capture workflow but can be useful during implementation
        /// and for trouble shooting. Gives a brief over view of libraries and versions of data in use 
        /// in one view.
        /// </summary>
        /// <returns>A basic collection of info.</returns>
        public IList<string> GetSystemInfo()
        {
            List<string> results = new List<string>();

            try
            {
                // Make the call to the server
                QAGetSystemInfo systemInfo = new QAGetSystemInfo();

                results = SearchService.DoGetSystemInfo(systemInfo).ToList();
            }
            catch (Exception exception)
            {
                MapException(exception);
            }

            return results;
        }

        #region Private methods

        /// <summary>
        /// Convert our Engine into a type suitable for the soap proxies. 
        /// </summary>
        /// <param name="engine">The Engine we want to supply to the server.</param>
        /// <returns>An EngineType - ready for sending to the sever.</returns>
        private static EngineType ConvertToEngineType(Engine engine)
        {
            EngineType engineType = new EngineType();
            engineType.Constraint = engine.Constraint;
            engineType.Flatten = engine.Flatten;
            engineType.Timeout = engine.Timeout;
            engineType.FlattenSpecified = true;

            switch (engine.Prompt)
            {
                case Engine.PromptSets.Default:
                    engineType.PromptSet = soap.PromptSetType.Default;
                    break;
                case Engine.PromptSets.Generic:
                    engineType.PromptSet = soap.PromptSetType.Generic;
                    break;
                case Engine.PromptSets.OneLine:
                    engineType.PromptSet = soap.PromptSetType.OneLine;
                    break;
                case Engine.PromptSets.Optimal:
                    engineType.PromptSet = soap.PromptSetType.Optimal;
                    break;
                case Engine.PromptSets.Alternate:
                    engineType.PromptSet = soap.PromptSetType.Alternate;
                    break;
                case Engine.PromptSets.Alternate2:
                    engineType.PromptSet = soap.PromptSetType.Alternate2;
                    break;
                case Engine.PromptSets.Alternate3:
                    engineType.PromptSet = soap.PromptSetType.Alternate3;
                    break;
                default:
                    engineType.PromptSet = soap.PromptSetType.Default;
                    break;
            }

            switch (engine.Intensity)
            {
                case Engine.EngineIntensity.Exact:
                    engineType.Intensity = soap.EngineIntensityType.Exact;
                    break;
                case Engine.EngineIntensity.Close:
                    engineType.Intensity = soap.EngineIntensityType.Close;
                    break;
                case Engine.EngineIntensity.Extensive:
                    engineType.Intensity = soap.EngineIntensityType.Extensive;
                    break;
                default:
                    engineType.Intensity = soap.EngineIntensityType.Close;
                    break;
            }

            switch (engine.EngineType)
            {
                case Engine.EngineTypes.Singleline:
                    engineType.Value = soap.EngineEnumType.Singleline;
                    break;
                case Engine.EngineTypes.Typedown:
                    engineType.Value = soap.EngineEnumType.Typedown;
                    break;
                case Engine.EngineTypes.Verification:
                    engineType.Value = soap.EngineEnumType.Verification;
                    break;
                case Engine.EngineTypes.Keyfinder:
                    engineType.Value = soap.EngineEnumType.Keyfinder;
                    break;
                case Engine.EngineTypes.Intuitive:
                    engineType.Value = soap.EngineEnumType.Intuitive;
                    break;
                default:
                    engineType.Value = soap.EngineEnumType.Singleline;
                    break;
            }

            return engineType;
        }

        /// <summary>
        /// DPV (Delivery point validation) is used in the US as an indicator of deliverability.
        /// </summary>
        /// <param name="statusType">The result from OD.</param>
        /// <returns>Our integration friendly conversion.</returns>
        private static Constants.DPVStatus ConvertDPVStatus(DPVStatusType statusType)
        {
            switch (statusType)
            {
                case DPVStatusType.DPVNotConfigured:
                    return Constants.DPVStatus.DPVNotConfigured;
                case DPVStatusType.DPVConfigured:
                    return Constants.DPVStatus.DPVConfigured;
                case DPVStatusType.DPVNotConfirmed:
                    return Constants.DPVStatus.DPVNotConfigured;
                case DPVStatusType.DPVConfirmed:
                    return Constants.DPVStatus.DPVConfirmed;
                case DPVStatusType.DPVConfirmedMissingSec:
                    return Constants.DPVStatus.DPVConfirmedMissingSec;
                case DPVStatusType.DPVLocked:
                    return Constants.DPVStatus.DPVLocked;
                case DPVStatusType.DPVSeedHit:
                    return Constants.DPVStatus.DPVSeedHit;
                default:
                    return Constants.DPVStatus.DPVNotConfigured;
            }
        }

        /// <summary>
        /// Create a FormattedAddress from the soap response.
        /// </summary>
        /// <param name="addressType">The QAAddressType from the underlying soap response.</param>
        /// <returns>A FormattedAddress - not tied to the underlying soap classes.</returns>
        private static FormattedAddress CreateFormattedAddress(QAAddressType addressType)
        {
            FormattedAddress address = new FormattedAddress();

            if (addressType != null)
            {
                address.IsOverflow = addressType.Overflow;
                address.IsTruncated = addressType.Truncated;
                address.DPVStatus = ConvertDPVStatus(addressType.DPVStatus);

                foreach (AddressLineType lineType in addressType.AddressLine)
                {
                    AddressLine line = CreateAddressLine(lineType);
                    address.AddAddressLine(line);
                }
            }
           
            return address;
        }

        /// <summary>
        /// Take the result from the AddressLineType (found in the ProWebProxy) and create
        /// a AddressLine from it. 
        /// </summary>
        /// <param name="lineType">The AddressLineType returned from Pro Web.</param>
        /// <returns>An AddressLine - not tied to the underlying soap classes.</returns>
        private static AddressLine CreateAddressLine(AddressLineType lineType)
        {      
            AddressLine line = new AddressLine();
            line.Label = lineType.Label;
            line.Line = lineType.Line;
            line.IsTruncated = lineType.Truncated;
            line.IsOverflow = lineType.Overflow;
            line.LineType = (LineType)lineType.LineContent;
            
            if (lineType.DataplusGroup != null)
            {
                foreach (DataplusGroupType groupType in lineType.DataplusGroup)
                {
                    DataplusGroup group = new DataplusGroup();
                    group.Name = groupType.GroupName;
                    group.Items = groupType.DataplusGroupItem;
                    line.AddDataPlusGroup(group);
                }
            }

            return line;
        }

        /// <summary>
        /// Take the result from the QAPicklistType (found in the ProWebProxy) and create
        /// a Picklist from it. 
        /// </summary>
        /// <param name="picklistType">The QAPicklistType returned from Pro Web.</param>
        /// <returns>An Picklist - not tied to the underlying soap classes.</returns>
        private static Picklist CreatePicklist(QAPicklistType picklistType)
        {
            Picklist picklist = new Picklist();

            if (picklistType != null)
            {
                picklist.Total = System.Convert.ToInt32(picklistType.Total);
                picklist.PotentialMatches = System.Convert.ToInt32(picklistType.PotentialMatches);
                picklist.Moniker = picklistType.FullPicklistMoniker;
                picklist.Prompt = picklistType.Prompt;
                picklist.IsAutoStepinSafe = picklistType.AutoStepinSafe;
                picklist.IsAutoStepinPastClose = picklistType.AutoStepinPastClose;
                picklist.IsAutoFormatSafe = picklistType.AutoFormatSafe;
                picklist.IsAutoFormatPastClose = picklistType.AutoFormatPastClose;
                picklist.IsLargePotential = picklistType.LargePotential;
                picklist.IsMaxMatches = picklistType.MaxMatches;
                picklist.IsMoreOtherMatches = picklistType.MoreOtherMatches;
                picklist.IsOverThreshold = picklistType.OverThreshold;
                picklist.IsTimeout = picklistType.Timeout;

                foreach (PicklistEntryType entryType in picklistType.PicklistEntry)
                {
                    PicklistItem item = CreatePicklistItem(entryType);
                    picklist.AddPicklistItem(item);
                }
            }

            return picklist;
        }

        /// <summary>
        /// Take the result from the PicklistEntryType (found in the ProWebProxy) and create
        /// a PicklistItem from it. 
        /// </summary>
        /// <param name="entryType">The PicklistEntryType returned from Pro Web.</param>
        /// <returns>An PicklistItem - not tied to the underlying soap classes.</returns>
        private static PicklistItem CreatePicklistItem(PicklistEntryType entryType)
        {
            PicklistItem item = new PicklistItem();

            item.Text = entryType.Picklist;
            item.Postcode = entryType.Postcode;
            item.Score = System.Convert.ToInt32(entryType.Score);
            item.Moniker = entryType.Moniker;
            item.PartialAddress = entryType.PartialAddress;

            // Flags
            item.IsFullAddress = entryType.FullAddress;
            item.IsMultipleAddresses = entryType.Multiples;
            item.CanStep = entryType.CanStep;
            item.IsAliasMatch = entryType.AliasMatch;
            item.IsPostcodeRecoded = entryType.PostcodeRecoded;
            item.IsCrossBorderMatch = entryType.CrossBorderMatch;
            item.IsDummyPOBox = entryType.DummyPOBox;
            item.IsName = entryType.Name;
            item.IsInformation = entryType.Information;
            item.IsWarnInformation = entryType.WarnInformation;
            item.IsIncompleteAddress = entryType.IncompleteAddr;
            item.IsUnresolvableRange = entryType.UnresolvableRange;
            item.IsPhantomPrimaryPoint = entryType.PhantomPrimaryPoint;
        
            return item;
        }

        /// <summary>
        /// Take the result from the QABulkSearchResult (found in the ProWebProxy) and create
        /// a BulkSearchResult from it. 
        /// </summary>
        /// <param name="bulkSearchResult">The QABulkSearchResult returned from Pro Web.</param>
        /// <returns>An BulkSearchResult - not tied to the underlying soap classes.</returns>
        private BulkSearchResult CreateBulkSearchResult(QABulkSearchResult bulkSearchResult)
        {
            BulkSearchResult searchResult = new BulkSearchResult();
            searchResult.ErrorMessage = bulkSearchResult.BulkError;
            searchResult.ErrorCode = System.Convert.ToInt32(bulkSearchResult.ErrorCode);
            
            foreach (QABulkSearchItemType itemType in bulkSearchResult.BulkAddress)
            {
                BulkSearchItem item = CreateBulkSearchItem(itemType);
                searchResult.AddBulkSearchItem(item);
            }

            return searchResult;
        }

        /// <summary>
        /// Take the result from the QABulkSearchItemType (found in the ProWebProxy) and create
        /// a BulkSearchItem from it. 
        /// </summary>
        /// <param name="itemType">The QABulkSearchItemType returned from Pro Web.</param>
        /// <returns>An BulkSearchItem - not tied to the underlying soap classes.</returns>
        private BulkSearchItem CreateBulkSearchItem(QABulkSearchItemType itemType)
        {
            BulkSearchItem item = new BulkSearchItem();
            item.VerifyLevel = (VerificationLevel)itemType.VerifyLevel;
            item.Address = CreateFormattedAddress(itemType.QAAddress);
            item.InputAddress = itemType.InputAddress;
            return item;
        }

        /// <summary>
        /// Take the result from the QALicensedSet (found in the ProWebProxy) and create
        /// a LicensedSet from it. 
        /// </summary>
        /// <param name="licensedSet">The QALicensedSet returned from Pro Web.</param>
        /// <returns>An LicensedSet - not tied to the underlying soap classes.</returns>
        private LicensedSet CreateLicensedSet(QALicensedSet licensedSet)
        {
            LicensedSet set = new LicensedSet();
            set.ID = licensedSet.ID;
            set.Description = licensedSet.Description;
            set.Copyright = licensedSet.Copyright;
            set.Version = licensedSet.Version;
            set.BaseCountry = licensedSet.BaseCountry;
            set.Status = licensedSet.Status;
            set.Server = licensedSet.Server;
            set.WarningLevel = (LicensedSet.WarningLevels)licensedSet.WarningLevel;
            set.DaysLeft = System.Convert.ToInt32(licensedSet.DaysLeft);
            set.DataDaysLeft = System.Convert.ToInt32(licensedSet.DataDaysLeft);
            set.LicenceDaysLeft = System.Convert.ToInt32(licensedSet.LicenceDaysLeft);

            return set;
        }

        /// <summary>
        /// Rethrow a remote SoapException exception, with details parsed and exposed.
        /// </summary>
        /// <param name="exception">Exception.</param>
        private void MapException(Exception exception)
        {
            System.Diagnostics.Debugger.Log(0, "Error", exception.ToString() + "\n");

            if (exception is System.Web.Services.Protocols.SoapHeaderException)
            {
                System.Web.Services.Protocols.SoapHeaderException header = exception as System.Web.Services.Protocols.SoapHeaderException;
                throw exception;
            }
            else if (exception is System.Web.Services.Protocols.SoapException)
            {
                // Parse out qas:QAFault string
                System.Web.Services.Protocols.SoapException soapException = exception as System.Web.Services.Protocols.SoapException;
                System.Xml.XmlNode xmlDetail = soapException.Detail;

                string detail = xmlDetail.InnerText.Trim();
                string[] asDetail = detail.Split('\n');

                string message = asDetail[1].Trim() + " [" + asDetail[0].Trim() + "]";

                // If there is detail available, add it to the message
                // Do this in reverse order - the most relevant detail is the last one
                if (asDetail.Length > 2)
                {
                    for (int i = asDetail.Length - 1; i > 1; --i)
                    {
                        message += '\n' + asDetail[i].Trim();
                    }
                }

                QAServerException serverException = new QAServerException(message, soapException);
                throw serverException;
            }
            else
            {
                throw exception;
            }
        }

        #endregion
    }
} 