# GoogleSmartHomeSignedJWT
In order to report status or to request sync for a Google Smarthome application, you need to create a signed JWT using
a certificate created on your Google Cloud Console(GCC).  Details are available using the below URL's
https://developers.google.com/assistant/smarthome/develop/report-state
https://developers.google.com/assistant/smarthome/develop/request-sync

Google doesn't provide a .Net solution as they do for Node.js and Java.  There are solutions available at http://jwt.io.

In order to get your GoogleHome app certified, you must implement at least request sync.

Download a .p12 to use for the certificate private key and that needs to be in the project root directory
The "iss" claim is the email address of the IAM account.  This is available in the GCC.
You will need these two items to run the program: The IAM email address and the .p12 file

 Note: the Crypto API only works on .Netcore 3.X. The Microsoft.IdentityModel.Tokens is a GUGet package.

 Finally, a command line application is not too practical for production.  This application creates a token that expires in 60 minutes.
 If you port this to a website, IIS must have "Load Profiles" set to "true" or the Crypto API will fail.
 
 The initial signed JWT is temporary and must be submitted to the Google OAuth token exchange site for a new access token.  
 
 The final token is returned from the token exchange as a JSON:
 
 {"access_token":"ya29.c.Ko8Bxgf8jeot3E8Y_6tzdpKOIj4LI7-TYnCxnWoF71EW2uCU0xZReIPpP-REVRh6eeV4tMB6ARQiQzeGq_PM9QWpo__at2H2PdXZZUsfbP3ZSlfqA-kztdtCOJD9TXQJPMCfU0fteBX_C_OWceA3YeOcLjBzVtpaL36e5qW0ZuYGndo2-txq_GpQQVpxET3D8aU","expires_in":3599,"token_type":"Bearer"}
 
 Use this token to report state or request sync.
 
