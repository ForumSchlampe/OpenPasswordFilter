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
        [global::System.Configuration.DefaultSettingValueAttribute("False")]
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
        [global::System.Configuration.DefaultSettingValueAttribute("True")]
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
        [global::System.Configuration.DefaultSettingValueAttribute("Server=myServerAddress;Database=myDataBase;IntegratedSecurity=yes;Uid=auth_window" +
            "s;SslMode=Required;UseCompression=True;\r\n")]
        public string PwnedLocalMySQLDBConnString {
            get {
                return ((string)(this["PwnedLocalMySQLDBConnString"]));
            }
        }
    }
}
