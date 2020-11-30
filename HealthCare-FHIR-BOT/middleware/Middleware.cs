// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.Bot.Connector;
using Newtonsoft.Json.Linq;
using System;
using System.Configuration;

namespace HealthCare.FHIR.BOT.Utility
{
    public static partial class Middleware
    {
        public static string TenantFilterSettingAny = "#ANY#";
     
        public static Activity ConvertActivityTextToLower(Activity activity)
        {
            //Convert input command in lower case for 1To1 and Channel users
            if (activity.Text != null)
            {
                activity.Text = activity.Text.ToLower();
            }

            return activity;
        }
    }
}