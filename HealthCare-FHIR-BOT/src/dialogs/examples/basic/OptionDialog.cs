// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using AdaptiveCards;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using HealthCare.FHIR.BOT.json.ImmunizationContract;
using HealthCare.FHIR.BOT.json.AllergyContract;
using HealthCare.FHIR.BOT.json.ImmunizationPostContract;
using HealthCare.FHIR.BOT.Properties;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Text;
using System.Net.Http.Headers;
using HealthCare.FHIR.BOT.json.PatientContract;
using System.Linq;
using System.Text.RegularExpressions;

namespace HealthCare.FHIR.BOT.Dialogs
{
    /// <summary>
    /// This is Adaptive Card Dialog Class. Main purpose of this class is to display the Adaptive Card example
    /// </summary>

    [Serializable]
    public class OptionDialog : IDialog<object>
    {
        private static string updateImmunization;
        private static PatientRoot PatientDetails;
        private static string FIR_Base_URL = System.Configuration.ConfigurationManager.AppSettings["FIR_Base_URL"];

        public OptionDialog()
        {
        }

        public async Task StartAsync(IDialogContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            // Set the Last Dialog in Conversation Data
            context.UserData.SetValue(Strings.LastDialogKey, Strings.LastDialogAdaptiveCard);

            Activity activity = context.Activity as Activity;

            // Hard code patient id for demo purpose, change it to receive from teams message.
            if (activity.Text.Any(char.IsDigit))
            {
                activity.Text = Regex.Match(activity.Text, @"\d+").Value;
            }
            else
            {
                activity.Text = GetNewPatientRecord();
            }

            // If request is from submit action, process adaptive card values
            if (IsActivityFromAdaptiveCard(activity))
            {
                if (!String.IsNullOrEmpty(updateImmunization))
                {
                    AddNewImmunization(updateImmunization, activity.Text);
                }
            }

            var message = context.MakeMessage();
            var attachment = GetAdaptiveCardAttachment(context, activity);
            if (attachment != null)
            {
                message.Attachments.Add(attachment);
                await context.PostAsync((message));
            }
            else
            {
                message.Text = "Operation can’t be completed as there is No patient with ID : " + activity.Text;
                await context.PostAsync((message));
            }
                
            context.Done<object>(null);
        }

