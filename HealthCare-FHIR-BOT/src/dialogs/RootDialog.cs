// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Scorables;
using Microsoft.Bot.Connector;
using Microsoft.Bot.Connector.Teams.Models;
using HealthCare.FHIR.BOT.Properties;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HealthCare.FHIR.BOT.Dialogs
{
    /// <summary>
    /// This is Root Dialog, its a triggring point for every Child dialog based on the RexEx Match with user input command
    /// </summary>

    [Serializable]
    public class RootDialog : DispatchDialog
    {
        #region Help Dialog

        [MethodBind]
        [ScorableGroup(2)]
        public void Default(IDialogContext context, IActivity activity)
        {
            context.Call(new OptionDialog(), this.EndDialog);
        }

        public Task EndDefaultDialog(IDialogContext context, IAwaitable<object> result)
        {
            context.Done<object>(null);
            return Task.CompletedTask;
        }

        public Task EndDialog(IDialogContext context, IAwaitable<object> result)
        {
            context.Done<object>(null);
            return Task.CompletedTask;
        }

        #endregion
    }
}