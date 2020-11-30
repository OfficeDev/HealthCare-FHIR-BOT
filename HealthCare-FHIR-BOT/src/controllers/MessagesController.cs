// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Autofac;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Dialogs.Internals;
using Microsoft.Bot.Connector;
using Microsoft.Bot.Connector.Teams;
using Microsoft.Bot.Connector.Teams.Models;
using HealthCare.FHIR.BOT.Dialogs;
using HealthCare.FHIR.BOT.Properties;
using HealthCare.FHIR.BOT.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;

namespace HealthCare.FHIR.BOT
{
    [BotAuthentication]
    public class MessagesController : ApiController
    {
        /// <summary>
        /// POST: api/Messages
        /// Receive a message from a user and reply to it
        /// </summary>
        public async Task<HttpResponseMessage> Post([FromBody]Activity activity, CancellationToken cancellationToken)
        {
            var connectorClient = new ConnectorClient(new Uri(activity.ServiceUrl));

            if (activity.Type == ActivityTypes.Message)
            {
                // Special handling for a command to simulate a reset of the bot chat
                if (!(activity.Conversation.IsGroup ?? false) && (activity.Text == "/resetbotchat"))
                {
                    return await HandleResetBotChatAsync(activity, cancellationToken);
                }

                //Set the Locale for Bot
                activity.Locale = TemplateUtility.GetLocale(activity);

                //Strip At mention from incoming request text
                activity = Middleware.StripAtMentionText(activity);

                //Convert incoming activity text to lower case, to match the intent irrespective of incoming text case
                activity = Middleware.ConvertActivityTextToLower(activity);             

                // Set activity text if request is from an adaptive card submit action
                activity = Middleware.AdaptiveCardSubmitActionHandler(activity);

                
                await Conversation.SendAsync(activity, () => new RootDialog());
           
            }
            else if (activity.Type == ActivityTypes.MessageReaction)
            {
                var reactionsAdded = activity.ReactionsAdded;
                var reactionsRemoved = activity.ReactionsRemoved;
                var replytoId = activity.ReplyToId;
                Activity reply;

                if (reactionsAdded != null && reactionsAdded.Count > 0)
                {
                    reply = activity.CreateReply(Strings.LikeMessage);
                    await connectorClient.Conversations.ReplyToActivityAsync(reply);
                }
                else if (reactionsRemoved != null && reactionsRemoved.Count > 0)
                {
                    reply = activity.CreateReply(Strings.RemoveLike);
                    await connectorClient.Conversations.ReplyToActivityAsync(reply);
                }

                return Request.CreateResponse(HttpStatusCode.OK);
            }
            else if (activity.Type == ActivityTypes.Invoke) // Received an invoke
            {
                // Handle ComposeExtension query
                if (activity.IsComposeExtensionQuery())
                {
                    //WikipediaComposeExtension wikipediaComposeExtension = new WikipediaComposeExtension();
                    HttpResponseMessage httpResponse = null;

                    using (var scope = DialogModule.BeginLifetimeScope(Conversation.Container, activity))
                    {
                        var botDataStore = scope.Resolve<IBotDataStore<BotData>>();
                        // Handle compose extension selected item
                        if (activity.Name == "composeExtension/selectItem")
                        {
                            // This handler is used to process the event when a user in Teams selects wiki item from wiki result
                            //ComposeExtensionResponse selectedItemResponse = await wikipediaComposeExtension.HandleComposeExtensionSelectedItem(activity, botDataStore);
                            //httpResponse = Request.CreateResponse<ComposeExtensionResponse>(HttpStatusCode.OK, selectedItemResponse);
                        }
                        else
                        {
                            // Handle the wiki compose extension request and returned the wiki result response
                            //ComposeExtensionResponse composeExtensionResponse = await wikipediaComposeExtension.GetComposeExtensionResponse(activity, botDataStore);
                            //httpResponse = Request.CreateResponse<ComposeExtensionResponse>(HttpStatusCode.OK, composeExtensionResponse);
                        }

                        var address = Address.FromActivity(activity);
                        await botDataStore.FlushAsync(address, CancellationToken.None);
                    }
                    return httpResponse;
                }
                // Handle rest of the invoke request
                else
                {
                    var messageActivity = (IMessageActivity)null;

                    //this will parse the invoke value and change the message activity as well
                    messageActivity = InvokeHandler.HandleInvokeRequest(activity);

                    await Conversation.SendAsync(messageActivity, () => new Dialogs.RootDialog());

                    return Request.CreateResponse(HttpStatusCode.OK);
                }
            }
            else
            {
                await HandleSystemMessageAsync(activity, connectorClient, cancellationToken);
            }

            var response = Request.CreateResponse(HttpStatusCode.OK);

            return response;
        }