        public static Attachment GetAdaptiveCardAttachment(IDialogContext context, Activity activity)
        {
            // Initialize the patient information
            bool successful = GetPatientRecord(activity.Text);

            if (!successful)
            {
                // If the patient doesn't exist, return no return message
                return null;

            }

            // Initialize the Immunization record.
            string immunizationRecord = GetImmunizationRecord(activity.Text);

            // Initialize the Allergy record.
            string allergyRecord = GetAllergyRecord(activity.Text);

            var card = new AdaptiveCard()
            {
                Body = new List<AdaptiveElement>()
                {
                    new AdaptiveContainer()
                    {
                        Items = new List<AdaptiveElement>()
                        {
                            // TextBlock Item allows for the inclusion of text, with various font sizes, weight and color
                            new AdaptiveTextBlock()
                            {
                                Text = "Patient Details:",
                                Weight = AdaptiveTextWeight.Bolder, // set the weight of text e.g. Bolder, Light, Normal
                                Size = AdaptiveTextSize.Large, // set the size of text e.g. Extra Large, Large, Medium, Normal, Small
                            },
                            new AdaptiveColumnSet()
                            {
                                Separator = true,
                                Columns = new List<AdaptiveColumn>()
                                {
                                    // defines a container that is part of a column set
                                    new AdaptiveColumn()
                                    {
                                        Width = "30",
                                        Items = new List<AdaptiveElement>()
                                        {
                                            new AdaptiveImage()
                                            {
                                                Separator = true,
                                                Url = new System.Uri("https://i.pinimg.com/originals/51/f6/fb/51f6fb256629fc755b8870c801092942.png"),
                                                Size = AdaptiveImageSize.Small,
                                                Style = AdaptiveImageStyle.Person
                                            }
                                        }
                                    },
                                    new AdaptiveColumn()
                                    {
                                        Separator = true,
                                        Width = "70",
                                        Items = new List<AdaptiveElement>()
                                        {
                                            new AdaptiveTextBlock()
                                            {
                                                Text = PatientDetails.entry[0].resource.name[0].given[0].ToString() + " " + PatientDetails.entry[0].resource.name[0].family,
                                                Size = AdaptiveTextSize.Small,
                                                Weight = AdaptiveTextWeight.Lighter,
                                                HorizontalAlignment = AdaptiveHorizontalAlignment.Center

                                            },
                                        }
                                    }
                                }
                            },
                            // Adaptive FactSet item makes it simple to display a series of facts (e.g. name/value pairs) in a tabular form
                            new AdaptiveFactSet
                            {
                                Separator = true,
                                Facts =
                                {
                                    // Describes a fact in a Adaptive FactSet as a key/value pair
                                    new AdaptiveFact
                                    {
                                        Title = "Gender:",
                                        Value = ToUpperFirstLetter(PatientDetails.entry[0].resource.gender != null ? PatientDetails.entry[0].resource.gender:"")
                                    },
                                    new AdaptiveFact
                                    {
                                        Title = "Birth:",
                                        Value = PatientDetails.entry[0].resource.birthDate != null ? PatientDetails.entry[0].resource.birthDate:""
                                    },
                                    new AdaptiveFact
                                    {
                                        Title = "MR:",
                                        Value = (PatientDetails.entry[0].resource.identifier != null &&  PatientDetails.entry[0].resource.identifier.Count > 1) ?
                                        PatientDetails.entry[0].resource.identifier[1].value: ""
                                    },
                                }
                            },

                        }
                    }
                },
                Actions = new List<AdaptiveAction>()
                {              
                 new AdaptiveShowCardAction()
                    {
                        Title = "Display Allergy Intolerance of the Patient",
                        Card = new AdaptiveCard()
                        {
                            Version = "1.0",
                            Body = new List<AdaptiveElement>()
                            {
                                new AdaptiveContainer()
                                {
                                    Items = new List<AdaptiveElement>()
                                    {
                                        new AdaptiveTextBlock()
                                        {
                                            Wrap = true,
                                            Text = allergyRecord,
                                            Size = AdaptiveTextSize.Small
                                        },
                                    }
                                }
                            },

                        }
                    },
                    new AdaptiveShowCardAction()
                    {
                        Title = "Display Immunization records of the Patient",
                        Card = new AdaptiveCard()
                        {
                            Version = "1.0",
                            Body = new List<AdaptiveElement>()
                            {
                                new AdaptiveContainer()
                                {
                                    Items = new List<AdaptiveElement>()
                                    {
                                        new AdaptiveTextBlock()
                                        {
                                            Wrap = true,
                                            Text = immunizationRecord,
                                            Size = AdaptiveTextSize.Small,
                                        },
                                    }
                                }
                            },

                        }
                    },

                    new AdaptiveShowCardAction()
                    {
                        Title = "Add Immunization record of the Patient to R4 FHIR",
                        Card = new AdaptiveCard()
                        {
                            Version = "1.0",
                            Body = new List<AdaptiveElement>()
                            {
                                new AdaptiveContainer()
                                {
                                    Items = new List<AdaptiveElement>()
                                    {
                                         new AdaptiveChoiceSetInput()
                                         {
                                             Separator = true,
                                             Id = "choiceSetCompact",
                                             Value = "HPV, quadrivalent", // please set default value here
                                             Style = AdaptiveChoiceInputStyle.Compact, // set the style of Choice set to compact
                                             Choices =
                                                {
                                                    // describes a choice input. the value should be a simple string without a ","
                                                    new AdaptiveChoice
                                                    {
                                                        Title = "HPV, quadrivalent",
                                                        Value = "HPV, quadrivalent" // do not use a “,” in the value, since MultiSelect ChoiceSet returns a comma-delimited string of choice values
                                                    },
                                                    new AdaptiveChoice
                                                    {
                                                        Title = "Influenza unspecified formulation",
                                                        Value = "Influenza unspecified formulation"
                                                    },
                                                    new AdaptiveChoice
                                                    {
                                                        Title = "Influenza, seasonal, injectable, preservative free",
                                                        Value = "Influenza, seasonal, injectable, preservative free"
                                                    },
                                                    new AdaptiveChoice
                                                    {
                                                        Title = "Meningococcal MCV4P",
                                                        Value = "Meningococcal MCV4P"
                                                    }
                                                },
                                         }
                                    }
                                }
                            },
                            Actions = new List<AdaptiveAction>()
                            {
                              new AdaptiveSubmitAction()
                              {
                                  Title = "Submit",
                                  DataJson = "{\"isFromAdaptiveCard\": \"true\", \"messageText\": \"" + activity.Text + "\", \"msteams\":{\"type\":\"messageBack\", \"displayText\":\"New Immunization submitted!\"}}",
                              }
                            }

                        }
                    },
                }
            };


            // Add separator line
            card.Body.Add(
                  new AdaptiveTextBlock()
                  {
                      Separator = true,
                      Text = "Choose your operation:"
                  }
                );


            Attachment attachment = new Attachment()
            {
                ContentType = AdaptiveCard.ContentType,
                Content = card
            };

            return attachment;
        }


