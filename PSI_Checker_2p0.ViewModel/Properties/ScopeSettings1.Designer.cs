﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace PSI_Checker_2p0.ViewModel.Properties {
    
    
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.Editors.SettingsDesigner.SettingsSingleFileGenerator", "17.4.0.0")]
    internal sealed partial class ScopeSettings : global::System.Configuration.ApplicationSettingsBase {
        
        private static ScopeSettings defaultInstance = ((ScopeSettings)(global::System.Configuration.ApplicationSettingsBase.Synchronized(new ScopeSettings())));
        
        public static ScopeSettings Default {
            get {
                return defaultInstance;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("PXI1Slot2")]
        public string ScopeName {
            get {
                return ((string)(this["ScopeName"]));
            }
            set {
                this["ScopeName"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("")]
        public string SavingPath {
            get {
                return ((string)(this["SavingPath"]));
            }
            set {
                this["SavingPath"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("False")]
        public bool KeepRawData {
            get {
                return ((bool)(this["KeepRawData"]));
            }
            set {
                this["KeepRawData"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("0")]
        public double SamplingRateMin {
            get {
                return ((double)(this["SamplingRateMin"]));
            }
            set {
                this["SamplingRateMin"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("0")]
        public double RecordLengthMin {
            get {
                return ((double)(this["RecordLengthMin"]));
            }
            set {
                this["RecordLengthMin"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("0")]
        public double SamplingRate {
            get {
                return ((double)(this["SamplingRate"]));
            }
            set {
                this["SamplingRate"] = value;
            }
        }
    }
}
