# Youtube Livestream Chat

Unity and C# implementation of displaying a Youtube livestream chat, as well as additional features that can help with filtering or interactivity. Done with [Youtube Data API V3](https://developers.google.com/youtube/v3/), this can be used as an alternative Youtube chat displayer with a Google API Key and a video ID of the current livestream. Future iterations will give additional functionality that's currently lacking on Youtube.

For an idea on what could be done with this system, [I currently have an interactive game running every day on a separate Youtube channel](https://www.youtube.com/channel/UCRcljlI4ACjc5VWZVr4WdnA) that is yet to find any players.

***

## Description
TODO - Once ready, Write up instructions/functions of the application here, with supporting images?

## Installation
Note to self - Once ready, add a link to a build that can be downloaded and used. 

Otherwise, download this project, open it from Unity and make a build from there.

## Usage

#### API Key
In order to use this, a Google API Key that supports Youtube Data API V3 is required. I will try to put instructions for this in the application as well, however:
- Go to the [Google Cloud Console](https://console.cloud.google.com) and log in with your Google/Youtube account.
- Create a new project (The top blue bar? need images? Up to 12 can be made for free.)
- From the top-left corner Navigation Menu, go to APIs & Services -> Enabled APIs & Services.
- Select the separate + icon to enable a new service, you should be redirected to the API Library
- Search for "youtube data api v3" and enable it for the current project.
- From the Navigation Menu, go to APIs & Services -> Credentials.
- Select "Create Credentials" and choose "API Key". A key is automatically generated.
- From this point, you can click on "Show Key" and copy it to use in this project. Optionally, you can edit the key to restrict access to only Youtube from the 3 dots at the right side of the key and clicking "Edit Key". If this key somehow becomes compromised, it can be regenerated to a new key from here, or deleted as well.
- Important Note - All API keys are default limited to 10,000 query points per day. I should put a note of this up somewhere, but with the current default 3 second delay, that means slightly more than 1h30m of use time per day. Since some of the future ideas aren't as dependant on quick refreshes, delay for those could be increased, allowing it to be used longer.

#### Video ID
https://www.youtube.com/watch?v={VIDEO_ID}. Copy the "VIDEO_ID" area from the actual livestream and paste it in the application.

## Project Status (Roadmap)

As of right now, this project is still in pre-development. An MVP of this can be ran by opening it through Unity and creating a build, but there are some convenience functions still missing before I would consider making the first public build available. Some of these examples include:

- Modifiable delay before checking for any new messages (currently only between 3 second intervals.)
- Restoring window screen size to what it was before entering fullscreen, when exiting fullscreen.
- Having all UI elements be somewhat normally viewable whenever the screen is resized.
- etc...

Once all of the baseline functions are made for the first public build, I'll make a list of other features I'm planning on adding here.

## Support
- Youtube Channel - [Stat Void](https://www.youtube.com/channel/UCRcljlI4ACjc5VWZVr4WdnA). Showing interest in the stuff I'm doing is probably the best way to support right now.

## Contributing
I'm currently not looking for advice or contributions for this project. 

However, if you want to have an idea on where the expand from on your own, everything under the folder Assets/Youtube should be a baseline of where to start. It has all the components that handle fetching data from a livestream and other details, but is also currently under construction.

## License
Uhh, whatever the license was to freely download and edit it as you see fit?

