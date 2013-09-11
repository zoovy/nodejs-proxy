nodejs-proxy
============

A system tray management utility for starting stopping node.js scripts.
Packaged with nodeproxy.js a utility for single-page app developers. 

RATIONALE
===
Node.js is a pretty f*cking amazing programming environment, but despite having full windows support of
Chromes V8 and ability to access hardware, and other system resources filesystem NPM's, 
it requires users to start/stop via arcane dos prompts.  It (at this point) isn't embeddable into C#/.net
apps .. although we're working on that. 

Now I realize that white-beareded wizard system-admins love command lines, and considering I've spent a 
good portion of my career in that role I've always appreciated how /simply intimidating/ they are. 

Old-school DOS directory navigation and command lines scare the piss out of web-developers and end-users.
But command lines in Windows are obtuse. Hell, command lines in any environment which allows spaces with
tab based command completion is just dancing with the devil as far as I'm concerned.
The programmer side of appreciates why "My Documents" is great for end-users by teaching them to isolate
their code and data, I personally loathe the fact that Vista/Win8 use the UAC to enforce placing files 
in the oddest freaking locations which ALWAYS seem to include spaces.  As a Linux admin fluent in command
line I find this alien landscape frightening. The goal of this is to bring the power of Node.js to end users
running Win7/8/Vista and teach more web-developers to use Git.  (Both things I consider to be noble causes)

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