        private async Task HandleSystemMessageAsync(Activity message, ConnectorClient connectorClient, CancellationToken cancellationToken)
        {
            if (message.Type == ActivityTypes.DeleteUserData)
            {
                // Implement user deletion here
                // If we handle user deletion, return a real message
            }
            else if (message.Type == ActivityTypes.ConversationUpdate)
            {
                // This shows how to send a welcome message in response to a conversationUpdate event

                // We're only interested in member added events
                if (message.MembersAdded?.Count > 0)
                {
                    // Determine if the bot was added to the team/conversation
                    var botId = message.Recipient.Id;
                    var botWasAdded = message.MembersAdded.Any(member => member.Id == botId);

                    // Create the welcome message to send
                    Activity welcomeMessage = message.CreateReply();
                    welcomeMessage.Text = Strings.BotWelcomeMessage;

                    if (!(message.Conversation.IsGroup ?? false))
                    {
                        // 1:1 conversation event

                        // If the user hasn't received a first-run message yet, then send a message to the user
                        // introducing your bot and what it can do. Do NOT send this blindly, as your bot can receive
                        // spurious conversationUpdate events, especially if you use proactive messaging.
                        using (var scope = DialogModule.BeginLifetimeScope(Conversation.Container, message))
                        {
                            var address = Address.FromActivity(message);
                            var botDataStore = scope.Resolve<IBotDataStore<BotData>>();
                            var botData = await botDataStore.LoadAsync(address, BotStoreType.BotUserData, cancellationToken);

                            if (!botData.GetProperty<bool>("IsFreSent"))
                            {
                                await connectorClient.Conversations.ReplyToActivityWithRetriesAsync(welcomeMessage, cancellationToken);

                                // Remember that we sent the welcome message already
                                botData.SetProperty("IsFreSent", true);
                                await botDataStore.SaveAsync(address, BotStoreType.BotUserData, botData, cancellationToken);
                            }
                            else
                            {
                                // First-run message has already been sent, so skip sending it again.
                                // Do not remove the check for IsFreSent above. Your bot can receive spurious conversationUpdate
                                // activities from chat service, so if you always respond to all of them, you will send random 
                                // welcome messages to users who have already received the welcome.
                            }
                        }
                    }
                    else
                    {
                        // Not 1:1 chat event (bot or user was added to a team or group chat)
                        if (botWasAdded)
                        {
                            // Bot was added to the team
                            // Send a message to the team's channel, introducing your bot and what you can do
                            await connectorClient.Conversations.ReplyToActivityWithRetriesAsync(welcomeMessage, cancellationToken);
                        }
                        else
                        {
                            // Other users were added to the team/conversation
                        }
                    }
                }
            }
            else if (message.Type == ActivityTypes.ContactRelationUpdate)
            {
                // Handle add/remove from contact lists
                // Activity.From + Activity.Action represent what happened
            }
            else if (message.Type == ActivityTypes.Typing)
            {
                // Handle knowing that the user is typing
            }
            else if (message.Type == ActivityTypes.Ping)
            {
            }
        }

        /// <summary>
        /// Handles a request from the user to simulate a new chat.
        /// </summary>
        /// <param name="message">The incoming message requesting the reset</param>
        /// <param name="cancellationToken">The cancellation token</param>
        /// <returns></returns>
        private async Task<HttpResponseMessage> HandleResetBotChatAsync(Activity message, CancellationToken cancellationToken)
        {
            // Forget everything we know about the user
            using (var scope = DialogModule.BeginLifetimeScope(Conversation.Container, message))
            {
                var address = Address.FromActivity(message);
                var botDataStore = scope.Resolve<IBotDataStore<BotData>>();
                await botDataStore.SaveAsync(address, BotStoreType.BotUserData, new BotData("*"), cancellationToken);
                await botDataStore.SaveAsync(address, BotStoreType.BotConversationData, new BotData("*"), cancellationToken);
                await botDataStore.SaveAsync(address, BotStoreType.BotPrivateConversationData, new BotData("*"), cancellationToken);
            }

            // If you need to reset the user state in other services your app uses, do it here.

            // Synthesize a conversation update event and simulate the bot receiving it
            // Note that this is a fake event, as Teams does not support deleting a 1:1 conversation and re-creating it
            var conversationUpdateMessage = new Activity {
                Type = ActivityTypes.ConversationUpdate,
                Id = message.Id,
                ServiceUrl = message.ServiceUrl,
                From = message.From,
                Recipient = message.Recipient,
                Conversation = message.Conversation,
                ChannelData = message.ChannelData,
                ChannelId = message.ChannelId,
                Timestamp = message.Timestamp,
                MembersAdded = new List<ChannelAccount> { message.From, message.Recipient },
            };
            return await this.Post(conversationUpdateMessage, cancellationToken);
        }

  
    }
}