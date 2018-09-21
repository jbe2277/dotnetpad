using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Waf.Applications.Services;

namespace Waf.DotNetPad.Applications.Properties
{
    [DataContract, KnownType(typeof(string[]))]
    public sealed class AppSettings : UserSettingsBase
    {
        [DataMember]
        public double Left { get; set; }

        [DataMember]
        public double Top { get; set; }

        [DataMember]
        public double Height { get; set; }

        [DataMember]
        public double Width { get; set; }

        [DataMember]
        public bool IsMaximized { get; set; }

        [DataMember]
        public double BottomPanesHeight { get; set; }

        [DataMember]
        public IReadOnlyList<string> LastOpenedFiles { get; set; }


        protected override void SetDefaultValues()
        {
            BottomPanesHeight = 75;
            LastOpenedFiles = Array.Empty<string>();
        }
    }
}
