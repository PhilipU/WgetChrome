# WgetChrome

![Screenshot of WgetChrome.exe](docs/WgetChrome_v1.0_Screenshot.png)

Download fully rendered Web Pages via Chrome Browser.

Often web pages loads additional content via Javascript. If these kind of pages are downloaded via CLI tools like `wget` or `curl` the scripts are not executed and the pages are not properly rendered.

With WgetChrome.exe the page is accessed via a Chrome Browser instance and downloaded after the page was fully loaded and rendered.

## FAQ

**Q: Do I need to install the Chrome Browser manually?**

**A:** No, the browser will be automatically downloaded and installed by the integrated Puppeteer Sharp library
