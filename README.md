"#WorkTimeLogger" 

A simple windows service to log working hours.

Installation - 

From elevated command prompt, run:
WorkTimeLogger install


OR run interactively - 

Run WorkTimeLogger.exe



Uninstall - 

WorkTimeLogger uninstall



Data and week hour summaries are stored under the following path - 
%programdata%\WorkTimeLogger\{year}\{month}


NOTE:  Default hours / lunch / holiday settings etc can be changed in Settings.cs.  At the moment this is not serialized to file.