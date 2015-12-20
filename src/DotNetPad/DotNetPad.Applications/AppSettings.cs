using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Waf.DotNetPad.Applications
{
    [DataContract]
    public sealed class AppSettings : IExtensibleDataObject
    {
        public AppSettings()
        {
            Initialize();
        }


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

        ExtensionDataObject IExtensibleDataObject.ExtensionData { get; set; }


        private void Initialize()
        {
            BottomPanesHeight = 75;
            LastOpenedFiles = new string[0];
        }

        [OnDeserializing]
        private void OnDeserializing(StreamingContext context)
        {
            Initialize();
        }
    }
}
