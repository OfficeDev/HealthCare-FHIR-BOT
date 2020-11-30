// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;

namespace HealthCare.FHIR.BOT.json.AllergyContract
{
    public class Meta
    {
        public DateTime lastUpdated { get; set; }
    }

    public class Link
    {
        public string relation { get; set; }
        public string url { get; set; }
    }

    public class Meta2
    {
        public string versionId { get; set; }
        public DateTime lastUpdated { get; set; }
        public string source { get; set; }
    }

    public class Coding
    {
        public string system { get; set; }
        public string code { get; set; }
        public string display { get; set; }
    }

    public class ClinicalStatus
    {
        public List<Coding> coding { get; set; }
    }

    public class Coding2
    {
        public string system { get; set; }
        public string code { get; set; }
        public string display { get; set; }
    }

    public class VerificationStatus
    {
        public List<Coding2> coding { get; set; }
    }

    public class Patient
    {
        public string reference { get; set; }
    }

    public class Coding3
    {
        public string system { get; set; }
        public string code { get; set; }
        public string display { get; set; }
    }

    public class Substance
    {
        public List<Coding3> coding { get; set; }
        public string text { get; set; }
    }

    public class Coding4
    {
        public string system { get; set; }
        public string code { get; set; }
        public string display { get; set; }
    }

    public class Manifestation
    {
        public List<Coding4> coding { get; set; }
    }

    public class Reaction
    {
        public Substance substance { get; set; }
        public List<Manifestation> manifestation { get; set; }
        public string onset { get; set; }
    }

    public class Resource
    {
        public string resourceType { get; set; }
        public string id { get; set; }
        public Meta2 meta { get; set; }
        public ClinicalStatus clinicalStatus { get; set; }
        public VerificationStatus verificationStatus { get; set; }
        public List<string> category { get; set; }
        public string criticality { get; set; }
        public Patient patient { get; set; }
        public string onsetDateTime { get; set; }
        public List<Reaction> reaction { get; set; }
    }

    public class Search
    {
        public string mode { get; set; }
    }

    public class Entry
    {
        public string fullUrl { get; set; }
        public Resource resource { get; set; }
        public Search search { get; set; }
    }

    public class AllergyRoot
    {
        public string resourceType { get; set; }
        public string id { get; set; }
        public Meta meta { get; set; }
        public string type { get; set; }
        public int total { get; set; }
        public List<Link> link { get; set; }
        public List<Entry> entry { get; set; }
    }

}