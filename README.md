# Clipboard Process Starter

This tool watches for any clipboard changes and if it matches a certain pattern. If so, it extracts a part of it and creates a file which is opened by the default handler.

The current implementation watches for `#Unique-ID: (.*)#`, creates an `.otr` (file to directly open an OMNITRACKER object) file in which it replaces the Unique-ID part with the first match in the clipboard. It therefore can be used to automatically open an OMNITRACKER object whenever the user copies such a text  (e.g. in an incoming mail).

## TODO

Further versions may allow to specify the search pattern as well as the file generation in the UI.