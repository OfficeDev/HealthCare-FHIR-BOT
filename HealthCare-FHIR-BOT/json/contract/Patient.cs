// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HealthCare.FHIR.BOT.json.PatientContract
{
    public class Patient
    {
    }

    public class Meta
    {
        public DateTime lastUpdated { get; set; }
    }

    public class Link
    {
        public string relation { get; set; }
        public string url { get; set; }
    }

    public class Coding
    {
        public string system { get; set; }
        public string code { get; set; }
        public string display { get; set; }
    }

    public class Type
    {
        public List<Coding> coding { get; set; }
        public string text { get; set; }
    }

    public class Identifier
    {
        public string system { get; set; }
        public string value { get; set; }
        public Type type { get; set; }
    }

    public class Name
    {
        public string use { get; set; }
        public string family { get; set; }
        public List<string> given { get; set; }
    }

    public class Telecom
    {
        public string system { get; set; }
        public string value { get; set; }
        public string use { get; set; }
    }

    public class Extension2
    {
        public string url { get; set; }
        public double valueDecimal { get; set; }
    }

    public class Extension
    {
        public string url { get; set; }
        public List<Extension2> extension { get; set; }
    }

    public class Address
    {
        public List<Extension> extension { get; set; }
        public List<string> line { get; set; }
        public string city { get; set; }
        public string state { get; set; }
        public string postalCode { get; set; }
        public string country { get; set; }
    }

    public class Coding2
    {
        public string system { get; set; }
        public string code { get; set; }
        public string display { get; set; }
    }

    public class MaritalStatus
    {
        public List<Coding2> coding { get; set; }
        public string text { get; set; }
    }

    public class Coding3
    {
        public string system { get; set; }
        public string code { get; set; }
        public string display { get; set; }
    }

    public class Language
    {
        public List<Coding3> coding { get; set; }
        public string text { get; set; }
    }

    public class Communication
    {
        public Language language { get; set; }
    }

    public class Resource
    {
        public string resourceType { get; set; }
        public string id { get; set; }
        public List<Identifier> identifier { get; set; }
        public List<Name> name { get; set; }
        public List<Telecom> telecom { get; set; }
        public string gender { get; set; }
        public string birthDate { get; set; }
        public List<Address> address { get; set; }
        public MaritalStatus maritalStatus { get; set; }
        public List<Communication> communication { get; set; }
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

    public class PatientRoot
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