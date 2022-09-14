using System.Management.Automation;

// https://blog.ironmansoftware.com/daily-powershell/powershell-windows-search-index-status/
// https://stackoverflow.com/a/13454571/1340075

namespace WinSearchIndexer
{
    [Cmdlet(VerbsCommon.Get, "SearchIndexIncludedLocations")]
    public class GetSearchIndexIncludedLocationsCommand : PSCmdlet
    {
        protected override void ProcessRecord()
        {
            try
            {
                WriteObject(Indexer.GetSearchIndexIncludedLocations());
            }
            catch (System.Exception ex)
            {
                WriteError(new ErrorRecord(ex, "InternalException", ErrorCategory.ReadError, "SystemIndex"));
            }
        }
    }

    [Cmdlet(VerbsCommon.Find, "SearchIndexLocationIncluded")]
    public class FindSearchIndexLocationIncludedCommand : PSCmdlet
    {
        [Parameter(Mandatory = true, Position = 0)]
        public string FilePath { get; set; }
        protected override void ProcessRecord()
        {
            try
            {
                WriteObject(Indexer.FindSearchIndexLocationIncluded(FilePath));
            }
            catch (System.Exception ex)
            {
                WriteError(new ErrorRecord(ex, "InternalException", ErrorCategory.ReadError, "SystemIndex"));
            }
        }
    }

    [Cmdlet(VerbsCommon.Add, "SearchIndexIncludedLocation")]
    public class AddSearchIndexIncludedLocationCommand : PSCmdlet
    {
        [Parameter(Mandatory = true, Position = 0)]
        public string FilePath { get; set; }
        [Parameter(Mandatory = false, Position = 1)]
        public bool UserScope { get; set; } = true;
        [Parameter(Mandatory = false, Position = 2)]
        public bool OverrideChildRules { get; set; } = false;
        protected override void ProcessRecord()
        {
            try
            {
                Indexer.AddSearchIndexIncludedLocation(FilePath, true, UserScope, OverrideChildRules);
            }
            catch (System.Exception ex)
            {
                WriteError(new ErrorRecord(ex, "InternalException", ErrorCategory.WriteError, FilePath));
            }
        }
    }

    [Cmdlet(VerbsCommon.Remove, "SearchIndexIncludedLocation")]
    public class RemoveSearchIndexIncludedLocationCommand : PSCmdlet
    {
        [Parameter(Mandatory = true, Position = 0)]
        public string FilePath { get; set; }
        [Parameter(Mandatory = false, Position = 1)]
        public bool UserScope { get; set; } = true;
        protected override void ProcessRecord()
        {
            try
            {
                Indexer.RemoveSearchIndexIncludedLocation(FilePath);
            }
            catch (System.Exception ex)
            {
                WriteError(new ErrorRecord(ex, "InternalException", ErrorCategory.WriteError, FilePath));
            }
        }
    }
}
