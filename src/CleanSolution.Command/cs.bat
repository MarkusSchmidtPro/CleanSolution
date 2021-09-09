REM %~dp0 refers to the full path to the batch file's directory (static)
REM cs CleanSolution -help
REM cs CleanSolution -test=false
REM cs CleanSolution @sln.profile -test=false

call %~dp0\CleanSolution\CleanSolution.exe %*