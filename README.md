Introduction
------------
OpenPasswordFilter is an open source custom password filter DLL and userspace service to better protect / control Active Directory domain passwords.

The genesis of this idea comes from conducting many penetration tests where organizations have users who choose common passwords
and the ultimate difficulty of controlling this behavior.  The fact is that any domain of size will have some user who chose
`Password1` or `Summer2015` or `Company123` as their password.  Any intruder or low-privilege user who can guess or obtain
usernames for the domain can easily run through these very common passwords and start expanding the level of access in the 
domain.

Microsoft provides a wonderful feature in Active Directory, which is the ability to create a custom password filter DLL.  This
DLL is loaded by LSASS on boot (if configured), and will be queried for each new password users attempt to set.  The DLL simply
replies with a `TRUE` or `FALSE`, as appropriate, to indicate that the password passes or fails the test.  

There are some commercial options, but they are usually in the "call for pricing" category, and that makes it a little 
prohibitive for some organizations to implement truly effective preventive controls for this class of very common bad passwords. 

This is where OpenPasswordFilter comes in -- an open source solution to add basic dictionary-based rejection of common
passwords, as well as a check against [haveibeenpwned.com](https://haveibeenpwned.com/)'s wonderful API.

OPF is comprised of two main parts:
   1. OpenPasswordFilter.dll -- this is a custom password filter DLL that can be loaded by LSASS to vet incoming password changes.
   2. OPFService.exe -- this is a C#-based service binary that provides a local user-space service for maintaining the dictionary and servicing requests.
  
The DLL communicates with the service on the loopback network interface to check passwords against the configured database
of forbidden values, the pwnedpasswords API of [haveibeenpwned.com](https://haveibeenpwned.com/) (cheers to Troy Hunt for that - what a guy) as well as ensuring that the account's SAMAccountName, given name, surname, and display name are not in the password. This architecture is selected because it is difficult to reload the DLL after boot, and administrators are likely loathe to reboot their DCs when they want to add another forbidden password to the list.  Just bear in mind how this architecture works so you understand what's going on.

**NOTE** The current version is pretty beta!  I have tested it on some of my DCs, but your mileage may vary and you may wish to test in a safe location before using this in production.

Goal of this Fork
-----------------
- Handle very large password lists  
-> There is no more 32Bit support  
- More configuration options, currently  
-> Configureable path for password lists  
-> haveibeenpwned API enable/disable ability  

Installation
------------
You can download a precompiled 64-bit version of OPF from the following link:

[OPFService.exe](https://github.com/ForumSchlampe/OpenPasswordFilter/tree/master/OPFService/bin/x64/Release)  
[OpenPasswordFilter.dll](-missing-)  
   
  1. Copy `OpenPasswordFilter.dll` to `%WINDIR%\System32`
  
  2. Configure the `HKLM\SYSTEM\CurrentControlSet\Control\Lsa\Notification Packages` registry key with the DLL name  
  **Note** do not include the `.dll` extension in the registry key -- just `OpenPasswordFilter`.
  
  3. Copy OPFService.exe and OPFService.exe.config to a destination you like, like C:\Program Files\OpenPasswordFilter
  
  4. Copy your lists to whatever destination you want, sysvol is not the worst place to do this to have all domain controllers in sync.  
     "opfmatch.txt"  
     "opfcont.txt"  
     "opfregex.txt"  
     "opfgroups.txt"  
   **Note** The service checks file modification time at the start of servicing a request and will read in the lists again if it has changed, so restarting the OPF service when modifying the lists is not necessary.  
   **Note** Working with large password files will lead in a huge memory overload (huge is very huge)  

   5. Edit OPFService.exe.config and set at least the path for  
      "OPFMatchPath"  
      "OPFContPath"  
      "OPFRegexhPath"  
      "OPFGroupsPath"  

   6. Install the OPF Service  
    -> sc create OPF binPath= <full path to exe>\opfservice.exe start= boot 

   7. If everything is in its place, try to start the service  
    -> sc start OPF  
    or  
    -> sc stop OPF   
    **Note** Working with a very large password file will lead to an extended starttime so there might be a message about "not responding"  

### opfmatch.txt and opfcont.txt
These should contain one forbidden password per line, such as:

    Password1
    Password2
    Company123
    Summer15
    Summer2015
    ...

Passwords in `opfmatch.txt` will be tested for full matches, and those in `opfcont.txt` will be tested for a partial match. This
is useful for rejecting any password containing poison strings such as `password` and `welcome`. I recommend constructing a list
of bad seeds, then using hashcat rules to build `opfcont.txt` with the sort of leet mangling users are likely to try, like so:

`hashcat -r /usr/share/hashcat/rules/Incisive-leetspeak.rule --stdout seedwordlist | tr A-Z a-z | sort | uniq > opfcont.txt`

Bear in mind that if you use a unix like system to create your wordlists, the line terminators will need changing to Windows
format:

`unix2dos opfcont.txt`

### opfregex.txt
Similar to the opfmatch and opfconf files, include here regular expression - one per line - for invalid passwords. Example, 
including 'xx.*xx' will catch all passwords that have two x's followed by any text followed by two x's. Keep this list short 
as regular expression matching is more computationally expensive than simple matching or contains searches.

### opfgroups.txt
This file contains zero or more Active-Directory group names - one per line. These can be security or distribution groups. 
A user is considered to be in a group if they are a descendent child of the group. A user's password will only be checked 
if the user is a member of any group listed in this file. If the file is present but contains no groups then every user will be checked.

## Event Logging
The opfservice.exe application logs to the Application Event Log using codes 100, and 101. Searching the event log will identify what the opfservice is checking.
If the service fails to start, it's likely an error ingesting the wordlists, and the line number of the problem entry will be
written to the Application event log.

## Production Installation Details
This requires a 64 bit OS as the password filter bitness must match that of the OS and I see no reason to target x86. 

If all has gone well, reboot your DC and test by using the normal GUI password reset function to choose a password that is on
your forbidden list.

