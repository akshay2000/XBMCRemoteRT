#Kodi Assist#

The ultimate Kodi Windows Phone remote.

##For Users##
If you just want to enjoy your media, these steps should get you running.

**Set up the server**  
This is a short version Kodi setup. If you are interested in knowing all the small details, you may follow [Kodi Quick Start Guide](http://kodi.wiki/view/Quick_start_guide) at the Kodi Wiki.
* Download and install Kodi from [Kodi download page](http://kodi.tv/download/) for your preferred platform.
* Navigate around and add some media.
* Go to `Settings>Webserver` and check 'Allow control of Kodi via HTTP'. Here, you may set up the port, username and password to whatever you want.
* Now, go to `Settings>Remote control` and check both the options, especially 'Allow programs on other systems to control Kodi'.

**Set up the remote**  
This should be easy and fun!
* Download the app from [Windows Phone Store](http://www.windowsphone.com/s?appid=3897b459-b11b-41eb-9cea-dd9e53c55b78).
* Tap the 'add connection' button in the app bar.
* Remember the details you chose while setting up the server? Enter those details here and save the connection.
* Tap on the connection and you are good to use the remote.

##For Translators##
Want to use app in your own language? You're in luck. Translations are open source too. All you have to do is follow the link below and start translating. You can improve existing translations, complete incomplete languages and even add new languages. [Click here](https://poeditor.com/join/project?hash=3d2dd59f0d8ac3c1fe895a5651c8b0f2) to start translating.

##For Developers##
Developers must follow the steps for users first. This section assumes that you have correctly set up the server and are able to control it using Windows Phone app.

**Get the source code**  
If you just want to browse through, code is available at [this GitHub repo](https://github.com/akshay2000/XBMCRemoteRT). If you plan on contributing, you should clone the repo and follow along for the environment setup.

**Set up the IDE**  
The project was set up using Visual Studio 2013 (Ultimate Edition). However, Windows Phone SDK should work too. You can obtain the latest version of SDK from the [Windows Dev Center](http://dev.windows.com/en-us/develop/download-phone-sdk).  
After you have installed the SDK, you should be able to open the project from source code you got in previous step.  
Before building the solution for the first time, NuGet will need to download dependencies. If this doesn't happen automatically, you should explicitly tell NuGet to update all the packages the project depends on.

**Build and run**  
Just hit the run button!

##Troubleshooting##
If you've followed all the steps above correctly, you shouldn't have any trouble. So, before jumping here, please make sure that you didn't miss anything out. If you still have some trouble, this should help you.

**Phone won't connect to the server**  
Open Internet Explorer on the phone and navigate to server. Make sure you use the correct port. Example URL looks like `http://192.168.0.10:8080`.  
If you are able to see the Kodi Web UI:
* Go back to Kodi Assist and double check your connection settings.
* Make sure that you have entered the correct password.

If you are _not_ able to see the Kodi Web UI:
* Check the you have enabled webserver and remote access in Kodi settings.
* Check if you can open the Web UI from any other computer on the local network. If not, you have bigger problems!
* Check the your firewalls are not blocking anything.
* If you are using Kodi server's host name, try using the local IP address instead.

**Emulator won't connect to server**  
All the points in previous section are applicable here. You might want to go through following points in addition.
* If you were thinking that `localhost` on emulator points to your host dev machine, think again. Emulator's `localhost` points to the emulator itself. If you want to access the host machine, use host machine's IP address.
* Check that emulator's connections are configured correctly.

**Miscellaneous**  
Some random things, in no particular order.
* If you want to make raw JSON-RPC calls and examine the output, you may use some sort of REST client. [Postman](http://www.getpostman.com/) is one excellent choice.
* Everything is easier when you have a wireless router.
