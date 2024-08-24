Introduction
------------
OpenPasswordFilter is an open source custom password filter DLL and userspace service to better protect / control Active Directory domain passwords.

The genesis of this idea comes from conducting many penetration tests where organizations have users who choose common passwords and the ultimate difficulty of controlling this behavior.  The fact is that any domain of size will have some user who chose `Password1` or `Summer2015` or `Company123` as their password.  Any intruder or low-privilege user who can guess or obtain usernames for the domain can easily run through these very common passwords and start expanding the level of access in the domain.

Microsoft provides a wonderful feature in Active Directory, which is the ability to create a custom password filter DLL.  This DLL is loaded by LSASS on boot (if configured), and will be queried for each new password users attempt to set. The DLL simply replies with a `TRUE` or `FALSE`, as appropriate, to indicate that the password passes or fails the test.  

There are some commercial options, but they are usually in the "call for pricing" category, and that makes it a little prohibitive for some organizations to implement truly effective preventive controls for this class of very common bad passwords. 

This is where OpenPasswordFilter comes in -- an open source solution to add basic dictionary-based rejection of common passwords, as well as a check against [haveibeenpwned.com](https://haveibeenpwned.com/)'s wonderful API.

OPF is comprised of two main parts:
   1. OpenPasswordFilter.dll -- this is a custom password filter DLL that can be loaded by LSASS to vet incoming password changes.
   2. OPFService.exe -- this is a C#-based service binary that provides a local user-space service for maintaining the dictionary and servicing requests.
  
The DLL communicates with the service on the loopback network interface to check passwords against the configured database of forbidden values, the pwnedpasswords API of [haveibeenpwned.com](https://haveibeenpwned.com/) (cheers to Troy Hunt for that - what a guy) as well as ensuring that the account's SAMAccountName, given name, surname, and display name are not in the password. This architecture is selected because it is difficult to reload the DLL after boot, and administrators are likely loathe to reboot their DCs when they want to add another forbidden password to the list.  Just bear in mind how this architecture works so you understand what's going on. Please keep in mind passwords will be rejected when the Service is not reachable!

Goal of this Fork
-----------------
- Code refactoring
- .net Framework 4.8 pre requirement
- Handle very large password lists  
-> There is no more 32Bit support  
- Add local Database options (Plain + SHA1)
-> You can Download haveibeenpwned password DB ( https://github.com/HaveIBeenPwned/PwnedPasswordsDownloader )
- Add Active Directory value checks
- Configuration Options
- Added logging to application log (Source OpenPasswordFilter)

Installation
------------
You can download a precompiled 64-bit version of OPF from the following link:

https://github.com/ForumSchlampe/OpenPasswordFilter/releases/tag/v1.0.0
   
  1. Make sure you have .net Framework 4.8 installed
  
  2. Create an gmsa account which domain controllers are able to retrieve passwort
      Something like: 
      (run once) New-ADServiceAccount -Name opf_service -DNSHostName opfservice.your.domain -PrincipalsAllowedToRetrieveManagedPassword "Domain Controllers"
      (run on each domain controller) Install-ADServiceAccount opf_service$
  
  3. Extract Release ZIP File to C:\Program Files\OpenPasswordFilter
  
  4. Copy your lists to whatever destination you want, sysvol is not the worst place to do this to have all domain controllers in sync but think about it with very large files. Default location:  C:\Program Files\OpenPasswordFilter\Data
     "opfmatch.txt"  
     "opfcont.txt"  
     "opfregex.txt"  
     "opfgroups.txt"  
      **Note** Working with large password files will lead in a huge memory overload (huge is very huge)  
      **Note** You can use emtpy ones (shipped with the release), disable those or use samples from here https://github.com/ForumSchlampe/OpenPasswordFilter/tree/master/SampleLists (not very large files included)

   5. Edit OPFService.exe.config to whatever your needs are, any checks which are set to true must reachable for the service otherwise opf service start will fail or all password change attemps are rejected. Hope OPFService.exe.config has comments which explain everything

   6. Copy C:\Program Files\OpenPasswordFilter\Tools\OpenPasswordFilter.dll to %windir%\system32

   7. Run C:\Program Files\OpenPasswordFilter\Tools\Install.ps1 as administrator

   8. Reboot your Domain Controllers

### opfmatch.txt and opfcont.txt
These should contain one forbidden password per line, such as:

    Password1
    Password2
    Company123
    Summer15
    Summer2015
    ...

Passwords in `opfmatch.txt` will be tested for full matches, and those in `opfcont.txt` will be tested for a partial match which can be adjusted in configuration. This is useful for rejecting any password containing poison strings such as `password` and `welcome`. I recommend constructing a list of bad seeds, then using hashcat rules to build `opfcont.txt` with the sort of leet mangling users are likely to try, like so:

`hashcat -r /usr/share/hashcat/rules/Incisive-leetspeak.rule --stdout seedwordlist | tr A-Z a-z | sort | uniq > opfcont.txt`

Bear in mind that if you use a unix like system to create your wordlists, the line terminators will need changing to Windows
format:

`unix2dos opfcont.txt`

### opfregex.txt
Similar to the opfmatch and opfconf files, include here regular expression - one per line - for invalid passwords. Example, including 'xx.*xx' will catch all passwords that have two x's followed by any text followed by two x's. Keep this list short as regular expression matching is more computationally expensive than simple matching or contains searches.
**Note** In the sample list, the regex will reject any password which contains lower case letters

### opfgroups.txt
This file contains zero or more Active-Directory group names - one per line. These can be security or distribution groups. A user is considered to be in a group if they are a descendent child of the group. A user's password will only be checked if the user is a member of any group listed in this file. If the file is present but contains no groups then every user will be checked!

### PwnedLocal*SQL
If you want to use a database as your password list you can do so with mysql and mssql, template to create the database you can find in "C:\Program Files\OpenPasswordFilter\Tools\OpenPasswordFilter_DB.sql"
In Short, there must be a table wich is named "Passwordlist" which contains a column named "Passwords"
You can enable SHA1 or disable it (clear type entries), with SHA1 enabled you are able to use the downloaded haveibeenpwned Database as source. haveibeenpwned has a delimiter ":" which states how often the password was used, you dont need this data just do not import this in the database 

## Event Logging
The opfservice.exe application logs to the Application Event Log with the Event Source OpenPasswordFilter. Searching the event log will identify what the opfservice is checking. If the service fails to start, it's likely an error ingesting the wordlists, and the line number of the problem entry will be written to the Application event log.

## Production Installation Details
This is tested on Windows Server 2012 R2, Windows Server 2016, Windows Server 2019, Windows Server 2022, Windows 10 (23h2), Windows 11 (23h2)
