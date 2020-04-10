# GoogleSmartHomeSignedJWT
In order to report status or to request sync for a Google Smarthome application, you need to create a signed JWT using
a certificate created on your Google Cloud Console(GCC).  Details are available using the below URL's
https://developers.google.com/assistant/smarthome/develop/report-state
https://developers.google.com/assistant/smarthome/develop/request-sync

Google doesn't provide a .Net solutions as they do for Node.js and Java.  There are solutions available at http://jwt.io.

In order to get your GoogleHome app certified, you must implement at least request sync.

Download a .p12 to use for the certicate private key and that needs to be in the project root directory
The "iss" claim is the email address of the IAM account.  This is available in the GCC.
You will need these two items to run the program: The IAM email address and the .p12 file

 Note: the Crypto API only works on .Netcore 3.X. The Microsoft.IdentityModel.Tokens is a GUGet package.

 Finally, a command application is not too practical for production.  This application creates a token that expires in 60 minutes.
 If you port this to a website, IIS must have "Load Profiles" set to "true" or the Crypto API will fail.