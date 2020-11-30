// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HealthCare.FHIR.BOT.json.ImmunizationContract
{
    // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse); 
    public class Meta
    {
        public DateTime lastUpdated { get; set; }
    }

    public class Link
    {
        public string relation { get; set; }
        public string url { get; set; }
    }

    public class Tag
    {
        public string system { get; set; }
        public string code { get; set; }
    }

    public class Meta2
    {
        public string versionId { get; set; }
        public DateTime lastUpdated { get; set; }
        public string source { get; set; }
        public List<string> profile { get; set; }
        public List<Tag> tag { get; set; }
    }

    public class Coding
    {
        public string system { get; set; }
        public string code { get; set; }
        public string display { get; set; }
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

    public class Encounter
    {
        public string reference { get; set; }
    }

    public class Location
    {
        public string reference { get; set; }
        public string display { get; set; }
    }

    public class Resource
    {
        public string resourceType { get; set; }
        public string id { get; set; }
        public Meta2 meta { get; set; }
        public string status { get; set; }
        public VaccineCode vaccineCode { get; set; }
        public Patient patient { get; set; }
        public Encounter encounter { get; set; }
        public DateTime occurrenceDateTime { get; set; }
        public bool primarySource { get; set; }
        public Location location { get; set; }
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

    public class ImmuRoot
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