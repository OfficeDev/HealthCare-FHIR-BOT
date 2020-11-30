# FHIR Teams Bot

This project is a reference implementation using HAPI FHIR project : http://hapi.fhir.org/ to help FHIR developers integrate their endpoint and get/update demographic information using Teams Bot.
This sample exercise following scenarios:
1.  Display Allergy Intolerance of the Patient.
2.  Display Immunization records of the Patient
3.  Add Immunization record of the Patient to UHN_HAPI Server (R4 FHIR)

## Prerequisites

* Install Git for windows: https://git-for-windows.github.io/

* Clone this repo:<br>
    ```bash
    git clone https://github.com/OfficeDev/HealthCare-FHIR-BOT.git
    ```

* Install Visual Studio and launch it as an administrator

* Build the solution to download all configured NuGet packages

* (Only needed if wanting to run in Microsoft Teams)<br>
Install some sort of tunnelling service. These instructions assume you are using ngrok: https://ngrok.com/

* (Only needed if wanting to run in the Bot Emulator)<br>
Download and install the Bot Emulator from https://github.com/microsoft/BotFramework-Emulator/releases/latest
    * NOTE: make sure to pin the emulator to your task bar because it can sometimes be difficult to find again

## Register your Bot in Azure

1. Follow the steps in this article to create a Bot service: https://docs.microsoft.com/en-us/azure/bot-service/abs-quickstart?view=azure-bot-service-4.0
    * NOTE: when choosing your Bot Template choose a .net/C# template.

2. In the Bot Management section, click Channels. Add a TEAMS channel

3. Once you are done above steps, you should have a bot service url like https://<servicename>.azurewebsites.net/api/messages.


## Update values in web.config of your Healthcare-FHIR-BOT solution

1. In Visual Studio, open the Web.config file. Locate the `<appSettings>` section.

2. Enter the BotId value. The BotId is the **Bot handle** from the **Configuration** section of the bot registration.

3. Enter the MicrosoftAppId. The MicrosoftAppId is the app ID from the **Configuration** section of the bot registration.

4. Enter the MicrosoftAppPassword. The MicrosoftAppPassword is the auto-generated app password displayed in the pop-up during bot registration.

5. Enter the BaseUri. The BaseUri is the Messaging endpoint of your bot.

	Here is an example for reference:

		<add key="BotId" value="Bot_Handle_Here" />
		<add key="MicrosoftAppId" value="88888888-8888-8888-8888-888888888888" />
		<add key="MicrosoftAppPassword" value="aaaa22229999dddd0000999" />
		<add key="BaseUri" value="https://xxxxx.azurewebsites.net/api/messages" />


## Publish your solution to Azure

When you register your Bot, a corresponding App Service with same name as Bot is created. You should use that App Service to publish your solution.

1. In Visual Studio, click Build -> Publish HealthCare-FHIR-BOT

2. Select existing App Service with same name as your Bot service

3. Click Publish

## Steps to sideload the app in Microsoft Teams

1. To sideload, a manifest file is needed:
    * On the solution explorer of Visual Studio, navigate to the file, manifest/manifest.json - change:
        * `botId` change to your registered bot's app ID, which is found in the Configuration tab of your App Service under "MicrosoftAppId"

    * Save the file and zip this file and the HealthCare.png file (located next to it) together to create a manifest.zip file

2. Once completed, sideload your zipped manifest to a team as described here (open in a new browser tab): https://docs.microsoft.com/en-us/microsoftteams/platform/concepts/deploy-and-publish/apps-upload

Congratulations!!! You have just sideloaded your HealthCare-FHIR-BOT!

## Steps to interact with the Bot

After uploading of manifest succeeds, you can interact with the Bot in two ways: 

In 1:1 conversation: 
1. Open Microsoft Teams, click Apps icon in the bottom left panel.
2. Find out your 'HealthCare-FHIR-BOT' App, click 'Open'.
3. From the Teams message box, you can chose your command by click "What Can I do?" list Or you can specify the Patient id that you know to get the patient information. e,g "Patient details by id 1474470"

   ![](/HealthCare-FHIR-BOT/src/gif/scenario1.gif)

In Teams Meeting: 
1. Join Teams meeting
2. Find out your 'HealthCare-FHIR-BOT' App, click 'Add'.
3. Invoke the Bot by using @HealthCareFhirBot command

   ![](/HealthCare-FHIR-BOT/src/gif/scenario2.gif)

## (Optional) For local debugging follow these steps:

NOTE: Teams does not work nor render things exactly like the Bot Emulator, but it is a quick way to see if your bot is running and functioning correctly.

1. Open the HealthCare-FHIR-Bot.sln solution with Visual Studio

2. In Visual Studio click the play button (should be defaulted to running the Microsoft Edge configuration) 

3. Once the code is running, connect with the Bot Emulator to the default endpoint, "http://localhost:3979/api/messages", fill the values of "MicrosoftAppID" and "MicrosoftAppPassword" from web.config.

## Trademark 

This project may contain trademarks or logos for projects, products, or services. Authorized use of Microsoft trademarks or logos is subject to and must follow Microsoft's Trademark & Brand Guidelines. Use of Microsoft trademarks or logos in modified versions of this project must not cause confusion or imply Microsoft sponsorship. Any use of third-party trademarks or logos are subject to those third-party's policies

## Contributing

    THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
    IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
    FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
    AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
    LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
    OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
    SOFTWARE. IT IS THE SOLE RESPONSIBILITY OF THOSE WHO USE THIS CODE TO ADHERE TO 
    AND COMPLIY WITH ALL APPLICIBLE LAWS AND REGULATIONS

This project welcomes contributions and suggestions.  Most contributions require you to agree to a
Contributor License Agreement (CLA) declaring that you have the right to, and actually do, grant us
the rights to use your contribution. For details, visit https://cla.microsoft.com.

This project has adopted the [Microsoft Open Source Code of Conduct](https://opensource.microsoft.com/codeofconduct/).
For more information see the [Code of Conduct FAQ](https://opensource.microsoft.com/codeofconduct/faq/) or
contact [opencode@microsoft.com](mailto:opencode@microsoft.com) with any additional questions or comments.
