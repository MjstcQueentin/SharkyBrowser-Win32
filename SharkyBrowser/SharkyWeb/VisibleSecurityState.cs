using System.Text.Json.Serialization;

namespace SharkyBrowser.SharkyWeb
{
    public class VisibleSecurityStateChangedEventArgs
    {
        [JsonPropertyName("visibleSecurityState")]
        public VisibleSecurityState VisibleSecurityState { get; set; }
    }

    public class VisibleSecurityState
    {
        [JsonPropertyName("securityState")]
        public string SecurityState { get; set; }

        [JsonPropertyName("certificateSecurityState")]
        public CertificateSecurityState CertificateSecurityState { get; set; }

        [JsonPropertyName("safetyTipInfo")]
        public SafetyTipInfo SafetyTipInfo { get; set; }

        [JsonPropertyName("securityStateIssueIds")]
        public string[] SecurityStateIssueIDs { get; set; }
    }

    /// <summary>
    /// https://chromedevtools.github.io/devtools-protocol/tot/Security/#type-CertificateSecurityState
    /// </summary>
    public class CertificateSecurityState
    {
        [JsonPropertyName("protocol")]
        public string Protocol { get; set; }

        [JsonPropertyName("keyExchange")]
        public string KeyExchange { get; set; }

        [JsonPropertyName("keyExchangeGroup")]
        public string KeyExchangeGroup { get; set; }

        [JsonPropertyName("cipher")]
        public string Cipher { get; set; }

        [JsonPropertyName("mac")]
        public string Mac { get; set; }

        [JsonPropertyName("certificate")]
        public string[] Certificate { get; set; }

        [JsonPropertyName("subjectName")]
        public string SubjectName { get; set; }

        [JsonPropertyName("issuer")]
        public string Issuer { get; set; }

        [JsonPropertyName("validFrom")]
        public double ValidFrom { get; set; }

        [JsonPropertyName("validTo")]
        public double ValidTo { get; set; }

        [JsonPropertyName("certificateNetworkError")]
        public string CertificateNetworkError { get; set; }

        [JsonPropertyName("certificateHasWeakSignature")]
        public bool CertificateHasWeakSignature { get; set; }

        [JsonPropertyName("certificateHasSha1Signature")]
        public bool CertificateHasSha1Signature { get; set; }

        [JsonPropertyName("modernSSL")]
        public bool ModernSSL { get; set; }

        [JsonPropertyName("obsoleteSslProtocol")]
        public bool ObsoleteSslProtocol { get; set; }

        [JsonPropertyName("obsoleteSslKeyExchange")]
        public bool ObsoleteSslKeyExchange { get; set; }

        [JsonPropertyName("obsoleteSslCipher")]
        public bool ObsoleteSslCipher { get; set; }

        [JsonPropertyName("obsoleteSslSignature")]
        public bool ObsoleteSslSignature { get; set; }
    }

    /// <summary>
    /// SafetyTipInfo
    /// </summary>
    /// <see cref="https://chromedevtools.github.io/devtools-protocol/tot/Security/#type-SafetyTipInfo"/>
    /// <seealso cref="https://chromedevtools.github.io/devtools-protocol/tot/Security/#type-SafetyTipInfo"/>
    public class SafetyTipInfo
    {
        [JsonPropertyName("safetyTipStatus")]
        public string SafetyTipStatus { get; set; }

        [JsonPropertyName("safeUrl")]
        public string SafeUrl { get; set; }
    }
}
