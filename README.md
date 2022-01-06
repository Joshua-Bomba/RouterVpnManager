## RouterVpnManager
I Have a DD-WRT router and an apple TV i want to be able to control what vpn my router is connect to from my apple tv.
just to play around  and see if I can accomplish this. 
I can get python to run to run in DD-WRT so I'm using that as a backend and a C# Xamarin Front end. 
I'm going to start small with a C# test client and a C# clientLibrary to get the ball rolling

## Quicky
Been kinda lazy of this project and kinda abandoned it
here a quick setup guide kinda. notice how little I cared
honestly this is just for me later when I forgot how everything worked

I'm using expressvpn so the process may differ on diffrent vpns

# linuxy bits
you need a router with DD-WRT installed on it(up to you to install, careful well flashing your router there is a posibility of brinking it).
need to install entware on the router
https://wiki.dd-wrt.com/wiki/index.php/Installing_Entware
this is a package manager so after you finish installing it
ensure all packages are up to date
install python
copy the RouterVpnManager folder to the /opt folder on the router
before starting the script I sugest you start a screen (you can install screen through entware)
You can run the following command to start it
python RouterVpnManager.py 192.168.x.x 8000

you can change the port but I picked 8000 because that was my default port for whatever reason
honestly I can't remember why the address is in there. I think it's so it can map it to the correct network interface

any commands sent should now be output in the linux terminal


# config bits
You will need to setup VPNConfigs now
I Remember I had alot of trouble deviating from the way openvpn is used in DD-WRT so we essentially copy how the config is done in dd-wrt

First you must setup the config using the DD-WRT interface
https://www.expressvpn.com/support/vpn-setup/manual-config-for-dd-wrt-router-with-openvpn/

Once this is done and your connect to your desired vpn you launch visual studio
set the startup project as RouterVpnManagerClientTest
Open the Program.cs file and edit the Host to your path and or edit the App.Config file (idk what I was doing here, being lazy I guess)
run the Program
type in help
there should be a command that let's you save the current config
this will copy the config to a folder where the python script is running on the router
exit the program (it's a bit buggy, it like to throw false errors)

# appley bits
In XCode make sure your signed into your apple account have the developer options enabled and a personal team setup (you don't need to pay for the apple developer account). 

Open Visual Studios for mac and open the solution
in the RouterVpnManagerClientAppleTv project in the RouterVpnManagerWrapper.cs you can change the ip (again with this crap)
Open the Info.plist file
under the Bundle identifier create a unique bundle id
Hold the Control key down a Click on the Main.storyboard and select openwith -> xcode Interface builder
this will open xcode
click the root project file and select the Signing & Capabilities tab
select your team and this should setup certificate signing
in xcode also connect your AppleTV to XCode (window -> devices and simulators)
go back to Visual Studio Control click on the AppleTv Project Root select options
then go to tvOS Bundle Signing and select your email from the dropdown
select your newly created Provisioning Profile
Click okay to save your changes
in the build config you should see the AppleTv as a target option
build the solution
you can try running it from Visual Studio. I've not been able to get this to work anymore guessing there have been alot of Mac Os updates since I made this and also I'm using a diffrent mac which also has the M1

# Stuck on deploy to device
Open Xcode
Window -> Devices & Simulators
Select your Apple TV from Connected devices
under the installed app section click the plus
go to the Bin Folder ... device-builds/ Appletv -> you will see the app there click open and xcode device manager should install the app for you

You should see the App on your Apple TV

## How to use
open the app click
Connect to VPN server
this will open a connection to the VpnManager running on the router
Click the Select A VPN Option
this will list the avaliable vpns
click on 1
you should get a conformation 
you can close the app and enjoy your code from a diffrent local




