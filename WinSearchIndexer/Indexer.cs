using Microsoft.Search.Interop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;
using System.ServiceProcess;

namespace WinSearchIndexer
{
    public static class Indexer
    {
        public static bool FindSearchIndexStatus()
        {
            bool IsWorkstation = false;
            bool IsFeatureAvailable = false;
            bool IsServiceAvailable = false;

            ManagementScope scope = new ManagementScope();
            ObjectQuery query = new ObjectQuery("SELECT ProductType FROM Win32_OperatingSystem");

            using (ManagementObjectSearcher searcher = new ManagementObjectSearcher(query))
            {
                using(ManagementObjectCollection collection = searcher.Get())
                {
                    uint productType = (uint)collection.OfType<ManagementObject>().First().Properties["ProductType"].Value;
                    IsWorkstation = productType == 1;
                }
            }

            if (IsWorkstation)
            {
                IsFeatureAvailable = true;
            }
            else
            {
                using (ManagementClass mgmtClass = new ManagementClass("Win32_ServerFeature"))
                {
                    using (ManagementObjectCollection collection = mgmtClass.GetInstances())
                    {
                        foreach(ManagementObject obj in collection)
                        {
                            if(obj.Properties["Name"].Value.ToString() == "Search-Service"){ IsFeatureAvailable = true; }
                        }
                    }
                }
            }

            try
            {
                ServiceController controller = new ServiceController("WSearch");
                IsServiceAvailable = controller.Status == ServiceControllerStatus.Running;
            }
            catch (Exception)
            {
                // Do nothing
            }

            return IsFeatureAvailable & IsServiceAvailable;
        }

        public static List<CSearchScopeRule> GetSearchIndexIncludedLocations()
        {
            CSearchManager manager = new CSearchManager();

            ISearchCatalogManager catalogManager = manager.GetCatalog("SystemIndex");

            ISearchCrawlScopeManager crawlManager = catalogManager.GetCrawlScopeManager();

            IEnumSearchScopeRules eLocations = crawlManager.EnumerateScopeRules();
            bool HasNotEnumerated = true;
            List<CSearchScopeRule> Locations = new List<CSearchScopeRule>();
            CSearchScopeRule LastLocation = null;

            while (null != LastLocation | HasNotEnumerated)
            {
                uint n = 0;
                eLocations.Next(1, out LastLocation, ref n);
                HasNotEnumerated = false;
                Locations.Add(LastLocation);
            }

            return Locations;
        }

        public static bool FindSearchIndexLocationIncluded(string FilePath)
        {
            CSearchManager manager = new CSearchManager();

            ISearchCatalogManager catalogManager = manager.GetCatalog("SystemIndex");

            ISearchCrawlScopeManager crawlManager = catalogManager.GetCrawlScopeManager();

            Uri Location = new Uri(FilePath);
            return crawlManager.IncludedInCrawlScope(Location.AbsoluteUri) == 1;
        }

        public static void AddSearchIndexIncludedLocation(string FilePath,
            bool Include = true, bool UserScope = true, bool OverrideChildRules = false)
        {
            Uri Location = new Uri(FilePath);

            CSearchManager manager = new CSearchManager();

            ISearchCatalogManager catalogManager = manager.GetCatalog("SystemIndex");

            ISearchCrawlScopeManager crawlManager = catalogManager.GetCrawlScopeManager();
            if (UserScope)
            {
                crawlManager.AddUserScopeRule(Location.AbsoluteUri, Convert.ToInt32(Include),
                    Convert.ToInt32(OverrideChildRules), 0);
            }
            else
            {
                crawlManager.AddDefaultScopeRule(Location.AbsoluteUri, Convert.ToInt32(Include), 0);
            }

            crawlManager.SaveAll();
        }

        public static void RemoveSearchIndexIncludedLocation(string FilePath,
            bool UserScope = true)
        {
            if (FindSearchIndexLocationIncluded(FilePath)) { AddSearchIndexIncludedLocation(FilePath, false); return; }

            Uri Location = new Uri(FilePath);

            CSearchManager manager = new CSearchManager();

            ISearchCatalogManager catalogManager = manager.GetCatalog("SystemIndex");

            ISearchCrawlScopeManager crawlManager = catalogManager.GetCrawlScopeManager();
            if (UserScope) {
                crawlManager.RemoveScopeRule(Location.AbsoluteUri);
            }
            else
            {
                crawlManager.RemoveDefaultScopeRule(Location.AbsoluteUri);
            }
            
            crawlManager.SaveAll();
        }
    }
}
