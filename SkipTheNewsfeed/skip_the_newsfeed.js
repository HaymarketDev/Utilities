chrome.webRequest.onBeforeRequest.addListener(
	function(info) {
	//alert(info.url);
		if (info.url === "https://www.facebook.com/" || info.url === "https://www.facebook.com/home.php"){
			return {redirectUrl: "https://www.facebook.com/profile.php"};
		}
		if (info.url === "https://twitter.com"){
			return {redirectUrl: "https://twitter.com/i/connect"};
		}
	},
	// filters
	{
	urls: [
		//All Urls (except twitter apparently...
		//"*://*/*"
		
		"https://www.facebook.com/*"
		
		//"https://twitter.com/"
		]
	},
	["blocking"]
);