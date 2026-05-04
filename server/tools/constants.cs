
namespace Contensive.Addons.Tools {
    public static class Constants {
        //
        // -- guid of sample record
        public const string guidAddonMockLayout = "{826BE30A-72A4-49DA-86F1-B744DC896A15}";
        //       

        public const string layoutInviteProfileGuid = "{76731E3C-8FD8-45D5-8F9C-F1265E807777}";
        public const string layoutInviteProfileName = "Invite Users Form Layout";
        public const string layoutInviteProfileCdnPathFilename = @"tools\InviteUsersForm.html";

        public const string inviteToolProfilePageEndpoint = "InviteProfilePage";
        // -- sample
        public const int version = 1;
        // 
        // -- request names 
        public const string rnInputValue = "inputValue";
        // 
        // -- errors for resultErrList
        public enum ResultErrorEnum {
            errPermission = 50,
            errDuplicate = 100,
            errVerification = 110,
            errRestriction = 120,
            errInput = 200,
            errAuthentication = 300,
            errAdd = 400,
            errSave = 500,
            errDelete = 600,
            errLookup = 700,
            errLoad = 710,
            errContent = 800,
            errMiscellaneous = 900
        }
        // 
        // -- http errors
        public enum HttpErrorEnum {
            badRequest = 400,
            unauthorized = 401,
            paymentRequired = 402,
            forbidden = 403,
            notFound = 404,
            methodNotAllowed = 405,
            notAcceptable = 406,
            proxyAuthenticationRequired = 407,
            requestTimeout = 408,
            conflict = 409,
            gone = 410,
            lengthRequired = 411,
            preconditionFailed = 412,
            payloadTooLarge = 413,
            urlTooLong = 414,
            unsupportedMediaType = 415,
            rangeNotSatisfiable = 416,
            expectationFailed = 417,
            teapot = 418,
            methodFailure = 420,
            enhanceYourCalm = 420,
            misdirectedRequest = 421,
            unprocessableEntity = 422,
            locked = 423,
            failedDependency = 424,
            upgradeRequired = 426,
            preconditionRequired = 428,
            tooManyRequests = 429,
            requestHeaderFieldsTooLarge = 431,
            loginTimeout = 440,
            noResponse = 444,
            retryWith = 449,
            redirect = 451,
            unavailableForLegalReasons = 451,
            sslCertificateError = 495,
            sslCertificateRequired = 496,
            httpRequestSentToSecurePort = 497,
            invalidToken = 498,
            clientClosedRequest = 499,
            tokenRequired = 499,
            internalServerError = 500
        }
    }
}