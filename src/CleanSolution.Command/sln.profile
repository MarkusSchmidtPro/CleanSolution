#
# A CLArgs config file containing predefined Options
#
# This set of options represent a Visual Studio SLN cleansing profile.
# 

# Use the ability to specify /p (Patterns) as often as you want.

# Directiory deletions
/p="*\obj;*\bin"

# File deletions
/p="*.pdb;*.user;*.snk"  

# Files and Directories can also be specified in one single list.
# Using two times /p is only for better readbility of this file.

# Exclusion: ignore those directories
/e="*\.git;*\.vs;*\pub"