        /// <summary>
        /// Check if submit action request is from an adaptive card
        /// </summary>
        /// <param name="activity"></param>
        /// <returns></returns>
        public static bool IsActivityFromAdaptiveCard(Activity activity)
        {
            // Check for the property in the value set by the adaptive card submit action
            if (activity.ReplyToId != null && activity?.Value != null)
            {
                JObject jsonObject = activity.Value as JObject;

                if (jsonObject != null && jsonObject.Count > 0)
                {
                    string isFromAdaptiveCard = Convert.ToString(jsonObject["isFromAdaptiveCard"]);

                    if (jsonObject["choiceSetCompact"] != null)
                    {
                        updateImmunization = Convert.ToString(jsonObject["choiceSetCompact"]);
                    }
                    if (!string.IsNullOrEmpty(isFromAdaptiveCard) && isFromAdaptiveCard == "true")
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        ///// <summary>
        ///// Handle adaptive card request
        ///// </summary>
        ///// <param name="activity"></param>
        ///// <returns></returns>
        private async Task SendAdaptiveCardValues(IDialogContext context, Activity activity)
        {
            var submitValue = context.MakeMessage();
            submitValue.Text = Convert.ToString(activity.Value);
            await context.PostAsync(submitValue);
        }

        private static string GetImmunizationRecord(string PatientId)
        {
            string immunizationRecord = String.Empty;
            List<string> immuList = new List<string>();
            // Get patient immunization record 
            var client = new HttpClient() { Timeout = TimeSpan.FromSeconds(5) };
            try
            {
                var immuSteamDetailsJson = client.GetStringAsync(FIR_Base_URL + "Immunization?patient=Patient/" + PatientId).GetAwaiter().GetResult();

                // Deserialize Immunization response
                var immuSteamDetails = JsonConvert.DeserializeObject<ImmuRoot>(immuSteamDetailsJson);
                int count = 1;

                if (immuSteamDetails.entry == null)
                {
                    throw new Exception();
                }

                foreach (var entry in immuSteamDetails.entry)
                {
                    if (entry != null) 
                        immuList.Add(entry.resource.vaccineCode.text);
                }

                immuList = immuList.Distinct().ToList();

                foreach (string immunization in immuList)
                {
                    immunizationRecord += count.ToString() + "." + " " + immunization;
                    immunizationRecord += Environment.NewLine;
                    immunizationRecord += Environment.NewLine;
                    count++;
                }

                if (String.IsNullOrEmpty(immunizationRecord))
                    immunizationRecord += "No immunization record is availble for this Patient";

            }
            catch (Exception)
            {
                immunizationRecord += "No immunization record is availble for this Patient";
            }

            return immunizationRecord;
        }

        private static string GetAllergyRecord(string PatientId)
        {
            string allergyRecord = String.Empty;
            List<string> allergyList = new List<string>();

            // Get patient Allergy Intolerance 
            var client = new HttpClient() { Timeout = TimeSpan.FromSeconds(5) };
            try
            {
                var mrSteamDetailsJson = client.GetStringAsync(FIR_Base_URL + "AllergyIntolerance?patient=Patient/" + PatientId).GetAwaiter().GetResult();

                // Deserialize MR response
                var mrSteamDetails = JsonConvert.DeserializeObject<AllergyRoot>(mrSteamDetailsJson);

                int count = 1;

                if (mrSteamDetails?.entry == null)
                {
                    throw new Exception();
                }

                foreach (var entry in mrSteamDetails.entry)
                {
                    if (entry.resource.reaction[0].substance != null && entry.resource.reaction[0].substance.text != null)
                        allergyList.Add(entry.resource.reaction[0].substance.text);
                }

                allergyList = allergyList.Distinct().ToList();

                foreach (string allergy in allergyList)
                {
                    allergyRecord += count.ToString() + "." + " " + allergy;
                    allergyRecord += Environment.NewLine;
                    allergyRecord += Environment.NewLine;
                    count++;
                }

                if (String.IsNullOrEmpty(allergyRecord))
                    allergyRecord += "No Allergy intolerance record is availble for this Patient";
            }
            catch (Exception)
            {
                allergyRecord += "No Allergy intolerance record is availble for this Patient";
            }

            return allergyRecord;
        }

        private static void AddNewImmunization(string newImmunization, string PatientId)
        {
            string resourceFile = HttpContext.Current.Server.MapPath("~");
            var postImmuBodyJson = File.ReadAllText(resourceFile + "/json/newImmunization.json");

            ImmuPostRoot postImmuBodyDeserialized = JsonConvert.DeserializeObject<ImmuPostRoot>(postImmuBodyJson);
            postImmuBodyDeserialized.vaccineCode.text = newImmunization;
            postImmuBodyDeserialized.patient.reference = "Patient/" + PatientId;
            postImmuBodyDeserialized.meta.lastUpdated = DateTime.Now;

            using (var client = new HttpClient { Timeout = TimeSpan.FromSeconds(5) })
            {
                string myContent = JsonConvert.SerializeObject(postImmuBodyDeserialized);
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                var builder = new UriBuilder(new Uri(FIR_Base_URL + "Immunization/"));
                HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, builder.Uri);
                request.Content = new StringContent(myContent, Encoding.UTF8, "application/json");
                HttpResponseMessage response = client.SendAsync(request).GetAwaiter().GetResult();
            };
        }

        private static bool GetPatientRecord(string PatientId)
        {
            // Get patient inforamtion
            var client = new HttpClient() { Timeout = TimeSpan.FromSeconds(5) };
            try
            {
                var MRSteamDetailsJson = client.GetStringAsync(FIR_Base_URL + "Patient?_id=" + PatientId).GetAwaiter().GetResult();
                // Deserialize MR response
                PatientDetails = JsonConvert.DeserializeObject<PatientRoot>(MRSteamDetailsJson);
                if (PatientDetails.entry == null)
                {
                    return false;
                }
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        private static string GetNewPatientRecord()
        {
            // Get new Patient record
            var client = new HttpClient() { Timeout = TimeSpan.FromSeconds(5) };
            try
            {
                var MRSteamDetailsJson = client.GetStringAsync(FIR_Base_URL + "Patient?_sort=-_id&&_count=1").GetAwaiter().GetResult();
                // Deserialize MR response
                PatientDetails = JsonConvert.DeserializeObject<PatientRoot>(MRSteamDetailsJson);
                if (PatientDetails.entry == null)
                {
                    return String.Empty;
                }
                return PatientDetails.entry[0].resource.id;
            }
            catch (Exception)
            {
                return String.Empty;
            }
        }

        private static string ToUpperFirstLetter(string source)
        {
            if (string.IsNullOrEmpty(source))
                return string.Empty;
            // convert to char array of the string
            char[] letters = source.ToCharArray();
            // upper case the first char
            letters[0] = char.ToUpper(letters[0]);
            // return the array made of the new char array
            return new string(letters);
        }
    }
}