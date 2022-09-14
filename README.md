# Overview
This project creates a PSCmdlet DLL to wrap [ISearchCrawlScopeManager](https://docs.microsoft.com/en-us/windows/win32/api/searchapi/nn-searchapi-isearchcrawlscopemanager). From PowerShell, you can do things like getting a list of indexed locations or adding a new indexed location (Indexing Options). This project was inspired by [this Reddit question](https://www.reddit.com/r/sysadmin/comments/xdis92/using_powershell_to_add_items_to_index_list/).

# Prerequisites
1. Ensure the following is installed
  - Windows 10 SDK (select **Windows SDK for Desktop C++ amd64 Apps**)
  - Visual Studio 2022 Community Edition (other versions may work, but 2022 is what I used)
  - The *Windows Search Service* feature is installed (Windows Server only)
2. Open the Visual Studio Command Prompt
  - If it does not appear in your start menu search, it can be found in Visual Studio under Tools > Command Line > Developer Command Prompt
3. Run `tlbimp.exe "C:\Program Files (x86)\Windows Kits\10\Lib\10.0.22621.0\um\x64\SearchAPI.tlb" /namespace:Microsoft.Search.Interop /out:C:\OutputFolder\Microsoft.Search.Interop.dll`
  - The version in the path may vary
  - Ensure to adjust the output path
4. When you download this repository, move `Microsoft.Search.Interop.dll` to `.\WinSearchIndexer\WinSearchIndexerPS\lib`.

# Examples
- [PowerShell Tests](./WinSearchIndexerPS/Test-WinSearchIndexer.ps1)
- [Unit Tests](./TestWinSearchIndexer)

# Resources
Thanks to the Internet, I don't think I would have been able to put this together otherwise.
- [Discovery and basic usage of ISearchCrawlScopeManager](https://blog.ironmansoftware.com/daily-powershell/powershell-windows-search-index-status/)
- [Adding a location to be indexed](https://stackoverflow.com/a/13454571/1340075)
