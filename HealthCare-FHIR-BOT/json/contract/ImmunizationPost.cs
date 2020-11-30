// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HealthCare.FHIR.BOT.json.ImmunizationPostContract
{
    // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse); 
    public class Meta
    {
        public string versionId { get; set; }
        public DateTime lastUpdated { get; set; }
        public string source { get; set; }
    }

    public class Coding
    {
        public string system { get; set; }
        public string code { get; set; }
    }

    public class VaccineCode
    {
        public List<Coding> coding { get; set; }
        public string text { get; set; }
    }

    public class Patient
    {
        public string reference { get; set; }
    }

    public class ReasonCode
    {
        public string text { get; set; }
    }

    public class ImmuPostRoot
    {
        public string resourceType { get; set; }
        public string id { get; set; }
        public Meta meta { get; set; }
        public string status { get; set; }
        public VaccineCode vaccineCode { get; set; }
        public Patient patient { get; set; }
        public string occurrenceDateTime { get; set; }
        public List<ReasonCode> reasonCode { get; set; }
    }

}