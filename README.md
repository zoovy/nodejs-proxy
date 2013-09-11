nodejs-proxy
============

A system tray management utility for starting stopping node.js scripts.
Packaged with nodeproxy.js a utility for single-page app developers. 

RATIONALE
===
Node.js is a pretty f*cking amazing programming environment.  BUT Despite having full support of
Chromes V8 Javascript Engine WITH the ability to access hardware, and other system resources filesystem via 
a plethora of NPM's, Node.js requires users to start/stop via kludgy dos prompts.  
It (at this point) isn't embeddable into C#/.net apps .. although we're working on that. 

Now I realize that many white-beareded wizard system-admins love command lines. I've personally spent a 
great portion of my career in a sys-admin role I've always appreciated how /simply intimidating/ 
a good ole' command line is --with --lots of --parameters "that aren't always clear". 

But command lines in Windows are obtuse.
DOS directory navigation and using command lines with no CTRL+C/CTRL+V cut/paste either scare or
frustrate nearly everybody who uses them.  Hell, as a programmer using command lines in any environment 
which allows spaces based tab command completion makes me feel dirty all over. 
The programmer side of me appreciates why "My Documents" is great to teach end-users to isolate
their code (applications) and data, but I loathe the fact that Vista/Win8 use the UAC to enforce placing files 
in the oddest freaking locations which ALWAYS seem to include spaces.  As a Linux admin fluent in command
line I find this alien landscape frightening, even though I know DOS very well.  The great 8.3 compromise of
Win95 never quite sat well with me.  So the goal of this app is to bring the power of Node.js to end users
running Win7/8/Vista and teach more web-developers to use Git.  (Both things I consider to be noble causes) by
eliminating the DOS command line that seems to be the barrier to adoption. 

LONG TERM GOALS
===
So Long term this will be a utility for starting and stopping multiple node.js scripts as windows services.
Node.js is just a fast easy way to write JS software that runs on windows, mac, linux/unix, android, ios (sorta).
Distributed with multiple node.js scripts for doing a variety of interesting things (not just creating 
webservers and proxies). 

SHORT TERM GOALS: NodeProxy.Js
===

Short term nodejs-proxy comes with one node.js script called "nodeproxy.js"
* a utility for testing single page html apps that need to make REST API calls to a backend before
doing a commit/sync in GIT.
* Useful for AnyCommerce (a development framework) for building full e-commerce "shopping applications"
in javascript, html, css.  These shopping apps can run fully client side using file:// in many browsers
but getting files onto things like iPads, etc. is a pain the rear. In pratices most developers will commit/sync
their changes to TEST their work .. creating MASSIVE commit logs. 
* Nodeproxy solves this by creating a simulated testing environment, use the the taskbar app with 
a domain and a (git) project directory, complete with webserver and self signed SSL certificates.
* Configure the browser to use the local proxy at 127.0.0.1:8080 and start your adventure!
* The browser will send all HTTP/HTTPS requests to the proxy, the Proxy will look at the domain and decide
if it can serve the file locally (from your github repo) OR if it should forward the request to the real
Internet (ex: www.google.com).   The request is then forwarded to an internal webserver (also supplied
within nodeproxy.js) which then recieves the request looks for the local file, if the file doesn't exist
the request is forwarded to the REAL domain (so things like REST API calls, 404's, etc. still work)
* Modify nodeproxy.js to have fun creating amazing man in the middle (mitm) attacks or pranks -- you decide!
* Change the proxy port to a public IP on your computer, disable the UAC/Firewall then point iPads, Phones, etc.
which are on the same wifi network at the proxy for even more fun!

Ulitimately nodeproxy.js gives developers a chance to test their projects locally BEFORE committing. 
Needless to say we're hoping this will reduce the number of lame-ass commits with poor descriptions 
that are committed to the master branch while doing responsive testing.  Giving system admins/support safe and
clearly document rollback points. And being able to run local files AS IF they are really served 
on the Internet completely side-steps a number of CORs issues and produces better code quality.
It also lets web developers enjoy the full power of git for change/tracking management that we've enjoyed
in the kernel/utility/perl development communities for such a long time.

Enjoy!

===

Credits:
* http://nini.sourceforge.net/
* https://github.com/MarvinAlpaca/SelfSignedX509OpenSslNet
* http://www.openssl.org/
