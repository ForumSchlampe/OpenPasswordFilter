﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace OPFService.Properties {
    
    
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.Editors.SettingsDesigner.SettingsSingleFileGenerator", "17.7.0.0")]
    internal sealed partial class Settings : global::System.Configuration.ApplicationSettingsBase {
        
        private static Settings defaultInstance = ((Settings)(global::System.Configuration.ApplicationSettingsBase.Synchronized(new Settings())));
        
        public static Settings Default {
            get {
                return defaultInstance;
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("True")]
        public bool PwnedPasswordsAPIEnabled {
            get {
                return ((bool)(this["PwnedPasswordsAPIEnabled"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("D:\\Dev\\OpenPasswordFilter\\Lists\\opfmatch.txt")]
        public string OPFMatchPath {
            get {
                return ((string)(this["OPFMatchPath"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("D:\\Dev\\OpenPasswordFilter\\Lists\\opfcont.txt")]
        public string OPFContPath {
            get {
                return ((string)(this["OPFContPath"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("D:\\Dev\\OpenPasswordFilter\\Lists\\opfregex.txt")]
        public string OPFRegexPath {
            get {
                return ((string)(this["OPFRegexPath"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("D:\\Dev\\OpenPasswordFilter\\Lists\\opfgroup.txt")]
        public string OPFGroupPath {
            get {
                return ((string)(this["OPFGroupPath"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("False")]
        public bool PwnedLocalMySQLDB {
            get {
                return ((bool)(this["PwnedLocalMySQLDB"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("False")]
        public bool PwnedLocalMSSQLDB {
            get {
                return ((bool)(this["PwnedLocalMSSQLDB"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("Server=myServerAddress;Database=myDataBase;Integrated Security=SSPI;Encrypt=true")]
        public string PwnedLocalMSSQLDBConnString {
            get {
                return ((string)(this["PwnedLocalMSSQLDBConnString"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("True")]
        public bool OPFMatchPathEnabled {
            get {
                return ((bool)(this["OPFMatchPathEnabled"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("True")]
        public bool OPFContPathEnabled {
            get {
                return ((bool)(this["OPFContPathEnabled"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("True")]
        public bool OPFRegexPathEnabled {
            get {
                return ((bool)(this["OPFRegexPathEnabled"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("True")]
        public bool OPFGroupPathEnabled {
            get {
                return ((bool)(this["OPFGroupPathEnabled"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("Server=myServerAddress;Database=myDataBase;IntegratedSecurity=yes;Uid=auth_window" +
            "s;SslMode=Required;UseCompression=True;")]
        public string PwnedLocalMySQLDBConnString {
            get {
                return ((string)(this["PwnedLocalMySQLDBConnString"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("False")]
        public bool OPFActiveDirectoryEnabled {
            get {
                return ((bool)(this["OPFActiveDirectoryEnabled"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("5999")]
        public int Port {
            get {
                return ((int)(this["Port"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("Information")]
        public global::Serilog.Events.LogEventLevel LogLevel {
            get {
                return ((global::Serilog.Events.LogEventLevel)(this["LogLevel"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute(@"<?xml version=""1.0"" encoding=""utf-16""?>
<ArrayOfString xmlns:xsd=""http://www.w3.org/2001/XMLSchema"" xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"">
  <string>GivenName</string>
  <string>Surname</string>
  <string>EmailAddress</string>
  <string>SamAccountName</string>
  <string>UserPrincipalName</string>
  <string>DisplayName</string>
  <string>Description</string>
  <string>EmployeeId</string>
  <string>VoiceTelephoneNumber</string>
</ArrayOfString>")]
        public global::System.Collections.Specialized.StringCollection OPFActiveDirectoryProperties {
            get {
                return ((global::System.Collections.Specialized.StringCollection)(this["OPFActiveDirectoryProperties"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("80")]
        public ushort OPFContPercentage {
            get {
                return ((ushort)(this["OPFContPercentage"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("80")]
        public ushort OPFActiveDirectoryPropertiesPercentage {
            get {
                return ((ushort)(this["OPFActiveDirectoryPropertiesPercentage"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("False")]
        public bool PwnedLocalDbByHash {
            get {
                return ((bool)(this["PwnedLocalDbByHash"]));
            }
        }
    }
}
