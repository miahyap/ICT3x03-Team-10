export const validateAzureADAccount = () => {
    let currentPage = window.location.href.split(/[?#]/)[0];
    let azureAdOauthUrl = "https://login.microsoftonline.com/sit.singaporetech.edu.sg/oauth2/v2.0/authorize?" +
        "client_id=3b11e746-dcf4-40a9-a269-efd461ba7443" +
        "&response_type=code" +
        "&redirect_uri=" + currentPage +
        "&response_mode=query" +
        "&scope=openid%20https%3A%2F%2Fgraph.microsoft.com%2Femail%20https%3A%2F%2Fgraph.microsoft.com%2Foffline_access%20https%3A%2F%2Fgraph.microsoft.com%2Fprofile%20https%3A%2F%2Fgraph.microsoft.com%2FUser.Read%20https%3A%2F%2Fgraph.microsoft.com%2FUser.ReadBasic.All\n" +
        "&state=12345" +
        "&prompt=consent"
    window.location.href = azureAdOauthUrl;
};